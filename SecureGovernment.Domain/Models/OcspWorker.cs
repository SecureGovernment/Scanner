using Org.BouncyCastle.Ocsp;
using SecureGovernment.Domain.Models.Ocsp;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace SecureGovernment.Domain.DnsRecords.Workers
{
    public class OcspWorker
    {

        private async Task<BasicOcspResp> GetOcspStatus(Ocsp ocsp)
        {
            byte[] reqArray = ocsp.CreateOcspReq().GetEncoded();
            var uris = ocsp.GetOcspUris();
            OcspResp resp;

            foreach (var uri in uris)
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                    request.Method = "POST";
                    var requestStream = request.GetRequestStream();
                    request.ContentLength = reqArray.Length;
                    request.ContentType = "application/ocsp-request";
                    request.Accept = "application/ocsp-response";
                    requestStream.Write(reqArray, 0, reqArray.Length);

                    using (WebResponse response = await request.GetResponseAsync())
                    using (Stream stream = response.GetResponseStream())
                    {
                        resp = new OcspResp(stream);
                    }
                    return (BasicOcspResp)resp.GetResponseObject();
                }
                catch { }
            }

            return null;
        }
    }
}
