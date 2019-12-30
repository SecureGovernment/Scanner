using DnsClient;
using DnsClient.Protocol;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureGovernment.Domain.Models.DnsReponse.Parsed;
using SecureGovernment.Domain.Workers.Dns;
using System.Collections.Generic;
using System.Linq;
using static SecureGovernment.Domain.Tests.AssertionHelpers;
using static SecureGovernment.Domain.Tests.CreationHelpers;
using static SecureGovernment.Domain.Tests.Mocks.DnsMocker;
using static SecureGovernment.Domain.Tests.Mocks.ScanResultsMocker;
using static SecureGovernment.Domain.Tests.Mocks.WorkerInformationMocker;

namespace SecureGovernment.Domain.Tests.Services.Workers
{
    [TestClass]
    public class SpfWorkerTests
    {
        [TestMethod]
        public void Test_SpfWorker_Scan_NoSpfRecords()
        {
            // Arrange
            var workerInformation = MockWorkerInformation(hostname: "google.com");
            var dnsResponse = MockDnsQueryResponse();
            var lookupClient = MockLookupClient(new List<(string hostname, QueryType queryType, IDnsQueryResponse response)>() {
                ("google.com", QueryType.TXT, dnsResponse.Object),
                ("google.com", QueryType.SPF, dnsResponse.Object)
            });
            var previousWorker = MockPreviousWorker(workerInformation);

            var service = new SpfWorker(previousWorker.Object, lookupClient.Object);

            // Act
            var rawSpfRecord = service.Scan(workerInformation);
            rawSpfRecord.Wait();

            // Assert
            var records = rawSpfRecord.Result;
            AssertPreviousScanResults(new[] { records[0], records[1] }.ToList());
            Assert.IsInstanceOfType(records[2], typeof(ParsedSpfResponse));
            var spfRecord = records[2] as ParsedSpfResponse;

            Assert.IsFalse(spfRecord.HasSpfTypeRecords);
            Assert.IsFalse(spfRecord.HasSpfTypeRecords);
            Assert.IsFalse(spfRecord.HasSpfRecords);
            Assert.AreEqual(0, spfRecord.SpfTypeSpfRecords.Count);
            Assert.AreEqual(0, spfRecord.TxtTypeSpfRecords.Count);
            Assert.AreEqual(0, spfRecord.Records.Count);
        }

        [TestMethod]
        public void Test_SpfWorker_Scan_NoTxtRecordsOneSpfRecord()
        {
            // Arrange
            var workerInformation = MockWorkerInformation(hostname: "google.com");
            var resourceRecord = CreateResourceRecordInfo("google.com", ResourceRecordType.SPF);
            var txtRecords = new[] { "v=spf1 -all" };
            var dnsRecords = new List<DnsResourceRecord>() {
                new SpfRecord(resourceRecord, txtRecords, txtRecords)
            };
            var dnsResponse = MockDnsQueryResponse(dnsRecords);
            var lookupClient = MockLookupClient(new List<(string hostname, QueryType queryType, IDnsQueryResponse response)>() {
                ("google.com", QueryType.TXT, dnsResponse.Object),
                ("google.com", QueryType.SPF, dnsResponse.Object)
            });
            var previousWorker = MockPreviousWorker(workerInformation);

            var service = new SpfWorker(previousWorker.Object, lookupClient.Object);

            // Act
            var rawSpfRecord = service.Scan(workerInformation);
            rawSpfRecord.Wait();

            // Assert
            var records = rawSpfRecord.Result;
            AssertPreviousScanResults(new[] { records[0], records[1] }.ToList());
            var expectedRecords = new List<string>() { "google.com.0 \tIN \tSPF \t\"v=spf1 -all\"" };
            AssertSpfResponse(records[2], expectedRecords, new List<string>(), txtRecords.ToList(), false, true, true);
        }

        [TestMethod]
        public void Test_SpfWorker_Scan_NoSpfRecordsTxtSpfRecord()
        {
            // Arrange
            var workerInformation = MockWorkerInformation(hostname: "google.com");
            var resourceRecord = CreateResourceRecordInfo("google.com", ResourceRecordType.TXT);
            var txtRecords = new[] { "v=spf1 -all", "NOT A VALID RECORD" };
            var dnsRecords = new List<DnsResourceRecord>() {
                new TxtRecord(resourceRecord, txtRecords, txtRecords)
            };
            var dnsResponse = MockDnsQueryResponse(dnsRecords);
            var lookupClient = MockLookupClient(new List<(string hostname, QueryType queryType, IDnsQueryResponse response)>() {
                ("google.com", QueryType.TXT, dnsResponse.Object),
                ("google.com", QueryType.SPF, dnsResponse.Object)
            });
            var previousWorker = MockPreviousWorker(workerInformation);

            var service = new SpfWorker(previousWorker.Object, lookupClient.Object);

            // Act
            var rawSpfRecord = service.Scan(workerInformation);
            rawSpfRecord.Wait();

            // Assert
            var records = rawSpfRecord.Result;
            AssertPreviousScanResults(new[] { records[0], records[1] }.ToList());
            var expectedRecords = new List<string>() { "google.com.0 \tIN \tTXT \t\"v=spf1 -all\" \"NOT A VALID RECORD\"" };
            AssertSpfResponse(records[2], expectedRecords, new[] { txtRecords.First() }.ToList(), new List<string>(), true, false, true);
        }

        [TestMethod]
        public void Test_SpfWorker_Scan_SpfAndTxtRecords()
        {
            // Arrange
            var workerInformation = MockWorkerInformation(hostname: "google.com");
            var txtResourceRecord = CreateResourceRecordInfo("google.com", ResourceRecordType.TXT);
            var spfResourceRecord = CreateResourceRecordInfo("google.com", ResourceRecordType.SPF);
            var txtRecords = new[] { "v=spf1 -all", "NOT A VALID RECORD" };
            var spfTxtRecords = new[] { "RECORD 1", "RECORD 2" };
            var dnsRecords = new List<DnsResourceRecord>() {
                new TxtRecord(txtResourceRecord, txtRecords, txtRecords),
                new SpfRecord(spfResourceRecord, spfTxtRecords, spfTxtRecords)
            };
            var dnsResponse = MockDnsQueryResponse(dnsRecords);
            var lookupClient = MockLookupClient(new List<(string hostname, QueryType queryType, IDnsQueryResponse response)>() {
                ("google.com", QueryType.TXT, dnsResponse.Object),
                ("google.com", QueryType.SPF, dnsResponse.Object)
            });
            var previousWorker = MockPreviousWorker(workerInformation);

            var service = new SpfWorker(previousWorker.Object, lookupClient.Object);

            // Act
            var rawSpfRecord = service.Scan(workerInformation);
            rawSpfRecord.Wait();

            // Assert
            var records = rawSpfRecord.Result;
            AssertPreviousScanResults(new[] { records[0], records[1] }.ToList());
            var expectedRecords = new List<string>() { "google.com.0 \tIN \tTXT \t\"v=spf1 -all\" \"NOT A VALID RECORD\"", "google.com.0 \tIN \tSPF \t\"RECORD 1\" \"RECORD 2\"" };
            AssertSpfResponse(records[2], expectedRecords, new[] { txtRecords.First() }.ToList(), spfTxtRecords.ToList(), true, true, true);
        }
    }
}
