using MediatR;
using Microsoft.AspNetCore.Mvc;
using MerchStore.Application.Products.Commands;

namespace MerchStore.WebUI.Controllers.Api.Products;


/// <summary>
/// Admin API controller for product management (create, update, delete).
/// </summary>
[ApiController]
[Route("api/admin/products")]
[ApiExplorerSettings(GroupName = "Admin")]
public class AdminProductsApiController : ControllerBase
{
	private readonly IMediator _mediator;

	public AdminProductsApiController(IMediator mediator)
	{
		_mediator = mediator;
	}

	/// <summary>
	/// Creates a new product
	/// </summary>
	/// <param name="request">The product data</param>
	/// <returns>The ID of the created product</returns>
	[HttpPost]
	public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var id = await _mediator.Send(new CreateProductCommand(request));
		return CreatedAtAction(nameof(GetById), new { id }, new { id });
	}

	[HttpGet("{id}")]
	public async Task<IActionResult> GetById(Guid id)
	{
		// TODO: implementera GetById-command i nästa steg
		return Ok($"(Placeholder) Admin-produkt med ID: {id}");
	}
}