using SecureGovernment.Domain.Interfaces.Facades;
using SecureGovernment.Domain.Interfaces.Services;
using SecureGovernment.Domain.Models.DnsRecords.Results;
using SecureGovernment.Domain.Models.DnsReponse.Parsed;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SecureGovernment.Domain.Facades
{
    public class ScannerFacade : IScannerFacade
    {
        public IDnsScannerService DnsScannerService { get; set; }
        public async Task<List<ParsedDnsReponse>> ScanDns(WorkerInformation workerInformation)
        {
            var dnsResponses = new List<ParsedDnsReponse>() {
                await DnsScannerService.ScanCaaAsync(workerInformation),
                await DnsScannerService.ScanDmarcAsync(workerInformation),
                await DnsScannerService.ScanDnssecAsync(workerInformation),
                await DnsScannerService.ScanSpfAsync(workerInformation),
                await DnsScannerService.ScanMxAsync(workerInformation)
            };

            return dnsResponses;
        }
    }
}
