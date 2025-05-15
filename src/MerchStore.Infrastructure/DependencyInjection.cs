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

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        services.AddPersistenceServices(configuration, env.IsDevelopment());
        services.AddReviewServices(configuration);
        return services;
    }

    // 🧩 Lägg till den här metoden i samma fil om du vill
    public static IServiceCollection AddReviewServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Exempel: services.AddScoped<IReviewService, ReviewService>();
        return services;
    }

    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
    {
        var envConnectionString = Environment.GetEnvironmentVariable("AZURE_SQL_DB_CONNECTIONSTRING");

        if (!string.IsNullOrWhiteSpace(envConnectionString))
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(envConnectionString,
                    sqlOptions => sqlOptions.EnableRetryOnFailure()));
        }
        else if (isDevelopment)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
        }
        else
        {
            var fallbackConnection = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("No valid connection string found for production.");

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(fallbackConnection,
                    sqlOptions => sqlOptions.EnableRetryOnFailure()));
        }

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IRepositoryManager, RepositoryManager>();
        services.AddLogging();
        services.AddScoped<AppDbContextSeeder>();

        return services;
    }
    public static async Task SeedDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<AppDbContextSeeder>();
        await seeder.SeedAsync();
    }
}
