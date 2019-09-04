using Newtonsoft.Json;
using SecureGovernment.Domain.Interfaces;
using SecureGovernment.Domain.Interfaces.Infastructure;
using SecureGovernment.Domain.Models;
using SecureGovernment.Domain.Models.DnsRecords.Results;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SecureGovernment.Domain.Workers.Process
{
    public class CipherscanWorker : IAsyncWorker
    {
        private string _CipherscanPath { get; }
        private IAsyncWorker _PreviousWorker { get; }
        private IFileSystem _FileSystem { get; set; }
        private CipherscanResult _Result { get; set; }

        public CipherscanWorker(IAsyncWorker previousWorker, IFileSystem fileSystem, string cipherscanPath)
        {
            this._PreviousWorker = previousWorker;
            this._FileSystem = fileSystem;
            this._CipherscanPath = cipherscanPath;
        }

        public async Task<List<ScanResult>> Scan(WorkerInformation workerInformation)
        {
            var previousResults = await this._PreviousWorker.Scan(workerInformation);

            if (!this._FileSystem.Exists(this._CipherscanPath))
                return previousResults;

            var process = new System.Diagnostics.Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = this._CipherscanPath,
                    Arguments = $"--no-tolerance -j --curves -servername {workerInformation.Hostname} {workerInformation.IPAddress}:443",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                }

            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string err = process.StandardError.ReadToEnd();
            
            // 3 minute timeout
            process.WaitForExit(180000);

            previousResults.Add(JsonConvert.DeserializeObject<CipherscanResult>(output));
            return previousResults;

        }
    }
}