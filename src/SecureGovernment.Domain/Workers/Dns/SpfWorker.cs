using DnsClient;
using SecureGovernment.Domain.DnsResponse;
using SecureGovernment.Domain.Interfaces;
using SecureGovernment.Domain.Models;
using SecureGovernment.Domain.Models.DnsRecords.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecureGovernment.Domain.Workers.Dns
{
    public class SpfWorker : IAsyncWorker
    {
        private ILookupClient _LookupClient { get; }
        private IAsyncWorker _PreviousWorker { get; }
        public SpfWorker(IAsyncWorker previousWorker, ILookupClient lookupClient)
        {
            this._PreviousWorker = previousWorker;
            this._LookupClient = lookupClient;
        }

        public async Task<List<ScanResult>> Scan(WorkerInformation workerInformation)
        {
            var txtDnsReponse = await _LookupClient.QueryAsync(workerInformation.Hostname, QueryType.TXT);
            var spfDnsResponse = await _LookupClient.QueryAsync(workerInformation.Hostname, QueryType.SPF);
            var spf = new SpfResponse(txtDnsReponse, spfDnsResponse);
            var previousResults = await this._PreviousWorker.Scan(workerInformation);
            previousResults.Add(spf.ParseReponse());

            return previousResults;
        }
    }
}
