namespace KineticTechnicalChallenge.Core.Data.Models
{
    public class ProgressInfo
    {
        public int TotalFiles { get; set; }
        public int ProcessedFiles { get; set; }
        public int Percentage =>
            TotalFiles == 0 ? 0 : (int)Math.Round((double)ProcessedFiles / TotalFiles * 100);
    }
}
