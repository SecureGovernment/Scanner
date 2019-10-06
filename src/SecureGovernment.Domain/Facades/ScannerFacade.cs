using DnsClient;
using SecureGovernment.Domain.Interfaces.Facades;
using SecureGovernment.Domain.Interfaces.Infastructure;
using SecureGovernment.Domain.Models;
using SecureGovernment.Domain.Models.DnsRecords.Results;
using SecureGovernment.Domain.Workers;
using SecureGovernment.Domain.Workers.Dns;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SecureGovernment.Domain.Facades
{
    public class ScannerFacade : IScannerFacade
    {
        public ILookupClient LookupClient { get; set; }
        public ISettings Settings { get; set; }

        public WorkerInformation ConnectToTarget(string url)
        {
            Connection connection = CreateConnection(url);
            var workerInformation = new WorkerInformation() { Hostname = url };

            try
            {
                var info = connection.LoadCertificates();
                workerInformation.Certificate = info.Certificate;
                workerInformation.Chain = info.Chain;
            } catch (SocketException) { } //TODO: Log failure

            return workerInformation;
        }

        public async Task<List<ScanResult>> ScanDns(WorkerInformation workerInformation)
        {
            var baseWorker = new BaseWorker();
            var mxWorker = new MxWorker(baseWorker, this.LookupClient);
            var dkimWorker = new DkimWorker(mxWorker, this.LookupClient, this.Settings);
            var caaWorker = new CaaWorker(dkimWorker, this.LookupClient);
            var spfWorker = new SpfWorker(caaWorker, this.LookupClient);
            var dnssecWorker = new DnssecWorker(spfWorker, this.LookupClient);
            var dmarcWorker = new DmarcWorker(dnssecWorker, this.LookupClient);

            var scanResults = await dmarcWorker.Scan(workerInformation);

            return scanResults;
        }

        public virtual Connection CreateConnection(string url)
        {
            return new Connection(url);
        }
    }
}
