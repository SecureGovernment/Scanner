using SecureGovernment.Domain.Interfaces.Infastructure;
using System.Collections.Generic;

namespace SecureGovernment.Domain.Infastructure.Settings
{
    public class Settings : ISettings
    {
        public List<TruststoreSetting> Truststores { get; set; }
        public List<string> DkimSelectors { get; set; }
    }
}
