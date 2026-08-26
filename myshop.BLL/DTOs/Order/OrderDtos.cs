namespace myshop.BLL.DTOs.Order;

public class OrderHeaderDto
{
    public int Id { get; set; }
    public int ApplicationUserId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime? ShippingDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string? OrderStatus { get; set; }
    public string? PaymentStatus { get; set; }
    public string? TrackingNumber { get; set; }
    public string? Carrier { get; set; }
    public DateTime? PaymentDate { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? PostalCode { get; set; }
    public string? PhoneNumber { get; set; }

    public List<OrderDetailDto> OrderDetails { get; set; } = new();
}
