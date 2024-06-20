using Microsoft.AspNetCore.Mvc;
using YourNamespace.Models;

using YourNamespace.Data.Repositories;

namespace YourNamespace.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IUnitOfwork _unitOfWork;

        private readonly OrderRepository _orderRepository;


        private readonly NationRepository _nationRepository;

        private readonly NewsletterSubscriptionRepository _newsletterSubscriptionRepository;

        public CheckoutController(IUnitOfwork unitOfwork)
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





            return RedirectToAction("Index");
        }



    }







}
