using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SecureGovernment.Domain.Interfaces;
using SecureGovernment.Domain.Models;
using SecureGovernment.Domain.Models.DnsRecords.Results;
using SecureGovernment.Domain.Models.DnsReponse.Parsed;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecureGovernment.Domain.Tests.Mocks
{
    public static class ScanResultsMocker
    {
        public static List<ScanResult> MockScanResults()
        {
            return new List<ScanResult>()
            {
                new ParsedSpfResponse(){ Records = new[]{ "v=spf -all" }.ToList() },
                new ParsedMxResponse(){ Records = new[]{ "aspmx.l.google.com.", "alt1.aspmx.l.google.com.", "alt2.aspmx.l.google.com." }.ToList() }
            };
        }

        public static Mock<IAsyncWorker> MockPreviousWorker(WorkerInformation workerInformation)
        {
            var previousWorkerMock = Utils.CreateMock<IAsyncWorker>();
            previousWorkerMock.Setup(x => x.Scan(workerInformation)).Returns(Task.FromResult(MockScanResults()));

            return previousWorkerMock;
        }
    }
}
