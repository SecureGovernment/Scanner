using System.Collections.Generic;

namespace SecureGovernment {
    public class ScanViewModel
    {
        public bool HasCaa { get; set; }
        public List<string> IssueCas { get; set; }
        public List<string> IssueWildcardCas { get; set; }
        public List<string> Iodef { get; set; }
        public bool HasDkim { get; set; }
        public List<string> RawCaaRecords { get; set; }
        public List<string> RawDmarcRecords { get; set; }
        public bool HasDmarc { get; set; }
        public bool HasDnssec { get; set; }
        public List<string> RawDnssecRecords { get; set; }
        public bool HasMx { get; set; }
        public List<string> RawMxRecords { get; set; }
        public bool HasSpf { get; set; }
        public bool HasOldSpf { get; set; }
        public List<string> RawOldSpfRecords { get; set; }
        public List<string> RawSpfRecords { get; set; }
    }
}