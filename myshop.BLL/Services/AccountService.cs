using Microsoft.AspNetCore.Identity;
using myshop.BLL.DTOs.Account;
using myshop.Entities.Models;

namespace myshop.BLL.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<IdentityResult> RegisterAsync(RegisterDto dto)
    {
        var user = new ApplicationUser
        {
            UserName = dto.UserName,
            Email = dto.Email,
            Name = dto.Name
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
            return result;

        await _userManager.AddToRoleAsync(user, "Customer");

        return result;
    }

    public async Task<SignInResult> LoginAsync(LoginDto dto)
    {
        var result = await _signInManager.PasswordSignInAsync(dto.UserName, dto.Password, dto.RememberMe, lockoutOnFailure: false);
        return result;
    }

    public async Task LogoutAsync()
    {
        await _signInManager.SignOutAsync();
    }
}