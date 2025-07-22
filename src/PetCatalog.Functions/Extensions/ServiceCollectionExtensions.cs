using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetCatalog.Application.Interfaces;
using PetCatalog.Application.Services;
using PetCatalog.Domain.Interfaces;
using PetCatalog.Functions.Services;
using PetCatalog.Functions.Services.Interfaces;
using PetCatalog.Infrastructure.Data;
using PetCatalog.Infrastructure.Repositories;

namespace PetCatalog.Functions.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPetCatalogServices(this IServiceCollection services)
    {
        // Get configuration
        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetService<IConfiguration>();
        
        if (configuration == null)
        {
            throw new InvalidOperationException("Configuration not available");
        }

        // Add Entity Framework
        var connectionString = Environment.GetEnvironmentVariable("POSTGRES_CONN") 
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Database connection string not configured");

        services.AddDbContext<PetCatalogDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        // Add repositories
        services.AddScoped<IPetRepository, PetRepository>();

        // Add application services
        services.AddScoped<IPetApplicationService, PetApplicationService>();

        // Add function services
        services.AddScoped<IPetService, PetService>();

        // Add Azure Storage
        var storageAccountName = Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_NAME");
        var storageAccountKey = Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_KEY");
        
        if (!string.IsNullOrEmpty(storageAccountName) && !string.IsNullOrEmpty(storageAccountKey))
        {
            // Add blob storage service here if needed
        }

        return services;
    }
}
