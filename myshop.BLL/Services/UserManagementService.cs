using Microsoft.AspNetCore.Identity;
using myshop.BLL.DTOs.User;
using myshop.Entities.Models;

namespace myshop.BLL.Services;

public class UserManagementService : IUserManagementService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserManagementService(
        UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IEnumerable<UserManagementDto>> GetUsersAsync()
    {
        var users = _userManager.Users.ToList();

        var result = new List<UserManagementDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            result.Add(new UserManagementDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email ?? string.Empty,
                Role = roles.FirstOrDefault() ?? "No Role",
                IsLockedOut = await _userManager.IsLockedOutAsync(user)
            });
        }

        return result;
    }

    public async Task<bool> ChangeRoleAsync(int userId, string role)
    {
        if (role != "Admin" && role != "Customer")
            return false;

        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return false;

        var currentRoles = await _userManager.GetRolesAsync(user);

        if (currentRoles.Any())
        {
            var removeResult =
                await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!removeResult.Succeeded)
                return false;
        }

        var addResult = await _userManager.AddToRoleAsync(user, role);

        return addResult.Succeeded;
    }

    public async Task<bool> LockUserAsync(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return false;

        user.LockoutEnabled = true;

        var result = await _userManager.SetLockoutEndDateAsync(
            user,
            DateTimeOffset.MaxValue);

        return result.Succeeded;
    }

    public async Task<bool> UnlockUserAsync(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return false;

        var result = await _userManager.SetLockoutEndDateAsync(
            user,
            null);

        return result.Succeeded;
    }

    public async Task<bool> DeleteUserAsync(
        int userId,
        int currentUserId)
    {
        if (userId == currentUserId)
            return false;

        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return false;

        var result = await _userManager.DeleteAsync(user);

        return result.Succeeded;
    }
}