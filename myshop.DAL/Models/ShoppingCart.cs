using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace myshop.Entities.Models;

public class ShoppingCart
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    [ValidateNever]
    public Product Product { get; set; } = null!;

    public int Count { get; set; }

    public int ApplicationUserId { get; set; }
    [ValidateNever]
    public ApplicationUser ApplicationUser { get; set; } = null!;
}
