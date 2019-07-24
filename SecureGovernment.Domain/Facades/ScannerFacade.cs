using DnsClient;
using SecureGovernment.Domain.Interfaces.Facades;
using SecureGovernment.Domain.Interfaces.Services;
using SecureGovernment.Domain.Models;
using SecureGovernment.Domain.Models.DnsRecords.Results;
using SecureGovernment.Domain.Workers;
using SecureGovernment.Domain.Workers.Dns;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecureGovernment.Domain.Facades
{
    public class ScannerFacade : IScannerFacade
    {
        public IDnsScannerService DnsScannerService { get; set; }
        public ILookupClient LookupClient { get; set; }

        public async Task<List<ScanResult>> ScanDns(WorkerInformation workerInformation)
        {
            var baseWorker = new BaseWorker();
            var mxWorker = new MxWorker(baseWorker, this.LookupClient);
            var caaWorker = new CaaWorker(mxWorker, this.LookupClient);
            var spfWorker = new SpfWorker(caaWorker, this.LookupClient);
            var dnssecWorker = new DnssecWorker(spfWorker, this.LookupClient);
            var dmarcWorker = new DmarcWorker(dnssecWorker, this.LookupClient);

            var scanResults = await dmarcWorker.Scan(workerInformation);

            return scanResults;
        }
    }
}
