using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RestSharp;
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
            var connectionMock = Utils.CreateMockOfSelf<Connection>(hostname);
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
            var connectionMock = Utils.CreateMockOfSelf<Connection>(hostname);
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
            var connectionMock = Utils.CreateMockOfSelf<Connection>(hostname);
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

        [TestMethod]
        public async Task Test_Connection_ConnectToUri_ValidResponse()
        {
            // Arrange
            var uri = new Uri("https://google.com");
            var responseUri = new Uri("https://www.bing.com");

            var restClient = Utils.CreateMock<IRestClient>();
            var restRequest = Utils.CreateMock<IRestRequest>();
            var restResponse = Utils.CreateMock<IRestResponse<RestRequest>>();
            var connectionMock = Utils.CreateMockOfSelf<Connection>(uri.Host);

            restResponse.Setup(x => x.ResponseUri).Returns(responseUri);
            restResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.OK);
            restClient.Setup(x => x.ExecuteGetTaskAsync<RestRequest>(restRequest.Object)).ReturnsAsync(restResponse.Object);
            connectionMock.Setup(x => x.CreateRestClient(uri)).Returns(restClient.Object);
            connectionMock.Setup(x => x.CreateRestRequest(uri)).Returns(restRequest.Object);

            var connection = connectionMock.Object;

            // Act
            var resolvedUri = await connection.ConnectToUri(uri);

            // Assert
            Assert.AreEqual(responseUri, resolvedUri);
        }

        [TestMethod]
        public async Task Test_Connection_ConnectToUri_InvalidResponse()
        {
            // Arrange
            var uri = new Uri("https://google.com");

            var restClient = Utils.CreateMock<IRestClient>();
            var restRequest = Utils.CreateMock<IRestRequest>();
            var restResponse = Utils.CreateMock<IRestResponse<RestRequest>>();
            var connectionMock = Utils.CreateMockOfSelf<Connection>(uri.Host);

            restResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.NotFound);
            restClient.Setup(x => x.ExecuteGetTaskAsync<RestRequest>(restRequest.Object)).ReturnsAsync(restResponse.Object);
            connectionMock.Setup(x => x.CreateRestClient(uri)).Returns(restClient.Object);
            connectionMock.Setup(x => x.CreateRestRequest(uri)).Returns(restRequest.Object);

            var connection = connectionMock.Object;

            // Act
            var resolvedUri = await connection.ConnectToUri(uri);

            // Assert
            Assert.IsNull(resolvedUri);
        }
    }
}
