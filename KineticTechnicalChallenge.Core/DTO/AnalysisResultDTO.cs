namespace KineticTechnicalChallenge.Core.DTO
{
    public class AnalysisResultDTO
    {
        public int TotalWords { get; set; }
        public int TotalLines { get; set; }
        public List<string> MostFrequentWords { get; set; }
        public List<string> FilesProcessed { get; set; }
    }
}
