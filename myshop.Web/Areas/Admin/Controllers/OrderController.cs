using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.BLL.Services;

namespace myshop.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "AdminOnly")]
public class OrderController : Controller
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    // GET: /Admin/Order/Index  — All orders list
    public async Task<IActionResult> Index()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return View(orders);
    }

    // GET: /Admin/Order/Details/{id}
    public async Task<IActionResult> Details(int id)
    {
        var order = await _orderService.GetOrderDetailsAsync(id);
        if (order == null)
            return NotFound();

        return View(order);
    }

    // POST: /Admin/Order/UpdateStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int orderId, string orderStatus, string? paymentStatus, string? trackingNumber, string? carrier)
    {
        var success = await _orderService.UpdateOrderStatusAsync(orderId, orderStatus, paymentStatus);
        if (success)
            TempData["Success"] = $"Order #{orderId} status updated successfully.";
        else
            TempData["Error"] = "Failed to update order status.";

        return RedirectToAction(nameof(Details), new { id = orderId });
    }

    // GET: /Admin/Order/GetAllOrdersData  — JSON for DataTable
    [HttpGet]
    public async Task<IActionResult> GetAllOrdersData()
    {
        var orders = await _orderService.GetAllOrdersAsync();
        return Json(new { data = orders });
    }
}
