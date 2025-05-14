using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using ReviewApiFunction;

var builder = FunctionsApplication.CreateBuilder(args);

// Registrera tjänster direkt på builder.Services
builder.Services.AddSingleton<IOpenApiConfigurationOptions, SwaggerConfiguration>();
builder.Services.AddSingleton<IOpenApiCustomUIOptions, SwaggerUIConfiguration>();

builder.ConfigureFunctionsWebApplication();

// Application Insights (valfritt)
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
