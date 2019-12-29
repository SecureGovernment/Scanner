using DnsClient;
using SecureGovernment.Domain.Interfaces;
using SecureGovernment.Domain.Interfaces.Infastructure;
using SecureGovernment.Domain.Models;
using SecureGovernment.Domain.Models.DnsRecords.Results;
using SecureGovernment.Domain.Models.DnsReponse;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureGovernment.Domain.Workers.Dns
{
    public class DkimWorker : IAsyncWorker
    {
        private ILookupClient _LookupClient { get; }
        private ISettings Settings { get; }
        private IAsyncWorker _PreviousWorker { get; }
        public DkimWorker(IAsyncWorker previousWorker, ILookupClient lookupClient, ISettings settings)
        {
            this._PreviousWorker = previousWorker;
            this._LookupClient = lookupClient;
            this.Settings = settings;
        }

        public async Task<List<ScanResult>> Scan(WorkerInformation workerInformation)
        {
            var dkimResponse = new DkimResponse();
            foreach (var selector in this.Settings.DkimSelectors.Distinct())
            {
                var queryType = QueryType.TXT;
                var dnsReponse = await _LookupClient.QueryAsync($"{selector}.{workerInformation.Hostname}", queryType);
                if (!dnsReponse.Answers.TxtRecords().Any())
                {
                    queryType = QueryType.CNAME;
                    dnsReponse = await _LookupClient.QueryAsync($"{selector}.{workerInformation.Hostname}", queryType);
                }

                dkimResponse.AddResponse(selector, dnsReponse, queryType);
            }
            
            var previousResults = await this._PreviousWorker.Scan(workerInformation);
            previousResults.Add(dkimResponse.ParseReponse());

            return previousResults;
        }
    }
}
