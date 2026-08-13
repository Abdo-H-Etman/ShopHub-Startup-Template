using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using myshop.Entities.Models;

namespace myshop.Entities.ViewModels
{
    public class ProductVM
    {
        public int Id { get; set; }
        public Product Product { get; set; } = new();
        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryList { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
