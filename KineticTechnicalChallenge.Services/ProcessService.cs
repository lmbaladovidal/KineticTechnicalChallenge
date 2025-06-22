using KineticTechnicalChallenge.Core.Contract.Configuration;
using KineticTechnicalChallenge.Core.Contract.DTO;
using KineticTechnicalChallenge.Core.Contract.DTO.Response;
using KineticTechnicalChallenge.Core.Contract.Interfaces;
using KineticTechnicalChallenge.Core.Data;
using KineticTechnicalChallenge.Core.Data.Models;
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
        public Task<ProcessResponse> GetProcessResultsAsync(string processGuid)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessResponse> GetProcessStatusAsync(string processGuid)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProcessResponse>> ListProcessesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<ProcessResponse> StartProcessAsync()
        {
            var files = await GetFiles();// Get the files from the input folder
            var processId = Guid.NewGuid();

            var process = new ProcessInfo
            {
                Id = processId,
                StartedAt = DateTime.UtcNow,
                EstimatedCompletion = DateTime.UtcNow.AddSeconds(files.filenames.Count * 10),
                Status = ProcessStatus.Pending,
                TotalFiles = files.filenames.Count,
                ProcessedFiles = 0,
                Percentage = 0,
                FilesJson = JsonSerializer.Serialize(files.filenames)
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

            await _context.Processes.AddAsync(process);
            await _context.SaveChangesAsync();

            _queue.Enqueue(processId);

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
                    MostFrequentWords = JsonSerializer.Deserialize<List<string>>(process.Results.MostFrequentWordsJson) ?? new List<string>(),
                    FilesProcessed = JsonSerializer.Deserialize<List<string>>(process.Results.FilesProcessedJson) ?? new List<string>()
                }
            };
        }



        private async Task<(List<string> contents, List<string> filenames)> GetFiles()
        {
            var files = Directory.GetFiles(_settings.InputFolder, "*.txt");
            var contents = new List<string>();
            var fileNames = new List<string>();

            foreach (var file in files)
            {
                contents.Add(await File.ReadAllTextAsync(file));
                fileNames.Add(Path.GetFileName(file));
            }
            return (contents, fileNames);

        }
        private object SetProcessResponse()
        {
            throw new NotImplementedException();
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
                process.Status = ProcessStatus.Stopped;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Process not found or not in running state");
            }
            process = await _context.Processes.AsNoTracking()
                .Include(p => p.Results)
                .FirstOrDefaultAsync(p => p.Id == Guid.Parse(processGuid));
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
                    MostFrequentWords = JsonSerializer.Deserialize<List<string>>(process.Results.MostFrequentWordsJson) ?? new List<string>(),
                    FilesProcessed = JsonSerializer.Deserialize<List<string>>(process.Results.FilesProcessedJson) ?? new List<string>()
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
                    MostFrequentWords = JsonSerializer.Deserialize<List<string>>(process.Results.MostFrequentWordsJson) ?? new List<string>(),
                    FilesProcessed = JsonSerializer.Deserialize<List<string>>(process.Results.FilesProcessedJson) ?? new List<string>()
                }
            };
        }
    }
}
