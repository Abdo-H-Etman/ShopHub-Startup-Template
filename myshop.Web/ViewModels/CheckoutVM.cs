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

    // Fake Payment simulation fields
    [Required(ErrorMessage = "Name on Card is required")]
    [Display(Name = "Name on Card")]
    public string CardHolderName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Card Number is required")]
    [Display(Name = "Card Number")]
    [RegularExpression(@"^(\d{4}[- ]?){3}\d{4}$|^\d{16}$", ErrorMessage = "Please enter a valid 16-digit card number (e.g. 4242 4242 4242 4242)")]
    public string CardNumber { get; set; } = "4242 •••• •••• 4242";

    [Required(ErrorMessage = "Expiration date is required")]
    [Display(Name = "Expiration (MM/YY)")]
    [RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{2})$", ErrorMessage = "Format must be MM/YY")]
    public string CardExpiration { get; set; } = "12/28";

    [Required(ErrorMessage = "CVV is required")]
    [Display(Name = "CVV")]
    [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVV must be 3 or 4 digits")]
    public string CardCvv { get; set; } = "123";

    // Summary data
    public List<CartItem> CartItems { get; set; } = new();
    public decimal OrderTotal { get; set; }
}
