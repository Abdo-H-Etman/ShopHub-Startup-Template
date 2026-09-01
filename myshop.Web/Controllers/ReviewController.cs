using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using myshop.BLL.DTOs.Review;
using myshop.BLL.Services;
using myshop.Entities.Models;

namespace myshop.Web.Controllers;

[Authorize]
public class ReviewController : Controller
{
    private readonly IReviewService _reviewService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReviewController(IReviewService reviewService, UserManager<ApplicationUser> userManager)
    {
        _reviewService = reviewService;
        _userManager = userManager;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(CreateReviewDto model, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (model.Rating < 1 || model.Rating > 5 || string.IsNullOrWhiteSpace(model.Comment))
        {
            TempData["Error"] = "Please provide a valid rating (1-5 stars) and a comment.";
            return RedirectToAction("Details", "Product", new { id = model.ProductId });
        }

        try
        {
            await _reviewService.AddReviewAsync(user.Id, model, cancellationToken: cancellationToken);
            TempData["Success"] = "Thank you! Your review has been submitted.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Failed to submit review: " + ex.Message;
        }

        return RedirectToAction("Details", "Product", new { id = model.ProductId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateReviewDto model, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (model.Rating < 1 || model.Rating > 5 || string.IsNullOrWhiteSpace(model.Comment))
        {
            TempData["Error"] = "Please provide a valid rating (1-5 stars) and a comment.";
            return RedirectToAction("Details", "Product", new { id = model.ProductId });
        }

        try
        {
            await _reviewService.UpdateReviewAsync(user.Id, model, cancellationToken: cancellationToken);
            TempData["Success"] = "Your review has been updated.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Details", "Product", new { id = model.ProductId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int productId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var isDeleted = await _reviewService.DeleteReviewAsync(user.Id, id, User.IsInRole("Admin"), cancellationToken: cancellationToken);
            if (isDeleted)
            {
                TempData["Success"] = "Your review has been deleted.";
            }
            else
            {
                TempData["Error"] = "Review not found.";
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Details", "Product", new { id = productId });
    }
}
