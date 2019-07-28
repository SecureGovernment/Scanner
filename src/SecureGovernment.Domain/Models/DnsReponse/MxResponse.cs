using DnsClient;
using SecureGovernment.Domain.Models.DnsReponse.Parsed;
using System.Linq;

namespace SecureGovernment.Domain.Models.DnsReponse
{
    public class MxResponse
    {
        public IDnsQueryResponse _Reponse { get; set; }
        public MxResponse(IDnsQueryResponse dnssecRecords)
        {
            this._Reponse = dnssecRecords;
        }

        public ParsedMxResponse ParseReponse()
        {
            var mxRecords = this._Reponse.Answers.MxRecords().Select(x => x.Exchange.Value).ToList();

            return new ParsedMxResponse()
            {
                Records = mxRecords
            };
        }
    }
}
