using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MerchStore.Application.Common.Interfaces;
using MerchStore.Domain.Interfaces;
using MerchStore.Infrastructure.Persistence;
using MerchStore.Infrastructure.Persistence.Repositories;
using MerchStore.Infrastructure.ExternalServices.Reviews.Configurations;
using MerchStore.Infrastructure.ExternalServices.Reviews;

namespace MerchStore.Infrastructure;

/// <summary>
/// Contains extension methods for registering Infrastructure layer services with the dependency injection container.
/// This keeps all registration logic in one place and makes it reusable.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Infrastructure layer services to the DI container
    /// </summary>
    /// <param name="services">The service collection to add services to</param>
    /// <param name="configuration">The configuration for database connection strings</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Call specific registration methods
        services.AddPersistenceServices(configuration);
        services.AddReviewServices(configuration);
        // Add calls to other infrastructure registration methods here if needed (e.g., file storage, email service)

        return services;
    }

    /// <summary>
    /// Registers services related to data persistence (EF Core, Repositories, UnitOfWork).
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register DbContext with in-memory database
        // In a real application, you'd use a real database
        services.AddDbContext<AppDbContext>(options =>
            options.sqlite(configuration.GetConnectionString("MerchStoreDb")));

        // Register repositories
        services.AddScoped<IProductRepository, ProductRepository>();

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Repository Manager
        services.AddScoped<IRepositoryManager, RepositoryManager>();

        // Add logging services if not already added
        services.AddLogging();

        // Register DbContext seeder
        services.AddScoped<AppDbContextSeeder>();

        return services;
    }

    /// <summary>
    /// Registers services related to the External Review API integration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddReviewServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register External Api options
        services.Configure<ReviewApiOptions>(configuration.GetSection(ReviewApiOptions.SectionName));

        // Register HttpClient for ReviewApiClient
        services.AddHttpClient<ReviewApiClient>()
            .SetHandlerLifetime(TimeSpan.FromMinutes(5)); // Set a lifetime for the handler

        // Register the mock service
        services.AddSingleton<MockReviewService>();

        // Register the repository with the circuit breaker
        services.AddScoped<IReviewRepository, ExternalReviewRepository>();

        return services;
    }

    /// <summary>
    /// Seeds the database with initial data.
    /// This is an extension method on IServiceProvider to allow it to be called from Program.cs.
    /// </summary>
    /// <param name="serviceProvider">The service provider to resolve dependencies</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public static async Task SeedDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<AppDbContextSeeder>();
        await seeder.SeedAsync();
    }
}