using Microsoft.AspNetCore.Mvc;
using YourNamespace.Models;

using YourNamespace.Data.Repositories;

namespace YourNamespace.Controllers
{
    public class CartController : Controller
    {
        private readonly IUnitOfwork _unitOfWork;

        private readonly ProductRepository _productRepository;
        private readonly OrderRepository _orderRepository;
        private readonly OrderProductRepository _orderProductRepository;

        public CartController(IUnitOfwork unitOfwork)
        {
            _unitOfWork = unitOfwork;
            _productRepository = new ProductRepository(_unitOfWork);
            _orderProductRepository = new OrderProductRepository(_unitOfWork);
            _orderRepository = new OrderRepository(_unitOfWork);
        }

        // Action method for displaying the cart page
        public async Task<IActionResult> Index()
        {
                    Console.WriteLine("CART PAGE!!!!");

            // Create a new empty order and pass it to the view
            var order = new Order
            {
                // Initialize any necessary properties
                OrderPayment = new OrderPayment(),
                Coupon = new Coupon(),
                CouponId = null,
                OrderInvoice = new OrderInvoice(),
                OrderProducts = new List<OrderProduct>(),
                Privacy = false,
                TermsAndConditions = false,
                OrderTimestamp = DateTime.Now,
                TotalAmount = 0,
                TotalAmountWithCoupon = 0,
                OrderUserDetail = new OrderUserDetail()

            };

            // Retrieve all products to display in the dropdown menu
    var productsResult = await _productRepository.Get();
  //  var products = productsResult.Value;
  //  if (productsResult.Result is NotFoundResult || products == null)
  //  {
  //      products = []; 
  //  } 
//
  //   foreach (var product in products)
  //  {
  //      Console.WriteLine($"Product Id: {product.Id}, Name: {product.Name}");
  //  }

  var products = new List<Product>();
  products.Add(new Product { Id = 1, Name = "Product 1", Price = 10 });
    products.Add(new Product { Id = 2, Name = "Product 2", Price = 20 });
 

    // Pass products to the view
    ViewBag.Products = products;

    return View();
        }


    }
}
