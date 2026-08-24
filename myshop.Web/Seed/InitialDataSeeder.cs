using Microsoft.AspNetCore.Identity;
using myshop.BLL.DTOs.Category;
using myshop.BLL.DTOs.Product;
using myshop.BLL.Services;
using myshop.Entities.Models;

namespace myshop.Web.Seed;

public class InitialDataSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly ICategoryService _categoryService;
    private readonly IProductService _productService;
    private readonly IFileService _fileService;

    public InitialDataSeeder(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        ICategoryService categoryService,
        IProductService productService,
        IFileService fileService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _categoryService = categoryService;
        _productService = productService;
        _fileService = fileService;
    }

    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedAdminUserAsync();
        await SeedSomeUsersAsync();
        await SeedSomeCategoriesAsync();
        await SeedSomeProductsAsync();
    }
    private async Task SeedAdminUserAsync()
    {
        var adminUser = await _userManager.FindByNameAsync("admin");
        if (adminUser == null)
        {
            string adminPassword = "Admin@123";
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

    private async Task SeedSomeUsersAsync()
    {
        var user1 = await _userManager.FindByNameAsync("user1");
        if (user1 == null)
        {
            string userPassword = "User@123";
            var newUser1 = new ApplicationUser
            {
                UserName = "user1",
                Name = "User One",
                Email = "user1@myshop.com",
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(newUser1, userPassword);
            if (!result.Succeeded)
                throw new Exception($"Failed to create user1: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            user1 = newUser1;
        }

        if (!await _userManager.IsInRoleAsync(user1, "Customer"))
            await _userManager.AddToRoleAsync(user1, "Customer");

        var user2 = await _userManager.FindByNameAsync("user2");
        if (user2 == null)
        {
            string userPassword = "User@123";
            var newUser2 = new ApplicationUser
            {
                UserName = "user2",
                Name = "User Two",
                Email = "user2@myshop.com",
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(newUser2, userPassword);
            if (!result.Succeeded)
                throw new Exception($"Failed to create user2: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            user2 = newUser2;
        }

        if (!await _userManager.IsInRoleAsync(user2, "Customer"))
            await _userManager.AddToRoleAsync(user2, "Customer");
    }

    private async Task SeedSomeCategoriesAsync()
    {
        var categories = await _categoryService.GetAllAsync();

        if (!categories.Any())
        {
            var defaultCategories = new List<CreateCategoryDto>
            {
                new CreateCategoryDto { Name = "Electronics" },
                new CreateCategoryDto { Name = "Books" },
                new CreateCategoryDto { Name = "Clothing" },
                new CreateCategoryDto { Name = "Home & Kitchen" }
            };

            foreach (var category in defaultCategories)
            {
                await _categoryService.CreateAsync(category);
            }
        }
    }

    private async Task SeedSomeProductsAsync()
    {
        var products = await _productService.GetAllAsync();

        if (!products.Any())
        {
            var smartPhoneImg = await _fileService.CopyFileAsync("Images/Products/5f4d61ba-5f02-4a22-8305-1148a341710f.jpg");

            var IPhoneImg = await _fileService.CopyFileAsync("Images/Products/e6c4961b-1c83-4da0-bf6d-f6c4901fb9b9.jpeg");

            var laptopImg = await _fileService.CopyFileAsync("Images/Products/42af8ba8-65df-417b-908c-ffb6cfcdfc0c.jpg");

            var mouseImg = await _fileService.CopyFileAsync("Images/Products/927524ce-cf58-4eea-9e36-d29b9edc539a.jpg");

            var barcaShirtImg = await _fileService.CopyFileAsync("Images/Products/eb1e8683-8559-4957-92c0-773fd02e4c7c.jpg");

            var screenImg = await _fileService.CopyFileAsync("Images/Products/02a7ea31-1096-4acc-99ad-d708a75c6688.jpg");

            var airConditionerImg = await _fileService.CopyFileAsync("Images/Products/035afc1e-b77f-4b5b-9eb7-7d7991bea349.jpg");

            var ShoesImg = await _fileService.CopyFileAsync("Images/Products/79e1b43f-7bc5-4019-9d0c-409a0275d0b9.jpg");

            var RealShirtImg = await _fileService.CopyFileAsync("Images/Products/d5033c93-add3-40bc-85f5-020363626720.jpg");

            var categories = await _categoryService.GetAllAsync();

            var electronicsId = categories.FirstOrDefault(c => c.Name == "Electronics")!.Id;

            var clothingId = categories.FirstOrDefault(c => c.Name == "Clothing")!.Id;

            var defaultProducts = new List<CreateProductDto>
            {
                new CreateProductDto { Name = "Smartphone", Description = "Latest model smartphone", Price = 699.99m,
                                CategoryId = electronicsId,
                                Img = smartPhoneImg },
                new CreateProductDto { Name = "Iphone 15 Pro Max", Description = "Flagship smartphone", Price = 1299.99m,
                                CategoryId = electronicsId,
                                Img = IPhoneImg },
                new CreateProductDto { Name = "Laptop", Description = "High performance laptop", Price = 1299.99m,
                                CategoryId = electronicsId,
                                Img = laptopImg },
                new CreateProductDto { Name = "Wireless Mouse", Description = "High quality wireless mouse", Price = 1299.99m,
                                CategoryId = electronicsId,
                                Img = mouseImg },
                new CreateProductDto { Name = "Screen", Description = "High quality screen", Price = 1199.99m,
                                CategoryId = electronicsId,
                                Img = screenImg },
                new CreateProductDto { Name = "Shoes", Description = "premium quality shoes", Price = 999.99m,
                                CategoryId = electronicsId,
                                Img = ShoesImg },
                new CreateProductDto { Name = "Air Conditioner", Description = "Air conditioner Sharp model", Price = 2199.99m,
                                CategoryId = electronicsId,
                                Img = airConditionerImg },
                new CreateProductDto { Name = "Real madrid Shirt", Description = "Comfortable cotton Sports shirt", Price = 15.99m,
                                CategoryId = clothingId,
                                Img = RealShirtImg },
                new CreateProductDto { Name = "Barca-Shirt", Description = "Comfortable cotton Sports shirt", Price = 14.99m,
                                CategoryId = clothingId,
                                Img = barcaShirtImg }
            };

            foreach (var product in defaultProducts)
            {
                await _productService.CreateAsync(product, null);
            }
        }
    }

    private async Task SeedRolesAsync()
    {
        if (!await _roleManager.RoleExistsAsync("Admin"))
            await _roleManager.CreateAsync(new IdentityRole<int>("Admin"));

        if (!await _roleManager.RoleExistsAsync("Customer"))
            await _roleManager.CreateAsync(new IdentityRole<int>("Customer"));
    }
}