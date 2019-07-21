using DnsClient;
using DnsClient.Protocol;
using SecureGovernment.Domain.Interfaces;
using SecureGovernment.Domain.Models.Workers;
using System.Linq;
using System.Threading.Tasks;

namespace SecureGovernment.Domain.Services.Workers
{
    public class DmarcWorker : IAsyncWorker
    {
        public ILookupClient LookupClient { get; set; }
        public async Task<WorkerResult> RunAsync(WorkerInformation workerInformation)
        {
            var rawCaaRecords = await LookupClient.QueryAsync($"_dmarc.{workerInformation.Hostname}", QueryType.TXT);
            var txtRecords = rawCaaRecords.Answers.Select(x => x as TxtRecord).ToList();

            return new DmarcWorkerResult()
            {
                Records = txtRecords.SelectMany(x => x.Text).Where(x => x.ToLower().Contains("dmarc")).ToList()
            };
        }
    }
}
