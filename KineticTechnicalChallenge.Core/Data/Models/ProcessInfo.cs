using KineticTechnicalChallenge.Core.Contract.Enums;
using System.ComponentModel.DataAnnotations;

namespace KineticTechnicalChallenge.Core.Data.Models
{
    public class ProcessInfo
    {
        [Key]
        public Guid Id { get; set; }
        public ProcessStatus Status { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EstimatedCompletion { get; set; }
        public int TotalFiles { get; set; }
        public int ProcessedFiles { get; set; }
        public int Percentage { get; set; }
        public AnalysisResult Results { get; set; }
        public string FilesJson { get; set; }
    }
}
