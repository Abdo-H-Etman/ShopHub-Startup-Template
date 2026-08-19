using System.Text.Json;
using Microsoft.AspNetCore.Http;
using myshop.BLL.DTOs.Cart;
using myshop.BLL.Services;

namespace myshop.Web.Services;

public class CartService : ICartService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IProductService _productService;

    private string CartSessionKey
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                             ?? user.Identity.Name;
                return $"ShoppingCart_User_{userId}";
            }

            return "ShoppingCart_Guest";
        }
    }

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public CartService(
        IHttpContextAccessor httpContextAccessor,
        IProductService productService)
    {
        _httpContextAccessor = httpContextAccessor;
        _productService = productService;
    }

    private ISession Session =>
        _httpContextAccessor.HttpContext!.Session;

    public List<CartItem> GetCart()
    {
        var cartJson = Session.GetString(CartSessionKey);

        if (string.IsNullOrEmpty(cartJson))
            return new List<CartItem>();

        return JsonSerializer.Deserialize<List<CartItem>>(
                   cartJson,
                   _jsonOptions)
               ?? new List<CartItem>();
    }

    public async Task AddItemAsync(int productId)
    {
        var product = await _productService.GetByIdAsync(productId);

        if (product is null)
            throw new InvalidOperationException("Product not found.");

        var cart = GetCart();

        var existingItem = cart.FirstOrDefault(
            x => x.ProductId == productId);

        if (existingItem is not null)
        {
            existingItem.Quantity++;
        }
        else
        {
            cart.Add(new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = 1,
                ImageUrl = product.Img
            });
        }

        SaveCart(cart);
    }

    public void RemoveItem(int productId)
    {
        var cart = GetCart();

        var item = cart.FirstOrDefault(
            x => x.ProductId == productId);

        if (item is null)
            return;

        cart.Remove(item);

        SaveCart(cart);
    }

    public void IncreaseQuantity(int productId)
    {
        var cart = GetCart();

        var item = cart.FirstOrDefault(
            x => x.ProductId == productId);

        if (item is null)
            return;

        item.Quantity++;

        SaveCart(cart);
    }

    public void DecreaseQuantity(int productId)
    {
        var cart = GetCart();

        var item = cart.FirstOrDefault(
            x => x.ProductId == productId);

        if (item is null)
            return;

        if (item.Quantity > 1)
        {
            item.Quantity--;
        }
        else
        {
            cart.Remove(item);
        }

        SaveCart(cart);
    }

    public void ClearCart()
    {
        Session.Remove(CartSessionKey);
    }

    public decimal GetCartTotal()
    {
        return GetCart().Sum(x => x.Price * x.Quantity);
    }

    public void MigrateGuestCart(string userId)
    {
        var guestJson = Session.GetString("ShoppingCart_Guest");
        if (string.IsNullOrWhiteSpace(guestJson))
            return;

        var guestCart = JsonSerializer.Deserialize<List<CartItem>>(guestJson, _jsonOptions);
        if (guestCart is null || !guestCart.Any())
            return;

        var userSessionKey = $"ShoppingCart_User_{userId}";
        var userJson = Session.GetString(userSessionKey);
        var userCart = string.IsNullOrWhiteSpace(userJson)
            ? new List<CartItem>()
            : JsonSerializer.Deserialize<List<CartItem>>(userJson, _jsonOptions) ?? new List<CartItem>();

        foreach (var guestItem in guestCart)
        {
            var existingItem = userCart.FirstOrDefault(x => x.ProductId == guestItem.ProductId);
            if (existingItem is not null)
            {
                existingItem.Quantity += guestItem.Quantity;
            }
            else
            {
                userCart.Add(guestItem);
            }
        }

        Session.SetString(userSessionKey, JsonSerializer.Serialize(userCart));
        Session.Remove("ShoppingCart_Guest");
    }

    private void SaveCart(List<CartItem> cart)
    {
        var cartJson = JsonSerializer.Serialize(cart);

        Session.SetString(CartSessionKey, cartJson);
    }
}