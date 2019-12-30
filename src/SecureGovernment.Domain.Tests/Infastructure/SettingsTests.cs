using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureGovernment.Domain.Infastructure.Settings;
using System.Collections.Generic;

namespace SecureGovernment.Domain.Tests.Infastructure
{
    [TestClass]
    public class SettingsTests
    {
        [TestMethod]
        public void Test_Settings()
        {
            // Arrange
            var settings = new Settings();

            //Act
            settings.DkimSelectors = new List<string>() { "google", "example1" };
            settings.Truststores = new List<TruststoreSetting>() { new TruststoreSetting() { Name = "Apple", Directory = "./apple" } };

            //Assert
            CollectionAssert.AreEqual(new List<string>() { "google", "example1" }, settings.DkimSelectors);
            Assert.AreEqual(1, settings.Truststores.Count);
            Assert.AreEqual("Apple", settings.Truststores[0].Name);
            Assert.AreEqual("./apple", settings.Truststores[0].Directory);
        }
    }
}
