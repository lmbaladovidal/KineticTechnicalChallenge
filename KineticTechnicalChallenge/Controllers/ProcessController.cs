using Microsoft.AspNetCore.Mvc;

namespace KineticTechnicalChallenge.Controllers
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
        public IActionResult StartProcess()
        {
            _logger.LogInformation("Process started successfully.");
            return Ok("Process started successfully.");
        }
        [HttpPost("/process/stop")]
        public IActionResult StopProcess()
        {
            _logger.LogInformation("Process stopped successfully.");
            return Ok("Process stopped successfully.");
        }
        [HttpGet("/process/status/{process_id}")]
        public IActionResult GetProcessStatus(int process_id)
        {
            _logger.LogInformation($"Status requested for process ID: {process_id}");
            return Ok($"Status for process ID {process_id} is running.");
        }
        [HttpGet("/process/list")]
        public IActionResult ListProcesses()
        {
            _logger.LogInformation("Listing all processes.");
            return Ok("List of all processes.");
        }
        [HttpGet("/process/results/{process_id}")]
        public IActionResult GetProcessResults(int process_id)
        {
            _logger.LogInformation($"Results requested for process ID: {process_id}");
            return Ok($"Results for process ID {process_id} are available.");
        }
    }
}
