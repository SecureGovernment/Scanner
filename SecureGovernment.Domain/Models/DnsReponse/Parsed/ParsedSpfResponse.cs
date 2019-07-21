using System.Collections.Generic;
using System.Linq;

namespace SecureGovernment.Domain.Models.DnsReponse.Parsed
{
    public class ParsedSpfResponse
    {
        public List<string> OldSpfRecords { get; set; }
        public bool HasOldSpf => this.OldSpfRecords.Any();
        public List<string> Records { get; set; }
        public bool HasSpfRecords => HasOldSpf || this.Records.Any();
    }
}
