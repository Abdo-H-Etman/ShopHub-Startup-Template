using Microsoft.AspNetCore.Identity;
using myshop.BLL.DTOs.Account;
using myshop.BLL.Interfaces;
using myshop.Entities.Models;

namespace myshop.BLL.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailService _emailService;

    public AccountService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
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

        if (!string.IsNullOrEmpty(dto.Email))
        {
            await _emailService.SendWelcomeEmailAsync(dto.Email, dto.Name);
        }

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