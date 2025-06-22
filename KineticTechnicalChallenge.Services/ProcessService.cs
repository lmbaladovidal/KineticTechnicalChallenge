using KineticTechnicalChallenge.Core.Contract.Configuration;
using KineticTechnicalChallenge.Core.Contract.DTO;
using KineticTechnicalChallenge.Core.Contract.DTO.Response;
using KineticTechnicalChallenge.Core.Contract.Enums;
using KineticTechnicalChallenge.Core.Contract.Interfaces;
using KineticTechnicalChallenge.Core.Data;
using KineticTechnicalChallenge.Core.Data.Models;
using KineticTechnicalChallenge.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Text.Json;

namespace KineticTechnicalChallenge.Services
{
    public class ProcessService : IProcessServices
    {
        private readonly DocumentContext _context;
        private readonly ILogger<ProcessService> _logger;
        private readonly TextProcessingSettings _settings;
        private readonly IAnalysisServices _analysisServices;
        private readonly IProcessStorage _storage;
        private readonly ConcurrentDictionary<Guid, ProcessControl> _processes = new();
        private readonly IProcessQueue _queue;
        public ProcessService(DocumentContext context,
            ILogger<ProcessService> logger,
            IOptions<TextProcessingSettings> settings,
            IAnalysisServices analysisServices,
            IProcessStorage processStorage,
            IProcessQueue processQueue)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
            _analysisServices = analysisServices ?? throw new ArgumentNullException(nameof(analysisServices));
            _storage = processStorage ?? throw new ArgumentNullException(nameof(processStorage));
            _queue = processQueue ?? throw new ArgumentNullException(nameof(processQueue));

        }
        public async Task<ProcessResponse> GetProcessResultsAsync(string processGuid)
        {
            if (!Guid.TryParse(processGuid, out var guid))
            {
                throw new ArgumentException("Invalid GUID format", nameof(processGuid));
            }
            var result = await _context.Results
                .FirstOrDefaultAsync(r => r.ProcessInfoId == Guid.Parse(processGuid));
            if (result != null)
            {
                _logger.LogInformation($"Process Result found for ID: {processGuid}");
                return new ProcessResponse
                {
                    Results = new AnalysisResultDTO
                    {
                        TotalWords = result.TotalWords,
                        TotalLines = result.TotalLines,
                        TotalCharacters = result.TotalCharacters,
                        MostFrequentWords = JsonManager.SafeDeserialize<List<string>>(result.MostFrequentWordsJson, _logger),
                        FilesProcessed = JsonManager.SafeDeserialize<List<string>>(result.FilesProcessedJson, _logger)
                    }
                };
            }
            else
            {
                throw new InvalidOperationException("Process Result not found");
            }

        }

        public async Task<ProcessStatus> GetProcessStatusAsync(string processGuid)
        {
            if (!Guid.TryParse(processGuid, out var guid))
            {
                throw new ArgumentException("Invalid GUID format", nameof(processGuid));
            }
            _logger.LogInformation($"Getting status for process with ID: {processGuid}");
            var processStatus = await _context.Processes
                .Where(ProcessStatus => ProcessStatus.Id == Guid.Parse(processGuid))
                .Select(p => p.Status).FirstOrDefaultAsync();

            if (processStatus == default)
            {
                throw new InvalidOperationException("Process not found");
            }
            return processStatus;

        }

        public async Task<List<ProcessResponse>> ListProcessesAsync()
        {
            _logger.LogInformation("Listing all processes");
            var processes = await _context.Processes
                .Include(p => p.Results).ToListAsync();
            var processResponses = new List<ProcessResponse>();
            if (processes != null)
            {
                foreach (var process in processes)
                {
                    _logger.LogInformation($"Process ID: {process.Id}, Status: {process.Status}, StartedAt: {process.StartedAt}, EstimatedCompletion: {process.EstimatedCompletion}");
                    processResponses.Add(new ProcessResponse
                    {
                        ProcessInfoDTO = new ProcessInfoDTO
                        {
                            Guid = process.Id,
                            Status = process.Status,
                            StartedAt = process.StartedAt,
                            EstimatedCompletion = process.EstimatedCompletion,
                            ProgressInfo = new ProgressInfo
                            {
                                TotalFiles = process.TotalFiles,
                                ProcessedFiles = process.ProcessedFiles,
                            }
                        },
                        Results = new AnalysisResultDTO
                        {
                            TotalWords = process.Results.TotalWords,
                            TotalLines = process.Results.TotalLines,
                            TotalCharacters = process.Results.TotalCharacters,
                            MostFrequentWords = JsonSerializer.Deserialize<List<string>>(process.Results.MostFrequentWordsJson) ?? new List<string>(),
                            FilesProcessed = JsonSerializer.Deserialize<List<string>>(process.Results.FilesProcessedJson) ?? new List<string>()
                        }
                    });
                }
            }
            else
            {
                throw new InvalidOperationException("No process found");
            }
            return processResponses;
        }

        public async Task<List<ProcessResponse>> StartProcessAsync()
        {
            var files = await GetFiles();// Get the files from the input folder
            List<ProcessInfo> batchesFiles = SetBatches(files);//Check if there are more than 10 files, if so, create batches of 5 files each
            List<ProcessResponse> processResponses = new List<ProcessResponse>();
            foreach (var process in batchesFiles)
            {
                _logger.LogInformation($"Starting process for batch with ID: {process.Id}");
                await _context.Processes.AddAsync(process);
                await _context.SaveChangesAsync();
                _queue.Enqueue(process.Id);
                processResponses.Add(new ProcessResponse
                {
                    ProcessInfoDTO = new ProcessInfoDTO
                    {
                        Guid = process.Id,
                        Status = process.Status,
                        StartedAt = process.StartedAt,
                        EstimatedCompletion = process.EstimatedCompletion,
                        ProgressInfo = new ProgressInfo
                        {
                            TotalFiles = process.TotalFiles,
                            ProcessedFiles = process.ProcessedFiles,
                        }
                    },
                    Results = new AnalysisResultDTO
                    {
                        TotalWords = process.Results.TotalWords,
                        TotalLines = process.Results.TotalLines,
                        TotalCharacters = process.Results.TotalCharacters,
                        MostFrequentWords = JsonManager.SafeDeserialize<List<string>>(process.Results.MostFrequentWordsJson, _logger),
                        FilesProcessed = JsonManager.SafeDeserialize<List<string>>(process.Results.FilesProcessedJson, _logger)
                    }
                });
                _logger.LogInformation($"Process for batch with ID: {process.Id} created and enqueued successfully");
            }

            return processResponses;
        }

        private List<ProcessInfo> SetBatches((List<string> contents, List<string> filenames) files)
        {
            _logger.LogInformation("Starting with batch control");
            var batches = new List<ProcessInfo>();
            for (var i = 0; i < files.filenames.Count; i += _settings.BatchSize)
            {
                _logger.LogInformation($"Creating batch N{batches.Count + 1}");
                var batchFiles = files.filenames.Skip(i).Take(_settings.BatchSize).ToList();
                if (batchFiles.Count > 0)
                {
                    var processId = Guid.NewGuid();
                    var process = new ProcessInfo
                    {
                        Id = processId,
                        StartedAt = DateTime.UtcNow,
                        EstimatedCompletion = DateTime.UtcNow.AddSeconds(batchFiles.Count * 2),
                        Status = ProcessStatus.Pending,
                        TotalFiles = batchFiles.Count,
                        ProcessedFiles = 0,
                        Percentage = 0,
                        FilesJson = JsonSerializer.Serialize(batchFiles)
                    };

                    var result = new AnalysisResult
                    {
                        Id = Guid.NewGuid(),
                        ProcessInfoId = processId,
                        TotalWords = 0,
                        TotalLines = 0,
                        TotalCharacters = 0,
                        MostFrequentWordsJson = JsonSerializer.Serialize(new List<string>()),
                        FilesProcessedJson = JsonSerializer.Serialize(new List<string>())
                    };
                    process.Results = result;
                    batches.Add(process);
                }
            }
            _logger.LogInformation($"Total batches created: {batches.Count}");
            return batches;
        }

        private async Task<(List<string> contents, List<string> filenames)> GetFiles()
        {
            _logger.LogInformation("Obtaining Files");
            var files = Directory.GetFiles(_settings.InputFolder, "*.txt");
            var contents = new List<string>();
            var fileNames = new List<string>();

            foreach (var file in files)

            {
                contents.Add(await File.ReadAllTextAsync(file));
                fileNames.Add(Path.GetFileName(file));
            }
            _logger.LogInformation("Files obtained successfully");
            return (contents, fileNames);

        }

        public async Task<ProcessResponse> StopProcessAsync(string processGuid)
        {
            if (!Guid.TryParse(processGuid, out var guid))
            {
                throw new ArgumentException("Invalid GUID format", nameof(processGuid));
            }
            var process = await _context.Processes
                .Include(p => p.Results)
                .FirstOrDefaultAsync(p => p.Id == Guid.Parse(processGuid)
                                       && (p.Status == ProcessStatus.Running || p.Status == ProcessStatus.Pending));

            if (process != null)
            {
                _logger.LogInformation($"Stopping process with ID: {process.Id}");
                process.Status = ProcessStatus.Stopped;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Process not found or not in running state");
            }
            return new ProcessResponse
            {
                ProcessInfoDTO = new ProcessInfoDTO
                {
                    Guid = process.Id,
                    Status = process.Status,
                    StartedAt = process.StartedAt,
                    EstimatedCompletion = process.EstimatedCompletion,
                    ProgressInfo = new ProgressInfo
                    {
                        TotalFiles = process.TotalFiles,
                        ProcessedFiles = process.ProcessedFiles,
                    }
                },
                Results = new AnalysisResultDTO
                {
                    TotalWords = process.Results.TotalWords,
                    TotalLines = process.Results.TotalLines,
                    TotalCharacters = process.Results.TotalCharacters,
                    MostFrequentWords = JsonManager.SafeDeserialize<List<string>>(process.Results.MostFrequentWordsJson, _logger),
                    FilesProcessed = JsonManager.SafeDeserialize<List<string>>(process.Results.FilesProcessedJson, _logger)
                }
            };
        }

        public async Task<ProcessResponse> ContinueProcessAsync(string processGuid)
        {
            if (!Guid.TryParse(processGuid, out var guid))
            {
                throw new ArgumentException("Invalid GUID format", nameof(processGuid));
            }
            var process = await _context.Processes
                .Include(p => p.Results)
                .FirstOrDefaultAsync(p => p.Id == Guid.Parse(processGuid)
                                       && (p.Status == ProcessStatus.Stopped || p.Status == ProcessStatus.Paused));
            if (process != null)
            {
                _logger.LogInformation($"Continuing process with ID: {process.Id}");
                process.Status = ProcessStatus.Pending;
                await _context.SaveChangesAsync();
                _queue.Enqueue(Guid.Parse(processGuid));
            }
            else
            {
                throw new InvalidOperationException("Process not found or not in running state");
            }

            return new ProcessResponse
            {
                ProcessInfoDTO = new ProcessInfoDTO
                {
                    Guid = process.Id,
                    Status = process.Status,
                    StartedAt = process.StartedAt,
                    EstimatedCompletion = process.EstimatedCompletion,
                    ProgressInfo = new ProgressInfo
                    {
                        TotalFiles = process.TotalFiles,
                        ProcessedFiles = process.ProcessedFiles,
                    }
                },
                Results = new AnalysisResultDTO
                {
                    TotalWords = process.Results.TotalWords,
                    TotalLines = process.Results.TotalLines,
                    TotalCharacters = process.Results.TotalCharacters,
                    MostFrequentWords = JsonManager.SafeDeserialize<List<string>>(process.Results.MostFrequentWordsJson, _logger),
                    FilesProcessed = JsonManager.SafeDeserialize<List<string>>(process.Results.FilesProcessedJson, _logger)
                }
            };
        }
    }
}
