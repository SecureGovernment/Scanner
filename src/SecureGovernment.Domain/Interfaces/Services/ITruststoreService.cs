using SecureGovernment.Domain.Models;
using System.Collections.Generic;

namespace SecureGovernment.Domain.Interfaces.Services
{
    public interface ITruststoreService
    {
        IList<Truststore> GetTruststores();
    }
}
