using SecureGovernment.Domain.Infastructure.Settings;
using System.Collections.Generic;

namespace SecureGovernment.Domain.Interfaces.Infastructure
{
    public interface ISettings
    {
        List<TruststoreSetting> Truststores { get; set; }
        List<string> DkimSelectors { get; set; }
    }
}
