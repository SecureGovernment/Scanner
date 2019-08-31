using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SecureGovernment.Domain.Infastructure.Settings;
using SecureGovernment.Domain.Interfaces.Infastructure;
using SecureGovernment.Domain.Models;
using SecureGovernment.Domain.Services;
using System.Collections.Generic;

namespace SecureGovernment.Domain.Tests.Services
{
    [TestClass]
    public class TruststoreServiceTests
    {
        [TestMethod]
        public void Test_TruststoreService_GetTruststores_NoTruststores()
        {
            // Arrange
            var settings = new Mock<ISettings>();
            settings.Setup(x => x.Truststores).Returns(new List<TruststoreSetting>());

            var service = new TruststoreService() { Settings = settings.Object };

            // Act
            var truststores = service.GetTruststores();

            // Assert
            Assert.AreEqual(0, truststores.Count);
        }

        [TestMethod]
        public void Test_TruststoreService_GetTruststores_MultipleValidTruststores()
        {
            // Arrange
            var truststoreSettings = new List<TruststoreSetting>()
            {
                new TruststoreSetting(){ Name = "Store 1", Directory = "." },
                new TruststoreSetting(){ Name = "Store 2", Directory = "../" },
            };

            var settings = new Mock<ISettings>();
            settings.Setup(x => x.Truststores).Returns(truststoreSettings);

            var service = new TruststoreService() { Settings = settings.Object };

            // Act
            var truststores = service.GetTruststores();

            // Assert
            Assert.AreEqual(2, truststores.Count);
            AssertTruststore(truststores[0], "Store 1", ".");
            AssertTruststore(truststores[1], "Store 2", "../");
        }

        [TestMethod]
        public void Test_TruststoreService_GetTruststores_MultipleValidAndOneInvalidTruststores()
        {
            // Arrange
            var truststoreSettings = new List<TruststoreSetting>()
            {
                new TruststoreSetting(){ Name = "Store 1", Directory = "." },
                new TruststoreSetting(){ Name = "Store 2", Directory = "INVALID_DIR_#($" },
                new TruststoreSetting(){ Name = "Store 3", Directory = "../" },
            };

            var settings = new Mock<ISettings>();
            settings.Setup(x => x.Truststores).Returns(truststoreSettings);

            var service = new TruststoreService() { Settings = settings.Object };

            // Act
            var truststores = service.GetTruststores();

            // Assert
            Assert.AreEqual(2, truststores.Count);
            AssertTruststore(truststores[0], "Store 1", ".");
            AssertTruststore(truststores[1], "Store 3", "../");
        }

        private void AssertTruststore(Truststore truststore, string name, string directory)
        {
            Assert.AreEqual(name, truststore.Name);
            Assert.AreEqual(directory, truststore.Directory);
            Assert.AreEqual(0, truststore.Certificates.Count);
        }
    }
}
