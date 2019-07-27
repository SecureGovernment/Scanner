using DnsClient;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using SecureGovernment.Domain.Interfaces.Facades;
using SecureGovernment.Domain.Interfaces.Services;
using SecureGovernment.Domain.Models;
using SecureGovernment.Domain.Models.DnsRecords.Results;
using SecureGovernment.Domain.Workers;
using SecureGovernment.Domain.Workers.Dns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SecureGovernment.Domain.Facades
{
    public class ScannerFacade : IScannerFacade
    {
        public IDnsScannerService DnsScannerService { get; set; }
        public ILookupClient LookupClient { get; set; }

        public WorkerInformation ConnectToTarget(string url)
        {
            var connection = new Connection(url);
            var info = connection.LoadCertificates();

            return new WorkerInformation() {
                Certificate = info.Certificate,
                Chain = info.Chain,
                Hostname = url
            };
        }

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
