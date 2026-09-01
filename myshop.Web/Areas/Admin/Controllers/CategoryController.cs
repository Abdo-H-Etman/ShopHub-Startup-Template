using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myshop.BLL.DTOs.Category;
using myshop.BLL.Services;

namespace myshop.Web.Controllers;

[Area("Admin")]
[Authorize(Policy = "AdminOnly")]
public class CategoryController : Controller
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryService.GetAllAsync(cancellationToken: cancellationToken);
        return View(categories.ToList());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateCategoryDto());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryDto category, CancellationToken cancellationToken = default)
    {
        if (ModelState.IsValid)
        {
            await _categoryService.CreateAsync(category, cancellationToken: cancellationToken);
            TempData["Create"] = "Item has Created Successfully";
            return RedirectToAction("Index");
        }
        return View(category);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken = default)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var category = await _categoryService.GetByIdForUpdateAsync(id.Value, cancellationToken: cancellationToken);
        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UpdateCategoryDto category, CancellationToken cancellationToken = default)
    {
        if (ModelState.IsValid)
        {
            await _categoryService.UpdateAsync(category, cancellationToken: cancellationToken);
            TempData["Update"] = "Data has Updated Successfully";
            return RedirectToAction("Index");
        }
        return View(category);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int? id, CancellationToken cancellationToken = default)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var category = await _categoryService.GetByIdAsync(id.Value, cancellationToken: cancellationToken);
        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAjax(int? id, CancellationToken cancellationToken = default)
    {
        if (id == null || id == 0)
        {
            return Json(new { success = false, message = "Invalid category id." });
        }

        var category = await _categoryService.GetByIdAsync(id.Value, cancellationToken: cancellationToken);
        if (category == null)
        {
            return Json(new { success = false, message = "Category not found." });
        }

        await _categoryService.DeleteAsync(id.Value, cancellationToken: cancellationToken);
        return Json(new { success = true, message = "Category deleted successfully." });
    }
}
