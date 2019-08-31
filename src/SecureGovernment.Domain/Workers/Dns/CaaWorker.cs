using DnsClient;
using SecureGovernment.Domain.DnsResponse;
using SecureGovernment.Domain.Interfaces;
using SecureGovernment.Domain.Models;
using SecureGovernment.Domain.Models.DnsRecords.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SecureGovernment.Domain.Workers.Dns
{
    public class CaaWorker : IAsyncWorker
    {
        private ILookupClient _LookupClient { get; }
        private IAsyncWorker _PreviousWorker { get; }
        public CaaWorker(IAsyncWorker previousWorker, ILookupClient lookupClient)
        {
            this._PreviousWorker = previousWorker;
            this._LookupClient = lookupClient;
        }

        public async Task<List<ScanResult>> Scan(WorkerInformation workerInformation)
        {
            var dnsReponse = await _LookupClient.QueryAsync(workerInformation.Hostname, QueryType.CAA);
            var caa = new CaaReponse(dnsReponse);
            var previousResults = await this._PreviousWorker.Scan(workerInformation);
            previousResults.Add(caa.ParseReponse());

            return previousResults;
        }
    }
}
