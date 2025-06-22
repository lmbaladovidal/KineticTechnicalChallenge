using KineticTechnicalChallenge.Core.Contract.DTO.Response;
using KineticTechnicalChallenge.Core.Contract.Enums;

namespace KineticTechnicalChallenge.Core.Contract.Interfaces
{
    public interface IProcessServices
    {
        Task<List<ProcessResponse>> StartProcessAsync();
        Task<ProcessResponse> StopProcessAsync(string processGuid);
        Task<ProcessResponse> ContinueProcessAsync(string processGuid);
        Task<ProcessStatus> GetProcessStatusAsync(string processGuid);
        Task<List<ProcessResponse>> ListProcessesAsync();
        Task<ProcessResponse> GetProcessResultsAsync(string processGuid);
    }
}
