namespace myshop.BLL.DTOs.Order;

public class OrderCreateDto
{
    public int ApplicationUserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? PostalCode { get; set; }
    public string? PhoneNumber { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
}
