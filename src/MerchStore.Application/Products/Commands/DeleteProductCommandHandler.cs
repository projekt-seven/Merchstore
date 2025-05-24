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
	public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
	{
		private readonly IProductRepository _productRepository;
		private readonly IUnitOfWork _unitOfWork;

		public DeleteProductCommandHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
		{
			_productRepository = productRepository;
			_unitOfWork = unitOfWork;
		}

		public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
		{
			var product = await _productRepository.GetByIdAsync(request.Id);

			if (product == null)
				return false;

			await _productRepository.RemoveAsync(product);
			await _unitOfWork.SaveChangesAsync(cancellationToken);

			return true;
		}
	}
}