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

        public EndCertificate GetCertificateMap(List<X509Certificate2> cas)
        {
            var endCertificate = new EndCertificate() { Certificate = this._Certificate };

            var chainCertificatesWithoutEndCert = GetChainCertificates();

            if(chainCertificatesWithoutEndCert.Count != 0)
            {
                //Remove the end certificate
                chainCertificatesWithoutEndCert.RemoveAt(0);
                var intermediates = chainCertificatesWithoutEndCert.Where(x => !cas.Any(y => y.SerialNumber == x.SerialNumber));
                var roots = chainCertificatesWithoutEndCert.Where(x => cas.Any(y => y.SerialNumber == x.SerialNumber));

                ChainLink previousLink = endCertificate;
                foreach (var intermediate in intermediates)
                {
                    var intermediateLink = new IntermediateCertificate() { Certificate = intermediate };
                    intermediateLink.AddPrevious(previousLink);
                    previousLink = previousLink.Next.Single();
                }

                foreach (var root in roots)
                {
                    var newRootCertificate = new RootCertificate() { Certificate = root };
                    newRootCertificate.AddPrevious(previousLink);
                    //previousLink.Next.Add(RecurseRootCertificate(newRootCertificate, cas));
                }
   
            }

            return endCertificate;
        }

        private ChainLink RecurseRootCertificate(ChainLink currentLink, List<X509Certificate2> cas)
        {

            foreach (var ca in cas)
            {
                var chain = new X509Chain(false);
                chain.ChainPolicy.ExtraStore.Add(ca);
                var path = chain.Build(currentLink.Certificate);

                if (path)
                {
                    var newRootCert = new RootCertificate() { Certificate = ca };
                    newRootCert.AddPrevious(RecurseRootCertificate(newRootCert, cas));
                }
            }

            return currentLink;
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
