using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using myshop.BLL.DTOs.Product;

namespace myshop.Entities.ViewModels;

public class ProdcutEditVM
{
    public int Id { get; set; }
    public UpdateProductDto Product { get; set; } = new();
    [ValidateNever]
    public IEnumerable<SelectListItem> CategoryList { get; set; } = Enumerable.Empty<SelectListItem>();
}