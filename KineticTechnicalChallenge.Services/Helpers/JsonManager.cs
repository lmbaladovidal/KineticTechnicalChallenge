using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace KineticTechnicalChallenge.Services.Helpers
{
    public static class JsonManager
    {
        public static T SafeDeserialize<T>(string json, ILogger _logger)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json) ?? Activator.CreateInstance<T>();
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Error al deserializar JSON a {Type}: {Json}", typeof(T).Name, json);
                return Activator.CreateInstance<T>();
            }
        }
    }
}
