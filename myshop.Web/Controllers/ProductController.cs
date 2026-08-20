using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.BLL.Services;
using myshop.Entities.ViewModels;

namespace myshop.Web.Controllers;

[AllowAnonymous]
public class ProductController : Controller
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
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
}