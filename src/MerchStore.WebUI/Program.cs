using System.Reflection;
using MediatR;
using System.Text.Json.Serialization;
using MerchStore.Application;
using MerchStore.Infrastructure;
using MerchStore.Models;
using MerchStore.Middleware;
using MerchStore.WebUI.Authentication.ApiKey;
using MerchStore.WebUI.Endpoints;
using MerchStore.WebUI.Infrastructure;
using MerchStore.Application.Services.Interfaces;
using MerchStore.Infrastructure.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(MerchStore.Application.AssemblyReference).Assembly);
});

if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddApplicationInsightsTelemetry();
}
else
{
    builder.Services.AddApplicationInsightsTelemetry(options =>
    {
        options.EnableAdaptiveSampling = false;
        options.ApplicationVersion = "dev-" + DateTime.Now.ToString("yyyyMMdd-HHmm");
        options.EnableDebugLogger = true;
    });

    builder.Services.Configure<TelemetryConfiguration>(config =>
    {
        config.TelemetryChannel = new InMemoryChannel
        {
            DeveloperMode = true
        };
    });
}


// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Use snake_case for JSON serialization
        options.JsonSerializerOptions.PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy();
        options.JsonSerializerOptions.DictionaryKeyPolicy = new JsonSnakeCaseNamingPolicy();
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

var apiKeyValue = Environment.GetEnvironmentVariable("BASIC_PRODUCT_API_KEY")
                ?? builder.Configuration["ApiKey:Value"];

if (string.IsNullOrWhiteSpace(apiKeyValue))
{
    throw new InvalidOperationException("API key must be provided via environment variable or appsettings.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.Name = "MerchStore.Auth";

    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;

    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
})
.AddApiKey(apiKeyValue);

// Add API Key authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole(UserRoles.Administrator));

    options.AddPolicy("AdminOrCustomer", policy =>
        policy.RequireRole(UserRoles.Administrator, UserRoles.Customer));

    options.AddPolicy("ApiKeyPolicy", policy =>
        policy.AddAuthenticationSchemes(ApiKeyAuthenticationDefaults.AuthenticationScheme)
              .RequireAuthenticatedUser());
});

// Add Application services - this includes Services, Interfaces, etc.
builder.Services.AddApplication();

// Add Infrastructure services - this includes DbContext, Repositories, etc.
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);

builder.Services.AddHttpClient<IAiReviewService, AiReviewService>();

builder.Services.AddHttpClient("AiReviewsHttpClient", client =>
{
    client.BaseAddress = new Uri("https://aireviews.drillbi.se"); // byt till korrekt adress
});

//builder.Services.AddHttpClient<AiReviewsClient>();

// Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MerchStore API",
        Version = "v1",
        Description = "API for MerchStore product catalog"
    });

    options.SwaggerDoc("admin", new OpenApiInfo
    {
        Title = "Admin API",
        Version = "v1"
    });

    // Include XML comments (if enabled in project settings)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Add API key security definition
    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key required to access endpoints. Use: X-API-Key: API_KEY",
        In = ParameterLocation.Header,
        Name = "X-API-Key",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme"
    });

    // ✅ Add Bearer token support
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    // Add security requirements
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            Array.Empty<string>()
        },
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Prevent duplicate operation IDs for minimal APIs
    options.CustomOperationIds(apiDesc =>
    {
        return apiDesc.ActionDescriptor?.DisplayName;
    });

    // Add grouping via namespace
    options.DocInclusionPredicate((docName, apiDesc) =>
    {
        if (docName == "v1")
            return !apiDesc.GroupName?.Equals("Admin", StringComparison.OrdinalIgnoreCase) ?? true;

        if (docName == "admin")
            return apiDesc.GroupName?.Equals("Admin", StringComparison.OrdinalIgnoreCase) ?? false;

        return false;
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()  // Allow requests from any origin
                   .AllowAnyHeader()  // Allow any headers
                   .AllowAnyMethod(); // Allow any HTTP method
        });
});


var app = builder.Build();

// CLI-baserad seeding – körs bara om flagga ges
if (args.Contains("--seed") || args.Contains("--reset-seed"))
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        var seeder = services.GetRequiredService<MerchStore.Infrastructure.Persistence.AppDbContextSeeder>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        var reset = args.Contains("--reset-seed");

        logger.LogInformation($"Running database seeding (Reset={reset})...");
        await seeder.SeedAsync(resetDatabase: reset);
        logger.LogInformation("Seeding complete.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Seeding failed: {ex.Message}");
        throw;
    }

    return; // Avsluta programmet efter seeding
}


// Konfigurera felhantering och HSTS i Production
if (app.Environment.IsProduction())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Aktivera Swagger i både Development och Production
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MerchStore API V1");
        options.SwaggerEndpoint("/swagger/admin/swagger.json", "Admin API");
    });
}
builder.Logging.AddConsole();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseSession();         
app.UseSessionLogging();  
app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapMinimalProductEndpoints();

app.Run();

