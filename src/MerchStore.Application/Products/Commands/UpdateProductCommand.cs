using MediatR;

namespace MerchStore.Application.Products.Commands;

public class UpdateProductCommand : IRequest<Unit>
{
	public Guid Id { get; }
	public CreateProductRequest Request { get; }

	public UpdateProductCommand(Guid id, CreateProductRequest request)
	{
		Id = id;
		Request = request;
	}
}