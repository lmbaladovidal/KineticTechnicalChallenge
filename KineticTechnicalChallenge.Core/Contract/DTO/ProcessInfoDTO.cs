using KineticTechnicalChallenge.Core.Data.Models;
using System.Text.Json.Serialization;

namespace KineticTechnicalChallenge.Core.Contract.DTO
{
    public class ProcessInfoDTO
    {
        [JsonPropertyName("process_id")]
        public Guid Guid { get; set; }
        [JsonPropertyName("status")]
        public ProcessStatus Status { get; set; }
        [JsonPropertyName("progress")]
        public ProgressInfo? ProgressInfo { get; set; }
        [JsonPropertyName("started_at")]
        public DateTime StartedAt { get; set; }
        [JsonPropertyName("estimated_completion")]
        public DateTime? EstimatedCompletion { get; set; }
    }
}
