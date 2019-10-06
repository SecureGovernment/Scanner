using DnsClient;
using System.Collections.Generic;
using System.Linq;

namespace SecureGovernment.Domain.Models.DnsReponse.Parsed
{
    public class ParsedDkimResponse : ParsedDnsReponse
    {
        public List<(string Selector, QueryType, List<string> Reponse)> DkimRecords { get; set; }
        public bool HasDkim => this.DkimRecords.Any();
    }
}
