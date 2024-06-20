using Microsoft.AspNetCore.Mvc;
using YourNamespace.Models;

using YourNamespace.Data.Repositories;

namespace YourNamespace.Controllers
{
    public class UserInfoController : Controller
    {
        private readonly IUnitOfwork _unitOfWork;

        private readonly ProductRepository _productRepository;
        private readonly OrderRepository _orderRepository;
        private readonly OrderProductRepository _orderProductRepository;

        private readonly CouponRepository _couponRepository;

        private readonly NationRepository _nationRepository;

        public UserInfoController(IUnitOfwork unitOfwork)
        {
            _unitOfWork = unitOfwork;
            _productRepository = new ProductRepository(_unitOfWork);
            _orderProductRepository = new OrderProductRepository(_unitOfWork);
            _orderRepository = new OrderRepository(_unitOfWork);
            _couponRepository = new CouponRepository(_unitOfWork);
            _nationRepository = new NationRepository(_unitOfWork);
        }

        public async Task<IActionResult> Index()
        {

            // Redirect to Cart if no order or no products in order (should not happen)
            int orderId = HttpContext.Session.GetInt32("OrderId") ?? 0;
            if (orderId == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            var orderResult = await _orderRepository.GetByIdWithRelatedEntities(orderId);
            if (orderResult.Value is Order order)
            {
                if (order.OrderProducts.Count == 0)
                {
                    return RedirectToAction("Index", "Cart");
                }
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



    }







}
