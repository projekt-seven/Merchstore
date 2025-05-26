using MediatR;
using MerchStore.Application.Products.DTOs;

namespace MerchStore.Application.Products.Queries;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDetailsDto>;