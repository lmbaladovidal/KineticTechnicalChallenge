using System.Text.Json.Serialization;

namespace KineticTechnicalChallenge.Core.DTO.Response
{
    public class ProcessResponse
    {
        public ProcessInfoDTO ProcessInfoDTO { get; set; }
        [JsonPropertyName("results")]
        public AnalysisResultDTO Results { get; set; }
    }
}
