using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using myshop.DataAccess;
using myshop.Entities.Models;
using myshop.Entities.ViewModels;
using Repositories.Interfaces;

namespace myshop.Web.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            var products = await _unitOfWork.Products.GetAllAsync(q => q.Include(x => x.Category));

            var result = products
                .Select(x => new
                {
                    id = x.Id,
                    name = x.Name,
                    description = x.Description,
                    price = x.Price,
                    categoryName = x.Category != null ? x.Category.Name : ""
                })
                .ToList();

            return Json(new { data = result });
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = (await _unitOfWork.Categories.GetAllAsync()).
                    Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    })
            };
            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductVM productVM, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                string RootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string filename = Guid.NewGuid().ToString();
                    var Upload = Path.Combine(RootPath, @"Images\Products");
                    var ext = Path.GetExtension(file.FileName);

                    using (var filestream = new FileStream(Path.Combine(Upload, filename + ext), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }
                    productVM.Product.Img = @"Images\Products\" + filename + ext;
                }

                await _unitOfWork.Products.AddAsync(productVM.Product);
                await _unitOfWork.SaveChangesAsync();
                TempData["Create"] = "Item has Created Successfully";
                return RedirectToAction("Index");
            }
            return View(productVM.Product);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            ProductVM productVM = new ProductVM()
            {
                Product = await _unitOfWork.Products.GetByIdAsync(id.Value),
                CategoryList = (await _unitOfWork.Categories.GetAllAsync()).Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };

            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string RootPath = _webHostEnvironment.WebRootPath;

                if (file != null)
                {
                    string filename = Guid.NewGuid().ToString();
                    var Upload = Path.Combine(RootPath, @"Images\Products");
                    var ext = Path.GetExtension(file.FileName);

                    if (productVM.Product.Img != null)
                    {
                        var oldimg = Path.Combine(RootPath, productVM.Product.Img.TrimStart('\\'));

                        if (System.IO.File.Exists(oldimg))
                        {
                            System.IO.File.Delete(oldimg);
                        }
                    }

                    using (var filestream = new FileStream(Path.Combine(Upload, filename + ext), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }

                    productVM.Product.Img = @"Images\Products\" + filename + ext;
                }

                await _unitOfWork.Products.UpdateAsync(productVM.Product);
                await _unitOfWork.SaveChangesAsync();
                TempData["Update"] = "Data has Updated Successfully";
                return RedirectToAction("Index");
            }

            return View(productVM.Product);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var product = await _unitOfWork.Products.GetByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            var category = (await _unitOfWork.Categories.GetAllAsync())
                .FirstOrDefault(x => x.Id == product.CategoryId);

            product.Category = category;

            return View(product);
        }

        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> DeleteConfirmed(int? id)
        // {
        //     if (id == null || id == 0)
        //     {
        //         return NotFound();
        //     }

        //     var productIndb = await _unitOfWork.Products.GetByIdAsync(id.Value);
        //     if (productIndb == null)
        //     {
        //         return NotFound();
        //     }

        //     if (!string.IsNullOrEmpty(productIndb.Img))
        //     {
        //         var oldimg = Path.Combine(_webHostEnvironment.WebRootPath, productIndb.Img.TrimStart('\\'));

        //         if (System.IO.File.Exists(oldimg))
        //         {
        //             System.IO.File.Delete(oldimg);
        //         }
        //     }

        //     await _unitOfWork.Products.DeleteAsync(id.Value);
        //     await _unitOfWork.SaveChangesAsync();

        //     TempData["Delete"] = "Item has Deleted Successfully";
        //     return RedirectToAction("Index");
        // }

        [HttpDelete]
        public async Task<IActionResult> DeleteAjax(int? id)
        {
            var productIndb = await _unitOfWork.Products.GetByIdAsync(id.Value);

            if (productIndb == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }

            await _unitOfWork.Products.DeleteAsync(id.Value);
            await _unitOfWork.SaveChangesAsync();

            if (!string.IsNullOrEmpty(productIndb.Img))
            {
                var oldimg = Path.Combine(_webHostEnvironment.WebRootPath, productIndb.Img.TrimStart('\\'));

                if (System.IO.File.Exists(oldimg))
                {
                    System.IO.File.Delete(oldimg);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return Json(new { success = true, message = "Product has been Deleted" });
        }


    }
}
