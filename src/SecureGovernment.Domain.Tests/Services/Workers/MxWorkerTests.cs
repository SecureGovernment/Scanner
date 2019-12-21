using DnsClient;
using DnsClient.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SecureGovernment.Domain.Interfaces;
using SecureGovernment.Domain.Workers.Dns;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SecureGovernment.Domain.Tests.AssertionHelpers;
using static SecureGovernment.Domain.Tests.Mocks.ScanResultsMocker;
using static SecureGovernment.Domain.Tests.Mocks.WorkerInformationMocker;

namespace SecureGovernment.Domain.Tests.Services.Workers
{
    [TestClass]
    public class MxWorkerTests
    {
        [TestMethod]
        public void Test_MxWorker_Scan_NoMxRecords()
        {
            //Arrange
            var workerInformation = MockWorkerInformation(hostname: "google.com");

            var dnsResponse = new Mock<IDnsQueryResponse>();
            dnsResponse.Setup(x => x.Answers).Returns(new List<DnsResourceRecord>());

            var lookupClientMock = Utils.CreateMock<ILookupClient>();
            lookupClientMock.Setup(x => x.QueryAsync(workerInformation.Hostname, QueryType.MX, QueryClass.IN, null, default)).Returns(Task.FromResult(dnsResponse.Object));
            var previousWorkerMock = Utils.CreateMock<IAsyncWorker>();
            previousWorkerMock.Setup(x => x.Scan(workerInformation)).Returns(Task.FromResult(MockScanResults()));

            var service = new MxWorker(previousWorkerMock.Object, lookupClientMock.Object);

            //Act
            var rawMxRecords = service.Scan(workerInformation);
            rawMxRecords.Wait();

            // Assert
            var records = rawMxRecords.Result;
            Assert.AreEqual(3, records.Count);
            AssertPreviousScanResults(new[] { records[0], records[1] }.ToList());
            AssertMxResponse(records[2], new List<string>(), false);
        }

        [TestMethod]
        public void Test_MxWorker_Scan_MxRecords()
        {
            //Arrange
            var workerInformation = MockWorkerInformation(hostname: "google.com");
            var dnsString = DnsString.FromResponseQueryString(workerInformation.Hostname);
            var resourceRecord = new ResourceRecordInfo(dnsString, ResourceRecordType.MX, QueryClass.IN, 0, 0);

            var dnsResponse = new Mock<IDnsQueryResponse>();
            dnsResponse.Setup(x => x.Answers).Returns(new List<DnsResourceRecord>() { new MxRecord(resourceRecord, 0, dnsString) });

            var lookupClientMock = Utils.CreateMock<ILookupClient>();
            lookupClientMock.Setup(x => x.QueryAsync(workerInformation.Hostname, QueryType.MX, QueryClass.IN, null, default)).Returns(Task.FromResult(dnsResponse.Object));
            var previousWorkerMock = Utils.CreateMock<IAsyncWorker>();
            previousWorkerMock.Setup(x => x.Scan(workerInformation)).Returns(Task.FromResult(MockScanResults()));

            var service = new MxWorker(previousWorkerMock.Object, lookupClientMock.Object);

            //Act
            var rawMxRecords = service.Scan(workerInformation);
            rawMxRecords.Wait();

            // Assert
            var records = rawMxRecords.Result;
            Assert.AreEqual(3, records.Count);
            AssertPreviousScanResults(new[] { records[0], records[1] }.ToList());
            AssertMxResponse(records[2], new List<string>() { "google.com." }, true);
        }
    }
}
