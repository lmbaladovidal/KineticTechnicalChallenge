﻿using KineticTechnicalChallenge.Core.Contract.Configuration;
using KineticTechnicalChallenge.Core.Contract.Enums;
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
    //Gen With IA
    public class ProcessWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IProcessQueue _queue;
        private readonly ILogger<ProcessWorker> _logger;
        private readonly TextProcessingSettings _settings;
        private bool isPausedOrStopped = false;

        public ProcessWorker(IServiceProvider serviceProvider,
                            IProcessQueue queue,
                            ILogger<ProcessWorker> logger,
                            IOptions<TextProcessingSettings> settings)
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
                // Check if there are any processes in the queue
                _logger.LogInformation("Checking for processes in the queue...");
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
                _logger.LogInformation($"Processing ID: {processId}, Status: {process?.Status}");
                if (process == null ||
                    process.Status != ProcessStatus.Pending ||
                    process.Status == ProcessStatus.Stopped ||
                    process.Status == ProcessStatus.Paused)
                    continue;

                process.Status = ProcessStatus.Running;
                await _context.SaveChangesAsync();

                var fileNames = JsonSerializer.Deserialize<List<string>>(process.FilesJson) ?? new();
                var processedCount = process.ProcessedFiles;
                var remainingFiles = fileNames.Skip(processedCount).ToList();

                var results = process.Results;
                _logger.LogInformation($"Remaining files to process: {remainingFiles.Count}");
                foreach (var file in remainingFiles)
                {
                    await Task.Delay(10000, stoppingToken);
                    if (await IsPausedOrStopped(process.Id, _context, _logger))
                    {
                        isPausedOrStopped = true;
                        break;
                    }
                    var inputFolder = _settings.InputFolder;
                    var filePath = Path.Combine(inputFolder, file);
                    var content = await File.ReadAllTextAsync(filePath);
                    _logger.LogInformation($"AnalyzeFile Statring");
                    var partial = analyzer.AnalyzeFile(content, file);
                    _logger.LogInformation($"AnalyzeFile Completed for {file}");
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
                    _logger.LogInformation($"Processed file: {file}, Total processed: {process.ProcessedFiles}/{process.TotalFiles}, Percentage: {process.Percentage}%");
                }
                if (process.Percentage == 100 && isPausedOrStopped)
                    isPausedOrStopped = false;

                if (process.Status == ProcessStatus.Running && !isPausedOrStopped)
                {
                    _logger.LogInformation($"Process {process.Id} completed successfully.");
                    process.Status = ProcessStatus.Completed;
                    process.EstimatedCompletion = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }
        }

        private static async Task<bool> IsPausedOrStopped(Guid id, DocumentContext context, ILogger<ProcessWorker> logger)
        {
            var latest = await context.Processes
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
            logger.LogInformation($"Process {id} is {latest?.Status}");
            return latest.Status == ProcessStatus.Paused || latest.Status == ProcessStatus.Stopped;
        }
    }


}