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

        public async Task<IActionResult> Index()
        {


            // Retrieve all products to display in the dropdown menu
            var productsResult = await _productRepository.GetAvailableProducts();
            var products = new List<Product>();
            if (productsResult.Value != null)
            {
                products = productsResult.Value.ToList();
            }
            var order = new Order();

            // First check if in the session there is an order id, if there is fetch the order from the database, else create new and save the id in the session
            int orderId = 0;
            if (HttpContext.Session.GetInt32("OrderId") != null)
            {
                orderId = (int)HttpContext.Session.GetInt32("OrderId");
                var orderResult = await _orderRepository.GetByIdWithRelatedEntities(orderId);
                // If the order is found put it in the order variable (the order is inside orderResult->Result->Value)
                if (orderResult.Value is Order foundOrder)
                {
                    order = foundOrder;
                }
                // write to console orderResult.toString()
                Console.WriteLine(orderResult.ToString());

            }
            else
            {



                // Create a new empty order and pass it to the view
                order = new Order
                {
                    // Initialize any necessary properties
                    CouponId = null,
                    Privacy = false,
                    TermsAndConditions = false,
                    OrderTimestamp = DateTime.Now,
                    TotalAmount = 0,
                    TotalAmountWithCoupon = 0,



                };

                // Get the just created order id
                var idResult = await _orderRepository.GetFirstAvailableId();

                if (idResult.Result is OkObjectResult okResult && okResult.Value is int id)
                {
                    orderId = id;
                }





                // save the order in the database
                await _orderRepository.Create(order);


                // for each product add one order product to the order
                order.Id = orderId;
                order.OrderProducts = new List<OrderProduct>();
                foreach (var product in products)
                {
                    order.OrderProducts.Add(new OrderProduct
                    {
                        ProductId = orderId,
                        Quantity = 1,
                        Price = product.Price,
                        Product = product
                    });

                    order.TotalAmount += product.Price;
                    order.TotalAmountWithCoupon += product.Price;
                    product.StockQuantity--;
                }

                // update the order in the database
                await _orderRepository.Update(orderId, order);



                // save the order id in the session
                HttpContext.Session.SetInt32("OrderId", order.Id);
            }





            // Pass products to the view
            ViewBag.Products = products;
            // Pass the order to the view
            ViewBag.Order = order;

            return View();
        }



        [HttpPost]
        public async Task<IActionResult> RemoveProduct(int orderProductId)
        {
            // Get the order id from the session else redirect to the index to create a new order
            int orderId = HttpContext.Session.GetInt32("OrderId") ?? 0;
            if (orderId == 0)
            {
                return RedirectToAction("Index");
            }

            var orderResult = await _orderRepository.GetByIdWithRelatedEntities(orderId);
            if (orderResult.Value is Order order)
            {
                var orderProduct = order.OrderProducts.FirstOrDefault(op => op.Id == orderProductId);
                if (orderProduct != null)
                {
                    order.OrderProducts.Remove(orderProduct);
                    order.TotalAmount -= orderProduct.Price * orderProduct.Quantity;
                    order.TotalAmountWithCoupon -= orderProduct.Price * orderProduct.Quantity;
                    await _orderRepository.Update(orderId, order);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> IncreaseProductQuantity(int orderProductId)
        {
            // Get the order id from the session else redirect to the index to create a new order
            int orderId = HttpContext.Session.GetInt32("OrderId") ?? 0;
            if (orderId == 0)
            {
                return RedirectToAction("Index");
            }

            var orderResult = await _orderRepository.GetByIdWithRelatedEntities(orderId);
            if (orderResult.Value is Order order)
            {
                var orderProduct = order.OrderProducts.FirstOrDefault(op => op.Id == orderProductId);
                if (orderProduct != null)
                {
                    orderProduct.Quantity++;
                    order.TotalAmount += orderProduct.Price;
                    order.TotalAmountWithCoupon += orderProduct.Price;
                    await _orderRepository.Update(orderId, order);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DecreaseProductQuantity(int orderProductId)
        {
            // Get the order id from the session else redirect to the index to create a new order
            int orderId = HttpContext.Session.GetInt32("OrderId") ?? 0;
            if (orderId == 0)
            {
                return RedirectToAction("Index");
            }

            var orderResult = await _orderRepository.GetByIdWithRelatedEntities(orderId);
            if (orderResult.Value is Order order)
            {
                var orderProduct = order.OrderProducts.FirstOrDefault(op => op.Id == orderProductId);
                if (orderProduct != null)
                {
                    orderProduct.Quantity--;
                    order.TotalAmount -= orderProduct.Price;
                    order.TotalAmountWithCoupon -= orderProduct.Price;
                    await _orderRepository.Update(orderId, order);
                }
            }

            return RedirectToAction("Index");
        }



    }


}
