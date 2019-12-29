using DnsClient;
using DnsClient.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureGovernment.Domain.Workers.Dns;
using System.Collections.Generic;
using System.Linq;
using static SecureGovernment.Domain.Tests.AssertionHelpers;
using static SecureGovernment.Domain.Tests.Mocks.DnsMocker;
using static SecureGovernment.Domain.Tests.Mocks.ScanResultsMocker;
using static SecureGovernment.Domain.Tests.Mocks.WorkerInformationMocker;
using static SecureGovernment.Domain.Tests.CreationHelpers;

namespace SecureGovernment.Domain.Tests.Services.Workers
{
    [TestClass]
    public class DnssecWorkerTests
    {
        [TestMethod]
        public void Test_DnssecWorker_Scan_NoRecords()
        {
            //Arrange
            var workerInformation = MockWorkerInformation(hostname: "google.com");
            var dnsResponse = MockDnsQueryResponse();
            var lookupClient = MockLookupClient(new List<(string hostname, QueryType queryType, IDnsQueryResponse response)>() {
                ("google.com", QueryType.RRSIG, dnsResponse.Object)
            });
            var previousWorker = MockPreviousWorker(workerInformation);

            //Act
            var dnssecWorker = new DnssecWorker(previousWorker.Object, lookupClient.Object);

            //Assert
            var rawDnssecRecords = dnssecWorker.Scan(workerInformation);
            rawDnssecRecords.Wait();

            // Assert
            var records = rawDnssecRecords.Result;
            Assert.AreEqual(3, records.Count);
            AssertPreviousScanResults(new[] { records[0], records[1] }.ToList());
            AssertDnssecResponse(records[2], new List<string>(), false);
        }

        [TestMethod]
        public void Test_DnssecWorker_Scan_Records()
        {
            //Arrange
            var workerInformation = MockWorkerInformation(hostname: "google.com");
            var resourceRecord = CreateResourceRecordInfo(CreateDnsString(workerInformation.Hostname), ResourceRecordType.RRSIG);
            var txtRecords = new[] { "lG5pWNIfzrxjgww0FIRL2P9c0ZQVQqJkDUX5WJjK5Y0fnSDsVGX7Zz/SVnAJ0zKuEEkC249RPOLb9l+QqBYVBuxiI4Dc8/qvXWSiHybyrPRu6ONQn8UYzLpH5obYfNBOy0154aKjzWJOq4wD7qnxOncOGW50PxJQ4omxPsso73c=" };
            var dnsRecords = new List<DnsResourceRecord>() {
                new TxtRecord(resourceRecord, txtRecords, txtRecords)
            };

            var dnsResponse = MockDnsQueryResponse(dnsRecords);
            var lookupClient = MockLookupClient(new List<(string hostname, QueryType queryType, IDnsQueryResponse response)>() {
                ("google.com", QueryType.RRSIG, dnsResponse.Object)
            });
            var previousWorker = MockPreviousWorker(workerInformation);

            //Act
            var dnssecWorker = new DnssecWorker(previousWorker.Object, lookupClient.Object);

            //Assert
            var rawDnssecRecords = dnssecWorker.Scan(workerInformation);
            rawDnssecRecords.Wait();

            // Assert
            var records = rawDnssecRecords.Result;
            Assert.AreEqual(3, records.Count);
            AssertPreviousScanResults(new[] { records[0], records[1] }.ToList());
            AssertDnssecResponse(records[2], new List<string>() { "google.com.0 \tIN \tRRSIG \t\"lG5pWNIfzrxjgww0FIRL2P9c0ZQVQqJkDUX5WJjK5Y0fnSDsVGX7Zz/SVnAJ0zKuEEkC249RPOLb9l+QqBYVBuxiI4Dc8/qvXWSiHybyrPRu6ONQn8UYzLpH5obYfNBOy0154aKjzWJOq4wD7qnxOncOGW50PxJQ4omxPsso73c=\"" }, true);
        }
    }
}
