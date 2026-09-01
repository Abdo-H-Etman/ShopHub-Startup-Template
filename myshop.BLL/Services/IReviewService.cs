using System.Collections.Generic;
using System.Threading.Tasks;
using myshop.BLL.DTOs.Review;

namespace myshop.BLL.Services;

public interface IReviewService
{
    Task<IEnumerable<ReviewDto>> GetProductReviewsAsync(int productId, int? currentUserId = null,
        CancellationToken cancellationToken = default);
    Task<ProductRatingSummaryDto> GetProductRatingSummaryAsync(int productId,
        CancellationToken cancellationToken = default);
    Task<ReviewDto?> GetUserReviewForProductAsync(int productId, int userId,
        CancellationToken cancellationToken = default);
    Task<ReviewDto> AddReviewAsync(int userId, CreateReviewDto dto,
        CancellationToken cancellationToken = default);
    Task<ReviewDto> UpdateReviewAsync(int userId, UpdateReviewDto dto,
        CancellationToken cancellationToken = default);
    Task<bool> DeleteReviewAsync(int userId, int reviewId, bool isAdmin = false,
        CancellationToken cancellationToken = default);
}
