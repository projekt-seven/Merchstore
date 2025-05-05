using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ReviewApiFunction
{
    public class GetProductReviews
    {
        private readonly ILogger<GetProductReviews> _logger;

        public GetProductReviews(ILogger<GetProductReviews> logger)
        {
            _logger = logger;
        }

        [Function("GetProductReviews")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
