using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using myshop.BLL.Services;
using myshop.Entities.Models;

namespace myshop.Web.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class UserController : Controller
{
    private readonly IUserManagementService _userManagementService;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserController(
        IUserManagementService userManagementService,
        UserManager<ApplicationUser> userManager)
    {
        _userManagementService = userManagementService;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }
    [HttpGet]
    public async Task<IActionResult> GetData()
    {
        var users = await _userManagementService.GetUsersAsync();

        return Json(new
        {
            data = users
        });
    }

    [HttpPost]
    public async Task<IActionResult> ChangeRole(
        int userId,
        string role)
    {
        var currentUserId = int.Parse(
            _userManager.GetUserId(User)!);

        // Prevent admin from changing his own role
        if (userId == currentUserId)
        {
            return Json(new
            {
                success = false,
                message = "You cannot change your own role."
            });
        }

        var result =
            await _userManagementService.ChangeRoleAsync(
                userId,
                role);

        return Json(new
        {
            success = result,
            message = result
            ? "User role updated successfully."
            : "Failed to change user role."
        });
    }

    [HttpPost]
    public async Task<IActionResult> Lock(int userId)
    {
        var currentUserId = int.Parse(
            _userManager.GetUserId(User)!);

        if (userId == currentUserId)
        {
            return Json(new
            {
                success = false,
                message = "You cannot lock your own account."
            });
        }

        var result =
            await _userManagementService.LockUserAsync(userId);

        return Json(new
        {
            success = result,
            message = result
            ? "User account locked successfully."
            : "Failed to lock user account."
        });
    }

    [HttpPost]
    public async Task<IActionResult> Unlock(int userId)
    {
        var result =
            await _userManagementService.UnlockUserAsync(userId);

        return Json(new
        {
            success = result,
            message = result
            ? "User account unlocked successfully."
            : "Failed to unlock user account."
        });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var user = _userManager.Users.FirstOrDefault(u => u.Id == id.Value);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAjax(int? id)
    {
        if (id == null || id == 0)
        {
            return Json(new { success = false, message = "Invalid user id." });
        }

        var currentUserId = int.Parse(
            _userManager.GetUserId(User)!);

        if (id.Value == currentUserId)
        {
            return Json(new { success = false, message = "You cannot delete your own account." });
        }

        var result = await _userManagementService.DeleteUserAsync(id.Value, currentUserId);

        if (!result)
        {
            return Json(new { success = false, message = "Failed to delete user." });
        }

        return Json(new { success = true, message = "User deleted successfully." });
    }
}