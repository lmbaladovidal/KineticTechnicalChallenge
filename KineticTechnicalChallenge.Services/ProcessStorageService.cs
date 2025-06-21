using KineticTechnicalChallenge.Core.Contract.DTO.Response;
using KineticTechnicalChallenge.Core.Contract.Interfaces;

namespace KineticTechnicalChallenge.Services
{
    public class ProcessStorageService : IProcessStorage
    {
        public Task AddProcessAsync(ProcessResponse processResponse)
        {
            throw new NotImplementedException();
        }

        public Task DeleteProcessAsync(string processGuid)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessResponse> GetProcessAsync(string processGuid)
        {
            throw new NotImplementedException();
        }

        public Task<List<ProcessResponse>> ListProcessesAsync()
        {
            throw new NotImplementedException();
        }

        public Task UpdateProcessAsync(ProcessResponse processResponse)
        {
            throw new NotImplementedException();
        }
    }
}
