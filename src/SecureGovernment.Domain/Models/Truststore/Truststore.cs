using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace SecureGovernment.Domain.Models.Truststore
{
    public abstract class Truststore
    {
        private protected abstract IList<X509Certificate2> GetCertificates();
    }
}
