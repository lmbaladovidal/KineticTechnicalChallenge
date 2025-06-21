using KineticTechnicalChallenge.Core.Contract.DTO;
using KineticTechnicalChallenge.Core.Contract.DTO.Response;
using Microsoft.AspNetCore.Mvc;

namespace KineticTechnicalChallenge.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProcessController : ControllerBase
    {

        private readonly ILogger<ProcessController> _logger;

        public ProcessController(ILogger<ProcessController> logger)
        {
            _logger = logger;
        }

        [HttpPost("/process/start")]
        [ProducesResponseType<ProcessResponse>(StatusCodes.Status201Created)]
        public IActionResult StartProcess()
        {
            var response = new ProcessResponse
            {
                ProcessInfoDTO = new ProcessInfoDTO
                {
                    Guid = Guid.NewGuid(),
                    Status = ProcessStatus.Running,
                    StartedAt = DateTime.UtcNow,
                    EstimatedCompletion = DateTime.UtcNow.AddMinutes(2)
                },
            };

            _logger.LogInformation("Process {ProcessId} started successfully.", response.ProcessInfoDTO.Guid);

            return CreatedAtAction(nameof(GetProcessStatus), new { processGuid = response.ProcessInfoDTO.Guid }, response);
        }

        [HttpPost("/process/stop/{processGuid}")]
        [ProducesResponseType<ProcessResponse>(StatusCodes.Status200OK)]
        public IActionResult StopProcess(string processGuid)
        {
            var response = new ProcessResponse
            {
                ProcessInfoDTO = new ProcessInfoDTO
                {
                    Guid = Guid.NewGuid(),
                    Status = ProcessStatus.Running,
                    StartedAt = DateTime.UtcNow,
                    EstimatedCompletion = DateTime.UtcNow.AddMinutes(2)
                },
            };
            _logger.LogInformation($"Process {processGuid} stopped successfully.");
            return Ok(response);
        }

        [HttpGet("/process/status/{processGuid}")]
        [ProducesResponseType<ProcessResponse>(StatusCodes.Status200OK)]
        public IActionResult GetProcessStatus(string processGuid)
        {
            var response = new ProcessResponse
            {
                ProcessInfoDTO = new ProcessInfoDTO
                {
                    Guid = Guid.NewGuid(),
                    Status = ProcessStatus.Running,
                    StartedAt = DateTime.UtcNow,
                    EstimatedCompletion = DateTime.UtcNow.AddMinutes(2)
                },
                Results = new AnalysisResultDTO
                { }
            };
            _logger.LogInformation($"Status requested for process ID: {processGuid}");
            return Ok(response);
        }

        [HttpGet("/process/list")]
        [ProducesResponseType<List<ProcessResponse>>(StatusCodes.Status200OK)]
        public IActionResult ListProcesses()
        {
            var processResponseA = new ProcessResponse
            {
                ProcessInfoDTO = new ProcessInfoDTO
                {
                    Guid = Guid.NewGuid(),
                    Status = ProcessStatus.Running,
                    StartedAt = DateTime.UtcNow,
                    EstimatedCompletion = DateTime.UtcNow.AddMinutes(2)
                },
                Results = new AnalysisResultDTO
                {
                    TotalWords = 1000,
                    TotalLines = 50,
                    MostFrequentWords = new List<string> { "example", "test", "data" },
                    FilesProcessed = new List<string> { "file1.txt", "file2.txt" }
                }
            };
            var processResponseB = new ProcessResponse
            {
                ProcessInfoDTO = new ProcessInfoDTO
                {
                    Guid = Guid.NewGuid(),
                    Status = ProcessStatus.Running,
                    StartedAt = DateTime.UtcNow,
                    EstimatedCompletion = DateTime.UtcNow.AddMinutes(2)
                },
                Results = new AnalysisResultDTO
                {
                    TotalWords = 1000,
                    TotalLines = 50,
                    MostFrequentWords = new List<string> { "example", "test", "data" },
                    FilesProcessed = new List<string> { "file1.txt", "file2.txt" }
                }
            };
            var processResponseList = new List<ProcessResponse> { processResponseA };

            _logger.LogInformation("Listing all processes.");
            return Ok(processResponseList);
        }

        [HttpGet("/process/results/{processGuid}")]
        [ProducesResponseType<ProcessResponse>(StatusCodes.Status200OK)]
        public IActionResult GetProcessResults(string processGuid)
        {
            var response = new ProcessResponse
            {
                ProcessInfoDTO = new ProcessInfoDTO
                {
                    Guid = Guid.NewGuid(),
                    Status = ProcessStatus.Running,
                    StartedAt = DateTime.UtcNow,
                    EstimatedCompletion = DateTime.UtcNow.AddMinutes(2)
                },
                Results = new AnalysisResultDTO
                {
                    TotalWords = 1000,
                    TotalLines = 50,
                    MostFrequentWords = new List<string> { "example", "test", "data" },
                    FilesProcessed = new List<string> { "file1.txt", "file2.txt" }
                }
            };
            _logger.LogInformation($"Results requested for process ID: {processGuid}");
            return Ok(response);
        }
    }
}
