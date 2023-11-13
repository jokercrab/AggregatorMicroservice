using Microsoft.Extensions.Configuration;
using Aggregator.DataFetchers;
using Aggregator.Services;
using Aggregator.DataStructs;
namespace Microsoft.Extensions.DependencyInjection;

public static class AggregatorServiceCollectionExtensions
{
    public static IServiceCollection AddConfig(
        this IServiceCollection services, IConfiguration config)
    {
        services.Configure<SettingsDB>(config.GetSection("AnimeDB"));

        return services;
    }
    public static IServiceCollection AddDependencyGroup(
        this IServiceCollection services)
    {
        services.AddScoped<IDataFetcher, Animevost>();
        services.AddScoped<IDataFetcher, Anilibria>();
        services.AddSingleton<PeriodicTask>();
        services.AddSingleton<IDataBase,MongoHelper>();
        services.AddHostedService(provider => provider.GetRequiredService<PeriodicTask>());
        return services;
    }
}
