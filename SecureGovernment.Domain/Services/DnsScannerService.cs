using DnsClient;
using SecureGovernment.Domain.DnsRecords.Workers;
using SecureGovernment.Domain.DnsResponse;
using SecureGovernment.Domain.Models.DnsRecords.Results;
using SecureGovernment.Domain.Models.DnsReponse.Parsed;
using System;
using System.Threading.Tasks;

namespace SecureGovernment.Domain.Services
{
    public class DnsScannerService
    {
        public ILookupClient LookupClient { get; set; }

        public async Task<ParsedDmarcResponse> ScanDmarcAsync(WorkerInformation workerInformation)
        {
            var dnsReponse = await LookupClient.QueryAsync($"_dmarc.{workerInformation.Hostname}", QueryType.TXT);
            var dmarc = new DmarcResponse(dnsReponse);

            return dmarc.ParseReponse();
        }

        public async Task<ParsedSpfResponse> ScanSpfAsync(WorkerInformation workerInformation)
        {
            var dnsReponse = await LookupClient.QueryAsync(workerInformation.Hostname, QueryType.TXT);
            var spf = new SpfResponse(dnsReponse);

            return spf.ParseReponse();
        }

        public async Task<ParsedDnssecResponse> ScanDnssecAsync(WorkerInformation workerInformation)
        {
            var dnsReponse = await LookupClient.QueryAsync(workerInformation.Hostname, QueryType.RRSIG);
            var dnssec = new DnssecReponse(dnsReponse);

            return dnssec.ParseReponse();
        }

        public async Task<ParsedCaaResponse> ScanCaaAsync(WorkerInformation workerInformation)
        {
            var dnsReponse = await LookupClient.QueryAsync(workerInformation.Hostname, QueryType.CAA);
            var caa = new CaaReponse(dnsReponse);

            return caa.ParseReponse();
        }
    }
}
