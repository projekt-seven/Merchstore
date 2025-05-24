using MerchStore.Application.Services.Interfaces;
using MerchStore.Domain.Entities;
using MerchStore.Application.Common.Interfaces;

namespace MerchStore.Application.Services.Implementations;

/// <summary>
/// Implementation of the review service.
/// Acts as a facade over the repository layer.
/// </summary>
public class ReviewService : IReviewService
{
	private readonly IReviewRepository _reviewRepository;

	/// <summary>
	/// Constructor with dependency injection
	/// </summary>
	/// <param name="reviewRepository">The review repository</param>
	public ReviewService(IReviewRepository reviewRepository)
	{
		_reviewRepository = reviewRepository;
	}

	/// <inheritdoc/>
	public async Task<IEnumerable<Review>> GetReviewsByProductIdAsync(Guid productId)
	{
		var (reviews, _) = await _reviewRepository.GetProductReviewsAsync(productId);
		return reviews;
	}

	/// <inheritdoc/>
	public async Task<double> GetAverageRatingForProductAsync(Guid productId)
	{
		var (_, stats) = await _reviewRepository.GetProductReviewsAsync(productId);
		return stats.AverageRating;
	}

	/// <inheritdoc/>
	public async Task<int> GetReviewCountForProductAsync(Guid productId)
	{
		var (_, stats) = await _reviewRepository.GetProductReviewsAsync(productId);
		return stats.ReviewCount;
	}

	/// <inheritdoc/>
	public Task AddReviewAsync(Review review)
	{
		// Since the IReviewRepository doesn't have an AddAsync method,
		// we need to figure out how to handle this case
		throw new NotImplementedException("Adding reviews is not supported by the current repository interface.");
	}
}