using KineticTechnicalChallenge.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace KineticTechnicalChallenge.API.Extensions
{
    public static class DBExtensions
    {
        public static IServiceCollection AddDb(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("SqlServerConnection");
            services.AddDbContext<DocumentContext>(options =>
            {
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
            });

            return services;
        }
    }
}
