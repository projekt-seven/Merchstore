using MediatR;
using System;
namespace MerchStore.Application.Products.Commands
{


	public class CreateProductCommand : IRequest<Guid>
	{
		public CreateProductRequest Product { get; }

		public CreateProductCommand(CreateProductRequest product)
		{
			Product = product ?? throw new ArgumentNullException(nameof(product));
		}
	}
}