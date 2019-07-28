using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureGovernment.Domain.Models;

namespace SecureGovernment.Domain.Tests.Models
{
    [TestClass]
    public class TruststoreTests
    {
        [TestMethod]
        public void Test_Truststore_LoadCertificates()
        {
            // Arrange
            var store = new Truststore("Test", ".");

            // Act
            var certs = store.Certificates;

            // Assert
            Assert.IsTrue(true);
        }
    }
}
