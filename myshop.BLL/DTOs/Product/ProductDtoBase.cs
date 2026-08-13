namespace myshop.BLL.DTOs.Product;

public abstract record ProductDtoBase
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}