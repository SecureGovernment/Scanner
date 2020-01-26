using SecureGovernment.Domain.Models;
using System;
using System.Collections.Generic;

namespace SecureGovernment.Api.ViewModel
{
    public class ScanResultViewModel
    {
        public DateTime Date { get; set; }
        public string Domain { get; set; }
        public List<ScanResult> Results { get; set; }
    }
}
