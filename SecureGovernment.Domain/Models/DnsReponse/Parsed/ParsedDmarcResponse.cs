using System.Collections.Generic;
using System.Linq;

namespace SecureGovernment.Domain.Models.DnsReponse.Parsed
{
    public class ParsedDmarcResponse : ParsedDnsReponse
    {
        public bool HasDmarcRecords => this.Records.Any();
    }
}
