using AutoMapper;
using Microsoft.EntityFrameworkCore;
using myshop.BLL.DTOs.Review;
using myshop.Entities.Models;
using Repositories.Interfaces;

namespace myshop.BLL.Services;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ReviewService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReviewDto>> GetProductReviewsAsync(int productId, int? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var reviews = await _unitOfWork.Reviews.GetAllAsync(
            q => q.Where(r => r.ProductId == productId)
                  .Include(r => r.User)
                  .OrderByDescending(r => r.CreatedAt),
            cancellationToken: cancellationToken
        );

        return reviews.Select(r => new ReviewDto
        {
            Id = r.Id,
            ProductId = r.ProductId,
            UserId = r.UserId,
            UserName = r.User?.UserName ?? "Anonymous",
            UserFullName = r.User?.Name ?? r.User?.UserName ?? "Anonymous",
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt,
            IsCurrentUser = currentUserId.HasValue && r.UserId == currentUserId.Value
        }).ToList();
    }

    public async Task<ProductRatingSummaryDto> GetProductRatingSummaryAsync(int productId,
        CancellationToken cancellationToken = default)
    {
        var reviews = (await _unitOfWork.Reviews.GetAllAsync(
            q => q.Where(r => r.ProductId == productId),
            cancellationToken: cancellationToken
        )).ToList();

        var summary = new ProductRatingSummaryDto
        {
            ProductId = productId,
            ReviewCount = reviews.Count,
            AverageRating = reviews.Any() ? Math.Round(reviews.Average(r => r.Rating), 1) : 0
        };

        foreach (var r in reviews)
        {
            if (summary.RatingCounts.ContainsKey(r.Rating))
            {
                summary.RatingCounts[r.Rating]++;
            }
        }

        return summary;
    }

    public async Task<ReviewDto?> GetUserReviewForProductAsync(int productId, int userId,
        CancellationToken cancellationToken = default)
    {
        var review = await _unitOfWork.Reviews.FirstOrDefaultAsync(
            r => r.ProductId == productId && r.UserId == userId,
            q => q.Include(r => r.User),
            cancellationToken: cancellationToken
        );

        if (review == null)
            return null;

        return new ReviewDto
        {
            Id = review.Id,
            ProductId = review.ProductId,
            UserId = review.UserId,
            UserName = review.User?.UserName ?? "Anonymous",
            UserFullName = review.User?.Name ?? review.User?.UserName ?? "Anonymous",
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt,
            UpdatedAt = review.UpdatedAt,
            IsCurrentUser = true
        };
    }

    public async Task<ReviewDto> AddReviewAsync(int userId, CreateReviewDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.Rating < 1 || dto.Rating > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(dto.Rating), "Rating must be between 1 and 5 stars.");
        }

        if (string.IsNullOrWhiteSpace(dto.Comment))
        {
            throw new ArgumentException("Review comment is required.", nameof(dto.Comment));
        }

        // Check if user already reviewed this product
        var existing = await _unitOfWork.Reviews.FirstOrDefaultAsync(
            r => r.ProductId == dto.ProductId && r.UserId == userId,
            cancellationToken: cancellationToken
        );

        if (existing != null)
        {
            throw new InvalidOperationException("You have already reviewed this product. You can update your existing review.");
        }

        var product = await _unitOfWork.Products.GetByIdAsync(dto.ProductId, cancellationToken: cancellationToken);
        if (product == null)
        {
            throw new InvalidOperationException("Product not found.");
        }

        var review = new Review
        {
            ProductId = dto.ProductId,
            UserId = userId,
            Rating = dto.Rating,
            Comment = dto.Comment.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Reviews.AddAsync(review, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return (await GetUserReviewForProductAsync(dto.ProductId, userId, cancellationToken))!;
    }

    public async Task<ReviewDto> UpdateReviewAsync(int userId, UpdateReviewDto dto,
        CancellationToken cancellationToken = default)
    {
        if (dto.Rating < 1 || dto.Rating > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(dto.Rating), "Rating must be between 1 and 5 stars.");
        }

        if (string.IsNullOrWhiteSpace(dto.Comment))
        {
            throw new ArgumentException("Review comment is required.", nameof(dto.Comment));
        }

        var review = await _unitOfWork.Reviews.GetByIdAsync(dto.Id, cancellationToken: cancellationToken);
        if (review == null)
        {
            throw new InvalidOperationException("Review not found.");
        }

        if (review.UserId != userId)
        {
            throw new UnauthorizedAccessException("You can only edit your own review.");
        }

        review.Rating = dto.Rating;
        review.Comment = dto.Comment.Trim();
        review.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Reviews.UpdateAsync(review, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return (await GetUserReviewForProductAsync(review.ProductId, userId, cancellationToken))!;
    }

    public async Task<bool> DeleteReviewAsync(int userId, int reviewId, bool isAdmin = false,
        CancellationToken cancellationToken = default)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId, cancellationToken: cancellationToken);
        if (review == null)
        {
            return false;
        }

        if (review.UserId != userId && !isAdmin)
        {
            throw new UnauthorizedAccessException("You can only delete your own review.");
        }

        await _unitOfWork.Reviews.DeleteAsync(reviewId, cancellationToken: cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
