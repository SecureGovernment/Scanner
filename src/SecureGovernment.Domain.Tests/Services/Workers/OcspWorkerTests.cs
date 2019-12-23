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
using static SecureGovernment.Domain.Tests.AssertionHelpers;
using static SecureGovernment.Domain.Tests.CreationHelpers;
using static SecureGovernment.Domain.Tests.Mocks.OcspMocker;
using static SecureGovernment.Domain.Tests.Mocks.ScanResultsMocker;
using static SecureGovernment.Domain.Tests.Mocks.WorkerInformationMocker;

namespace SecureGovernment.Domain.Tests.Services.Workers
{
    [TestClass]
    public class OcspWorkerTests
    {
        [TestMethod]
        public void Test_OcspWorker_Scan_NoCertificate()
        {
            //Arrange
            var workerInformation = MockWorkerInformation(hostname: "google.com");
            var previousWorker = MockPreviousWorker(workerInformation);
            var worker = new OcspWorker(previousWorker.Object);

            //Act
            var response = worker.Scan(workerInformation);
            response.Wait();

            //Assert
            var records = response.Result;
            Assert.AreEqual(2, records.Count);
            AssertPreviousScanResults(new[] { records[0], records[1] }.ToList());
        }

        [TestMethod]
        public void Test_OcspWorker_Scan_FirstValidOcspResponse()
        {
            // Arrange
            var certificate = CreationHelpers.CreateCertificate();
            var bcCertificate = CreationHelpers.CreateBCCertificate();
            var issuer = CreationHelpers.CreateIntermediate();
            var bcIssuer = CreationHelpers.CreateBCIntermediate();
            var uri = new Uri("https://google.com");
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);
            var ocspResponse = new OcspResponse() { Status = 0 };

            var workerInformation = MockWorkerInformation(hostname: "google.com", certificate: certificate, issuer: issuer);
            var ocsp = MockOcsp(certificate: bcCertificate, issuer: bcIssuer, ocspUris: new[] { uri, new Uri("https://shouldneverhapp.en") });

            var previousWorker = new Mock<IAsyncWorker>();
            previousWorker.Setup(x => x.Scan(workerInformation)).ReturnsAsync(new List<ScanResult>());

            var workerMock = new Mock<OcspWorker>(previousWorker.Object) { CallBase = true };
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
            var certificate = CreationHelpers.CreateCertificate();
            var bcCertificate = CreationHelpers.CreateBCCertificate();
            var issuer = CreationHelpers.CreateIntermediate();
            var bcIssuer = CreationHelpers.CreateBCIntermediate();
            var uri = new Uri("https://google.com");
            var bingUri = new Uri("https://bing.com");
            HttpWebRequest bingWebRequest = (HttpWebRequest)WebRequest.Create(bingUri);
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);
            var ocspResponse = new OcspResponse() { Status = 0 };

            var workerInformation = MockWorkerInformation(hostname: "google.com", certificate: certificate, issuer: issuer);
            var ocsp = MockOcsp(certificate: bcCertificate, issuer: bcIssuer, ocspUris: new[] { uri, bingUri });

            var previousWorker = new Mock<IAsyncWorker>();
            previousWorker.Setup(x => x.Scan(workerInformation)).ReturnsAsync(new List<ScanResult>());

            var workerMock = new Mock<OcspWorker>(previousWorker.Object) { CallBase = true };
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
            var certificate = CreationHelpers.CreateCertificate();
            var bcCertificate = CreationHelpers.CreateBCCertificate();
            var issuer = CreationHelpers.CreateIntermediate();
            var bcIssuer = CreationHelpers.CreateBCIntermediate();
            var uri = new Uri("https://google.com");
            var bingUri = new Uri("https://bing.com");
            HttpWebRequest bingWebRequest = (HttpWebRequest)WebRequest.Create(bingUri);
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);

            var workerInformation = MockWorkerInformation(hostname: "google.com", certificate: certificate, issuer: issuer);
            var ocsp = MockOcsp(certificate: bcCertificate, issuer: bcIssuer, ocspUris: new[] { uri, bingUri });

            var previousWorker = new Mock<IAsyncWorker>();
            previousWorker.Setup(x => x.Scan(workerInformation)).ReturnsAsync(new List<ScanResult>());

            var workerMock = new Mock<OcspWorker>(previousWorker.Object) { CallBase = true };
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
            var certificate = CreationHelpers.CreateCertificate();
            var bcCertificate = CreationHelpers.CreateBCCertificate();
            var issuer = CreationHelpers.CreateIntermediate();
            var bcIssuer = CreationHelpers.CreateBCIntermediate();

            var workerInformation = MockWorkerInformation(hostname: "google.com", certificate: certificate, issuer: issuer);
            var ocsp = MockOcsp(certificate: bcCertificate, issuer: bcIssuer, ocspUris: new Uri[0]);

            var previousWorker = new Mock<IAsyncWorker>();
            previousWorker.Setup(x => x.Scan(workerInformation)).ReturnsAsync(new List<ScanResult>());

            var workerMock = new Mock<OcspWorker>(previousWorker.Object) { CallBase = true };
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

        [TestMethod]
        public void Test_OcspWorker_ParseOcspReponse_GoodReponse()
        {
            //Arrange
            var workerInformation = MockWorkerInformation(hostname: "google.com");
            var previousWorker = MockPreviousWorker(workerInformation);
            var worker = new OcspWorker(previousWorker.Object);
            var ocsp = GetValidOcspResp();

            //Act
            var result = worker.ParseOcspResponse(ocsp);

            //Assert
            Assert.AreEqual(Enums.OcspRevocationStatus.Good, result.Status);
            Assert.AreEqual(DateTime.Parse("12/29/2019 6:49:37 PM"), result.NextUpdate);
            Assert.AreEqual(DateTime.Parse("12/22/2019 6:49:37 PM"), result.ProducedAt);
            Assert.AreEqual(DateTime.Parse("12/22/2019 6:49:37 PM"), result.ThisUpdate);
        }

        [TestMethod]
        public void Test_OcspWorker_ParseOcspReponse_RevokedResponse()
        {
            //Arrange
            var workerInformation = MockWorkerInformation(hostname: "google.com");
            var previousWorker = MockPreviousWorker(workerInformation);
            var worker = new OcspWorker(previousWorker.Object);
            var ocsp = GetInvalidOcspResp();

            //Act
            var result = worker.ParseOcspResponse(ocsp);

            //Assert
            Assert.AreEqual(Enums.OcspRevocationStatus.Revoked, result.Status);
            Assert.AreEqual(DateTime.Parse("12/29/2019 6:36:18 AM"), result.NextUpdate);
            Assert.AreEqual(DateTime.Parse("12/22/2019 7:21:18 AM"), result.ProducedAt);
            Assert.AreEqual(DateTime.Parse("12/22/2019 7:21:18 AM"), result.ThisUpdate);
            Assert.AreEqual(DateTime.Parse("4/9/2017 9:21:34 PM"), result.RevocationTime);
            Assert.AreEqual(-1, result.RevocationReason);
        }

        [TestMethod]
        public void Test_OcspWorker_CreateOcspRequest_Basic()
        {
            //Arrange
            var certificate = CreationHelpers.CreateCertificate();
            var bcCertificate = CreationHelpers.CreateBCCertificate();
            var issuer = CreationHelpers.CreateIntermediate();
            var bcIssuer = CreationHelpers.CreateBCIntermediate();

            var ocspUri = new Uri("http://ocsp.pki.goog/gts1o1");
            var workerInformation = MockWorkerInformation(hostname: "google.com", certificate: certificate, issuer: issuer);
            var ocsp = MockOcsp(certificate: bcCertificate, issuer: bcIssuer, ocspUris: new Uri[] { ocspUri });
            var previousWorker = MockPreviousWorker(workerInformation);
            var worker = new OcspWorker(previousWorker.Object);

            //Act
            var result = worker.CreateOcspRequest(ocsp, ocspUri);

            //Assert
            Assert.AreEqual(ocspUri, result.Address);
            Assert.AreEqual("POST", result.Method);
            Assert.AreEqual("application/ocsp-request", result.ContentType);
            Assert.AreEqual("application/ocsp-response", result.Accept);
        }
    }
}
