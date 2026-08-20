using myshop.BLL.DTOs.Product;

namespace myshop.Entities.ViewModels;

public class ProductIndexVM
{
    public IEnumerable<ProductListDto> Products { get; set; } = [];

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalCount { get; set; }

    public int TotalPages =>
        (int)Math.Ceiling(TotalCount / (double)PageSize);

    public bool HasPreviousPage =>
        PageNumber > 1;

    public bool HasNextPage =>
        PageNumber < TotalPages;

    public string? Search { get; set; }

    public string? Sort { get; set; }
}