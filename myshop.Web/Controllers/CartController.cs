using Microsoft.AspNetCore.Mvc;
using myshop.BLL.Services;

namespace myshop.Web.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var cart = _cartService.GetCart();

        return View(cart);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId)
    {
        try
        {
            await _cartService.AddItemAsync(productId);

            TempData["Success"] = "Product added to cart.";

            return RedirectToAction("Index");
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;

            return RedirectToAction("Index", "Product");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int productId)
    {
        _cartService.RemoveItem(productId);

        if (IsAjaxRequest())
        {
            var cart = _cartService.GetCart();
            var cartTotal = _cartService.GetCartTotal();
            return Json(new
            {
                success = true,
                productId,
                cartTotal = cartTotal.ToString("C"),
                cartItemCount = cart.Sum(x => x.Quantity),
                isEmpty = !cart.Any()
            });
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Increase(int productId)
    {
        _cartService.IncreaseQuantity(productId);

        if (IsAjaxRequest())
        {
            var cart = _cartService.GetCart();
            var item = cart.FirstOrDefault(x => x.ProductId == productId);
            var cartTotal = _cartService.GetCartTotal();
            return Json(new
            {
                success = true,
                productId,
                quantity = item?.Quantity ?? 0,
                itemTotal = item?.TotalPrice.ToString("C") ?? "$0.00",
                cartTotal = cartTotal.ToString("C"),
                cartItemCount = cart.Sum(x => x.Quantity),
                isRemoved = false,
                isEmpty = !cart.Any()
            });
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Decrease(int productId)
    {
        _cartService.DecreaseQuantity(productId);

        if (IsAjaxRequest())
        {
            var cart = _cartService.GetCart();
            var item = cart.FirstOrDefault(x => x.ProductId == productId);
            var cartTotal = _cartService.GetCartTotal();
            return Json(new
            {
                success = true,
                productId,
                quantity = item?.Quantity ?? 0,
                itemTotal = item?.TotalPrice.ToString("C") ?? "$0.00",
                cartTotal = cartTotal.ToString("C"),
                cartItemCount = cart.Sum(x => x.Quantity),
                isRemoved = item is null,
                isEmpty = !cart.Any()
            });
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Clear()
    {
        _cartService.ClearCart();

        return RedirectToAction(nameof(Index));
    }

    private bool IsAjaxRequest()
    {
        return Request.Headers["X-Requested-With"] == "XMLHttpRequest" ||
               Request.Headers.Accept.ToString().Contains("application/json");
    }
}