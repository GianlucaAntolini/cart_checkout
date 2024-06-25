using Microsoft.AspNetCore.Mvc;
using YourNamespace.Models;

using YourNamespace.Data.Repositories;
using Microsoft.AspNetCore.Identity;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class CartController : Controller
    {
        private readonly IUnitOfwork _unitOfWork;

        private readonly ProductRepository _productRepository;
        private readonly OrderRepository _orderRepository;
        private readonly OrderProductRepository _orderProductRepository;

        private readonly CouponRepository _couponRepository;

        private readonly UserOrderRepository _userOrderRepository;

        private readonly OrderPaymentRepository _orderPaymentRepository;


        private readonly SignInManager<User> _signInManager;

        private readonly UserManager<User> _userManager;


        public CartController(IUnitOfwork unitOfwork, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _unitOfWork = unitOfwork;
            _productRepository = new ProductRepository(_unitOfWork);
            _orderProductRepository = new OrderProductRepository(_unitOfWork);
            _orderRepository = new OrderRepository(_unitOfWork);
            _couponRepository = new CouponRepository(_unitOfWork);
            _userOrderRepository = new UserOrderRepository(_unitOfWork);
            _orderPaymentRepository = new OrderPaymentRepository(_unitOfWork);
            _signInManager = signInManager;
            _userManager = userManager;

        }

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]

        public async Task<IActionResult> Index()
        {
            // if user is not logged in redirect to the login page (use signInManager.IsSignedIn(User) to check if the user is logged in)
            if (_signInManager.IsSignedIn(User) == false)
            {
                return RedirectToAction("Login", "Account");
            }


            // Retrieve all products to display in the dropdown menu
            var productsResult = await _productRepository.GetAvailableProducts();
            var products = new List<Product>();
            if (productsResult.Value != null)
            {
                products = productsResult.Value.ToList();
            }
            Order? order = null;

            // Check last order of the user in the database
            var orderFound = false;
            int orderId = 0;

            var latestOrder = await _userOrderRepository.GetLatestOrderOfUserByUserId(_userManager.GetUserId(User));
            if (latestOrder.Value is UserOrder foundUserOrder)
            {
                // Check if the order has not been paid yet
                var hasOrderBeenPaid = await _orderPaymentRepository.HasOrderBeenPaidByOrderId(foundUserOrder.OrderId);
                if (hasOrderBeenPaid.Value == false)
                {
                    orderId = foundUserOrder.OrderId;
                    orderFound = true;
                }
            }


            // First check if in the session there is an order id, if there is fetch the order from the database, else create new and save the id in the session
            if (orderFound)
            {

                var orderResult = await _orderRepository.GetByIdWithRelatedEntities(orderId);
                // If the order is found put it in the order variable (the order is inside orderResult->Result->Value)
                if (orderResult.Value is Order queryOrder)
                {
                    order = queryOrder;
                    // set the order id in the session
                    HttpContext.Session.SetInt32("OrderId", order.Id);
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

                // insert the tuple in the userorder table
                var UserOrder = new UserOrder
                {
                    OrderId = orderId,
                    UserId = _userManager.GetUserId(User)
                };

                await _userOrderRepository.Create(UserOrder);



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
                        PriceWithCoupon = product.Price,
                        Product = product
                    });

                    order.TotalAmount += product.Price;
                    order.TotalAmountWithCoupon += product.Price;
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



        [HttpPost("RemoveProduct")]
        public async Task<IActionResult> RemoveProduct([FromBody] RemoveProductRequest request)
        {
            // Validate input
            if (request.OrderProductId <= 0)
            {
                return BadRequest("Invalid OrderProductId");
            }

            // Get the order id from the session else return bad request
            int orderId = HttpContext.Session.GetInt32("OrderId") ?? 0;
            if (orderId == 0)
            {
                return BadRequest("Order not found in session");
            }

            var orderResult = await _orderRepository.GetByIdWithRelatedEntities(orderId);
            if (orderResult.Value is Order order)
            {
                var orderProduct = order.OrderProducts.FirstOrDefault(op => op.Id == request.OrderProductId);
                if (orderProduct != null)
                {
                    order.OrderProducts.Remove(orderProduct);
                    order.TotalAmount -= orderProduct.Price * orderProduct.Quantity;
                    order.TotalAmountWithCoupon -= orderProduct.Price * orderProduct.Quantity;
                    await UpdateOrderPriceAsync(order, null);

                    // Return success response
                    return Ok(new { message = "Product removed successfully", order });
                }
                else
                {
                    return NotFound("OrderProduct not found");
                }
            }
            else
            {
                return NotFound("Order not found");
            }
        }

        public class RemoveProductRequest
        {
            public int OrderProductId { get; set; }
        }




        [HttpPost("IncreaseProductQuantity")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> IncreaseProductQuantity()

        {
            // Get orderProductId from the form
            int orderProductId = int.TryParse(Request.Form["orderProductId"], out int result) ? result : 0;

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
                    // If the user is trying to be sneaky and increase the quantity of a product that is already the stock quantity, just return to the index
                    if (orderProduct.Quantity == orderProduct.Product.StockQuantity)
                    {
                        return RedirectToAction("Index");
                    }
                    orderProduct.Quantity++;
                    orderProduct.Price = orderProduct.Product.Price * orderProduct.Quantity;
                    order.TotalAmount += orderProduct.Price;
                    order.TotalAmountWithCoupon += orderProduct.Price;
                    await UpdateOrderPriceAsync(order, null);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost("DecreaseProductQuantity")]
        [ApiExplorerSettings(IgnoreApi = true)]

        public async Task<IActionResult> DecreaseProductQuantity()
        {
            // Get orderProductId from the form
            int orderProductId = int.TryParse(Request.Form["orderProductId"], out int result) ? result : 0;
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
                    // If the user is trying to be sneaky and decrease the quantity of a product that is already 1, just return to the index
                    if (orderProduct.Quantity == 1)
                    {
                        return RedirectToAction("Index");
                    }
                    orderProduct.Quantity--;
                    orderProduct.Price = orderProduct.Product.Price * orderProduct.Quantity;
                    order.TotalAmount -= orderProduct.Price;
                    order.TotalAmountWithCoupon -= orderProduct.Price;
                    await UpdateOrderPriceAsync(order, null);
                }
            }

            return RedirectToAction("Index");
        }

        private void resetOrderPrice(Order order)
        {
            // Reset the order price and remove the coupon
            foreach (var orderProduct in order.OrderProducts)
            {
                orderProduct.PriceWithCoupon = orderProduct.Price;
            }
            order.TotalAmountWithCoupon = order.TotalAmount;

            order.CouponId = null;

        }

        // Function to calculate amount of the order (considering the coupon if present)
        private async Task<IActionResult> UpdateOrderPriceAsync(Order order, Coupon? paramCoupon)
        {
            // If coupon is null take it from the order, if still null reset all prices
            // This is done becuse:
            // - if the user is trying to apply a coupon, the coupon is passed as a parameter
            // - if the user is trying to increase/decrease the quantity of a product, the coupon is taken from the order
            // - also if he 

            var toApplyCoupon = paramCoupon ?? order.Coupon;
            // If the coupon is active and it applies to at least one of the products in the order, apply the discount to the order
            if (toApplyCoupon != null && toApplyCoupon.Active && toApplyCoupon.CouponProducts != null && order.OrderProducts.Any(op => toApplyCoupon.CouponProducts.Any(cp => cp.ProductId == op.ProductId)))
            {
                var applied = false;
                order.CouponId = toApplyCoupon.Id;
                order.Coupon = toApplyCoupon;
                // How to calculate the discount:
                // For each orderroduct in the order:
                //         SKIP IF:
                //         - product is not discountable by the coupon skip
                //         - orderproduct price is less than the minprice of the coupn
                //         APPLY DISCOUNT (only if not all products are skipped):
                //         - create a cap variable toDiscountPrice that is the minimum of the maxprice of the coupon and the orderproduct price
                //         - apply the Coupon.DiscountPercentage to the toDiscountPrice
                //         - set the orderproduct priceWithCoupon to the toDiscountPrice
                //         - set the order totalAmountWithCoupon to the sum of all orderProducts priceWithCoupon
                //         - set the coupon active to false (TODO: maybe coupon are one time use only)


                //TODO: chiedere a Roberto, forse ho pensato min e max del coupon in modo sbagliato:
                // io ho pensato così: 
                //esempio coupon sconto 10% min 20 max 100
                // se il prezzo del prodotto*quantita è 10 non posso applicare il coupon
                // se il prezzo è 50 allora sconto del 10% su 50 quindi di 5 quindi prezzo finale 45
                // se il prezzo è 200 allora sconto del 10% su 100 quindi di 10 quindi prezzo finale 190 
                foreach (var orderProduct in order.OrderProducts)
                {
                    if (toApplyCoupon.CouponProducts.Any(cp => cp.ProductId == orderProduct.ProductId) && orderProduct.Price >= toApplyCoupon.MinPrice)
                    {
                        var toDiscountPrice = Math.Min(toApplyCoupon.MaxPrice, orderProduct.Price);
                        var newPrice = orderProduct.Price - toDiscountPrice * toApplyCoupon.PercentageDiscount / 100;
                        // If the price is actually different set applied to true
                        if (newPrice != orderProduct.Price)
                        {
                            applied = true;
                        }
                        orderProduct.PriceWithCoupon = newPrice;
                    }
                    else
                    {
                        // Reset just the price with coupon of this product
                        orderProduct.PriceWithCoupon = orderProduct.Price;
                    }

                }
                // If at least one product was appliable set the total amount with coupon to the sum of all orderproducts priceWithCoupon
                if (applied)
                {
                    order.TotalAmount = order.OrderProducts.Sum(op => op.Price);
                    order.TotalAmountWithCoupon = order.OrderProducts.Sum(op => op.PriceWithCoupon);
                }
                else
                {
                    resetOrderPrice(order);
                }
            }
            else
            {
                // Else reset the prices
                resetOrderPrice(order);
            }



            return await _orderRepository.Update(order.Id, order);

        }


        [HttpPost("ApplyCoupon")]
        [ApiExplorerSettings(IgnoreApi = true)]


        public async Task<IActionResult> ApplyCoupon()
        {
            // Get the coupon code from the form
            string? couponCode = Request.Form["couponCode"];
            if (couponCode == null || couponCode == "")
            {
                return RedirectToAction("Index");
            }

            // Get the order id from the session else redirect to the index to create a new order
            int orderId = HttpContext.Session.GetInt32("OrderId") ?? 0;
            if (orderId == 0)
            {
                return RedirectToAction("Index");
            }

            var orderResult = await _orderRepository.GetByIdWithRelatedEntities(orderId);
            if (orderResult.Value is Order order)
            {
                // Check if the coupon code is valid
                var coupon = await _couponRepository.GetByCodeWithProducts(couponCode);
                await UpdateOrderPriceAsync(order, coupon.Value is Coupon validCoupon ? validCoupon : null);
            }

            return RedirectToAction("Index");


        }


        [HttpPost("RemoveCoupon")]
        [ApiExplorerSettings(IgnoreApi = true)]

        public async Task<IActionResult> RemoveCoupon()
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
                resetOrderPrice(order);
                await _orderRepository.Update(orderId, order);
            }

            return RedirectToAction("Index");
        }

        [HttpPost("UserInfo")]
        [ApiExplorerSettings(IgnoreApi = true)]

        public async Task<IActionResult> UserInfoAsync()
        {
            // Redirect to the UserInfo page only if there is at least one product in the order
            int orderId = HttpContext.Session.GetInt32("OrderId") ?? 0;
            if (orderId == 0)
            {
                return RedirectToAction("Index");
            }

            var orderResult = await _orderRepository.GetByIdWithRelatedEntities(orderId);
            if (orderResult.Value is Order order)
            {
                if (order.OrderProducts.Count == 0)
                {
                    return RedirectToAction("Index");
                }
            }


            return RedirectToAction("Index", "UserInfo");
        }

        [HttpPost("CreateOrder")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> CreateOrder()
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
                // Add 1 of each available product to the order
                var productsResult = await _productRepository.GetAvailableProducts();
                var products = new List<Product>();
                if (productsResult.Value != null)
                {
                    products = productsResult.Value.ToList();
                }
                foreach (var product in products)
                {
                    order.OrderProducts.Add(new OrderProduct
                    {
                        ProductId = product.Id,
                        Quantity = 1,
                        Price = product.Price,
                        PriceWithCoupon = product.Price,
                        Product = product
                    });

                    order.TotalAmount += product.Price;
                    order.TotalAmountWithCoupon += product.Price;
                }

                // Update the order in the database
                await _orderRepository.Update(orderId, order);
            }

            return RedirectToAction("Index");






        }
    }







}
