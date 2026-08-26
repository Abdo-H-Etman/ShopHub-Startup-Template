using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using myshop.BLL.DTOs.Order;
using myshop.BLL.Interfaces;
using myshop.BLL.Services;
using myshop.Entities.Models;
using myshop.Web.ViewModels;

namespace myshop.Web.Controllers;

[Authorize]
public class OrderController : Controller
{
    private readonly IOrderService _orderService;
    private readonly ICartService _cartService;
    private readonly IEmailService _emailService;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrderController(
        IOrderService orderService,
        ICartService cartService,
        IEmailService emailService,
        UserManager<ApplicationUser> userManager)
    {
        _orderService = orderService;
        _cartService = cartService;
        _emailService = emailService;
        _userManager = userManager;
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
            CardHolderName = user.Name ?? user.UserName ?? string.Empty,
            CartItems = cart,
            OrderTotal = _cartService.GetCartTotal()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutVM vm)
    {
        var cart = _cartService.GetCart();
        if (cart == null || !cart.Any())
        {
            TempData["Error"] = "Your cart is empty.";
            return RedirectToAction("Index", "Cart");
        }

        if (!ModelState.IsValid)
        {
            vm.CartItems = cart;
            vm.OrderTotal = _cartService.GetCartTotal();
            return View(vm);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var orderDto = new OrderCreateDto
        {
            ApplicationUserId = user.Id,
            Name = vm.Name.Trim(),
            Address = vm.Address.Trim(),
            City = vm.City.Trim(),
            PostalCode = vm.PostalCode?.Trim(),
            PhoneNumber = vm.PhoneNumber.Trim(),
            CustomerEmail = user.Email ?? string.Empty
        };

        var placedOrder = await _orderService.CreateOrderAsync(user.Id, orderDto, cart);

        // Send confirmation email
        if (!string.IsNullOrEmpty(user.Email))
        {
            await _emailService.SendOrderConfirmationEmailAsync(user.Email, user.Name, placedOrder.Id, placedOrder.TotalPrice);
        }

        // Clear cart
        _cartService.ClearCart();

        TempData["Success"] = $"Order #{placedOrder.Id} placed successfully!";
        return RedirectToAction(nameof(OrderConfirmation), new { id = placedOrder.Id });
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
}
