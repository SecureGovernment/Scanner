using System.Linq;

namespace SecureGovernment.Domain.Models.DnsReponse.Parsed
{
    public class ParsedDnssecResponse : ParsedDnsReponse
    {
        public bool HasDnssec => this.Records.Any();
    }
}
