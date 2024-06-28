using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public class PayPalService
{
    private readonly IConfiguration _configuration;

    public PayPalService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private PayPalEnvironment GetPayPalEnvironment()
    {
        string clientId = _configuration["PayPal:ClientId"];
        string clientSecret = _configuration["PayPal:ClientSecret"];
        string environment = _configuration["PayPal:Environment"];

        return environment == "live" ? new LiveEnvironment(clientId, clientSecret) : new SandboxEnvironment(clientId, clientSecret);
    }

    private PayPalHttpClient GetPayPalClient()
    {
        return new PayPalHttpClient(GetPayPalEnvironment());
    }

    public async Task<Order> CreateOrderAsync(decimal amount, string currency)
    {
        var request = new OrdersCreateRequest();
        request.Prefer("return=representation");
        request.RequestBody(new OrderRequest
        {
            CheckoutPaymentIntent = "CAPTURE",
            PurchaseUnits = new List<PurchaseUnitRequest>
            {
                new PurchaseUnitRequest
                {
                    AmountWithBreakdown = new AmountWithBreakdown
                    {
                        CurrencyCode = currency,
                        Value = amount.ToString("F2")
                    }
                }
            },
            ApplicationContext = new ApplicationContext
            {
                ReturnUrl = "http://localhost:5198/Checkout/Success",
                CancelUrl = "http://localhost:5198/Checkout/Error"
            }
        });

        var response = await GetPayPalClient().Execute(request);
        return response.Result<Order>();
    }

    public async Task<Order> CaptureOrderAsync(string orderId)
    {
        var request = new OrdersCaptureRequest(orderId);
        request.RequestBody(new OrderActionRequest());
        var response = await GetPayPalClient().Execute(request);
        return response.Result<Order>();
    }
}
