using Microsoft.AspNetCore.Mvc;
using myshop.BLL.DTOs.Category;
using myshop.BLL.Services;

namespace myshop.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();
            return View(categories.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateCategoryDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryDto category)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.CreateAsync(category);
                TempData["Create"] = "Item has Created Successfully";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var category = await _categoryService.GetByIdForUpdateAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateCategoryDto category)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.UpdateAsync(category);
                TempData["Update"] = "Data has Updated Successfully";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var category = await _categoryService.GetByIdAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAjax(int? id)
        {
            if (id == null || id == 0)
            {
                return Json(new { success = false, message = "Invalid category id." });
            }

            var category = await _categoryService.GetByIdAsync(id.Value);
            if (category == null)
            {
                return Json(new { success = false, message = "Category not found." });
            }

            await _categoryService.DeleteAsync(id.Value);
            return Json(new { success = true, message = "Category deleted successfully." });
        }
    }
}
