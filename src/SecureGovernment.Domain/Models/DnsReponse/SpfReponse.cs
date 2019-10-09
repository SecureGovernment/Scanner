using DnsClient;
using SecureGovernment.Domain.Models.DnsReponse.Parsed;
using System.Collections.Generic;
using System.Linq;

namespace SecureGovernment.Domain.DnsResponse
{
    public class SpfResponse
    {
        private IDnsQueryResponse _TxtReponse { get; set; }
        private IDnsQueryResponse _SpfReponse { get; set; }

        public SpfResponse(IDnsQueryResponse dnsReponse, IDnsQueryResponse spfDnsResponse)
        {
            this._TxtReponse = dnsReponse;
            this._SpfReponse = spfDnsResponse;
        }

        public ILookupClient LookupClient { get; set; }

        public ParsedSpfResponse ParseReponse()
        {
            var txtRecords = this._TxtReponse.Answers.TxtRecords().ToList();
            var spfRecordsFromTxt = txtRecords.SelectMany(x => x.Text).Where(x => x.ToLower().Contains("spf")).ToList();

            var spfRecords = this._SpfReponse.Answers.SpfRecords().ToList();
            var spfRecordsFromSpf = spfRecords.SelectMany(x => x.Text).ToList();

            return new ParsedSpfResponse() {
                Records = spfRecordsFromTxt,
                OldSpfRecords = spfRecordsFromSpf
            };
        }
    }
}
