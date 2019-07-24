using System.Security.Cryptography.X509Certificates;

namespace SecureGovernment.Domain.Models.DnsRecords.Results
{
    public class WorkerInformation
    {
        public string Hostname { get; set; }
        public Org.BouncyCastle.X509.X509Certificate Certificate { get; set; }
        public Org.BouncyCastle.X509.X509Certificate Issuer { get; set; }
        public X509Chain Chain { get; set; }
    }
}
