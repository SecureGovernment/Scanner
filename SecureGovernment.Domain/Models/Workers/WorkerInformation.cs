using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SecureGovernment.Domain.Models.Workers
{
    public class WorkerInformation
    {
        public string Hostname { get; set; }
        public X509Certificate2 Certificate { get; set; }
        public X509Chain Chain { get; set; }
    }
}
