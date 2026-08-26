using Microsoft.Extensions.Options;
using myshop.BLL.Stripe;
using Stripe;

namespace myshop.BLL.Services;

public class StripePaymentService : IStripePaymentService
{
    private readonly StripeSettings _settings;
    private readonly PaymentIntentService _paymentIntentService;

    public StripePaymentService(
        IOptions<StripeSettings> options)
    {
        _settings = options.Value;

        StripeConfiguration.ApiKey = _settings.SecretKey;

        _paymentIntentService = new PaymentIntentService();
    }

    public async Task<StripePaymentIntentResult> CreatePaymentIntentAsync(
        decimal amount,
        int userId,
        string? email)
    {
        if (amount <= 0)
        {
            throw new ArgumentException(
                "Payment amount must be greater than zero.",
                nameof(amount));
        }

        var amountInMinorUnits =
            (long)Math.Round(
                amount * 100m,
                MidpointRounding.AwayFromZero);

        var options = new PaymentIntentCreateOptions
        {
            Amount = amountInMinorUnits,
            Currency = _settings.Currency.ToLowerInvariant(),

            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true
            },

            Metadata = new Dictionary<string, string>
            {
                ["user_id"] = userId.ToString()
            }
        };

        if (!string.IsNullOrWhiteSpace(email))
        {
            options.ReceiptEmail = email;
        }

        var paymentIntent =
            await _paymentIntentService.CreateAsync(options);

        return Map(paymentIntent);
    }

    public async Task<StripePaymentIntentResult?> GetPaymentIntentAsync(
        string paymentIntentId)
    {
        if (string.IsNullOrWhiteSpace(paymentIntentId))
        {
            return null;
        }

        var paymentIntent =
            await _paymentIntentService.GetAsync(paymentIntentId);

        return paymentIntent == null
            ? null
            : Map(paymentIntent);
    }

    private static StripePaymentIntentResult Map(
        PaymentIntent paymentIntent)
    {
        return new StripePaymentIntentResult
        {
            Id = paymentIntent.Id,
            ClientSecret = paymentIntent.ClientSecret ?? string.Empty,
            Status = paymentIntent.Status ?? string.Empty,
            Amount = paymentIntent.Amount,
            Currency = paymentIntent.Currency ?? string.Empty,
            UserId = paymentIntent.Metadata.TryGetValue(
                        "user_id",
                        out var userId)
                        ? userId
                        : null
        };
    }
}