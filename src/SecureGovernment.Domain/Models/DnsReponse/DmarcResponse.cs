using DnsClient;
using SecureGovernment.Domain.Models.DnsReponse.Parsed;
using System.Linq;

namespace SecureGovernment.Domain.DnsResponse
{
    public class DmarcResponse
    {
        public IDnsQueryResponse _Reponse { get; set; }

        public DmarcResponse(IDnsQueryResponse dnsReponse)
        {
            this._Reponse = dnsReponse;
        }

        public ParsedDmarcResponse ParseReponse()
        {
            var txtRecords = this._Reponse.Answers.TxtRecords().ToList();
            var dmarcRecords = txtRecords.SelectMany(x => x.Text).Where(x => x.ToLower().Contains("dmarc")).ToList();

            return new ParsedDmarcResponse()
            {
                Records = dmarcRecords
            };
        }
    }
}
