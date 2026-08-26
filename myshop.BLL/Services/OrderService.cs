using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using myshop.BLL.DTOs.Cart;
using myshop.BLL.DTOs.Common;
using myshop.BLL.DTOs.Order;
using myshop.Entities.Models;
using Repositories.Interfaces;

namespace myshop.BLL.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<OrderHeaderDto> CreateOrderAsync(int userId, OrderCreateDto orderCreateDto, List<CartItem> cartItems)
    {
        if (cartItems == null || !cartItems.Any())
        {
            throw new InvalidOperationException("Cannot place an order with an empty cart.");
        }

        var totalPrice = cartItems.Sum(c => c.Price * c.Quantity);

        var orderHeader = new OrderHeader
        {
            ApplicationUserId = userId,
            Name = orderCreateDto.Name,
            Address = orderCreateDto.Address,
            City = orderCreateDto.City,
            PostalCode = orderCreateDto.PostalCode,
            PhoneNumber = orderCreateDto.PhoneNumber,
            OrderDate = DateTime.UtcNow,
            TotalPrice = totalPrice,
            OrderStatus = "Approved",
            PaymentStatus = "Approved",
            PaymentDate = DateTime.UtcNow
        };

        await _unitOfWork.OrderHeaders.AddAsync(orderHeader);
        await _unitOfWork.SaveChangesAsync();

        foreach (var item in cartItems)
        {
            var orderDetail = new OrderDetail
            {
                OrderHeaderId = orderHeader.Id,
                ProductId = item.ProductId,
                Price = item.Price,
                Count = item.Quantity
            };

            await _unitOfWork.OrderDetails.AddAsync(orderDetail);
        }

        await _unitOfWork.SaveChangesAsync();

        return (await GetOrderDetailsAsync(orderHeader.Id))!;
    }

    public async Task<PagedResultDto<OrderSummaryDto>> GetPagedUserOrdersAsync(int userId, int pageNumber,
                int pageSize, string? sort)
    {
        Func<IQueryable<OrderHeader>, IOrderedQueryable<OrderHeader>> orderBy =
            sort?.ToLower() switch
            {
                "dateasc" => q => q.OrderBy(o => o.CreatedAt),
                "totalpriceasc" => q => q.OrderBy(o => o.TotalPrice),
                "totalpricedesc" => q => q.OrderByDescending(o => o.TotalPrice),
                _ => q => q.OrderByDescending(o => o.CreatedAt)
            };

        var (items, totalCount) = await _unitOfWork.OrderHeaders.GetPagedAsync(
            pageNumber, pageSize,
            o => o.ApplicationUserId == userId,
            q => q.Include(o => o.OrderDetails),
            orderBy
        );

        var result = new PagedResultDto<OrderSummaryDto>
        {
            Items = items.Select(o => new OrderSummaryDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalPrice = o.TotalPrice,
                OrderStatus = o.OrderStatus,
                PaymentStatus = o.PaymentStatus,
                ItemCount = o.OrderDetails.Sum(d => d.Count)
            }).ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return result;
    }

    public async Task<OrderHeaderDto?> GetOrderDetailsAsync(int orderId, int? userId = null)
    {
        var order = await _unitOfWork.OrderHeaders.FirstOrDefaultAsync(
            o => o.Id == orderId,
            q => q.Include(o => o.ApplicationUser)
                  .Include(o => o.OrderDetails)
                  .ThenInclude(d => d.Product)
        );

        if (order == null)
            return null;

        if (userId.HasValue && order.ApplicationUserId != userId.Value)
            return null;

        return new OrderHeaderDto
        {
            Id = order.Id,
            ApplicationUserId = order.ApplicationUserId,
            CustomerName = order.ApplicationUser?.Name ?? order.Name,
            CustomerEmail = order.ApplicationUser?.Email ?? string.Empty,
            OrderDate = order.OrderDate,
            ShippingDate = order.ShippingDate,
            TotalPrice = order.TotalPrice,
            OrderStatus = order.OrderStatus,
            PaymentStatus = order.PaymentStatus,
            TrackingNumber = order.TrackingNumber,
            Carrier = order.Carrier,
            PaymentDate = order.PaymentDate,
            Name = order.Name,
            Address = order.Address,
            City = order.City,
            PostalCode = order.PostalCode,
            PhoneNumber = order.PhoneNumber,
            OrderDetails = order.OrderDetails.Select(d => new OrderDetailDto
            {
                Id = d.Id,
                ProductId = d.ProductId,
                ProductName = d.Product?.Name ?? "Product",
                ProductImg = d.Product?.Img ?? string.Empty,
                Price = d.Price,
                Count = d.Count
            }).ToList()
        };
    }

    public async Task<IEnumerable<OrderSummaryDto>> GetAllOrdersAsync()
    {
        var orders = await _unitOfWork.OrderHeaders.GetAllAsync(
            q => q.Include(o => o.OrderDetails)
                  .OrderByDescending(o => o.OrderDate)
        );

        return orders.Select(o => new OrderSummaryDto
        {
            Id = o.Id,
            OrderDate = o.OrderDate,
            TotalPrice = o.TotalPrice,
            OrderStatus = o.OrderStatus,
            PaymentStatus = o.PaymentStatus,
            ItemCount = o.OrderDetails.Sum(d => d.Count)
        }).ToList();
    }

    public async Task<bool> UpdateOrderStatusAsync(int orderId, string orderStatus, string? paymentStatus = null)
    {
        var order = await _unitOfWork.OrderHeaders.GetByIdAsync(orderId);
        if (order == null)
            return false;

        order.OrderStatus = orderStatus;
        if (!string.IsNullOrEmpty(paymentStatus))
        {
            order.PaymentStatus = paymentStatus;
        }

        if (orderStatus == "Shipped")
        {
            order.ShippingDate = DateTime.UtcNow;
        }

        await _unitOfWork.OrderHeaders.UpdateAsync(order);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
