using System.Collections.Generic;
using System.Threading.Tasks;
using myshop.BLL.DTOs.Cart;
using myshop.BLL.DTOs.Common;
using myshop.BLL.DTOs.Order;

namespace myshop.BLL.Services;

public interface IOrderService
{
    Task<OrderHeaderDto> CreateOrderAsync(int userId, OrderCreateDto orderCreateDto, List<CartItem> cartItems,
        CancellationToken cancellationToken = default);
    Task<PagedResultDto<OrderSummaryDto>> GetPagedUserOrdersAsync(int userId, int pageNumber, int pageSize,
        string? sort, CancellationToken cancellationToken = default);
    Task<OrderHeaderDto?> GetOrderDetailsAsync(int orderId, int? userId = null,
        CancellationToken cancellationToken = default);
    Task<OrderHeaderDto?> GetOrderByPaymentIntentIdAsync(string paymentIntentId, int userId,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderSummaryDto>> GetAllOrdersAsync(CancellationToken cancellationToken = default);
    Task<bool> UpdateOrderStatusAsync(int orderId, string orderStatus, string? paymentStatus = null,
        CancellationToken cancellationToken = default);
}
