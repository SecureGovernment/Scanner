using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureGovernment.Domain.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SecureGovernment.Domain.Tests.Models
{
    [TestClass]
    public class CertificateAnalysisTests
    {
        [TestMethod]
        public void Test_CertificateAnalysis_GetCertificatePaths_NoCaCerts()
        {
            // Arrange
            var certificate = CreationHelper.CreateCertificate();
            var chain = new X509Chain();

            var certificateAnalysis = new CertificateAnalysis(certificate, chain);

            // Act
            var paths = certificateAnalysis.GetCertificatePaths(new List<X509Certificate2>());

            // Assert
            Assert.AreEqual(1, paths.Count);
            AssertChainLink(paths[0], certificate, null, null);
        }

        [TestMethod]
        public void Test_CertificateAnalysis_GetCertificatePaths_OneIntermediateCert()
        {
            // Arrange
            var certificate = CreationHelper.CreateCertificate();
            var intermediateCertificate = CreationHelper.CreateIntermediate();
            var chain = new X509Chain();
            chain.ChainPolicy.ExtraStore.Add(intermediateCertificate);
            chain.Build(certificate);

            var certificateAnalysis = new CertificateAnalysis(certificate, chain);

            // Act
            var paths = certificateAnalysis.GetCertificatePaths(new List<X509Certificate2>());

            // Assert
            Assert.AreEqual(2, paths.Count);
            AssertChainLink(paths[0], certificate, null, null);
            AssertChainLink(paths[1], certificate, intermediateCertificate, null);
            AssertChainLink(paths[1].Next, intermediateCertificate, null, certificate);
        }

        private void AssertChainLink(ChainLink link, X509Certificate2 certificate, X509Certificate2 next, X509Certificate2 previous)
        {
            Assert.AreEqual(certificate, link.Certificate);
            Assert.AreEqual(next, link.Next?.Certificate);
            Assert.AreEqual(previous, link.Previous?.Certificate);
        }
    }
}
