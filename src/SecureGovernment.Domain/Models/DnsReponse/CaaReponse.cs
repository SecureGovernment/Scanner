using DnsClient;
using DnsClient.Protocol;
using SecureGovernment.Domain.Models.DnsReponse.Parsed;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SecureGovernment.Domain.DnsResponse
{
    public class CaaReponse
    {
        public IDnsQueryResponse _Response { get; set; }

        public CaaReponse(IDnsQueryResponse dnsReponse)
        {
            this._Response = dnsReponse;
        }

        public ParsedCaaResponse ParseReponse()
        {
            var caa = this._Response.Answers.CaaRecords().ToList();
            var caaIssue = GetValuesForTag(caa, "issue");
            var caaIssuesWild = GetValuesForTag(caa, "issuewild");
            var caaIodef = GetValuesForTag(caa, "iodef");

            return new ParsedCaaResponse()
            {
                IssueCas = caaIssue,
                IssueWildCas = caaIssuesWild,
                Iodef = caaIodef
            };
        }

        private static List<string> GetValuesForTag(List<CaaRecord> caa, string tag)
        {
            return caa.Where(x => x.Tag.Equals(tag, StringComparison.OrdinalIgnoreCase)).Select(x => x.Value).ToList();
        }
    }
}
