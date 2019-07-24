using Org.BouncyCastle.Ocsp;
using SecureGovernment.Domain.Interfaces;
using SecureGovernment.Domain.Models;
using SecureGovernment.Domain.Models.DnsRecords.Results;
using SecureGovernment.Domain.Models.Ocsp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SecureGovernment.Domain.Workers
{
    public class OcspWorker : IAsyncWorker
    {
        public async Task<List<ScanResult>> Scan(WorkerInformation workerInformation)
        {
            var ocsp = new Ocsp(workerInformation.Issuer, workerInformation.Certificate);
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
                    return null;
                    //return (BasicOcspResp)resp.GetResponseObject();
                }
                catch { }
            }
            return null;
        }
    }
}
