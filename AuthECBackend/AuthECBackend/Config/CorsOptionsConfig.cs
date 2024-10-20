using Microsoft.AspNetCore.Cors.Infrastructure;

namespace AuthECBackend.Config
{
    public class CorsOptionsConfig
    {
        public static void ConfigureCorsPolicy(CorsPolicyBuilder options)
        {
            // Define CORS options
            var allowedOrigins = new[] { "http://localhost:4200" };
            var allowedHeaders = new[] {
                "accept",
                "authorization",
                "content-type",
                "user-agent",
                "x-csrftoken",
                "x-requested-with"
            };
            var allowedMethods = new[] {
                "GET",
                "POST",
                "PUT",
                "PATCH",
                "DELETE",
                "OPTIONS"
            };
            var allowCredentials = true;


            // Configure CORS policy
            options.WithOrigins(allowedOrigins)
                .WithHeaders(allowedHeaders)
                .WithMethods(allowedMethods);

            if (allowCredentials)
            {
                options.AllowCredentials();
            }
        }
    }
}
