using SecureGovernment.Domain.Infastructure.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecureGovernment.Domain.Interfaces.Infastructure
{
    public interface ISettings
    {
        List<TruststoreSetting> Truststores { get; set; }
    }
}
