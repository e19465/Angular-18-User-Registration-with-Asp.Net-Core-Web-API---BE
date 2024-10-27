using AuthECBackend.Config;

namespace AuthECBackend.Extensions
{
    public static class AppConfigExtensions
    {
        public static WebApplication ConfigureCORS(this WebApplication app, IConfiguration configuration)
        {
            app.UseCors(options => CorsOptionsConfig.ConfigureCorsPolicy(options));
            return app;
        }

        public static IServiceCollection AddAppConfig(this IServiceCollection services, IConfiguration configuration) {
            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
            return services;
        }
    }
}
