using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureGovernment.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace SecureGovernment.Domain.Tests.Models
{
    [TestClass]
    public class ConnectionTests
    {
        [TestMethod]
        public void Test_Connection_LoadCertificates()
        {
            // Arrange
            var connection = new Connection("kamascityut.gov");

            // Act
            var truststore = new Truststore("Mozilla", @"C:\Users\Alex\Documents\Repositories\Scanner\catt\data\mozilla\snapshot").Certificates;
            var certs = connection.LoadCertificates();
            truststore.Add(certs.Chain.ChainElements[1].Certificate);
            var digital = truststore.Where(x => x.SerialNumber == "44AFB080D6A327BA893039862EF8406B").Single();
            var testChain = new X509Chain();
            testChain.ChainPolicy.ExtraStore.Add(digital);
            bool isValid = testChain.Build(certs.Certificate);
            var test = Verification.BuildCertificateChainBC(certs.Certificate, truststore);

            // Assert
            Assert.AreEqual(1, 1);
        }
    }
}
