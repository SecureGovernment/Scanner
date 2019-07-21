using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SecureGovernment.Domain.Models.Workers
{
    public class SpfWorkerResult : WorkerResult
    {
        public List<string> OldSpfRecords { get; set; }
        public bool HasOldSpf => this.OldSpfRecords.Any();
        public List<string> Records { get; set; }
        public bool HasSpfRecords => HasOldSpf || this.Records.Any();
    }
}
