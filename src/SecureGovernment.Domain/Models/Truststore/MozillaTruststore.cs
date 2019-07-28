using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SecureGovernment.Domain.Models.Truststore
{
    public class MozillaTruststore : Truststore
    {
        private string _Directory { get; }

        public IList<X509Certificate2> Certificates => GetCertificates();
        public MozillaTruststore(string directory)
        {
            this._Directory = directory;
        }

        private protected override IList<X509Certificate2> GetCertificates()
        {
            throw new NotImplementedException();
        }
    }
}
