﻿using DnsClient;
using SecureGovernment.Domain.Models.DnsReponse.Parsed;
using System.Collections.Generic;
using System.Linq;

namespace SecureGovernment.Domain.Models.DnsReponse
{
    public class DkimResponse
    {
        private List<(string Selector, IDnsQueryResponse Reponse, QueryType QueryType)> _Reponse { get; set; }

        public DkimResponse()
        {
            _Reponse = new List<(string, IDnsQueryResponse, QueryType)>();
        }

        public void AddResponse(string selector, IDnsQueryResponse dnsQueryResponse, QueryType queryType) => _Reponse.Add((selector, dnsQueryResponse, queryType));

        public ParsedDkimResponse ParseReponse()
        {
            var dkimRecords = new List<(string, QueryType, List<string>)>();
            foreach (var response in _Reponse)
            {
                if(response.QueryType == QueryType.TXT)
                {
                    var txtRecords = response.Reponse.Answers.TxtRecords().ToList();
                    var dkimRecordsForSelector = txtRecords.SelectMany(x => x.Text).Where(x => x.ToLower().Contains("dkim")).ToList();
                    if(dkimRecordsForSelector.Any())
                        dkimRecords.Add((response.Selector, response.QueryType, dkimRecordsForSelector));
                } else
                {
                    var cnameRecords = response.Reponse.Answers.CnameRecords().Select(x => x.CanonicalName.Value).ToList();
                    if(cnameRecords.Any())
                        dkimRecords.Add((response.Selector, response.QueryType, cnameRecords));
                }
            }

            return new ParsedDkimResponse()
            {
                DkimRecords = dkimRecords
            };
        }
    }
}
