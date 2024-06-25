using Microsoft.AspNetCore.Mvc;
using YourNamespace.Models;

using YourNamespace.Data.Repositories;
using Stripe.BillingPortal;
using Stripe.Checkout;
using System.Reflection.Metadata.Ecma335;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace YourNamespace.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IUnitOfwork _unitOfWork;

        private readonly OrderRepository _orderRepository;


        private readonly NationRepository _nationRepository;

        private readonly NewsletterSubscriptionRepository _newsletterSubscriptionRepository;

        private readonly PaymentTypeRepository _paymentTypeRepository;

        private readonly OrderPaymentRepository _orderPaymentRepository;

        private readonly ProductRepository _productRepository;


        public CheckoutController(IUnitOfwork unitOfwork)
        {
            _unitOfWork = unitOfwork;
            _orderRepository = new OrderRepository(_unitOfWork);
            _nationRepository = new NationRepository(_unitOfWork);
            _newsletterSubscriptionRepository = new NewsletterSubscriptionRepository(_unitOfWork);
            _paymentTypeRepository = new PaymentTypeRepository(_unitOfWork);
            _orderPaymentRepository = new OrderPaymentRepository(_unitOfWork);
            _productRepository = new ProductRepository(_unitOfWork);
        }

        public async Task<IActionResult> Index()
        {

            // Redirect to Cart if no order or no products in order (should not happen)
            if (await CheckInvalidOrder())
            {
                return RedirectToAction("Index", "Cart");
            }

            // get the order with related entities
            var orderId = HttpContext.Session.GetInt32("OrderId") ?? 0;
            var orderResult = await _orderRepository.GetByIdWithRelatedEntities(orderId);
            if (orderResult.Value is Order order)
            {
                ViewBag.Order = order;


                // get the newsletter subscription  by the email of the order user detail if there is one
                var newsletterSubscriptionResult = await _newsletterSubscriptionRepository.GetByEmail(order.OrderUserDetail.Email);
                if (newsletterSubscriptionResult.Value is NewsletterSubscription newsletterSubscription)
                {
                    ViewBag.NewsletterSubscription = newsletterSubscription;
                }

                // get the payment types
                var paymentTypesResult = await _paymentTypeRepository.Get();
                var paymentTypes = new List<PaymentType>();
                if (paymentTypesResult.Result is OkObjectResult okPaymentTypesResult && okPaymentTypesResult.Value is IEnumerable<PaymentType> paymentTypesResultList)
                {
                    paymentTypes = paymentTypesResultList.ToList();
                    ViewBag.PaymentTypes = paymentTypes;

                }
            }







            return View();
        }

        // Check if order id is not in session or if the order has no products or if order has no user details
        private async Task<bool> CheckInvalidOrder()
        {
            int orderId = HttpContext.Session.GetInt32("OrderId") ?? 0;
            if (orderId == 0)
            {
                return true;
            }

            var orderResult = await _orderRepository.GetByIdWithRelatedEntities(orderId);
            if (orderResult.Value is Order order)
            {
                if (order.OrderProducts.Count == 0)
                {
                    return true;
                }
                if (order.OrderUserDetail == null)
                {
                    return true;
                }
                else
                {
                    // check if order user details are valid
                    if (order.OrderUserDetail.Name == null || order.OrderUserDetail.Name == "" || order.OrderUserDetail.Surname == null || order.OrderUserDetail.Surname == "" || order.OrderUserDetail.Email == null || order.OrderUserDetail.Email == "" || order.OrderUserDetail.NationId == 0)
                    {
                        return true;
                    }
                }
            }
            return false;

        }



        [HttpPost]
        public async Task<IActionResult> GoToPayment()


        {
            string? action = Request.Form["action"];

            if (action == null || action != "continue" || await CheckInvalidOrder())
            {
                return RedirectToAction("Index", "UserInfo");
            }


            bool terms_and_conditions = Request.Form.ContainsKey("terms_and_conditions") ? Request.Form["terms_and_conditions"] == "on" : false;

            if (!terms_and_conditions)
            {
                return RedirectToAction("Index");
            }

            int? paymentTypeId = Request.Form.ContainsKey("payment_type") && int.TryParse(Request.Form["payment_type"], out int castPaymentTypeId) ? castPaymentTypeId : null;


            // Get payment types 
            var paymentTypesResult = await _paymentTypeRepository.Get();
            var paymentTypes = new List<PaymentType>();
            if (paymentTypesResult.Result is OkObjectResult okPaymentTypesResult && okPaymentTypesResult.Value is IEnumerable<PaymentType> paymentTypesResultList)
            {
                paymentTypes = paymentTypesResultList.ToList();
            }
            else
            {
                return RedirectToAction("Index");
            }

            if (paymentTypeId == null || !paymentTypes.Any(pt => pt.Id == paymentTypeId) || paymentTypes.First(pt => pt.Id == paymentTypeId).Name != "Stripe")
            {
                return RedirectToAction("Index");
            }

            //TODO: rifare tutti controlli su prodotti qt/attivi,  coupon attivi/esistenti + controllare che non esista gia un pagamento 
            // completato x questo ordine che potrebbe essere stato fatto da un altra parte



            // Get the order
            int orderId = HttpContext.Session.GetInt32("OrderId") ?? 0;
            var orderResult = await _orderRepository.GetByIdWithRelatedEntities(orderId);
            if (orderResult.Value is Order order)
            {
                // Create a new OrderPayment
                var orderPayment = new OrderPayment
                {
                    OrderId = order.Id,
                    TotalAmount = order.TotalAmountWithCoupon,
                    PaymentTypeId = (int)paymentTypeId
                };

                order.OrderPayment = orderPayment;

                // Set new order timestamp, maybe don't need this
                order.OrderTimestamp = DateTime.Now;

                order.TermsAndConditions = terms_and_conditions;


                // Save the order
                var updateOrderResult = await _orderRepository.Update(orderId, order);

                // Now get the order payment with the highest id (meaning the latest payment attempt), so the one just done
                var orderPaymentResult = await _orderPaymentRepository.GetByOrderIdWithHighestId(orderId);
                // Put it in session
                if (orderPaymentResult is OrderPayment queryResultOrderPayment)
                {
                    HttpContext.Session.SetInt32("OrderPaymentId", orderPayment.Id);
                }


                var domain1 = Request.Host.Host;
                // Stripe checkout
                var domain = "http://localhost:5198/";

                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = domain + "Checkout/Success",
                    CancelUrl = domain + "Checkout/Error",
                    Mode = "payment",
                    CustomerEmail = order.OrderUserDetail.Email,
                    LineItems = order.OrderProducts.Select(orderProduct => new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "eur",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = orderProduct.Product.Name,
                                Images = new List<string> { domain + "img/products/" + orderProduct.Product.Id + ".png" }
                            },
                            // Convert the price to cents
                            UnitAmount = (long)((orderProduct.PriceWithCoupon / orderProduct.Quantity) * 100),
                        },
                        Quantity = orderProduct.Quantity,
                    }).ToList(),

                };

                var service = new Stripe.Checkout.SessionService();
                var session = service.Create(options);

                HttpContext.Session.SetString("StripeSessionId", session.Id);


                Response.Headers.Append("Location", session.Url);
                return new StatusCodeResult(303);



            }
            else
            {
                return RedirectToAction("Index");
            }







        }

        public async Task<IActionResult> Success()
        {
            var sessionId = HttpContext.Session.GetString("StripeSessionId");
            if (sessionId == null)
            {
                return RedirectToAction("Error");
            }

            var sessionService = new Stripe.Checkout.SessionService();
            var session = sessionService.Get(sessionId);

            if (session.PaymentStatus == "paid")
            {
                // Get the order payment id from session
                int orderPaymentId = HttpContext.Session.GetInt32("OrderPaymentId") ?? 0;
                if (orderPaymentId == 0)
                {
                    return RedirectToAction("Error");
                }
                // Get the order payment
                var orderPaymentResult = await _orderPaymentRepository.GetById(orderPaymentId);
                if (orderPaymentResult.Result is OkObjectResult okOrderPaymentResult && okOrderPaymentResult.Value is OrderPayment orderPayment)
                {
                    // Set the order payment as completed
                    orderPayment.ExecutionDate = DateTime.Now;
                    // Update the order payment
                    await _orderPaymentRepository.Update(orderPaymentId, orderPayment);
                    // Now get the order and reduce the product quantities
                    var orderId = orderPayment.OrderId;
                    var orderResult = await _orderRepository.GetByIdWithRelatedEntities(orderId);
                    if (orderResult.Value is Order order)
                    {
                        foreach (var orderProduct in order.OrderProducts)
                        {
                            var productResult = await _productRepository.GetById(orderProduct.ProductId);
                            if (productResult.Result is OkObjectResult okProductResult && okProductResult.Value is Product product)
                            {
                                product.StockQuantity -= orderProduct.Quantity;
                                await _productRepository.Update(product.Id, product);
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Error");
                    }
                    // Remove the order id from session
                    HttpContext.Session.Remove("OrderId");
                    // Remove the order payment id from session
                    HttpContext.Session.Remove("OrderPaymentId");
                    // Set the order id in viewbag
                    ViewBag.Order = order;
                    return View();
                }
            }

            return RedirectToAction("Error");
        }

        public async Task<IActionResult> Error()
        {
            var sessionId = HttpContext.Session.GetString("StripeSessionId");
            if (sessionId == null)
            {
                return RedirectToAction("Index", "Cart");
            }


            // Get the order payment id from session
            int orderPaymentId = HttpContext.Session.GetInt32("OrderPaymentId") ?? 0;
            if (orderPaymentId == 0)
            {
                return RedirectToAction("Index", "Cart");
            }
            // Get the order payment
            var orderPaymentResult = await _orderPaymentRepository.GetById(orderPaymentId);
            if (orderPaymentResult.Result is OkObjectResult okOrderPaymentResult && okOrderPaymentResult.Value is OrderPayment orderPayment)
            {
                // Set the order payment as completed
                orderPayment.CancelDate = DateTime.Now;
                // Update the order payment
                await _orderPaymentRepository.Update(orderPaymentId, orderPayment);
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Cart");
            }

        }


        [HttpPost]
        public async Task<IActionResult> InvoicePDF()
        {
            // I now cleared the session after the payment, so I need to get the order id from the query string
            // Since there is no login nor register for now this is the only way to get the order id
            int? orderId = Request.Form.ContainsKey("orderId") && int.TryParse(Request.Form["orderId"], out int castOrderId) ? castOrderId : null;
            if (orderId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Get the order with related entities
            var orderResult = await _orderRepository.GetByIdWithRelatedEntities(orderId.Value);
            if (orderResult.Value is Order order)
            {
                // Fetch order payment with execution date from orderpaymentrepository
                var orderPaymentResult = await _orderPaymentRepository.GetByOrderIdWithExecutionDatAndPaymentType(orderId.Value);
                OrderPayment? orderPayment = null;
                if (orderPaymentResult is OrderPayment queryResultOrderPayment && queryResultOrderPayment != null && queryResultOrderPayment.ExecutionDate != null)
                {
                    orderPayment = queryResultOrderPayment;
                }
                else
                {
                    return RedirectToAction("Index", "Home");
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
                return RedirectToAction("Index", "Home");
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
            await new BrowserFetcher().DownloadAsync();
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
