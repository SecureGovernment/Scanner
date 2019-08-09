using SecureGovernment.Domain.Interfaces;
using SecureGovernment.Domain.Models;
using SecureGovernment.Domain.Models.DnsRecords.Results;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SecureGovernment.Domain.Workers.Process
{
    public class CipherscanWorker : IAsyncWorker
    {
        private string _CipherscanPath { get; }
        private IAsyncWorker _PreviousWorker { get; }
        public string _Output { get; set; }

        public CipherscanWorker(IAsyncWorker previousWorker, string cipherscanPath)
        {
            this._PreviousWorker = previousWorker;
            this._CipherscanPath = cipherscanPath;
        }

        public async Task<List<ScanResult>> Scan(WorkerInformation workerInformation)
        {
            var previousResults = await this._PreviousWorker.Scan(workerInformation);

            if (!File.Exists(this._CipherscanPath))
                return previousResults;

            var process = new System.Diagnostics.Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = this._CipherscanPath,
                    Arguments = $"--no-tolerance -j --curves -servername {workerInformation.Hostname}",
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

            return previousResults;

        }

        private static void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            //* Do your stuff with the output (write to console/log/StringBuilder)
            Console.WriteLine(outLine.Data);
        }
    }
}
