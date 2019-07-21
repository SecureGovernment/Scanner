using DnsClient;
using DnsClient.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SecureGovernment.Domain.Models.DnsRecords.Results;
using SecureGovernment.Domain.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecureGovernment.Domain.Tests.Services.Workers
{
    [TestClass]
    public class CaaWorkerTests
    {
        [TestMethod]
        public void Test_CaaWorker_RunAsync_NoRecords()
        {
            // Arrange
            var workerInformation = new WorkerInformation() { Hostname = "http://www.google.com" };

            var dnsResponse = new Mock<IDnsQueryResponse>();
            dnsResponse.Setup(x => x.Answers).Returns(new List<DnsResourceRecord>());

            var lookupClientMock = new Mock<ILookupClient>(MockBehavior.Strict);
            lookupClientMock.Setup(x => x.QueryAsync(workerInformation.Hostname, QueryType.CAA, QueryClass.IN, default)).Returns(Task.FromResult(dnsResponse.Object));

            var service = new DnsScannerService() { LookupClient = lookupClientMock.Object };

            // Act
            var rawCaaRecords = service.ScanCaaAsync(workerInformation);
            rawCaaRecords.Wait();

            // Assert
            var caaRecords = rawCaaRecords.Result;
            Assert.IsFalse(caaRecords.HasCaaRecords);
            Assert.AreEqual(0, caaRecords.IssueCas.Count);
            Assert.AreEqual(0, caaRecords.IssueWildCas.Count);
        }

        [TestMethod]
        public void Test_CaaWorker_RunAsync_Records()
        {
            // Arrange
            var workerInformation = new WorkerInformation() { Hostname = "http://www.google.com" };
            var resourceRecord = new ResourceRecordInfo(DnsString.FromResponseQueryString(workerInformation.Hostname), ResourceRecordType.CAA, QueryClass.IN, 0, 0);

            var dnsRecords = new List<DnsResourceRecord>() {
                new CaaRecord(resourceRecord, 0, "issuewild", "pki.googl"),
                new CaaRecord(resourceRecord, 0, "issue", "letsencrypt.org"),
                new CaaRecord(resourceRecord, 0, "issuewild", "sslcerts.com"),
                new CaaRecord(resourceRecord, 0, "issue", "freecerts.com"),
            };

            var dnsResponse = new Mock<IDnsQueryResponse>();
            dnsResponse.Setup(x => x.Answers).Returns(dnsRecords);

            var lookupClientMock = new Mock<ILookupClient>(MockBehavior.Strict);
            lookupClientMock.Setup(x => x.QueryAsync(workerInformation.Hostname, QueryType.CAA, QueryClass.IN, default)).Returns(Task.FromResult(dnsResponse.Object));

            var service = new DnsScannerService() { LookupClient = lookupClientMock.Object };

            // Act
            var rawCaaRecords = service.ScanCaaAsync(workerInformation);
            rawCaaRecords.Wait();

            // Assert
            var caaRecords = rawCaaRecords.Result;
            Assert.IsTrue(caaRecords.HasCaaRecords);
            Assert.AreEqual(2, caaRecords.IssueCas.Count);
            Assert.AreEqual("letsencrypt.org", caaRecords.IssueCas[0]);
            Assert.AreEqual("freecerts.com", caaRecords.IssueCas[1]);
            Assert.AreEqual(2, caaRecords.IssueWildCas.Count);
            Assert.AreEqual("pki.googl", caaRecords.IssueWildCas[0]);
            Assert.AreEqual("sslcerts.com", caaRecords.IssueWildCas[1]);
        }
    }
}
