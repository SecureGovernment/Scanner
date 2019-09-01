using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SecureGovernment.Domain.Interfaces;
using SecureGovernment.Domain.Models;
using SecureGovernment.Domain.Models.Ocsp;
using SecureGovernment.Domain.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using static SecureGovernment.Domain.Tests.Mocks.OcspMocker;
using static SecureGovernment.Domain.Tests.Mocks.WorkerInformationMocker;

namespace SecureGovernment.Domain.Tests.Services.Workers
{
    [TestClass]
    public class OcspWorkerTests
    {
        [TestMethod]
        public void Test_OcspWorker_Scan_FirstValidOcspResponse()
        {
            // Arrange
            var certificate = CreationHelper.CreateCertificate();
            var bcCertificate = CreationHelper.CreateBCCertificate();
            var issuer = CreationHelper.CreateIntermediate();
            var bcIssuer = CreationHelper.CreateBCIntermediate();
            var uri = new Uri("https://google.com");
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);
            var ocspResponse = new OcspResponse() { Status = 0 };

            var workerInformation = MockWorkerInformation(hostname: "google.com", certificate: certificate, issuer: issuer);
            var ocsp = MockOcsp(certificate: bcCertificate, issuer: bcIssuer, ocspUris: new[] { uri, new Uri("https://shouldneverhapp.en") });

            var previousWorker = Utils.CreateMock<IAsyncWorker>();
            previousWorker.Setup(x => x.Scan(workerInformation)).ReturnsAsync(new List<ScanResult>());

            var workerMock = Utils.CreateMockOfSelf<OcspWorker>(previousWorker.Object);
            workerMock.Setup(x => x.CreateOcsp(bcCertificate, bcIssuer)).Returns(ocsp);
            workerMock.Setup(x => x.CreateOcspRequest(ocsp, uri)).Returns(webRequest);
            workerMock.Setup(x => x.SendOcspRequest(webRequest)).ReturnsAsync(ocspResponse);

            // Act
            var worker = workerMock.Object;
            var response = worker.Scan(workerInformation);
            response.Wait();

            // Assert
            var result = response.Result.Single() as OcspResponse;
            Mock.VerifyAll();
            Assert.AreEqual(Enums.OcspRevocationStatus.Good, result.Status);
            Assert.IsNull(result.RevocationReason);
        }

        [TestMethod]
        public void Test_OcspWorker_Scan_OneInvalidThenValidOcspResponse()
        {
            // Arrange
            var certificate = CreationHelper.CreateCertificate();
            var bcCertificate = CreationHelper.CreateBCCertificate();
            var issuer = CreationHelper.CreateIntermediate();
            var bcIssuer = CreationHelper.CreateBCIntermediate();
            var uri = new Uri("https://google.com");
            var bingUri = new Uri("https://bing.com");
            HttpWebRequest bingWebRequest = (HttpWebRequest)WebRequest.Create(bingUri);
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);
            var ocspResponse = new OcspResponse() { Status = 0 };

            var workerInformation = MockWorkerInformation(hostname: "google.com", certificate: certificate, issuer: issuer);
            var ocsp = MockOcsp(certificate: bcCertificate, issuer: bcIssuer, ocspUris: new[] { uri, bingUri });

            var previousWorker = Utils.CreateMock<IAsyncWorker>();
            previousWorker.Setup(x => x.Scan(workerInformation)).ReturnsAsync(new List<ScanResult>());

            var workerMock = Utils.CreateMockOfSelf<OcspWorker>(previousWorker.Object);
            workerMock.Setup(x => x.CreateOcsp(bcCertificate, bcIssuer)).Returns(ocsp);
            workerMock.Setup(x => x.CreateOcspRequest(ocsp, uri)).Returns(webRequest);
            workerMock.Setup(x => x.CreateOcspRequest(ocsp, bingUri)).Returns(bingWebRequest);
            workerMock.Setup(x => x.SendOcspRequest(webRequest)).ThrowsAsync(new Exception());
            workerMock.Setup(x => x.SendOcspRequest(bingWebRequest)).ReturnsAsync(ocspResponse);

            // Act
            var worker = workerMock.Object;
            var response = worker.Scan(workerInformation);
            response.Wait();

            // Assert
            var result = response.Result.Single() as OcspResponse;
            Mock.VerifyAll();
            Assert.AreEqual(Enums.OcspRevocationStatus.Good, result.Status);
            Assert.IsNull(result.RevocationReason);
        }

        [TestMethod]
        public void Test_OcspWorker_Scan_NoValidOcspResponses()
        {
            // Arrange
            var certificate = CreationHelper.CreateCertificate();
            var bcCertificate = CreationHelper.CreateBCCertificate();
            var issuer = CreationHelper.CreateIntermediate();
            var bcIssuer = CreationHelper.CreateBCIntermediate();
            var uri = new Uri("https://google.com");
            var bingUri = new Uri("https://bing.com");
            HttpWebRequest bingWebRequest = (HttpWebRequest)WebRequest.Create(bingUri);
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);

            var workerInformation = MockWorkerInformation(hostname: "google.com", certificate: certificate, issuer: issuer);
            var ocsp = MockOcsp(certificate: bcCertificate, issuer: bcIssuer, ocspUris: new[] { uri, bingUri });

            var previousWorker = Utils.CreateMock<IAsyncWorker>();
            previousWorker.Setup(x => x.Scan(workerInformation)).ReturnsAsync(new List<ScanResult>());

            var workerMock = Utils.CreateMockOfSelf<OcspWorker>(previousWorker.Object);
            workerMock.Setup(x => x.CreateOcsp(bcCertificate, bcIssuer)).Returns(ocsp);
            workerMock.Setup(x => x.CreateOcspRequest(ocsp, uri)).Returns(webRequest);
            workerMock.Setup(x => x.CreateOcspRequest(ocsp, bingUri)).Returns(bingWebRequest);
            workerMock.Setup(x => x.SendOcspRequest(webRequest)).ThrowsAsync(new Exception());
            workerMock.Setup(x => x.SendOcspRequest(bingWebRequest)).ThrowsAsync(new Exception());

            // Act
            var worker = workerMock.Object;
            var response = worker.Scan(workerInformation);
            response.Wait();

            // Assert
            var result = response.Result.Single() as OcspResponse;
            Mock.VerifyAll();
            Assert.AreEqual(Enums.OcspRevocationStatus.Error, result.Status);
            Assert.IsNull(result.RevocationReason);
        }

        [TestMethod]
        public void Test_OcspWorker_Scan_NoUris()
        {
            // Arrange
            var certificate = CreationHelper.CreateCertificate();
            var bcCertificate = CreationHelper.CreateBCCertificate();
            var issuer = CreationHelper.CreateIntermediate();
            var bcIssuer = CreationHelper.CreateBCIntermediate();

            var workerInformation = MockWorkerInformation(hostname: "google.com", certificate: certificate, issuer: issuer);
            var ocsp = MockOcsp(certificate: bcCertificate, issuer: bcIssuer, ocspUris: new Uri[0]);

            var previousWorker = Utils.CreateMock<IAsyncWorker>();
            previousWorker.Setup(x => x.Scan(workerInformation)).ReturnsAsync(new List<ScanResult>());

            var workerMock = Utils.CreateMockOfSelf<OcspWorker>(previousWorker.Object);
            workerMock.Setup(x => x.CreateOcsp(bcCertificate, bcIssuer)).Returns(ocsp);

            // Act
            var worker = workerMock.Object;
            var response = worker.Scan(workerInformation);
            response.Wait();

            // Assert
            var result = response.Result.Single() as OcspResponse;
            Mock.VerifyAll();
            Assert.AreEqual(Enums.OcspRevocationStatus.Unknown, result.Status);
            Assert.IsNull(result.RevocationReason);
        }
    }
}
