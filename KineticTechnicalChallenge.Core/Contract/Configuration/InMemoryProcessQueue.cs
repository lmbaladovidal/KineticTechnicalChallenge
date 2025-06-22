using KineticTechnicalChallenge.Core.Contract.Interfaces;
using System.Collections.Concurrent;

namespace KineticTechnicalChallenge.Core.Contract.Configuration
{
    public class InMemoryProcessQueue : IProcessQueue
    {
        private readonly ConcurrentQueue<Guid> _queue = new();
        public void Enqueue(Guid processId) => _queue.Enqueue(processId);
        public bool TryDequeue(out Guid processId) => _queue.TryDequeue(out processId);
    }
}
