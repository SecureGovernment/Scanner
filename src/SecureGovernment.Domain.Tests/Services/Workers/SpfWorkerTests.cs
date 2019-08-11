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
    public class SpfWorkerTests
    {
        [TestMethod]
        public void Test_SpfWorker_Scan_NoTxtRecords()
        {
            // Arrange
            var workerInformation = new WorkerInformation() { Hostname = "http://www.google.com" };

            var dnsResponse = new Mock<IDnsQueryResponse>();
            dnsResponse.Setup(x => x.Answers).Returns(new List<DnsResourceRecord>());

            var lookupClientMock = new Mock<ILookupClient>(MockBehavior.Strict);
            lookupClientMock.Setup(x => x.QueryAsync(workerInformation.Hostname, QueryType.TXT, QueryClass.IN, default)).Returns(Task.FromResult(dnsResponse.Object));
            var previousWorkerMock = new Mock<IAsyncWorker>(MockBehavior.Strict);
            previousWorkerMock.Setup(x => x.Scan(workerInformation)).Returns(Task.FromResult(new List<ScanResult>()));

            var service = new SpfWorker(previousWorkerMock.Object, lookupClientMock.Object);

            // Act
            var rawSpfRecord = service.Scan(workerInformation);
            rawSpfRecord.Wait();

            // Assert
            var records = rawSpfRecord.Result;
            Assert.IsInstanceOfType(records.Single(), typeof(ParsedSpfResponse));
            var spfRecord = records.Single() as ParsedSpfResponse;

            Assert.IsFalse(spfRecord.HasOldSpf);
            Assert.IsFalse(spfRecord.HasSpfRecords);
            Assert.AreEqual(0, spfRecord.OldSpfRecords.Count);
            Assert.AreEqual(0, spfRecord.Records.Count);
        }

        [TestMethod]
        public void Test_SpfWorker_Scan_NoSpfRecords()
        {
            // Arrange
            var workerInformation = new WorkerInformation() { Hostname = "http://www.google.com" };
            var resourceRecord = new ResourceRecordInfo(DnsString.FromResponseQueryString(workerInformation.Hostname), ResourceRecordType.TXT, QueryClass.IN, 0, 0);
            var txtRecords = new[] { "docusign=1b0a6754-49b1-4db5-8540-d2c12664b289", "globalsign-smime-dv=CDYX+XFHUw2wml6/Gb8+59BsH31KzUr6c1l2BPvqKX8=" };

            var dnsRecords = new List<DnsResourceRecord>() {
                new TxtRecord(resourceRecord, txtRecords, txtRecords)
            };

            var dnsResponse = new Mock<IDnsQueryResponse>();
            dnsResponse.Setup(x => x.Answers).Returns(dnsRecords);

            var lookupClientMock = new Mock<ILookupClient>(MockBehavior.Strict);
            lookupClientMock.Setup(x => x.QueryAsync(workerInformation.Hostname, QueryType.TXT, QueryClass.IN, default)).Returns(Task.FromResult(dnsResponse.Object));
            var previousWorkerMock = new Mock<IAsyncWorker>(MockBehavior.Strict);
            previousWorkerMock.Setup(x => x.Scan(workerInformation)).Returns(Task.FromResult(new List<ScanResult>()));

            var service = new SpfWorker(previousWorkerMock.Object, lookupClientMock.Object);

            // Act
            var rawSpfRecord = service.Scan(workerInformation);
            rawSpfRecord.Wait();

            // Assert
            var records = rawSpfRecord.Result;
            Assert.IsInstanceOfType(records.Single(), typeof(ParsedSpfResponse));
            var spfRecord = records.Single() as ParsedSpfResponse;

            Assert.IsFalse(spfRecord.HasOldSpf);
            Assert.IsFalse(spfRecord.HasSpfRecords);
            Assert.AreEqual(0, spfRecord.OldSpfRecords.Count);
            Assert.AreEqual(0, spfRecord.Records.Count);
        }

        [TestMethod]
        public void Test_SpfWorker_Scan_OneValidRecord()
        {
            // Arrange
            var workerInformation = new WorkerInformation() { Hostname = "http://www.google.com" };
            var resourceRecord = new ResourceRecordInfo(DnsString.FromResponseQueryString(workerInformation.Hostname), ResourceRecordType.TXT, QueryClass.IN, 0, 0);
            var txtRecords = new[] { "v=spf1 include:_spf.google.com ~all", "globalsign-smime-dv=CDYX+XFHUw2wml6/Gb8+59BsH31KzUr6c1l2BPvqKX8=" };

            var dnsRecords = new List<DnsResourceRecord>() {
                new TxtRecord(resourceRecord, txtRecords, txtRecords)
            };

            var dnsResponse = new Mock<IDnsQueryResponse>();
            dnsResponse.Setup(x => x.Answers).Returns(dnsRecords);

            var lookupClientMock = new Mock<ILookupClient>(MockBehavior.Strict);
            lookupClientMock.Setup(x => x.QueryAsync(workerInformation.Hostname, QueryType.TXT, QueryClass.IN, default)).Returns(Task.FromResult(dnsResponse.Object));
            var previousWorkerMock = new Mock<IAsyncWorker>(MockBehavior.Strict);
            previousWorkerMock.Setup(x => x.Scan(workerInformation)).Returns(Task.FromResult(new List<ScanResult>()));

            var service = new SpfWorker(previousWorkerMock.Object, lookupClientMock.Object);

            // Act
            var rawSpfRecord = service.Scan(workerInformation);
            rawSpfRecord.Wait();

            // Assert
            var records = rawSpfRecord.Result;
            Assert.IsInstanceOfType(records.Single(), typeof(ParsedSpfResponse));
            var spfRecord = records.Single() as ParsedSpfResponse;

            Assert.IsFalse(spfRecord.HasOldSpf);
            Assert.IsTrue(spfRecord.HasSpfRecords);
            Assert.AreEqual(0, spfRecord.OldSpfRecords.Count);
            Assert.AreEqual(1, spfRecord.Records.Count);
            Assert.AreEqual(txtRecords[0], spfRecord.Records[0]);
        }
    }
}
