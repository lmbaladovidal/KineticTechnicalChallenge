using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KineticTechnicalChallenge.Core.Models
{
    public class AnalysisResult
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("ProcessInfo")]
        public Guid ProcessInfoId { get; set; }
        public ProcessInfo ProcessInfo { get; set; }

        public int TotalWords { get; set; }
        public int TotalLines { get; set; }
        public int TotalCharacters { get; set; }

        public string MostFrequentWordsJson { get; set; }

        public string FilesProcessedJson { get; set; }
    }
}
