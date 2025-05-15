using System.Reflection;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
/* using MerchStore.Application.Common.Behaviors; */
using MerchStore.Application.Services.Interfaces;
using MerchStore.Application.Services.Implementations;

namespace MerchStore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register application services
        services.AddScoped<IReviewService, ReviewService>(); // Add this line

        return services;
    }
}