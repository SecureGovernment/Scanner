using SecureGovernment.Domain.Models;
using SecureGovernment.Domain.Models.DnsRecords.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecureGovernment.Domain.Interfaces.Facades
{
    public interface IScannerFacade
    {
        Task<List<ScanResult>> ScanDns(WorkerInformation workerInformation);
    }
}
