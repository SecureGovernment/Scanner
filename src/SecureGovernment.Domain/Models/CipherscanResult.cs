using Newtonsoft.Json;
using System.Collections.Generic;

namespace SecureGovernment.Domain.Models
{
    public class CipherscanResult : ScanResult
    {
        [JsonProperty("target")]
        public string Target { get; set; }
        [JsonProperty("ip")]
        public string IP { get; set; }
        [JsonProperty("utctimestamp")]
        public string UtcTimestamp { get; set; }
        [JsonProperty("serverside")]
        public string ServerSide { get; set; }
        [JsonProperty("curves_fallback")]
        public string CurvesFallback { get; set; }
        [JsonProperty("ciphersuite")]
        public List<CipherscanCipherSuites> CipherSuites { get; set; }
    }

    public class CipherscanCipherSuites
    {
        [JsonProperty("cipher")]
        public string Cipher { get; set; }
        [JsonProperty("protocols")]
        public List<string> Protocols { get; set; }
        [JsonProperty("pubkey")]
        public List<string> PubKey { get; set; }
        [JsonProperty("sigalg")]
        public List<string> Sigalg { get; set; }
        [JsonProperty("trusted")]
        public string Trusted { get; set; }
        [JsonProperty("ticket_hint")]
        public string TicketHint { get; set; }
        [JsonProperty("ocsp_stapling")]
        public string OCSPStapling { get; set; }
        [JsonProperty("pfs")]
        public string Pfs { get; set; }
        [JsonProperty("curves")]
        public List<string> Curves { get; set; }

    }
}
