using System.Linq;

namespace SecureGovernment.Domain.Models.DnsReponse.Parsed
{
    public class ParsedMxResponse : ParsedDnsReponse
    {
        public bool HasMxRecords => this.Records.Any();
    }
}
