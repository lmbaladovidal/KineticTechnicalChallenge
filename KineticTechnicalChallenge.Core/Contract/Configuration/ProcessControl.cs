using KineticTechnicalChallenge.Core.Contract.DTO.Response;

namespace KineticTechnicalChallenge.Core.Contract.Configuration
{
    public class ProcessControl
    {
        public CancellationTokenSource CancellationTokenSource { get; set; }
        public Task ProcessingTask { get; set; }
        public ProcessResponse Response { get; set; }
    }
}
