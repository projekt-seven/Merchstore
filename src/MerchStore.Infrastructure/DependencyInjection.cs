using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
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
    /// Registers all infrastructure-related services including the database.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application's configuration.</param>
    /// <param name="env">Hosting environment (to detect Development or Production).</param>
    /// <returns>The modified service collection for chaining.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        // Register database services depending on environment
        services.AddPersistenceServices(configuration, env.IsDevelopment());

        // Register additional infrastructure services here if needed (e.g. email, blob storage)
        services.AddReviewServices(configuration); // Optional - only if implemented

        return services;
    }

    /// <summary>
    /// Registers persistence-related services: DbContext, Repositories, UnitOfWork, etc.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="isDevelopment">Flag indicating whether the app is running in Development.</param>
    /// <returns>The modified service collection for chaining.</returns>
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
    {
        if (isDevelopment)
        {
            // Use SQLite locally in development
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
        }
        else
        {
            // Use Azure SQL or other production DB, from environment variable or fallback to config
            var connectionString = Environment.GetEnvironmentVariable("DEFAULT_CONNECTION") 
                                ?? configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString,
                    sqlOptions => sqlOptions.EnableRetryOnFailure()));
        }

        // Register repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Register Unit of Work and repository manager
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IRepositoryManager, RepositoryManager>();

        // Register logger
        services.AddLogging();

        // Register seeder for test/admin data
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