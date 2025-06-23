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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> StartProcess()
        {
            _logger.LogInformation("Starting a new process...");
            var result = await _processServices.StartProcessAsync();
            if (result == null)
            {
                _logger.LogError("Failed to start process.");
                return StatusCode(500, "Internal server error");
            }
            _logger.LogInformation("Process created successfully");
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
                _logger.LogInformation("Stopping process with ID: {ProcessGuid}", processGuid);
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
                _logger.LogInformation("Continuing process with ID: {ProcessGuid}", processGuid);
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
                _logger.LogError(ex, "Error continuing process {ProcessGuid}", processGuid);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("/process/status/{processGuid}")]
        [ProducesResponseType<ProcessResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProcessStatus(string processGuid)
        {
            try
            {
                var response = await _processServices.GetProcessStatusAsync(processGuid);
                _logger.LogInformation($"Status requested for process ID: {processGuid}");
                return Ok(response);
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
                _logger.LogError(ex, "Error getting status process {ProcessGuid}", processGuid);
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpGet("/process/list")]
        [ProducesResponseType<List<ProcessResponse>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ListProcesses()
        {
            try
            {
                var processResponseList = await _processServices.ListProcessesAsync();
                _logger.LogInformation("Listing all processes.");
                return Ok(processResponseList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting process");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("/process/results/{processGuid}")]
        [ProducesResponseType<ProcessResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProcessResults(string processGuid)
        {
            try
            {
                _logger.LogInformation("Getting results for process with ID: {ProcessGuid}", processGuid);
                var response = await _processServices.GetProcessResultsAsync(processGuid);
                _logger.LogInformation($"Results requested for process ID: {processGuid}");
                return Ok(response);
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
                _logger.LogError(ex, "Error getting result process {ProcessGuid}", processGuid);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
