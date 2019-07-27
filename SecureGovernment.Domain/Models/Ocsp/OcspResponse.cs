using SecureGovernment.Domain.Enums;
using System;

namespace SecureGovernment.Domain.Models.Ocsp
{
    public class OcspResponse : ScanResult
    {
        public DateTime ProducedAt { get; set; }
        public DateTime ThisUpdate { get; set; }
        public DateTime NextUpdate { get; set; }
        public OcspRevocationStatus Status { get; set; }

        public DateTime? RevocationTime { get; set; }
        public int? RevocationReason { get; set; }
    }
}
