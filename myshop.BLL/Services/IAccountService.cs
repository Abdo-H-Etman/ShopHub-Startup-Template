using Microsoft.AspNetCore.Identity;
using myshop.BLL.DTOs.Account;

namespace myshop.BLL.Services;

public interface IAccountService
{
    Task<IdentityResult> RegisterAsync(RegisterDto dto);

    Task<SignInResult> LoginAsync(LoginDto dto);

    Task LogoutAsync();
}