using System.Security.Cryptography.X509Certificates;

namespace SecureGovernment.Domain.Models.DnsRecords.Results
{
    public class WorkerInformation
    {
        public string Hostname { get; set; }
        public X509Certificate2 Certificate { get; set; }
        public X509Chain Chain { get; set; }
    }
}
