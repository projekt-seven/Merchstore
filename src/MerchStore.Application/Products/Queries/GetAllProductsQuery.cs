using MediatR;

namespace MerchStore.Application.Products.Queries;

public class GetAllProductsQuery : IRequest<List<ProductListItemDto>>
{
}