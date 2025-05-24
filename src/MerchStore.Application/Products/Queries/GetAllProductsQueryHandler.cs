using MediatR;
using MerchStore.Application.Products.Queries;
using MerchStore.Application.Common.Interfaces;
using MerchStore.Domain.Entities;

namespace MerchStore.Application.Products.Queries;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<ProductListItemDto>>
{
	private readonly IProductRepository _productRepository;

	public GetAllProductsQueryHandler(IProductRepository productRepository)
	{
		_productRepository = productRepository;
	}

	public async Task<List<ProductListItemDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
	{
		var products = await _productRepository.GetAllAsync(cancellationToken);
var sorted = products
    .OrderByDescending(p => p.Id) // om `CreatedAt` finns
    .Select(p => new ProductListItemDto
		{
			Id = p.Id,
			Name = p.Name,
			Category = p.Category,
			Price = p.Price.Amount,
			StockQuantity = p.StockQuantity
		}).ToList();
		return sorted;
	}
}