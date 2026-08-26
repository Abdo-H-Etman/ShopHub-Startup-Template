namespace myshop.BLL.DTOs.Order;

public class OrderSummaryDto
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string? OrderStatus { get; set; }
    public string? PaymentStatus { get; set; }
    public int ItemCount { get; set; }
}
