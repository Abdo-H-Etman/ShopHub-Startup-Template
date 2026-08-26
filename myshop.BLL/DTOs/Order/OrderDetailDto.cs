namespace myshop.BLL.DTOs.Order;

public class OrderDetailDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductImg { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Count { get; set; }
    public decimal TotalPrice => Price * Count;
}
