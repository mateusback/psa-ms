using System.Reflection;
using Microsoft.AspNetCore.Localization;
using psa_legacy_sis.Database;
using psa_legacy_sis.ExceptionHandling;

namespace psa_legacy_sis;

internal static class IoC
{
internal static IServiceCollection AddApplication(this IHostApplicationBuilder builder)
{
    return builder.Services
        .AddApiCore()
        .AddAppSwagger()
        .AddDatabase(builder.Configuration);
}

    private static IServiceCollection AddAppSwagger(this IServiceCollection services)
    {
        return services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(c =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
    }
    

    private static IServiceCollection AddApiCore(this IServiceCollection services)
    {
        services.AddControllers();
        return services
            .AddExceptionHandler<GlobalExceptionHandler>()
            .AddProblemDetails()
            .Configure<RequestLocalizationOptions>(options => options.DefaultRequestCulture = new RequestCulture("pt-BR"));
    }
}
