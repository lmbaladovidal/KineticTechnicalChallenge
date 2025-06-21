using KineticTechnicalChallenge.Core.Data;
using KineticTechnicalChallenge.Core.DTO.Response;
using KineticTechnicalChallenge.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace KineticTechnicalChallenge.Services
{
    public class ProcessService : IProcessServices
    {
        private readonly DocumentContext _context;
        private readonly ILogger<ProcessService> _logger;
        public ProcessService(DocumentContext context, ILogger<ProcessService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

        public Task<ProcessResponse> StartProcessAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ProcessResponse> StopProcessAsync(string processGuid)
        {
            throw new NotImplementedException();
        }
    }
}
