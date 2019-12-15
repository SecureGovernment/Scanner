using SecureGovernment.Domain.Interfaces.Infastructure;
using SecureGovernment.Domain.Interfaces.Services;
using SecureGovernment.Domain.Models;
using Serilog;
using System.Collections.Generic;
using System.IO;

namespace SecureGovernment.Domain.Services
{
    public class TruststoreService : ITruststoreService
    {
        public ISettings Settings { get; set; }

        public IList<Truststore> GetTruststores()
        {
            var truststores = new List<Truststore>();

            foreach (var truststore in this.Settings.Truststores)
                if (Directory.Exists(truststore.Directory))
                    truststores.Add(new Truststore(truststore.Name, truststore.Directory));
                else
                    Log.Information($"{truststore.Name} truststore does not exist at {truststore.Directory}.");

            return truststores;
        }
    }
}
