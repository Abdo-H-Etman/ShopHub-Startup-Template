namespace myshop.BLL.DTOs.Product;

public record UpdateProductDto : ProductDtoBase
{
    public int Id { get; set; }
    public string Img { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
}
