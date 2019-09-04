using DnsClient;
using SecureGovernment.Domain.Interfaces.Facades;
using SecureGovernment.Domain.Interfaces.Infastructure;
using SecureGovernment.Domain.Models;
using SecureGovernment.Domain.Models.DnsRecords.Results;
using SecureGovernment.Domain.Workers;
using SecureGovernment.Domain.Workers.Dns;
using SecureGovernment.Domain.Workers.Process;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SecureGovernment.Domain.Facades
{
    public class ScannerFacade : IScannerFacade
    {
        public ILookupClient LookupClient { get; set; }
        public IFileSystem FileSystem { get; set; }
        public ISettings Settings { get; set; }

        public async Task<List<ScanResult>> Scan(string url)
        {
            var workerInfo = await ConnectToTarget(url);
            var dns = await ScanDns(workerInfo);

            var tls = await ScanTls(workerInfo);
            
            return dns;
        }

        public async Task<WorkerInformation> ConnectToTarget(string url)
        {
            Connection connection = CreateConnection(url);
            var workerInformation = await connection.Connect();

            return workerInformation;
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

        public async Task<List<ScanResult>> ScanTls(WorkerInformation workerInformation){
            var baseWorker = new BaseWorker();
            var cipherscanWorker = new CipherscanWorker(baseWorker, FileSystem, Settings.CipherscanPath);
            var result = await cipherscanWorker.Scan(workerInformation);

            return result;
        }

        public virtual Connection CreateConnection(string url)
        {
            return new Connection(url);
        }
    }
}
