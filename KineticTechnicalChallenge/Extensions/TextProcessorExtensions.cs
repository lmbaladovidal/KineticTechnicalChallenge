using KineticTechnicalChallenge.Core.Contract.Configuration;

namespace KineticTechnicalChallenge.API.Extensions
{
    public static class TextProcessorExtensions
    {
        public static IServiceCollection AddTextProcessor(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TextProcessingSettings>(configuration.GetSection("TextProcessing"));
            return services;
        }
    }
}
