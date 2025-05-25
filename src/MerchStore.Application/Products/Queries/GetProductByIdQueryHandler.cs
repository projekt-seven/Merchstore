using MediatR;
using MerchStore.Application.Products.DTOs;
using MerchStore.Application.Common.Interfaces;

namespace MerchStore.Application.Products.Queries;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDetailsDto?>
{
	private readonly IProductRepository _productRepository;

	public GetProductByIdQueryHandler(IProductRepository productRepository)
	{
		_productRepository = productRepository;
	}

	public async Task<ProductDetailsDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
	{
		var product = await _productRepository.GetByIdAsync(request.Id);
		if (product == null)
			return null;

		return new ProductDetailsDto
		{
			Id = product.Id,
			Name = product.Name,
			Description = product.Description,
			Price = product.Price.Amount,
			Category = product.Category,
			ImageUrl = product.ImageUrl?.ToString(),
			StockQuantity = product.StockQuantity,
			Tags = product.Tags
		};
	}
}