using KineticTechnicalChallenge.Core.DTO.Response;

namespace KineticTechnicalChallenge.Core.Interfaces
{
    public interface IProcessServices
    {
        Task<ProcessResponse> StartProcessAsync();
        Task<ProcessResponse> StopProcessAsync(string processGuid);
        Task<ProcessResponse> GetProcessStatusAsync(string processGuid);
        Task<List<ProcessResponse>> ListProcessesAsync();
        Task<ProcessResponse> GetProcessResultsAsync(string processGuid);
    }
}
