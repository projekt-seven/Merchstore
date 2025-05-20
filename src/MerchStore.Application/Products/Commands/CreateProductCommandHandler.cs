using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MerchStore.Application.Common.Interfaces;
using MerchStore.Application.Products.Commands;
using MerchStore.Domain.Entities;
using MerchStore.Domain.ValueObjects;

namespace MerchStore.Application.Products.Commands
{
	public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
	{
		private readonly IProductRepository _productRepository;
		private readonly IUnitOfWork _unitOfWork;

		public CreateProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
		{
			_productRepository = productRepository;
			_unitOfWork = unitOfWork;
		}

		public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
		{
			var dto = request.Product;

			var product = new Product(
				dto.Name,
				dto.Description,
				string.IsNullOrWhiteSpace(dto.ImageUrl) ? null : new Uri(dto.ImageUrl),
				Money.FromSEK(dto.Price),
				dto.StockQuantity,
				dto.Category,
				dto.Tags
			);

			await _productRepository.AddAsync(product);
			await _unitOfWork.SaveChangesAsync(cancellationToken);

			return product.Id;
		}
	}
}