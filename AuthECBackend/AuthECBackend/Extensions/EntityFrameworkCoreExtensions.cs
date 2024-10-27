using System.Runtime.CompilerServices;
using AuthECBackend.Data;
using AuthECBackend.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthECBackend.Extensions
{
    public static class EntityFrameworkCoreExtensions
    {
        public static IServiceCollection InjetcDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => 
                    options.UseSqlServer(configuration.GetConnectionString("DevDbConnection")));
            return services;

        }
    }
}
