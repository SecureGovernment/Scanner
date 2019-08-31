using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SecureGovernment.Domain.Models
{
    public class Chain
    {
        private X509Chain _Chain { get; }

        public Chain(X509Chain chain)
        {
            this._Chain = chain;
        }

        public List<X509Certificate2> Certificates => GetCertificates();

        private List<X509Certificate2> GetCertificates()
        {
            var certs = new List<X509Certificate2>();
            foreach (var element in this._Chain.ChainElements)
                certs.Add(element.Certificate);
            return certs;
        }
    }
}
