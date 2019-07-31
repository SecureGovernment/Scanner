using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecureGovernment.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var paths = certificateAnalysis.GetCertificateMap(new List<X509Certificate2>());

            // Assert
            AssertChainLink(paths, certificate, null, null);
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
            var paths = certificateAnalysis.GetCertificateMap(new List<X509Certificate2>());

            // Assert
            AssertChainLink(paths, certificate, intermediateCertificate, null);
            AssertChainLink(paths.Next.Single(), intermediateCertificate, null, certificate);
        }


        [TestMethod]
        public void Test_CertificateAnalysis_GetCertificatePaths_FullChain()
        {
            // Arrange
            var certificate = CreationHelper.CreateCertificate();
            var intermediateCertificate = CreationHelper.CreateIntermediate();
            var root = CreationHelper.CreateCaCertificate();
            var chain = new X509Chain();
            chain.ChainPolicy.ExtraStore.Add(intermediateCertificate);
            chain.ChainPolicy.ExtraStore.Add(root);
            chain.Build(certificate);

            var certificateAnalysis = new CertificateAnalysis(certificate, chain);

            // Act
            var paths = certificateAnalysis.GetCertificateMap(new List<X509Certificate2>() { root });

            // Assert
            AssertChainLink(paths, certificate, intermediateCertificate, null);
            AssertChainLink(paths.Next.Single(), intermediateCertificate, root, certificate);
            AssertChainLink(paths.Next.Single().Next.Single(), root, null, intermediateCertificate);
        }

        [TestMethod]
        public void Test2()
        {
            var connection = new Connection("ouboces.org");
            var connectionCertificates = connection.LoadCertificates();
            var certAnalysis = new CertificateAnalysis(connectionCertificates.Certificate, connectionCertificates.Chain);
            var trustStore = new Truststore("Microsoft", @"C:\Users\Alex\Documents\Repositories\Scanner\catt\data\mozilla\snapshot");
            var trustStore1 = new Truststore("Microsoft", @"C:\Users\Alex\Documents\Repositories\Scanner\catt\data\adobe\snapshot");
            var trustStore2 = new Truststore("Microsoft", @"C:\Users\Alex\Documents\Repositories\Scanner\catt\data\apple\snapshot");
            var trustStor3 = new Truststore("Microsoft", @"C:\Users\Alex\Documents\Repositories\Scanner\catt\data\microsoft");

            var cas = trustStore.Certificates;
            cas.AddRange(trustStore1.Certificates);
            cas.AddRange(trustStore2.Certificates);
            cas.AddRange(trustStor3.Certificates);

            var map = certAnalysis.GetCertificateMap(cas);
            var x = 1;
        }

        private void AssertChainLink(ChainLink link, X509Certificate2 certificate, X509Certificate2 next, X509Certificate2 previous)
        {
            Assert.AreEqual(certificate, link.Certificate);
            Assert.AreEqual(next, link.Next?.SingleOrDefault()?.Certificate);
            Assert.AreEqual(previous, link.Previous?.SingleOrDefault()?.Certificate);
        }
    }
}
