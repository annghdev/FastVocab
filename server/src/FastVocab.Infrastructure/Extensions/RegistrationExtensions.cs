using FastVocab.Application.Common.Interfaces;
using FastVocab.Domain.Repositories;
using FastVocab.Infrastructure.Data.EFCore;
using FastVocab.Infrastructure.Data.Repositories;
using FastVocab.Infrastructure.Extensions.Options;
using FastVocab.Infrastructure.Services.FileServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FastVocab.Infrastructure.Extensions;

public static class RegistrationExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Options
        services.Configure<CloudinarySettings>(configuration.GetSection(CloudinarySettings.Position));

        // DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("SqlServer"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        // Register repositories and other services
        AddRepositoriesAndServices(services);

        return services;
    }

    /// <summary>
    /// For testing - allows custom DbContext configuration
    /// </summary>
    public static IServiceCollection AddInfrastructureForTesting(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> configureDbContext)
    {
        // DbContext with custom configuration
        services.AddDbContext<AppDbContext>(configureDbContext);

        // Register repositories and other services
        AddRepositoriesAndServices(services);

        return services;
    }

    private static void AddRepositoriesAndServices(IServiceCollection services)
    {
        // Repositories - Generic
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Repositories - Specific
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITopicRepository, TopicRepository>();
        services.AddScoped<IWordRepository, WordRepository>();
        services.AddScoped<ICollectionRepository, CollectionRepository>();
        services.AddScoped<IPracticeSessionRepository, PracticeSessionRepository>();
        services.AddScoped<ITakedWordRepository, TakedWordRepository>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Application Services
        services.AddScoped<IFileStorageService, CloudinaryService>();
        
    }
}
