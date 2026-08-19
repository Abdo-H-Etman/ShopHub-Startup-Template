using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using myshop.BLL.DTOs;
using myshop.BLL.DTOs.Product;
using myshop.BLL.Mapping;
using myshop.BLL.Services;
using myshop.Entities.Models;
using myshop.Entities.ViewModels;

namespace myshop.Web.Areas.Admin.Controllers;

[Authorize(Policy = "AdminOnly")]
public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IMapper _mapper;

    public ProductController(IProductService productService,
                ICategoryService categoryService,
                IMapper mapper)
    {
        _productService = productService;
        _categoryService = categoryService;
        _mapper = mapper;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetData()
    {
        var products = await _productService.GetAllAsync();


        return Json(new { data = products });
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var productVM = new ProductVM
        {
            Product = new Product(),
            CategoryList = await GetCategories()
        };
        return View(productVM);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductVM productVM, IFormFile? file)
    {
        if (!ModelState.IsValid)
        {
            productVM.CategoryList = await GetCategories();
            return View(productVM);
        }

        try
        {
            CreateProductDto createProduct = _mapper.Map<CreateProductDto>(productVM.Product);

            await _productService.CreateAsync(createProduct, file);

            TempData["Create"] = "Product has Created Successfully";

            return RedirectToAction("Index");
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("file", ex.Message);

            productVM.CategoryList = await GetCategories();

            return View(productVM);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var dto = await _productService.GetByIdForUpdateAsync(id.Value);

        if (dto == null)
            return NotFound();

        var productVM = new ProdcutEditVM
        {
            Id = id.Value,
            Product = dto,
            CategoryList = await GetCategories()
        };

        return View(productVM);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ProdcutEditVM productVM, IFormFile? file)
    {
        if (!ModelState.IsValid)
        {
            productVM.CategoryList = await GetCategories();
            return View(productVM);
        }

        try
        {
            await _productService.UpdateAsync(productVM.Product.Id, productVM.Product, file);

            TempData["Update"] = "Product has Updated Successfully";

            return RedirectToAction("Index");
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError("file", ex.Message);

            productVM.CategoryList = await GetCategories();

            return View(productVM);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var product = await _productService.GetByIdAsync(id.Value);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAjax(int? id)
    {
        if (id == null || id == 0)
        {
            return Json(new { success = false, message = "Invalid product id." });
        }

        try
        {
            await _productService.DeleteAsync(id.Value);
            return Json(new { success = true, message = "Product has been Deleted" });
        }
        catch (InvalidOperationException)
        {
            return Json(new { success = false, message = "Error while Deleting" });
        }
    }


    private async Task<IEnumerable<SelectListItem>> GetCategories()
    {
        var categories = await _categoryService.GetAllAsync();

        return categories.Select(x => new SelectListItem
        {
            Text = x.Name,
            Value = x.Id.ToString()
        });
    }
}