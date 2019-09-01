using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SecureGovernment.Domain.Models;
using System;
using System.Net;
using System.Threading.Tasks;
using static SecureGovernment.Domain.Tests.Mocks.ChainMocker;

namespace SecureGovernment.Domain.Tests.Models
{
    [TestClass]
    public class ConnectionTests
    {
        [TestMethod]
        public async Task Test_Connection_Connect_FirstUriConnects()
        {
            // Arrange
            var hostname = "whitehouse.gov";
            var resolvedUri = new Uri("https://whitehouse.gov");
            var connectionMock = Utils.CreateMock<Connection>(hostname);
            connectionMock.Setup(x => x.ConnectToUri(It.Is<Uri>(y => y.AbsoluteUri == "https://whitehouse.gov/"))).ReturnsAsync(resolvedUri);
            connectionMock.Setup(x => x.LoadCertificates(resolvedUri)).Returns(new EndPointInformation()
            {
                IPAddress = IPAddress.Parse("8.8.8.8"),
                Certificate = CreationHelper.CreateCertificate(),
                Chain = MockChain()
            });
            var connection = connectionMock.Object;

            // Act
            var workerInformation = await connection.Connect();

            // Assert
            Assert.AreEqual("whitehouse.gov", workerInformation.Hostname);
            Assert.AreEqual(resolvedUri, workerInformation.ResolvedHostname);
            Assert.IsTrue(workerInformation.IsConnectionSuccessful);
            Assert.AreEqual("8.8.8.8", workerInformation.IPAddress.ToString());
            Assert.IsNotNull(workerInformation.Certificate);
            Assert.IsNotNull(workerInformation.Chain);
        }

        [TestMethod]
        public async Task Test_Connection_Connect_LastUriConnects()
        {
            // Arrange
            var hostname = "whitehouse.gov";
            var resolvedUri = new Uri("http://www.whitehouse.gov");
            var connectionMock = Utils.CreateMock<Connection>(hostname);
            connectionMock.Setup(x => x.ConnectToUri(It.Is<Uri>(y => y.AbsoluteUri == "https://whitehouse.gov/"))).ReturnsAsync((Uri)null);
            connectionMock.Setup(x => x.ConnectToUri(It.Is<Uri>(y => y.AbsoluteUri == "https://www.whitehouse.gov/"))).ReturnsAsync((Uri)null);
            connectionMock.Setup(x => x.ConnectToUri(It.Is<Uri>(y => y.AbsoluteUri == "http://whitehouse.gov/"))).ReturnsAsync((Uri)null);
            connectionMock.Setup(x => x.ConnectToUri(It.Is<Uri>(y => y.AbsoluteUri == "http://www.whitehouse.gov/"))).ReturnsAsync(resolvedUri);
            connectionMock.Setup(x => x.LoadCertificates(resolvedUri)).Returns(new EndPointInformation()
            {
                IPAddress = IPAddress.Parse("8.8.8.8")
            });
            var connection = connectionMock.Object;

            // Act
            var workerInformation = await connection.Connect();

            // Assert
            Assert.AreEqual("whitehouse.gov", workerInformation.Hostname);
            Assert.AreEqual(resolvedUri, workerInformation.ResolvedHostname);
            Assert.IsTrue(workerInformation.IsConnectionSuccessful);
            Assert.AreEqual("8.8.8.8", workerInformation.IPAddress.ToString());
            Assert.IsNull(workerInformation.Certificate);
            Assert.IsNull(workerInformation.Chain);
        }

        [TestMethod]
        public async Task Test_Connection_Connect_CannotConnect()
        {
            // Arrange
            var hostname = "whitehouse.gov";
            var connectionMock = Utils.CreateMock<Connection>(hostname);
            connectionMock.Setup(x => x.ConnectToUri(It.Is<Uri>(y => y.AbsoluteUri == "https://whitehouse.gov/"))).ReturnsAsync((Uri)null);
            connectionMock.Setup(x => x.ConnectToUri(It.Is<Uri>(y => y.AbsoluteUri == "https://www.whitehouse.gov/"))).ReturnsAsync((Uri)null);
            connectionMock.Setup(x => x.ConnectToUri(It.Is<Uri>(y => y.AbsoluteUri == "http://whitehouse.gov/"))).ReturnsAsync((Uri)null);
            connectionMock.Setup(x => x.ConnectToUri(It.Is<Uri>(y => y.AbsoluteUri == "http://www.whitehouse.gov/"))).ReturnsAsync((Uri)null);

            var connection = connectionMock.Object;

            // Act
            var workerInformation = await connection.Connect();

            // Assert
            Assert.AreEqual("whitehouse.gov", workerInformation.Hostname);
            Assert.IsNull(workerInformation.ResolvedHostname);
            Assert.IsFalse(workerInformation.IsConnectionSuccessful);
            Assert.IsNull(workerInformation.IPAddress);
            Assert.IsNull(workerInformation.Certificate);
            Assert.IsNull(workerInformation.Chain);
        }
    }
}
