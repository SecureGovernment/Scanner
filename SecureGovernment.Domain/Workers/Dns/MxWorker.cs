using DnsClient;
using SecureGovernment.Domain.Interfaces;
using SecureGovernment.Domain.Models;
using SecureGovernment.Domain.Models.DnsRecords.Results;
using SecureGovernment.Domain.Models.DnsReponse;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecureGovernment.Domain.Workers.Dns
{
    public class MxWorker : IAsyncWorker
    {
        private ILookupClient _LookupClient { get; }
        private IAsyncWorker _PreviousWorker { get; }
        public MxWorker(IAsyncWorker previousWorker, ILookupClient lookupClient)
        {
            this._PreviousWorker = previousWorker;
            this._LookupClient = lookupClient;
        }

        public async Task<List<ScanResult>> Scan(WorkerInformation workerInformation)
        {
            var dnsReponse = await _LookupClient.QueryAsync(workerInformation.Hostname, QueryType.MX);
            var mx = new MxResponse(dnsReponse);
            var previousResults = await this._PreviousWorker.Scan(workerInformation);
            previousResults.Add(mx.ParseReponse());

            return previousResults;
        }
    }
}
