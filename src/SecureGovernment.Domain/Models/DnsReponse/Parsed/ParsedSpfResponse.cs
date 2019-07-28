using System.Collections.Generic;
using System.Linq;

namespace SecureGovernment.Domain.Models.DnsReponse.Parsed
{
    public class ParsedSpfResponse : ParsedDnsReponse
    {
        public List<string> OldSpfRecords { get; set; }
        public bool HasOldSpf => this.OldSpfRecords != null && this.OldSpfRecords.Any();
        public bool HasSpfRecords => HasOldSpf || (this.Records != null && this.Records.Any());
    }
}
