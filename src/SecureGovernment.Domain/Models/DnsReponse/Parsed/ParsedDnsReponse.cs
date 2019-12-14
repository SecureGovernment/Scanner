using System.Collections.Generic;

namespace SecureGovernment.Domain.Models.DnsReponse.Parsed
{
    public class ParsedDnsReponse : ScanResult
    {
        public List<string> Records { get; set; }
    }
}
