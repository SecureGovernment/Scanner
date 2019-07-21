using System.Collections.Generic;

namespace SecureGovernment.Domain.Models.Workers
{
    public class CaaWorkerResult : WorkerResult
    {
        public bool HasCaaRecords => this.IssueCas.Count > 0 || this.IssueWildCas.Count > 0;
        public List<string> IssueCas { get; set; }
        public List<string> IssueWildCas { get; set; }
        public List<string> Iodef { get; set; }

        public CaaWorkerResult()
        {
            this.IssueCas = new List<string>();
            this.IssueWildCas = new List<string>();
            this.Iodef = new List<string>();
        }
    }
}
