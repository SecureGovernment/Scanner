using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SecureGovernment.Domain.Interfaces;
using SecureGovernment.Domain.Interfaces.Infastructure;
using SecureGovernment.Domain.Models;
using SecureGovernment.Domain.Models.DnsReponse.Parsed;
using SecureGovernment.Domain.Workers.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SecureGovernment.Domain.Tests.Mocks.WorkerInformationMocker;

namespace SecureGovernment.Domain.Tests.Services.Workers.Process
{
    [TestClass]
    public class CipherscanWorkerTests
    {
        [TestMethod]
        public void Test_CipherscanWorker_Scan_ExecutableDoesntExist()
        {
            // Arrange
            var certificate = CreationHelpers.CreateCertificate();
            var issuer = CreationHelpers.CreateIntermediate();
            var uri = new Uri("https://google.com");

            var workerInformation = MockWorkerInformation(hostname: "google.com", certificate: certificate, issuer: issuer);
            var scanResults = new List<ScanResult>() { new ParsedDnsReponse() { Records = new[] { "DNS RECORD 1", "DNS RECORD 2" }.ToList() } };

            var previousWorkerMock = Utils.CreateMock<IAsyncWorker>();
            previousWorkerMock.Setup(x => x.Scan(workerInformation)).Returns(Task.FromResult(scanResults));

            var fileSystemMock = Utils.CreateMock<IFileSystem>();
            fileSystemMock.Setup(x => x.Exists("DOESNOTEXIST")).Returns(false);

            var cipherscanWorker = new CipherscanWorker(previousWorkerMock.Object, fileSystemMock.Object, "DOESNOTEXIST");

            // Act
            var rawCipherscanResult = cipherscanWorker.Scan(workerInformation);
            rawCipherscanResult.Wait();

            // Assert
            var records = rawCipherscanResult.Result;
            Assert.IsInstanceOfType(records.Single(), typeof(ParsedDnsReponse));

            var dnsResult = records.Single() as ParsedDnsReponse;
            Assert.AreEqual(2, dnsResult.Records.Count);
            Assert.AreEqual("DNS RECORD 1", dnsResult.Records[0]);
            Assert.AreEqual("DNS RECORD 2", dnsResult.Records[1]);
        }
    }
}
