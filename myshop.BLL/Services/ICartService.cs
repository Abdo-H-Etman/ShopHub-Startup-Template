using myshop.BLL.DTOs.Cart;

namespace myshop.BLL.Services;

public interface ICartService
{
    List<CartItem> GetCart();

    Task AddItemAsync(int productId);

    void RemoveItem(int productId);

    void IncreaseQuantity(int productId);

    void DecreaseQuantity(int productId);

    void ClearCart();

    decimal GetCartTotal(List<CartItem> cart);

    void MigrateGuestCart(string userId);
}