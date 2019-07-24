using DnsClient;
using DnsClient.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SecureGovernment.Domain.Interfaces;
using SecureGovernment.Domain.Models;
using SecureGovernment.Domain.Models.DnsRecords.Results;
using SecureGovernment.Domain.Models.DnsReponse.Parsed;
using SecureGovernment.Domain.Workers.Dns;
using System.Collections.Generic;
using System.Linq;
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
            var previousWorkerMock = new Mock<IAsyncWorker>(MockBehavior.Strict);
            previousWorkerMock.Setup(x => x.Scan(workerInformation)).Returns(Task.FromResult(new List<ScanResult>()));

            var service = new CaaWorker(previousWorkerMock.Object, lookupClientMock.Object);

            // Act
            var rawCaaRecords = service.Scan(workerInformation);
            rawCaaRecords.Wait();

            // Assert
            var records = rawCaaRecords.Result;
            Assert.IsInstanceOfType(records.Single(), typeof(ParsedCaaResponse));
            var caaRecord = records.Single() as ParsedCaaResponse;

            Assert.IsFalse(caaRecord.HasCaaRecords);
            Assert.AreEqual(0, caaRecord.IssueCas.Count);
            Assert.AreEqual(0, caaRecord.IssueWildCas.Count);
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
            var previousWorkerMock = new Mock<IAsyncWorker>(MockBehavior.Strict);
            previousWorkerMock.Setup(x => x.Scan(workerInformation)).Returns(Task.FromResult(new List<ScanResult>()));

            var service = new CaaWorker(previousWorkerMock.Object, lookupClientMock.Object);

            // Act
            var rawCaaRecords = service.Scan(workerInformation);
            rawCaaRecords.Wait();

            // Assert
            var records = rawCaaRecords.Result;
            Assert.IsInstanceOfType(records.Single(), typeof(ParsedCaaResponse));
            var caaRecord = records.Single() as ParsedCaaResponse;

            Assert.IsTrue(caaRecord.HasCaaRecords);
            Assert.AreEqual(2, caaRecord.IssueCas.Count);
            Assert.AreEqual("letsencrypt.org", caaRecord.IssueCas[0]);
            Assert.AreEqual("freecerts.com", caaRecord.IssueCas[1]);
            Assert.AreEqual(2, caaRecord.IssueWildCas.Count);
            Assert.AreEqual("pki.googl", caaRecord.IssueWildCas[0]);
            Assert.AreEqual("sslcerts.com", caaRecord.IssueWildCas[1]);
        }
    }
}
