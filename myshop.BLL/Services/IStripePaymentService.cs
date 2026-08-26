using myshop.BLL.Stripe;

namespace myshop.BLL.Services;

public interface IStripePaymentService
{
    Task<StripePaymentIntentResult> CreatePaymentIntentAsync(
        decimal amount,
        int userId,
        string? email);

    Task<StripePaymentIntentResult?> GetPaymentIntentAsync(
        string paymentIntentId);
}