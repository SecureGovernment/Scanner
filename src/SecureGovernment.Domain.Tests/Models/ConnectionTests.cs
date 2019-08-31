using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureGovernment.Domain.Models;

namespace SecureGovernment.Domain.Tests.Models
{
    [TestClass]
    public class ConnectionTests
    {
        [TestMethod]
        public void Test_Connection_ValidHost()
        {
            // Arrange
            var hostname = "whitehouse.gov";
            var connection = new Connection(hostname);

            // Act
            var workerInformation = connection.Connect();
            workerInformation.Wait();

            // Assert
            Assert.IsTrue(true);
        }
    }
}
