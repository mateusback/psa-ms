using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using psa_legacy_sis.Database.Repositories;
using psa_legacy_sis.Database.Settings;
using psa_legacy_sis.Domain.Repositories;

namespace psa_legacy_sis.Database;

public static class IoC
{
    internal const string DataSourceKey = nameof(ConnectionStringsSettings.Legado);

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddDbConnection(configuration)
            .AddRepositories();
    }

    private static IServiceCollection AddDbConnection(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddConfigs()
            .AddDbDataSource()
            .AddDbContext(configuration)
            .AddTransient<IDbConnectionFactory, AppConnectionFactory>();
    }

    private static IServiceCollection AddConfigs(this IServiceCollection services)
    {
        services
            .AddOptions<ConnectionStringsSettings>()
            .BindConfiguration(ConnectionStringsSettings.SectionName)
            .ValidateDataAnnotations()
            .Validate(c => !string.IsNullOrWhiteSpace(c.Legado),
                "Connection string 'Legado' não pode ser vazia.")
            .ValidateOnStart();

        return services;
    }

    private static IServiceCollection AddDbDataSource(this IServiceCollection services)
    {
        return services
            .AddKeyedSingleton(DataSourceKey, (sp, _) =>
            {
                var cfg = sp.GetRequiredService<IOptions<ConnectionStringsSettings>>().Value;
                var builder = new NpgsqlDataSourceBuilder(cfg.Legado);
                return builder.Build();
            });
    }
    
    private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("Legado"));
        });

        return services;
    }
    

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        return services
            .AddScoped<ICustomerRepository, CustomerRepository>();
    }
}
