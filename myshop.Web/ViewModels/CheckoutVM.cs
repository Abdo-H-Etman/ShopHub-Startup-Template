using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using myshop.BLL.DTOs.Cart;

namespace myshop.Web.ViewModels;

public class CheckoutVM
{
    [Required(ErrorMessage = "Full Name is required")]
    [Display(Name = "Full Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone Number is required")]
    [Display(Name = "Phone Number")]
    [Phone(ErrorMessage = "Please enter a valid phone number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Street Address is required")]
    [Display(Name = "Street Address")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "City is required")]
    [Display(Name = "City")]
    public string City { get; set; } = string.Empty;

    [Display(Name = "Postal / Zip Code")]
    public string? PostalCode { get; set; }

    public List<CartItem> CartItems { get; set; } = new();

    public decimal OrderTotal { get; set; }
}
