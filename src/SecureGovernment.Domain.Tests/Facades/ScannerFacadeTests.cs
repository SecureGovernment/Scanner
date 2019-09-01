using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SecureGovernment.Domain.Facades;
using System.Threading.Tasks;
using static SecureGovernment.Domain.Tests.Mocks.ChainMocker;
using static SecureGovernment.Domain.Tests.Mocks.ConnectionMocker;
using static SecureGovernment.Domain.Tests.Mocks.WorkerInformationMocker;

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
            var facadeMock = Utils.CreateMockOfSelf<ScannerFacade>();
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
