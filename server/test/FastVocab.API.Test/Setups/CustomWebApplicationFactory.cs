using FastVocab.Infrastructure.Data.EFCore;
using FastVocab.Infrastructure.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FastVocab.API.Test.Setups;

/// <summary>
/// Custom WebApplicationFactory for integration testing
/// Configures SQLite in-memory database and test-specific services
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IDisposable
{
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        
        builder.ConfigureServices((context, services) =>
        {
            // Remove the production Infrastructure registration
            var infrastructureDescriptors = services
                .Where(d => d.ImplementationType?.Assembly == typeof(RegistrationExtensions).Assembly)
                .ToList();
            
            foreach (var descriptor in infrastructureDescriptors)
            {
                services.Remove(descriptor);
            }

            // Remove all EF Core related services
            var efCoreDescriptors = services
                .Where(d => d.ServiceType.Namespace?.StartsWith("Microsoft.EntityFrameworkCore") == true ||
                           d.ServiceType == typeof(AppDbContext) ||
                           d.ServiceType.IsGenericType && d.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>))
                .ToList();

            foreach (var descriptor in efCoreDescriptors)
            {
                services.Remove(descriptor);
            }

            // Create and configure SQLite connection
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            // Register Infrastructure with SQLite for testing
            services.AddInfrastructureForTesting(options =>
            {
                options.UseSqlite(_connection);
            });
        });
    }

    /// <summary>
    /// Initialize database schema after host is created
    /// </summary>
    public void InitializeDatabase()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _connection?.Close();
            _connection?.Dispose();
        }
        base.Dispose(disposing);
    }

    public new void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

