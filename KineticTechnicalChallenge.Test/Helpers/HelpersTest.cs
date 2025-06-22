using KineticTechnicalChallenge.Core.Contract.Configuration;

namespace KineticTechnicalChallenge.Test.Helpers
{
    internal static class HelpersTest
    {
        internal static void CreateTempTestFiles(TextProcessingSettings _settings)
        {
            for (int i = 1; i <= 3; i++)
            {
                var filePath = Path.Combine(_settings.InputFolder, $"test{i}.txt");
                File.WriteAllText(filePath, $"Test content for file {i}");
            }
        }

        internal static void CreateAdditionalTempFiles(TextProcessingSettings _settings, int count)
        {
            for (int i = 4; i <= count + 3; i++)
            {
                var filePath = Path.Combine(_settings.InputFolder, $"test{i}.txt");
                File.WriteAllText(filePath, $"Test content for file {i}");
            }
        }

        internal static void CleanupTempTestFiles(TextProcessingSettings _settings)
        {
            try
            {
                var files = Directory.GetFiles(_settings.InputFolder, "test*.txt");
                foreach (var file in files)
                {
                    File.Delete(file);
                }
            }
            catch
            {
                // Ignore cleanup errors in tests
            }
        }
    }
}
