using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using myshop.BLL.DTOs.Review;
using myshop.BLL.Services;
using myshop.Entities.Models;
using myshop.Entities.ViewModels;
using myshop.Web.ViewModels;

namespace myshop.Web.Controllers;

[AllowAnonymous]
public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly IReviewService _reviewService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProductController(
        IProductService productService,
        IReviewService reviewService,
        UserManager<ApplicationUser> userManager)
    {
        _productService = productService;
        _reviewService = reviewService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        int pageNumber = 1,
        int pageSize = 8,
        string? search = null,
        string? sort = "nameasc")
    {
        var result = await _productService.GetPagedAsync(
            pageNumber,
            pageSize,
            search,
            sort);

        var vm = new ProductIndexVM
        {
            Products = result.Items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = result.TotalCount,
            Search = search,
            Sort = sort
        };

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Query.ContainsKey("ajax"))
        {
            return PartialView("_ProductListPartial", vm);
        }

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        int? currentUserId = null;
        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                currentUserId = user.Id;
            }
        }

        var ratingSummary = await _reviewService.GetProductRatingSummaryAsync(id);
        var reviews = (await _reviewService.GetProductReviewsAsync(id, currentUserId)).ToList();

        ReviewDto? userReview = null;
        if (currentUserId.HasValue)
        {
            userReview = await _reviewService.GetUserReviewForProductAsync(id, currentUserId.Value);
        }

        var vm = new ProductDetailsVM
        {
            Product = product,
            RatingSummary = ratingSummary,
            Reviews = reviews,
            UserReview = userReview,
            NewReview = new CreateReviewDto { ProductId = id },
            IsAuthenticated = User.Identity?.IsAuthenticated == true
        };

        return View(vm);
    }
}