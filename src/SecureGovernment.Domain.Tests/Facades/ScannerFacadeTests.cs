using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SecureGovernment.Domain.Facades;
using SecureGovernment.Domain.Models;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using static SecureGovernment.Domain.Tests.Mocks.ConnectionMocker;
using static SecureGovernment.Domain.Tests.Mocks.WorkerInformationMocker;
using static SecureGovernment.Domain.Tests.Mocks.ChainMocker;

namespace SecureGovernment.Domain.Tests.Facades
{
    [TestClass]
    public class ScannerFacadeTests
    {
        [TestMethod]
        public async Task Test_ScannerFacade_ConnectToTarget_ValidConnection()
        {
            // Arrange
            var url = "google.com";
            var facadeMock = new Mock<ScannerFacade>() { CallBase = true };
            var workerInfo = MockWorkerInformation(hostname: url, certificate: CreationHelper.CreateCertificate(), chain: MockChain());
            facadeMock.Setup(x => x.CreateConnection(url)).Returns(MockConnection(url: url, workerInformation: workerInfo));

            var facade = facadeMock.Object;
            // Act
            var workerInformation = await facade.ConnectToTarget(url);

            // Assert
            Assert.AreSame(workerInfo, workerInformation);
        }
    }
}
