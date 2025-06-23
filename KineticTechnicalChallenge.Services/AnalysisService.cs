using KineticTechnicalChallenge.Core.Contract.DTO;
using KineticTechnicalChallenge.Core.Contract.Interfaces;
using System.Text.RegularExpressions;

namespace KineticTechnicalChallenge.Services
{
    public class AnalysisService : IAnalysisServices
    {

        public AnalysisResultDTO AnalyzeFile(string fileContent, string fileName)
        {
            var totalWords = 0;
            var totalLines = 0;
            var totalChars = 0;
            var wordFrequency = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            var lines = fileContent.Split(Environment.NewLine);
            totalLines += lines.Length;
            totalChars += fileContent.Length;

            foreach (var line in lines)
            {
                var words = Regex.Matches(line, @"\b[\w']+\b", RegexOptions.Multiline);
                totalWords += words.Count;

                foreach (Match match in words)
                {
                    var word = match.Value.ToLowerInvariant();
                    if (!wordFrequency.TryAdd(word, 1))
                        wordFrequency[word]++;
                }
            }

            var mostFrequentWords = wordFrequency
                .OrderByDescending(kvp => kvp.Value)
                .Take(5)
                .Select(kvp => kvp.Key)
                .ToList();

            return new AnalysisResultDTO
            {
                TotalWords = totalWords,
                TotalLines = totalLines,
                TotalCharacters = totalChars,
                MostFrequentWords = mostFrequentWords,
                FilesProcessed = new List<string> { fileName }
            };
        }

        //Gen with IA
        public AnalysisResultDTO AnalyzeFiles(List<string> fileContents, List<string> fileNames)
        {
            var totalWords = 0;
            var totalLines = 0;
            var totalChars = 0;
            var wordFrequency = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < fileContents.Count; i++)
            {
                var content = fileContents[i];
                var lines = content.Split(Environment.NewLine);
                totalLines += lines.Length;
                totalChars += content.Length;

                foreach (var line in lines)
                {
                    var words = Regex.Matches(line, @"\b[\w']+\b", RegexOptions.Multiline);
                    totalWords += words.Count;

                    foreach (Match match in words)
                    {
                        var word = match.Value.ToLowerInvariant();
                        if (!wordFrequency.TryAdd(word, 1))
                            wordFrequency[word]++;
                    }
                }
            }

            var mostFrequentWords = wordFrequency
                .OrderByDescending(kvp => kvp.Value)
                .Take(5)
                .Select(kvp => kvp.Key)
                .ToList();

            return new AnalysisResultDTO
            {
                TotalWords = totalWords,
                TotalLines = totalLines,
                TotalCharacters = totalChars,
                MostFrequentWords = mostFrequentWords,
                FilesProcessed = fileNames
            };
        }
    }
}
