using System.Reflection;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MerchStore.Application.Services.Interfaces;
using MerchStore.Application.Services.Implementations;

namespace MerchStore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register MediatR handlers in this assembly
        services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

        // Register application services
        services.AddScoped<IReviewService, ReviewService>();

        return services;
    }
}