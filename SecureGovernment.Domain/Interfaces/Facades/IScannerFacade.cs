using SecureGovernment.Domain.Models.DnsRecords.Results;
using SecureGovernment.Domain.Models.DnsReponse.Parsed;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SecureGovernment.Domain.Interfaces.Facades
{
    public interface IScannerFacade
    {
        Task<List<ParsedDnsReponse>> ScanDns(WorkerInformation workerInformation);
    }
}
