using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using myshop.BLL.DTOs.Order;
using myshop.BLL.Interfaces;
using myshop.BLL.Services;
using myshop.Entities.Models;
using myshop.Web.ViewModels;
using myshop.BLL.Stripe;

namespace myshop.Web.Controllers;

[Authorize]
public class OrderController : Controller
{
    private readonly IOrderService _orderService;
    private readonly ICartService _cartService;
    private readonly IEmailService _emailService;
    private readonly IStripePaymentService _stripePaymentService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly StripeSettings _stripeSettings;

    public OrderController(
        IOrderService orderService,
        ICartService cartService,
        IEmailService emailService,
        IStripePaymentService stripePaymentService,
        UserManager<ApplicationUser> userManager,
        IOptions<StripeSettings> stripeOptions)
    {
        _orderService = orderService;
        _cartService = cartService;
        _emailService = emailService;
        _stripePaymentService = stripePaymentService;
        _userManager = userManager;
        _stripeSettings = stripeOptions.Value;
    }

    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        var cart = _cartService.GetCart();
        if (cart == null || !cart.Any())
        {
            TempData["Error"] = "Your cart is empty. Please add some products before checking out.";
            return RedirectToAction("Index", "Cart");
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var vm = new CheckoutVM
        {
            Name = user.Name ?? user.UserName ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            Address = user.Address ?? string.Empty,
            City = user.City ?? string.Empty,
            CartItems = cart,
            OrderTotal = _cartService.GetCartTotal()
        };

        ViewBag.StripePublishableKey =
            _stripeSettings.PublishableKey;
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> OrderConfirmation(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var order = await _orderService.GetOrderDetailsAsync(id, user.Id);
        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    [HttpGet]
    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string sort = "datedesc")
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var orders = await _orderService.GetPagedUserOrdersAsync(user.Id, pageNumber, pageSize, sort);

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return PartialView("_OrderList", orders);
        }

        return View(orders);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var order = await _orderService.GetOrderDetailsAsync(id, user.Id);
        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePaymentIntent()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Unauthorized();
        }

        var cart = _cartService.GetCart();

        if (cart == null || !cart.Any())
        {
            return BadRequest(new
            {
                message = "Your cart is empty."
            });
        }

        var total = _cartService.GetCartTotal();

        if (total <= 0)
        {
            return BadRequest(new
            {
                message = "Invalid order total."
            });
        }

        try
        {
            var paymentIntent =
                await _stripePaymentService.CreatePaymentIntentAsync(
                    total,
                    user.Id,
                    user.Email);

            return Json(new
            {
                clientSecret = paymentIntent.ClientSecret,
                paymentIntentId = paymentIntent.Id
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new
            {
                message = "Unable to initialize payment."
            });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> FinalizePayment(
    [FromBody] FinalizePaymentRequest request)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Unauthorized();
        }

        var existingOrder =
        await _orderService.GetOrderByPaymentIntentIdAsync(request.PaymentIntentId, user.Id);

        if (existingOrder != null)
        {
            return Json(new
            {
                success = true,
                orderId = existingOrder.Id
            });
        }

        if (string.IsNullOrWhiteSpace(request.PaymentIntentId))
        {
            return BadRequest(new
            {
                message = "Payment information is missing."
            });
        }

        var paymentIntent =
            await _stripePaymentService.GetPaymentIntentAsync(
                request.PaymentIntentId);

        if (paymentIntent == null)
        {
            return BadRequest(new
            {
                message = "Payment could not be found."
            });
        }

        if (paymentIntent.UserId != user.Id.ToString())
        {
            return Unauthorized();
        }

        if (!string.Equals(
                paymentIntent.Status,
                "succeeded",
                StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new
            {
                message = $"Payment has not succeeded. Current status: {paymentIntent.Status}"
            });
        }

        var cart = _cartService.GetCart();

        if (cart == null || !cart.Any())
        {
            return BadRequest(new
            {
                message = "Your cart is empty."
            });
        }

        var expectedTotal = _cartService.GetCartTotal();

        var expectedAmount =
            (long)Math.Round(
                expectedTotal * 100m,
                MidpointRounding.AwayFromZero);

        if (paymentIntent.Amount != expectedAmount)
        {
            return BadRequest(new
            {
                message =
                    "The cart total changed while payment was being processed."
            });
        }

        var orderDto = new OrderCreateDto
        {
            ApplicationUserId = user.Id,

            Name = request.Name.Trim(),
            Address = request.Address.Trim(),
            City = request.City.Trim(),
            PostalCode = request.PostalCode?.Trim(),
            PhoneNumber = request.PhoneNumber?.Trim(),

            CustomerEmail = user.Email ?? string.Empty,

            PaymentIntentId = paymentIntent.Id
        };

        try
        {
            var placedOrder =
                await _orderService.CreateOrderAsync(
                    user.Id,
                    orderDto,
                    cart);

            // Email ONLY after successful Stripe payment
            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                await _emailService.SendOrderConfirmationEmailAsync(
                    user.Email,
                    user.Name,
                    placedOrder.Id,
                    placedOrder.TotalPrice);
            }

            _cartService.ClearCart();

            return Json(new
            {
                success = true,
                orderId = placedOrder.Id
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new
            {
                message = "Payment succeeded, but the order could not be finalized."
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> PaymentComplete(
    string? payment_intent)
    {
        if (string.IsNullOrWhiteSpace(payment_intent))
        {
            TempData["Error"] = "Payment information was not provided.";
            return RedirectToAction(nameof(Checkout));
        }

        var payment =
            await _stripePaymentService.GetPaymentIntentAsync(
                payment_intent);

        if (payment == null ||
            !string.Equals(
                payment.Status,
                "succeeded",
                StringComparison.OrdinalIgnoreCase))
        {
            TempData["Error"] =
                "Payment was not completed.";

            return RedirectToAction(nameof(Checkout));
        }

        TempData["PaymentIntentId"] = payment_intent;

        return RedirectToAction(nameof(Checkout));
    }
    public class FinalizePaymentRequest
    {
        public string PaymentIntentId { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string? PostalCode { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
