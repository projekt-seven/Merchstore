using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MerchStore.Application.Common.Interfaces;
using MerchStore.Application.Products.Commands;
using MerchStore.Domain.Entities;
using MerchStore.Domain.ValueObjects;

namespace MerchStore.Application.Products.Commands;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
{
	private readonly IProductRepository _repository;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateProductCommandHandler(IProductRepository repository, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_unitOfWork = unitOfWork;
	}

	public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
	{
		var product = await _repository.GetByIdAsync(request.Id);
		if (product == null)
			throw new Exception("Product not found");

		product.UpdateDetails(
			request.Request.Name,
			request.Request.Description,
			string.IsNullOrWhiteSpace(request.Request.ImageUrl) ? null : new Uri(request.Request.ImageUrl),
			request.Request.Category
		);

		product.UpdatePrice(Money.FromSEK(request.Request.Price));
		product.UpdateStock(request.Request.StockQuantity);

		if (request.Request.Tags != null)
			product.UpdateTags(request.Request.Tags);

		await _unitOfWork.SaveChangesAsync(cancellationToken);

		return Unit.Value;
	}
}
