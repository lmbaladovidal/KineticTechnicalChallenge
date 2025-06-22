using KineticTechnicalChallenge.Core.Contract.DTO;
using KineticTechnicalChallenge.Core.Contract.DTO.Response;
using KineticTechnicalChallenge.Core.Contract.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KineticTechnicalChallenge.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProcessController : ControllerBase
    {

        private readonly ILogger<ProcessController> _logger;
        private readonly IProcessServices _processServices;
        public ProcessController(ILogger<ProcessController> logger, IProcessServices process)
        {
            _logger = logger;
            _processServices = process ?? throw new ArgumentNullException(nameof(process));
        }

        [HttpPost("/process/start")]
        [ProducesResponseType<ProcessResponse>(StatusCodes.Status201Created)]
        public async Task<IActionResult> StartProcess()
        {
            _logger.LogInformation("Starting a new process...");
            var result = await _processServices.StartProcessAsync();
            _logger.LogInformation("Process {ProcessId} started successfully.", result.ProcessInfoDTO.Guid);
            return CreatedAtAction(nameof(GetProcessStatus), new { processGuid = result.ProcessInfoDTO.Guid }, result);
        }

        [HttpPost("/process/stop/{processGuid}")]
        [ProducesResponseType<ProcessResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> StopProcess(string processGuid)
        {
            try
            {
                var result = await _processServices.StopProcessAsync(processGuid);
                _logger.LogInformation("Process {ProcessGuid} stopped successfully", processGuid);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid GUID format: {ProcessGuid}", processGuid);
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Process not found or invalid state: {ProcessGuid}", processGuid);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping process {ProcessGuid}", processGuid);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("/process/continue/{processGuid}")]
        [ProducesResponseType<ProcessResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ContinueProcess(string processGuid)
        {
            try
            {
                var result = await _processServices.ContinueProcessAsync(processGuid);
                _logger.LogInformation("Process {ProcessGuid} stopped successfully", processGuid);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid GUID format: {ProcessGuid}", processGuid);
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Process not found or invalid state: {ProcessGuid}", processGuid);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping process {ProcessGuid}", processGuid);
                return StatusCode(500, "Internal server error");
            }
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
