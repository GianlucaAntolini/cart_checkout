using Microsoft.AspNetCore.Mvc;
using YourNamespace.Models;

using YourNamespace.Data.Repositories;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class OrdersController : Controller
    {
        private readonly IUnitOfwork _unitOfWork;

        private readonly OrderRepository _orderRepository;


        private readonly NationRepository _nationRepository;

        private readonly NewsletterSubscriptionRepository _newsletterSubscriptionRepository;

        private readonly PaymentTypeRepository _paymentTypeRepository;

        private readonly OrderPaymentRepository _orderPaymentRepository;

        private readonly ProductRepository _productRepository;

        private readonly UserOrderRepository _userOrderRepository;

        private readonly SignInManager<User> _signInManager;

        private readonly UserManager<User> _userManager;


        public OrdersController(IUnitOfwork unitOfwork, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _unitOfWork = unitOfwork;
            _orderRepository = new OrderRepository(_unitOfWork);
            _nationRepository = new NationRepository(_unitOfWork);
            _newsletterSubscriptionRepository = new NewsletterSubscriptionRepository(_unitOfWork);
            _paymentTypeRepository = new PaymentTypeRepository(_unitOfWork);
            _orderPaymentRepository = new OrderPaymentRepository(_unitOfWork);
            _productRepository = new ProductRepository(_unitOfWork);
            _userOrderRepository = new UserOrderRepository(_unitOfWork);
            _signInManager = signInManager;
            _userManager = userManager;

        }

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]

        public async Task<IActionResult> Index()
        {
            if (_signInManager.IsSignedIn(User) == false)
            {
                return RedirectToAction("Login", "Account");
            }

            // get the payment types
            var paymentTypesResult = await _paymentTypeRepository.Get();
            var paymentTypes = new List<PaymentType>();
            if (paymentTypesResult.Result is OkObjectResult okPaymentTypesResult && okPaymentTypesResult.Value is IEnumerable<PaymentType> paymentTypesResultList)
            {
                paymentTypes = paymentTypesResultList.ToList();
                ViewBag.PaymentTypes = paymentTypes;

            }


            // get the orders with related entities
            var orders = new List<Order>();
            var userOrders = _userOrderRepository.GetAllByUserId(_userManager.GetUserId(User));
            if (userOrders.Result != null && userOrders.Result.Value != null && userOrders.Result.Value is IEnumerable<UserOrder> userOrdersResultList)
            {
                foreach (var userOrder in userOrdersResultList)
                {
                    var hasOrderBeenPaid = await _orderPaymentRepository.HasOrderBeenPaidByOrderId(userOrder.OrderId);
                    if (hasOrderBeenPaid.Value)
                    {
                        var orderResult = await _orderRepository.GetByIdWithRelatedEntities(userOrder.OrderId);
                        if (orderResult.Value is Order order)
                        {
                            orders.Add(order);

                            // get the newsletter subscription  by the email of the order user detail if there is one
                            var newsletterSubscriptionResult = await _newsletterSubscriptionRepository.GetByEmail(order.OrderUserDetail.Email);
                            if (newsletterSubscriptionResult.Value is NewsletterSubscription newsletterSubscription)
                            {
                                ViewBag.NewsletterSubscription = newsletterSubscription;
                            }
                        }
                    }
                }
            }

            ViewBag.Orders = orders;








            return View();
        }


        // Checks if invoice can be sent
        // return false if the order has not been paid yet, if the order has no invoice or if the order is not of the user
        private async Task<bool> CheckInvoiceOrder(Order order)
        {
            // Check if the order is of the user
            var userOrder = await _userOrderRepository.GetByOrderIdAndUserId(order.Id, _userManager.GetUserId(User));
            if (userOrder.Value is UserOrder foundUserOrder)
            {
                // Check if the order has not been paid yet
                var hasOrderBeenPaid = await _orderPaymentRepository.HasOrderBeenPaidByOrderId(foundUserOrder.OrderId);
                if (hasOrderBeenPaid.Value)
                {
                    // Check if the order has an invoice
                    return order.OrderInvoice != null;
                }


            }

            return false;

        }


        [Authorize]
        //InvoicePDF with orderId as parameter (int)
        [HttpGet("InvoicePdf/{orderId:int}")]

        public async Task<IActionResult> InvoicePDF(int orderId)
        {
            // Retrieve the user ID from the token (JWT authentication)
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            if (orderId <= 0)
            {
                return BadRequest();
            }

            // Get the order with related entities
            var orderResult = await _orderRepository.GetByIdWithRelatedEntities(orderId);
            if (orderResult.Value is Order order)
            {
                if (await CheckInvoiceOrder(order) == false)
                {
                    return BadRequest("Action unavailable");
                }
                // Fetch order payment with execution date from orderpaymentrepository
                var orderPaymentResult = await _orderPaymentRepository.GetByOrderIdWithExecutionDatAndPaymentType(orderId);
                OrderPayment? orderPayment = null;
                if (orderPaymentResult is OrderPayment queryResultOrderPayment && queryResultOrderPayment != null && queryResultOrderPayment.ExecutionDate != null)
                {
                    orderPayment = queryResultOrderPayment;
                }
                else
                {
                    return BadRequest();
                }

                var invoiceJSON = new Dictionary<string, string>
        {
            { "Name", order.OrderUserDetail.Name },
            { "Surname", order.OrderUserDetail.Surname },
            { "Nation", order.OrderUserDetail.Nation.Name },
            { "PIVA", order.OrderInvoice.PIVA },
            { "CF", order.OrderInvoice.CF },
            { "Price", orderPayment.TotalAmount.ToString() },
            { "PaymentMethod", orderPayment.PaymentType.Name },
            { "InvoiceNumber", order.OrderInvoice.Id.ToString() },
            { "Date", orderPayment.ExecutionDate.Value.ToString("dd/MM/yyyy") }
        };

                // Path to the HTML template
                string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "templates", "invoice.html");
                string htmlTemplate = await System.IO.File.ReadAllTextAsync(templatePath);

                // Replace placeholders with actual data
                string htmlContent = ReplacePlaceholders(htmlTemplate, invoiceJSON);

                // Generate PDF
                byte[] pdfBytes = await GeneratePdfFromHtml(htmlContent);

                // Return the PDF file as a download
                return File(pdfBytes, "application/pdf", "fattura.pdf");

            }
            else
            {
                return BadRequest();
            }


        }

        private static string ReplacePlaceholders(string htmlTemplate, Dictionary<string, string> data)
        {
            foreach (var placeholder in data)
            {
                htmlTemplate = htmlTemplate.Replace("{" + placeholder.Key + "}", placeholder.Value);
            }
            return htmlTemplate;
        }

        private static async Task<byte[]> GeneratePdfFromHtml(string htmlContent)
        {
            //await new BrowserFetcher().DownloadAsync();
            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
            await using var page = await browser.NewPageAsync();
            await page.SetContentAsync(htmlContent);
            return await page.PdfDataAsync(new PdfOptions
            {
                Format = PaperFormat.A4
            });
        }














    }







}
