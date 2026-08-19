using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using myshop.BLL.DTOs.Account;
using myshop.BLL.Services;
using myshop.Entities.Models;
using myshop.Entities.ViewModels.Account;

namespace myshop.Web.Controllers;

[AllowAnonymous]
public class AccountController : Controller
{
    private readonly IAccountService _accountService;
    private readonly IMapper _mapper;
    private readonly ICartService _cartService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(
        IAccountService accountService,
        IMapper mapper,
        ICartService cartService,
        UserManager<ApplicationUser> userManager)
    {
        _accountService = accountService;
        _mapper = mapper;
        _cartService = cartService;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterVM vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var result = await _accountService.RegisterAsync(
            _mapper.Map<RegisterDto>(vm));

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(vm);
        }

        return RedirectToAction("Login", "Account");
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVM vm, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(vm);
        }

        var result = await _accountService.LoginAsync(
            _mapper.Map<LoginDto>(vm));

        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                ModelState.AddModelError(
                    "",
                    "Your account is locked. Please try again later.");
            }
            else if (result.IsNotAllowed)
            {
                ModelState.AddModelError(
                    "",
                    "Please confirm your email before logging in.");
            }
            else
            {
                ModelState.AddModelError(
                    "",
                    "Invalid username or password.");
            }

            ViewBag.ReturnUrl = returnUrl;

            return View(vm);
        }

        var user = await _userManager.FindByNameAsync(vm.UserName);
        if (user != null)
        {
            _cartService.MigrateGuestCart(user.Id.ToString());
        }

        if (!string.IsNullOrEmpty(returnUrl) &&
            Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _accountService.LogoutAsync();
        HttpContext.Session.Clear();

        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}