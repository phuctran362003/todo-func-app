using Application.Interfaces;
using Application.Services;
using Infrastructure;
using Infrastructure.Commons;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Csv.FuncApp1.IOContainer;

public static class IocContainer
{
    public static IServiceCollection SetupIocContainer(this IServiceCollection services)
    {
        services.SetupBusinessServicesLayer();

        services.AddHttpContextAccessor();
        services.SetupCosmosDb();

        return services;
    }

    public static IServiceCollection SetupBusinessServicesLayer(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(CosmosGenericRepository<>));
        services.AddSingleton<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IClaimsService, ClaimsService>();
        services.AddSingleton<ICurrentTime, CurrentTime>();
        services.AddSingleton<ILoggerService, LoggerService>();
        services.AddScoped<ITodoService, TodoService>();
        services.AddHttpContextAccessor();

        return services;
    }
    public static IServiceCollection SetupCosmosDb(this IServiceCollection services)
    {
        var config = services.BuildServiceProvider()
            .GetRequiredService<IConfiguration>();
        // Load config section
        services.Configure<CosmosDbContext.CosmosDbSettings>(
            config.GetSection("CosmosDb"));

        // Register CosmosDbContext
        services.AddSingleton<CosmosDbContext>();

        // Build a temporary provider to run InitializeAsync()
        using (var provider = services.BuildServiceProvider())
        {
            var cosmos = provider.GetRequiredService<CosmosDbContext>();
            cosmos.InitializeAsync().GetAwaiter().GetResult();
        }

        return services;
    }
}