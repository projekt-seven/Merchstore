using MerchStore.Domain.Entities;

namespace MerchStore.Application.Services.Interfaces;

/// <summary>
/// Service interface for Review-related operations.
/// Provides a simple abstraction over the repository layer.
/// </summary>
public interface IReviewService
{
	/// <summary>
	/// Gets all reviews for a specific product
	/// </summary>
	/// <param name="productId">The product ID</param>
	/// <returns>A collection of reviews for the specified product</returns>
	Task<IEnumerable<Review>> GetReviewsByProductIdAsync(Guid productId);

	/// <summary>
	/// Gets the average rating for a specific product
	/// </summary>
	/// <param name="productId">The product ID</param>
	/// <returns>The average rating as a double</returns>
	Task<double> GetAverageRatingForProductAsync(Guid productId);

	/// <summary>
	/// Gets the total number of reviews for a specific product
	/// </summary>
	/// <param name="productId">The product ID</param>
	/// <returns>The count of reviews for the specified product</returns>
	Task<int> GetReviewCountForProductAsync(Guid productId);

	/// <summary>
	/// Adds a new review for a product
	/// </summary>
	/// <param name="review">The review to add</param>
	/// <returns>A task representing the asynchronous operation</returns>
	Task AddReviewAsync(Review review);
}