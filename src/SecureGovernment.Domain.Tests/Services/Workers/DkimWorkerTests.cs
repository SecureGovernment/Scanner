using DnsClient;
using DnsClient.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SecureGovernment.Domain.Interfaces;
using SecureGovernment.Domain.Interfaces.Infastructure;
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
    public class DkimWorkerTests
    {
        [TestMethod]
        public void Test_DkimWorker_NoSelectors()
        {
            // Arrange
            var workerInformation = MockWorkerInformation(hostname: "google.com");

            var settingsMock = Utils.CreateMock<ISettings>();
            settingsMock.Setup(x => x.DkimSelectors).Returns(new List<string>());

            var lookupClientMock = Utils.CreateMock<ILookupClient>();

            var previousWorkerMock = Utils.CreateMock<IAsyncWorker>();
            previousWorkerMock.Setup(x => x.Scan(workerInformation)).Returns(Task.FromResult(MockScanResults()));

            var worker = new DkimWorker(previousWorkerMock.Object, lookupClientMock.Object, settingsMock.Object);

            // Act
            var rawRecords = worker.Scan(workerInformation);
            rawRecords.Wait();

            // Assert
            var records = rawRecords.Result;
            Assert.AreEqual(3, records.Count);

            AssertPreviousScanResults(new[] { records[0], records[1] }.ToList());
            AssertDkimResponse(records[2], null, new List<(string, QueryType, List<string>)>(), false);
        }

        [TestMethod]
        public void Test_DkimWorker_OneSelectors_Txt()
        {
            // Arrange
            var workerInformation = MockWorkerInformation(hostname: "google.com");
            var hostnameWithSelector = "selector1._domainkey.google.com";

            var settingsMock = Utils.CreateMock<ISettings>();
            settingsMock.Setup(x => x.DkimSelectors).Returns(new List<string>() { "selector1._domainkey" });

            var lookupClientMock = Utils.CreateMock<ILookupClient>();

            var resourceRecord = new ResourceRecordInfo(DnsString.FromResponseQueryString(hostnameWithSelector), ResourceRecordType.TXT, QueryClass.IN, 0, 0);
            var txtRecords = new[] { "v=DKIM <key>" };

            var dnsRecords = new List<TxtRecord>() {
                new TxtRecord(resourceRecord, txtRecords, txtRecords)
            };

            var dnsResponse = new Mock<IDnsQueryResponse>();
            dnsResponse.Setup(x => x.Answers).Returns(dnsRecords);

            lookupClientMock.Setup(x => x.QueryAsync(hostnameWithSelector, QueryType.TXT, QueryClass.IN, null, default)).Returns(Task.FromResult(dnsResponse.Object));

            var previousWorkerMock = MockPreviousWorker(workerInformation);
            var worker = new DkimWorker(previousWorkerMock.Object, lookupClientMock.Object, settingsMock.Object);

            // Act
            var rawRecords = worker.Scan(workerInformation);
            rawRecords.Wait();

            // Assert
            var records = rawRecords.Result;
            Assert.AreEqual(3, records.Count);

            AssertPreviousScanResults(new[] { records[0], records[1] }.ToList());
            AssertDkimResponse(records[2], null, new[] { ("selector1._domainkey", QueryType.TXT, txtRecords.ToList()) }.ToList(), true);
        }

        [TestMethod]
        public void Test_DkimWorker_OneSelectors_Cname()
        {
            // Arrange
            var workerInformation = MockWorkerInformation(hostname: "google.com");
            var hostnameWithSelector = "selector1._domainkey.google.com";
            var dnsString = DnsString.FromResponseQueryString(hostnameWithSelector);

            var settingsMock = Utils.CreateMock<ISettings>();
            settingsMock.Setup(x => x.DkimSelectors).Returns(new List<string>() { "selector1._domainkey" });

            var lookupClientMock = Utils.CreateMock<ILookupClient>();

            var resourceRecord = new ResourceRecordInfo(dnsString, ResourceRecordType.CNAME, QueryClass.IN, 0, 0);

            var dnsRecords = new List<CNameRecord>() {
                new CNameRecord(resourceRecord, dnsString)
            };

            var dnsResponse = new Mock<IDnsQueryResponse>();
            dnsResponse.Setup(x => x.Answers).Returns(dnsRecords);

            lookupClientMock.Setup(x => x.QueryAsync(hostnameWithSelector, It.IsAny<QueryType>(), QueryClass.IN, null, default)).Returns(Task.FromResult(dnsResponse.Object));

            var previousWorkerMock = MockPreviousWorker(workerInformation);
            var worker = new DkimWorker(previousWorkerMock.Object, lookupClientMock.Object, settingsMock.Object);

            // Act
            var rawRecords = worker.Scan(workerInformation);
            rawRecords.Wait();

            // Assert
            var records = rawRecords.Result;
            Assert.AreEqual(3, records.Count);

            AssertPreviousScanResults(new[] { records[0], records[1] }.ToList());
            AssertDkimResponse(records[2], null, new[] { ("selector1._domainkey", QueryType.CNAME, new[] { dnsString.Value }.ToList())}.ToList(), true);
        }
    }
}
