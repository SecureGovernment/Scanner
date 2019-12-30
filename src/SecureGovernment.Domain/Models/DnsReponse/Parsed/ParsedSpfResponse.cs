using System.Collections.Generic;
using System.Linq;

namespace SecureGovernment.Domain.Models.DnsReponse.Parsed
{
    public class ParsedSpfResponse : ParsedDnsReponse
    {
        public List<string> SpfTypeSpfRecords { get; set; }
        public List<string> TxtTypeSpfRecords { get; set; }
        public bool HasTxtTypeRecords => this.TxtTypeSpfRecords != null && this.TxtTypeSpfRecords.Any();
        public bool HasSpfTypeRecords => this.SpfTypeSpfRecords != null && this.SpfTypeSpfRecords.Any();
        public bool HasSpfRecords => this.HasSpfTypeRecords || this.HasTxtTypeRecords;
    }
}
