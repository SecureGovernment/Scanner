using DnsClient;
using SecureGovernment.Domain.Models.DnsReponse.Parsed;
using System.Linq;

namespace SecureGovernment.Domain.DnsResponse
{
    public class DnssecReponse
    {
        public IDnsQueryResponse _Reponse { get; set; }
        public DnssecReponse(IDnsQueryResponse dnssecRecords)
        {
            this._Reponse = dnssecRecords;
        }

        public ParsedDnssecResponse ParseReponse()
        {
            var hasRrsigRecords = this._Reponse.Answers.Any();

            return new ParsedDnssecResponse()
            {
                HasDnssec = hasRrsigRecords
            };
        }
    }
}
