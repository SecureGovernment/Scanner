using DnsClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureGovernment.Domain.Workers.Dns;
using System.Collections.Generic;
using static SecureGovernment.Domain.Tests.Mocks.DnsMocker;
using static SecureGovernment.Domain.Tests.Mocks.WorkerInformationMocker;
using static SecureGovernment.Domain.Tests.Mocks.ScanResultsMocker;
using static SecureGovernment.Domain.Tests.AssertionHelpers;
using static SecureGovernment.Domain.Tests.CreationHelpers;
using System.Linq;
using DnsClient.Protocol;

namespace SecureGovernment.Domain.Tests.Services.Workers
{
    [TestClass]
    public class DmarcWorkerTests
    {
        [TestMethod]
        public void Test_DmarcWorker_Scan_NoRecords()
        {
            //Arrange
            var workerInformation = MockWorkerInformation(hostname: "google.com");
            var dnsResponse = MockDnsQueryResponse();
            var lookupClient = MockLookupClient(new List<(string hostname, QueryType queryType, IDnsQueryResponse response)>() {
                ("_dmarc.google.com", QueryType.TXT, dnsResponse.Object)
            });
            var previousWorker = MockPreviousWorker(workerInformation);

            //Act
            var dmarcWorker = new DmarcWorker(previousWorker.Object, lookupClient.Object);

            //Assert
            var rawDmarcRecords = dmarcWorker.Scan(workerInformation);
            rawDmarcRecords.Wait();

            // Assert
            var records = rawDmarcRecords.Result;
            Assert.AreEqual(3, records.Count);
            AssertPreviousScanResults(new[] { records[0], records[1] }.ToList());
            AssertDmarcResponse(records[2], new List<string>(), false);
        }

        [TestMethod]
        public void Test_DmarcWorker_Scan_RecordsDoNotContainDmarcString()
        {
            //Arrange
            var workerInformation = MockWorkerInformation(hostname: "google.com");
            var dnsString = CreateDnsString($"_dmarc.{workerInformation.Hostname}");
            var resourceRecord = CreateResourceRecordInfo(dnsString, ResourceRecordType.TXT);
            var txtRecords = new[] { "docusign=1b0a6754-49b1-4db5-8540-d2c12664b289", "globalsign-smime-dv=CDYX+XFHUw2wml6/Gb8+59BsH31KzUr6c1l2BPvqKX8=" };
            var dnsRecords = new List<DnsResourceRecord>() { 
                new TxtRecord(resourceRecord, txtRecords, txtRecords)
            };
            var dnsResponse = MockDnsQueryResponse(dnsRecords);
            var lookupClient = MockLookupClient(new List<(string hostname, QueryType queryType, IDnsQueryResponse response)>() {
                ("_dmarc.google.com", QueryType.TXT, dnsResponse.Object)
            });
            var previousWorker = MockPreviousWorker(workerInformation);

            //Act
            var dmarcWorker = new DmarcWorker(previousWorker.Object, lookupClient.Object);

            //Assert
            var rawDmarcRecords = dmarcWorker.Scan(workerInformation);
            rawDmarcRecords.Wait();

            // Assert
            var records = rawDmarcRecords.Result;
            Assert.AreEqual(3, records.Count);
            AssertPreviousScanResults(new[] { records[0], records[1] }.ToList());
            AssertDmarcResponse(records[2], new List<string>(), false);
        }

        [TestMethod]
        public void Test_DmarcWorker_Scan_RecordsContainDmarcString()
        {
            //Arrange
            var workerInformation = MockWorkerInformation(hostname: "google.com");
            var dnsString = CreateDnsString($"_dmarc.{workerInformation.Hostname}");
            var resourceRecord = CreateResourceRecordInfo(dnsString, ResourceRecordType.TXT);
            var txtRecords = new[] { "docusign=1b0a6754-49b1-4db5-8540-d2c12664b289", "v=DMARC1; p=none; rua=mailto:dmarc@google.com" };
            var dnsRecords = new List<DnsResourceRecord>() {
                new TxtRecord(resourceRecord, txtRecords, txtRecords)
            };
            var dnsResponse = MockDnsQueryResponse(dnsRecords);
            var lookupClient = MockLookupClient(new List<(string hostname, QueryType queryType, IDnsQueryResponse response)>() {
                ("_dmarc.google.com", QueryType.TXT, dnsResponse.Object)
            });
            var previousWorker = MockPreviousWorker(workerInformation);

            //Act
            var dmarcWorker = new DmarcWorker(previousWorker.Object, lookupClient.Object);

            //Assert
            var rawDmarcRecords = dmarcWorker.Scan(workerInformation);
            rawDmarcRecords.Wait();

            // Assert
            var records = rawDmarcRecords.Result;
            Assert.AreEqual(3, records.Count);
            AssertPreviousScanResults(new[] { records[0], records[1] }.ToList());
            AssertDmarcResponse(records[2], new List<string>() { "v=DMARC1; p=none; rua=mailto:dmarc@google.com" }, true);
        }
    }
}
