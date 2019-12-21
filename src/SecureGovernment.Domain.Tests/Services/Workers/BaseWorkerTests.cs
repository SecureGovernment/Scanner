using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureGovernment.Domain.Workers;
using static SecureGovernment.Domain.Tests.Mocks.WorkerInformationMocker;

namespace SecureGovernment.Domain.Tests.Services.Workers
{
    [TestClass]
    public class BaseWorkerTests
    {
        [TestMethod]
        public void Test_BaseWorker_Scan()
        {
            //Arrange
            var workerInformation = MockWorkerInformation(hostname: "google.com");
            var baseWorker = new BaseWorker();

            //Act
            var rawRecords = baseWorker.Scan(workerInformation);
            rawRecords.Wait();

            // Assert
            var records = rawRecords.Result;
            Assert.AreEqual(0, records.Count);
        }
    }
}
