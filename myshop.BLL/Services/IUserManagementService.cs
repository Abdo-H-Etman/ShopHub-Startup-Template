using myshop.BLL.DTOs.User;

namespace myshop.BLL.Services;

public interface IUserManagementService
{
    Task<IEnumerable<UserManagementDto>> GetUsersAsync();

    Task<bool> ChangeRoleAsync(int userId, string role);

    Task<bool> LockUserAsync(int userId);

    Task<bool> UnlockUserAsync(int userId);

    Task<bool> DeleteUserAsync(int userId, int currentUserId);
}