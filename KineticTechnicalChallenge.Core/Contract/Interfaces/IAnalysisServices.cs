using KineticTechnicalChallenge.Core.Contract.DTO;

namespace KineticTechnicalChallenge.Core.Contract.Interfaces
{
    public interface IAnalysisServices
    {
        public AnalysisResultDTO AnalyzeFiles(List<string> fileContents, List<string> fileNames);
        public AnalysisResultDTO AnalyzeFile(string fileContent, string fileName);
    }
}
