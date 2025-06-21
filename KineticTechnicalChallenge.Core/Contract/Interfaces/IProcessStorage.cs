using KineticTechnicalChallenge.Core.Contract.DTO.Response;

namespace KineticTechnicalChallenge.Core.Contract.Interfaces
{
    public interface IProcessStorage
    {
        Task AddProcessAsync(ProcessResponse processResponse);
        Task<ProcessResponse> GetProcessAsync(string processGuid);
        Task<List<ProcessResponse>> ListProcessesAsync();
        Task UpdateProcessAsync(ProcessResponse processResponse);
        Task DeleteProcessAsync(string processGuid);
    }
}
