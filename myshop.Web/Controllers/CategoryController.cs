using Microsoft.AspNetCore.Mvc;
using myshop.DataAccess;
using myshop.Entities.Models;
using Repositories.Interfaces;

namespace myshop.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();

            return View(categories.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Categories.AddAsync(category);
                await _unitOfWork.SaveChangesAsync();
                TempData["Create"] = "Item has Created Successfully";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null | id == 0)
            {
                return NotFound();
            }
            var categoryIndb = await _unitOfWork.Categories.GetByIdAsync(id!.Value);

            return View(categoryIndb);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Categories.UpdateAsync(category);

                await _unitOfWork.SaveChangesAsync();
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

            var categoryIndb = await _unitOfWork.Categories.GetByIdAsync(id!.Value);
            if (categoryIndb == null)
            {
                return NotFound();
            }

            return View(categoryIndb);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAjax(int? id)
        {
            if (id == null || id == 0)
            {
                return Json(new { success = false, message = "Invalid category id." });
            }

            var categoryIndb = await _unitOfWork.Categories.GetByIdAsync(id!.Value);
            if (categoryIndb == null)
            {
                return Json(new { success = false, message = "Category not found." });
            }

            await _unitOfWork.Categories.DeleteAsync(id!.Value);
            await _unitOfWork.SaveChangesAsync();

            return Json(new { success = true, message = "Category deleted successfully." });
        }
    }
}
