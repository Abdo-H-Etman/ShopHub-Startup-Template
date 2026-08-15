using Microsoft.AspNetCore.Identity;
using myshop.Entities.Models;

namespace myshop.Web.Seed;

public class InitialDataSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    public InitialDataSeeder(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedAdminUserAsync();
    }
    private async Task SeedAdminUserAsync()
    {
        var adminUser = await _userManager.FindByNameAsync("admin");
        if (adminUser == null)
        {
            string adminPassword = "Admin@123"; // Set a strong password for the admin user
            var newAdmin = new ApplicationUser
            {
                UserName = "admin",
                Name = "Admin User",
                Email = "admin@myshop.com",
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(newAdmin, adminPassword);
            if (!result.Succeeded)
                throw new Exception($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            adminUser = newAdmin;
        }

        if (!await _userManager.IsInRoleAsync(adminUser, "Admin"))
            await _userManager.AddToRoleAsync(adminUser, "Admin");
    }

    private async Task SeedRolesAsync()
    {
        if (!await _roleManager.RoleExistsAsync("Admin"))
            await _roleManager.CreateAsync(new IdentityRole<int>("Admin"));

        if (!await _roleManager.RoleExistsAsync("Customer"))
            await _roleManager.CreateAsync(new IdentityRole<int>("Customer"));
    }
}