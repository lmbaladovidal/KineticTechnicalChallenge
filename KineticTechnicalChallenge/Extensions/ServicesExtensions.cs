using KineticTechnicalChallenge.Core.Contract.Configuration;
using KineticTechnicalChallenge.Core.Contract.Interfaces;
using KineticTechnicalChallenge.Services;
using KineticTechnicalChallenge.Services.BackGroundServices;

namespace KineticTechnicalChallenge.API.Extensions
{
    public static class ServiceExtensions
    {

        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IProcessServices, ProcessService>();
            services.AddTransient<IAnalysisServices, AnalysisService>();
            services.AddTransient<IProcessStorage, ProcessStorageService>();
            services.AddSingleton<IProcessQueue, InMemoryProcessQueue>();
            services.AddHostedService<ProcessWorker>();
            //services.AddTransient<ErrorHandlerMiddleware>();
            return services;
        }
    }
}
