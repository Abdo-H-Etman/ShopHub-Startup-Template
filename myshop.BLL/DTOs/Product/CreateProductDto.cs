namespace myshop.BLL.DTOs.Product;

public record CreateProductDto : ProductDtoBase
{
    public string Img { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
}
