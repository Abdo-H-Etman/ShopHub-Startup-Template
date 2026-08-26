using System.Collections.Generic;
using myshop.BLL.DTOs.Product;
using myshop.BLL.DTOs.Review;

namespace myshop.Web.ViewModels;

public class ProductDetailsVM
{
    public ProductListDto Product { get; set; } = null!;
    public ProductRatingSummaryDto RatingSummary { get; set; } = new();
    public List<ReviewDto> Reviews { get; set; } = new();
    public ReviewDto? UserReview { get; set; }
    public CreateReviewDto NewReview { get; set; } = new();
    public bool IsAuthenticated { get; set; }
}
