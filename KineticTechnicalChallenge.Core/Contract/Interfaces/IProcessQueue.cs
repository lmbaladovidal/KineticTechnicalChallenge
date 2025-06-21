namespace KineticTechnicalChallenge.Core.Contract.Interfaces
{
    public interface IProcessQueue
    {
        void Enqueue(Guid processId);
        bool TryDequeue(out Guid processId);
    }
}
