using DnsClient;
using SecureGovernment.Domain.Models.DnsReponse.Parsed;
using System.Collections.Generic;
using System.Linq;

namespace SecureGovernment.Domain.DnsResponse
{
    public class SpfResponse
    {
        private IDnsQueryResponse _Reponse { get; set; }
        public SpfResponse(IDnsQueryResponse dnsReponse)
        {
            this._Reponse = dnsReponse;
        }

        public ILookupClient LookupClient { get; set; }

        public ParsedSpfResponse ParseReponse()
        {
            var txtRecords = this._Reponse.Answers.TxtRecords().ToList();
            var spfRecords = txtRecords.SelectMany(x => x.Text).Where(x => x.ToLower().Contains("spf")).ToList();

            return new ParsedSpfResponse() {
                Records = spfRecords,
                OldSpfRecords = new List<string>() //TODO: Add support for old SPF records
            };
        }
    }
}
