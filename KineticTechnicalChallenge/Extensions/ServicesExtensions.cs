using KineticTechnicalChallenge.Core.Interfaces;
using KineticTechnicalChallenge.Services;

namespace KineticTechnicalChallenge.API.Extensions
{
    public static class ServiceExtensions
    {

        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IProcessServices, ProcessService>();
            //services.AddTransient<ErrorHandlerMiddleware>();
            return services;
        }
    }
}
