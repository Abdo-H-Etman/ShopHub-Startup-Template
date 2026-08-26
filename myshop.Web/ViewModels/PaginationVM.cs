namespace myshop.Entities.ViewModels;

public class PaginationVM
{
    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalPages { get; set; }

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;

    public string Controller { get; set; } = string.Empty;

    public string Action { get; set; } = "Index";

    public Dictionary<string, string?> RouteValues { get; set; } = new();
}