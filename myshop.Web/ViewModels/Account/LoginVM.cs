using System.ComponentModel.DataAnnotations;

namespace myshop.Entities.ViewModels.Account;

public class LoginVM
{
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}