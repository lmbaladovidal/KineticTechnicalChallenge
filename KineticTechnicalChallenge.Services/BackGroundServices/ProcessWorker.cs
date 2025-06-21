using KineticTechnicalChallenge.Core.Contract.Configuration;
using KineticTechnicalChallenge.Core.Contract.Interfaces;
using KineticTechnicalChallenge.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace KineticTechnicalChallenge.Services.BackGroundServices
{
    public class ProcessWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IProcessQueue _queue;
        private readonly ILogger<ProcessWorker> _logger;
        private readonly TextProcessingSettings _settings;

        public ProcessWorker(IServiceProvider serviceProvider, IProcessQueue queue, ILogger<ProcessWorker> logger, IOptions<TextProcessingSettings> settings)
        {
            _serviceProvider = serviceProvider;
            _queue = queue;
            _logger = logger;
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!_queue.TryDequeue(out var processId))
                {
                    await Task.Delay(500, stoppingToken);
                    continue;
                }

                using var scope = _serviceProvider.CreateScope();
                var _context = scope.ServiceProvider.GetRequiredService<DocumentContext>();
                var analyzer = scope.ServiceProvider.GetRequiredService<IAnalysisServices>();

                var process = await _context.Processes
                    .Include(p => p.Results)
                    .FirstOrDefaultAsync(p => p.Id == processId);

                if (process == null || process.Status != ProcessStatus.Pending)
                    continue;

                process.Status = ProcessStatus.Running;
                await _context.SaveChangesAsync();

                var fileNames = JsonSerializer.Deserialize<List<string>>(process.FilesJson) ?? new();
                var processedCount = process.ProcessedFiles;
                var remainingFiles = fileNames.Skip(processedCount).ToList();

                var results = process.Results;

                foreach (var file in remainingFiles)
                {
                    if (await IsPausedOrStopped(process.Id, _context)) break;
                    var inputFolder = _settings.InputFolder;
                    var filePath = Path.Combine(inputFolder, file);
                    var content = await File.ReadAllTextAsync(filePath);

                    var partial = analyzer.AnalyzeFile(content, file);

                    results.TotalWords += partial.TotalWords;
                    results.TotalLines += partial.TotalLines;
                    results.TotalCharacters += partial.TotalCharacters;

                    var mostFreq = JsonSerializer.Deserialize<List<string>>(results.MostFrequentWordsJson) ?? new List<string>();
                    var filesProc = JsonSerializer.Deserialize<List<string>>(results.FilesProcessedJson) ?? new List<string>();

                    mostFreq = mostFreq
                        .Concat(partial.MostFrequentWords)
                        .GroupBy(w => w)
                        .OrderByDescending(g => g.Count())
                        .Select(g => g.Key)
                        .Take(5)
                        .ToList();

                    filesProc.AddRange(partial.FilesProcessed);

                    results.MostFrequentWordsJson = JsonSerializer.Serialize(mostFreq);
                    results.FilesProcessedJson = JsonSerializer.Serialize(filesProc);

                    process.ProcessedFiles++;
                    process.Percentage = (int)((double)process.ProcessedFiles / process.TotalFiles * 100);

                    await _context.SaveChangesAsync();
                }

                if (process.Status == ProcessStatus.Running)
                {
                    process.Status = ProcessStatus.Completed;
                    process.EstimatedCompletion = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }
        }

        private static async Task<bool> IsPausedOrStopped(Guid id, DocumentContext db)
        {
            var latest = await db.Processes.FindAsync(id);
            return latest.Status == ProcessStatus.Paused || latest.Status == ProcessStatus.Stopped;
        }
    }


}