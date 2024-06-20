using Microsoft.AspNetCore.Mvc;
using YourNamespace.Models;

using YourNamespace.Data.Repositories;

namespace YourNamespace.Controllers
{
    public class UserInfoController : Controller
    {
        private readonly IUnitOfwork _unitOfWork;

        private readonly OrderRepository _orderRepository;


        private readonly NationRepository _nationRepository;

        private readonly NewsletterSubscriptionRepository _newsletterSubscriptionRepository;

        public UserInfoController(IUnitOfwork unitOfwork)
        {
            _unitOfWork = unitOfwork;
            _orderRepository = new OrderRepository(_unitOfWork);
            _nationRepository = new NationRepository(_unitOfWork);
            _newsletterSubscriptionRepository = new NewsletterSubscriptionRepository(_unitOfWork);
        }

        public async Task<IActionResult> Index()
        {

            // Redirect to Cart if no order or no products in order (should not happen)
            if (await CheckInvalidOrder())
            {
                return RedirectToAction("Index", "Cart");
            }




            var nationsResult = await _nationRepository.Get();
            var nations = new List<Nation>();
            if (nationsResult.Result is OkObjectResult okNationResult && okNationResult.Value is IEnumerable<Nation> nationsResultList)
            {
                nations = nationsResultList.ToList();
            }
            ViewBag.Nations = nations;



            return View();
        }


        [HttpPost]
        public IActionResult Cart()
        {
            return RedirectToAction("Index", "Cart");
        }


        // Check if order id is not in session or if the order has no products
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
            }
            return false;

        }



        [HttpPost]
        public async Task<IActionResult> SetUserData()


        {
            string? action = Request.Form["action"];

            if (action == null || action != "continue" || await CheckInvalidOrder())
            {
                return RedirectToAction("Index", "Cart");
            }
            // Get the values  from the form
            string? name = Request.Form["first-name"];
            string? surname = Request.Form["last-name"];
            string? email = Request.Form["email"];
            int? nationId = Request.Form.ContainsKey("nation") && int.TryParse(Request.Form["nation"], out int castNationId) ? castNationId : null;
            bool newsletter = Request.Form.ContainsKey("newsletter") ? Request.Form["newsletter"] == "on" : false;
            bool invoice = Request.Form.ContainsKey("invoice") ? Request.Form["invoice"] == "on" : false;
            bool privacy = Request.Form.ContainsKey("privacy") ? Request.Form["privacy"] == "on" : false;
            string? piva = Request.Form["fiscal-tax-number"];
            string? cf = Request.Form["fiscal-code-number"];

            var nationsResult = await _nationRepository.Get();
            var nations = new List<Nation>();
            if (nationsResult.Result is OkObjectResult okNationResult && okNationResult.Value is IEnumerable<Nation> nationsResultList)
            {
                nations = nationsResultList.ToList();
            }


            //  Redirect to self if any of the required fields is null or for the strings if they are empty (TODO: validation server side?) (except for piva and cf if invoice is false)
            //  or if privacy is false or if the nation is not in the list
            if (name == null || name == "" || surname == null || surname == "" || nationId == null || email == null || email == "" || (invoice && (piva == null || piva == "" || cf == null || cf == "")) || !privacy || !nations.Any(n => n.Id == nationId))
            {
                return View("Index");
            }



            // At this point everything is valid
            // Get the order

            int orderId = HttpContext.Session.GetInt32("OrderId") ?? 0;
            var orderResult = await _orderRepository.GetByIdWithRelatedEntities(orderId);

            if (orderResult.Value is Order order)
            {
                order.Privacy = privacy;


                var orderUserDetails = new OrderUserDetail
                {
                    Name = name,
                    Surname = surname,
                    Email = email,
                    NationId = nationId.Value,
                };
                order.OrderUserDetail = orderUserDetails;


                if (invoice)
                {

                    var orderInvoiceDetails = new OrderInvoice
                    {
                        PIVA = piva!,
                        CF = cf!
                    };
                    order.OrderInvoice = orderInvoiceDetails;

                }

                // Update the order
                await _orderRepository.Update(orderId, order);

                // If newsletter is true add the email to the newsletter list, already set it to active since all sites do that
                if (newsletter)
                {
                    var newsletterSubscription = new NewsletterSubscription
                    {
                        Email = email,
                        SubscriptionDate = DateTime.Now,
                        Active = true
                    };
                    await _newsletterSubscriptionRepository.CreateOrUpdateByEmail(email, newsletterSubscription);

                }

                return RedirectToAction("Index", "Home");
            }






            return RedirectToAction("Index");
        }



    }







}
