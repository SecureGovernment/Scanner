using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace SecureGovernment.Domain.Models.DnsRecords.Results
{
    public class WorkerInformation
    {
        public string Hostname { get; set; }
        public X509Certificate2 Certificate { get; set; }
        public virtual X509Certificate2 Issuer => this.Chain.Certificates.Count >= 2 ? this.Chain.Certificates[1] : this.Certificate;
        public Chain Chain { get; set; }
    }
}
