using KineticTechnicalChallenge.Core.Contract.Interfaces;
using KineticTechnicalChallenge.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace KineticTechnicalChallenge.Services.BackGroundServices
{
    public class ProcessWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IProcessQueue _queue;
        private readonly ILogger<ProcessWorker> _logger;

        public ProcessWorker(IServiceProvider serviceProvider, IProcessQueue queue, ILogger<ProcessWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _queue = queue;
            _logger = logger;
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
                var db = scope.ServiceProvider.GetRequiredService<DocumentContext>();
                var analyzer = scope.ServiceProvider.GetRequiredService<AnalysisService>();

                var process = await db.Processes
                    .Include(p => p.Results)
                    .FirstOrDefaultAsync(p => p.Id == processId);

                if (process == null || process.Status != ProcessStatus.Pending)
                    continue;

                process.Status = ProcessStatus.Running;
                await db.SaveChangesAsync();

                var fileNames = JsonSerializer.Deserialize<List<string>>(process.FilesJson) ?? new();
                var processedCount = process.ProcessedFiles;
                var remainingFiles = fileNames.Skip(processedCount).ToList();

                var results = process.Results;

                foreach (var file in remainingFiles)
                {
                    if (await IsPausedOrStopped(process.Id, db)) break;

                    var content = await File.ReadAllTextAsync(Path.Combine("TextFolder", file));

                    // Análisis del archivo individual
                    var partial = analyzer.AnalyzeFile(content, file);

                    // Acumulamos resultados
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

                    // Actualizamos progreso
                    process.ProcessedFiles++;
                    process.Percentage = (int)((double)process.ProcessedFiles / process.TotalFiles * 100);

                    await db.SaveChangesAsync();
                }

                if (process.Status == ProcessStatus.Running)
                {
                    process.Status = ProcessStatus.Completed;
                    process.EstimatedCompletion = DateTime.UtcNow;
                    await db.SaveChangesAsync();
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