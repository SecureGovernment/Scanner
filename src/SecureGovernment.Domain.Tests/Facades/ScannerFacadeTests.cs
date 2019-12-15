using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SecureGovernment.Domain.Facades;
using SecureGovernment.Domain.Models;
using Serilog;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using static SecureGovernment.Domain.Tests.Mocks.ConnectionMocker;

namespace SecureGovernment.Domain.Tests.Facades
{
    [TestClass]
    public class ScannerFacadeTests
    {
        [TestMethod]
        public void Test_ScannerFacade_ConnectToTarget_ValidConnection()
        {
            // Arrange
            var url = "google.com";
            var facadeMock = new Mock<ScannerFacade>() { CallBase = true };
            var cert = new X509Certificate2();
            var chain = new Chain(new X509Chain());
            facadeMock.Setup(x => x.CreateConnection(url)).Returns(MockConnection(url: url, loadCertificate: (cert, chain)));

            var facade = facadeMock.Object;
            // Act
            var workerInformation = facade.ConnectToTarget(url);

            // Assert
            Assert.AreEqual("google.com", workerInformation.Hostname);
            Assert.AreSame(cert, workerInformation.Certificate);
            Assert.AreSame(chain, workerInformation.Chain);
        }

        [TestMethod]
        public void Test_ScannerFacade_ConnectToTarget_InvalidConnection()
        {
            // Arrange
            var url = "google.com";
            var facadeMock = Utils.CreateMock<ScannerFacade>();
            var loggerMock = Utils.CreateMock<ILogger>();
            var cert = new X509Certificate2();
            var chain = new X509Chain();
            facadeMock.Setup(x => x.CreateConnection(url)).Returns(MockConnection(url: url, throwLoadCertificateException: true));
            loggerMock.Setup(x => x.Error(It.IsAny<SocketException>(), "Cannot connect to google.com.")).Verifiable();

            var facade = facadeMock.Object;
            facade.Logger = loggerMock.Object;

            // Act
            var workerInformation = facade.ConnectToTarget(url);

            // Assert
            loggerMock.Verify(x => x.Error(It.IsAny<SocketException>(), "Cannot connect to google.com."), Times.Once);
            Assert.AreEqual("google.com", workerInformation.Hostname);
            Assert.IsNull(workerInformation.Certificate);
            Assert.IsNull(workerInformation.Chain);
        }

    }
}
