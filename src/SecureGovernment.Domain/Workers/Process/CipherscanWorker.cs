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
                    UseShellExecute = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                }

            };

            process.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
            process.BeginOutputReadLine();

            // 3 minute timeout
            process.WaitForExit(180000);

            previousResults.Add(_Result);
            return previousResults;

        }

        private void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            _Result = JsonConvert.DeserializeObject<CipherscanResult>(outLine.Data);
        }
    }
}