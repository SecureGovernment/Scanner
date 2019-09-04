using SecureGovernment.Domain.Interfaces.Infastructure;
using System.Collections.Generic;

namespace SecureGovernment.Domain.Infastructure.Settings
{
    public class Settings : ISettings
    {
        public string DnsIp { get; set; }
        public string CipherscanPath { get; set; }
        public List<TruststoreSetting> Truststores { get; set; }
    }
}