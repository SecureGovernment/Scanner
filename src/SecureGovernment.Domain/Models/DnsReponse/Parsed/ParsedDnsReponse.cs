using System;
using System.Collections.Generic;
using System.Text;

namespace SecureGovernment.Domain.Models.DnsReponse.Parsed
{
    public class ParsedDnsReponse : ScanResult
    {
        public List<string> Records { get; set; }
    }
}
