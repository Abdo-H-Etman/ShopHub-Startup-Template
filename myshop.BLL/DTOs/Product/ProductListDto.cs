using System;

namespace myshop.BLL.DTOs.Product;

public record ProductListDto : ProductDtoBase
{
    public int Id { get; set; }
    public string Img { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}