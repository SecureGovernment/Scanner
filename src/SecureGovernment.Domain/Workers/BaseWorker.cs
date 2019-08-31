using SecureGovernment.Domain.Interfaces;
using SecureGovernment.Domain.Models;
using SecureGovernment.Domain.Models.DnsRecords.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecureGovernment.Domain.Workers
{
    public class BaseWorker : IAsyncWorker
    {
        public Task<List<ScanResult>> Scan(WorkerInformation workerInformation)
        {
            return Task.FromResult(new List<ScanResult>());
        }
    }
}
