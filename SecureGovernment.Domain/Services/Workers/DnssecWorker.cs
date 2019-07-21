using DnsClient;
using SecureGovernment.Domain.Interfaces;
using SecureGovernment.Domain.Models.Workers;
using System.Linq;
using System.Threading.Tasks;

namespace SecureGovernment.Domain.Services.Workers
{
    public class DnssecWorker : IAsyncWorker
    {
        public ILookupClient LookupClient { get; set; }
        public async Task<WorkerResult> RunAsync(WorkerInformation workerInformation)
        {
            var rawCaaRecords = await LookupClient.QueryAsync(workerInformation.Hostname, QueryType.RRSIG);
            var hasRrsigRecords = rawCaaRecords.Answers.Any();

            return new DnssecWorkerResult()
            {
                HasDnssec = hasRrsigRecords
            };
        }
    }
}
