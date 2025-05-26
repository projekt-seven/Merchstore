using MerchStore.Domain.Entities;
using MerchStore.Domain.ValueObjects;

namespace MerchStore.Domain.Interfaces;

/// <summary>
/// Repository interface for accessing product reviews
/// </summary>
public interface IReviewRepository
{
	/// <summary>
	/// Gets both reviews and statistics for a product in a single operation
	/// </summary>
	/// <param name="productId">The product ID</param>
	/// <returns>A tuple containing reviews and review statistics</returns>
	Task<(IEnumerable<Review> Reviews, ReviewStats Stats)> GetProductReviewsAsync(Guid productId);
}