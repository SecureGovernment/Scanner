using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace SecureGovernment.Domain.Models
{
    public class EndPointInformation
    {
        public IPAddress IPAddress { get; set; }
        public X509Certificate2 Certificate { get; set; }
        public Chain Chain { get; set; }
    }
}
