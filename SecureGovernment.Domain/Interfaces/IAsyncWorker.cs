using SecureGovernment.Domain.Models.Workers;
using System.Threading.Tasks;

namespace SecureGovernment.Domain.Interfaces
{
    public interface IAsyncWorker
    {
        Task<WorkerResult> RunAsync(WorkerInformation workerInformation);
    }
}
