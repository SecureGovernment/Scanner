using SecureGovernment.Domain.Infastructure.Settings;
using System.Collections.Generic;

namespace SecureGovernment.Domain.Interfaces.Infastructure
{
    public interface ISettings
    {
        string DnsIp { get; set; }
        string CipherscanPath { get; set; }
        List<TruststoreSetting> Truststores { get; set; }
    }
}
