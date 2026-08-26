using System.Collections.Generic;
using System.Threading.Tasks;
using myshop.BLL.DTOs.Cart;
using myshop.BLL.DTOs.Common;
using myshop.BLL.DTOs.Order;

namespace myshop.BLL.Services;

public interface IOrderService
{
    Task<OrderHeaderDto> CreateOrderAsync(int userId, OrderCreateDto orderCreateDto, List<CartItem> cartItems);
    Task<PagedResultDto<OrderSummaryDto>> GetPagedUserOrdersAsync(int userId, int pageNumber, int pageSize, string? sort);
    Task<OrderHeaderDto?> GetOrderDetailsAsync(int orderId, int? userId = null);
    Task<IEnumerable<OrderSummaryDto>> GetAllOrdersAsync();
    Task<bool> UpdateOrderStatusAsync(int orderId, string orderStatus, string? paymentStatus = null);
}
