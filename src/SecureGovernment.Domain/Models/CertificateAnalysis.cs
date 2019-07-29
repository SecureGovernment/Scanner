using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Linq;

namespace SecureGovernment.Domain.Models
{

    public class CertificateAnalysis
    {
        private X509Certificate2 _Certificate { get; }
        private X509Chain _Chain { get; }

        public CertificateAnalysis(X509Certificate2 endCertificate, X509Chain chain)
        {
            this._Certificate = endCertificate;
            this._Chain = chain;
        }

        public List<EndCertificate> GetCertificatePaths(List<X509Certificate2> cas)
        {
            var paths = new List<EndCertificate>
            {
                new EndCertificate() { Certificate = this._Certificate, Next = null }
            };

            var chainCertificatesWithoutEndCert = GetChainCertificates();

            if(chainCertificatesWithoutEndCert.Count != 0)
            {
                //Remove the end certificate
                chainCertificatesWithoutEndCert.RemoveAt(0);
                var intermediates = chainCertificatesWithoutEndCert.Where(x => !cas.Any(y => y.SerialNumber == x.SerialNumber));
    
                var intermediateChain = new EndCertificate() { Certificate = this._Certificate, Next = null };
                ChainLink previousLink = intermediateChain;
                foreach (var intermediate in intermediates)
                {
                    previousLink.Next = new IntermediateCertificate() { Certificate = intermediate, Next = null, Previous = previousLink };
                    previousLink = previousLink.Next;
                }
    
                if(intermediateChain.Next != null)
                    paths.Add(intermediateChain);
    
            }

            return paths;
        }

        private List<X509Certificate2> GetChainCertificates()
        {
            var certificates = new List<X509Certificate2>();

            foreach (var cert in this._Chain.ChainElements)
            {
                certificates.Add(cert.Certificate);
            }

            return certificates;
        }
    }
}
