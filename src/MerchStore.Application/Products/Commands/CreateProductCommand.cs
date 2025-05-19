using MediatR;
using System;
public class CreateProductCommand : IRequest<Guid>
{
	public CreateProductRequest Product { get; }

	Public CreateProductCommand(CreateProductRequest product)
	{
		Product = product ?? throw new ArgumentNullException(nameof(product));
	}
}