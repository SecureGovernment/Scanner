using Org.BouncyCastle.Ocsp;
using SecureGovernment.Domain.Interfaces;
using SecureGovernment.Domain.Models.Ocsp;
using SecureGovernment.Domain.Models.Workers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SecureGovernment.Domain.Services.Workers
{
    public class OcspWorker : IAsyncWorker
    {
        public Task<WorkerResult> RunAsync(WorkerInformation workerInformation)
        {
            throw new NotImplementedException();
        }

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
