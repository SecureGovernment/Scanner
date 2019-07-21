using System;
using System.Collections.Generic;
using System.Text;

namespace SecureGovernment.Domain.Models.Workers
{
    public class DmarcWorkerResult : WorkerResult
    {
        public List<string> Records { get; set; }
    }
}
