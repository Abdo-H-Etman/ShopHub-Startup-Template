using System.Collections.Generic;
using System.Threading.Tasks;
using myshop.BLL.DTOs.Review;

namespace myshop.BLL.Services;

public interface IReviewService
{
    Task<IEnumerable<ReviewDto>> GetProductReviewsAsync(int productId, int? currentUserId = null);
    Task<ProductRatingSummaryDto> GetProductRatingSummaryAsync(int productId);
    Task<ReviewDto?> GetUserReviewForProductAsync(int productId, int userId);
    Task<ReviewDto> AddReviewAsync(int userId, CreateReviewDto dto);
    Task<ReviewDto> UpdateReviewAsync(int userId, UpdateReviewDto dto);
    Task<bool> DeleteReviewAsync(int userId, int reviewId, bool isAdmin = false);
}
