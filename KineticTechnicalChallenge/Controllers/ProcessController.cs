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
            return StatusCode(StatusCodes.Status201Created, result);
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
        public async Task<IActionResult> GetProcessStatus(string processGuid)
        {
            var response = await _processServices.GetProcessStatusAsync(processGuid);
            _logger.LogInformation($"Status requested for process ID: {processGuid}");
            return Ok(response);
        }

        [HttpGet("/process/list")]
        [ProducesResponseType<List<ProcessResponse>>(StatusCodes.Status200OK)]
        public async Task<IActionResult> ListProcesses()
        {
            var processResponseList = await _processServices.ListProcessesAsync();
            _logger.LogInformation("Listing all processes.");
            return Ok(processResponseList);
        }

        [HttpGet("/process/results/{processGuid}")]
        [ProducesResponseType<ProcessResponse>(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProcessResults(string processGuid)
        {
            var response = await _processServices.GetProcessResultsAsync(processGuid);
            _logger.LogInformation($"Results requested for process ID: {processGuid}");
            return Ok(response);
        }
    }
}
