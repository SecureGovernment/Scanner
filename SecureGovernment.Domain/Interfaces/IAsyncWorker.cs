using SecureGovernment.Domain.Models;
using SecureGovernment.Domain.Models.DnsRecords.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecureGovernment.Domain.Interfaces
{
    public interface IAsyncWorker
    {
        Task<List<ScanResult>> Scan(WorkerInformation workerInformation);
    }
}
