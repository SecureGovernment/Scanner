using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Security;
using SecureGovernment.Domain.Enums;
using SecureGovernment.Domain.Interfaces;
using SecureGovernment.Domain.Models;
using SecureGovernment.Domain.Models.DnsRecords.Results;
using SecureGovernment.Domain.Models.Ocsp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SecureGovernment.Domain.Workers
{
    public class OcspWorker : IAsyncWorker
    {
        private IAsyncWorker _PreviousWorker { get; }
        public OcspWorker(IAsyncWorker previousWorker)
        {
            this._PreviousWorker = previousWorker;
        }

        public async Task<List<ScanResult>> Scan(WorkerInformation workerInformation)
        {
            var previousResults = await this._PreviousWorker.Scan(workerInformation);
            if (workerInformation.Certificate == null)
                return previousResults;

            var cert = DotNetUtilities.FromX509Certificate(workerInformation.Certificate);
            var issuer = DotNetUtilities.FromX509Certificate(workerInformation.Issuer);
            var ocsp = CreateOcsp(cert, issuer);
            var uris = ocsp.GetOcspUris();
            OcspResponse response = new OcspResponse() { Status = OcspRevocationStatus.Unknown };

            foreach (var uri in uris)
            {
                try
                {
                    HttpWebRequest request = CreateOcspRequest(ocsp, uri);
                    response = await SendOcspRequest(request);
                    if (response != null)
                        break;
                }
                catch {
                    //TODO: Log error
                    if (response == null)
                        response = new OcspResponse();

                    response.Status = OcspRevocationStatus.Error; 
                }
            }

            previousResults.Add(response);

            return previousResults;
        }

        #region - Helpers -

        public virtual Ocsp CreateOcsp(Org.BouncyCastle.X509.X509Certificate cert, Org.BouncyCastle.X509.X509Certificate issuer)
        {
            return new Ocsp(issuer, cert);
        }

        public virtual async Task<OcspResponse> SendOcspRequest(HttpWebRequest request)
        {
            OcspResp resp;
            using (WebResponse response = await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            {
                resp = new OcspResp(stream);
                return ParseOcspResponse((BasicOcspResp)resp.GetResponseObject());
            }
        }

        public virtual HttpWebRequest CreateOcspRequest(Ocsp ocsp, Uri uri)
        {
            var reqArray = ocsp.CreateOcspReq().GetEncoded();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "POST";
            var requestStream = request.GetRequestStream();
            request.ContentLength = reqArray.Length;
            request.ContentType = "application/ocsp-request";
            request.Accept = "application/ocsp-response";
            requestStream.Write(reqArray, 0, reqArray.Length);
            return request;
        }

        public OcspResponse ParseOcspResponse(BasicOcspResp brep)
        {
            SingleResp singleResp = brep.Responses.Single();
            var itstatus = singleResp.GetCertStatus();
            var status = new OcspResponse()
            {
                ProducedAt = brep.ProducedAt,
                ThisUpdate = singleResp.ThisUpdate,
                NextUpdate = singleResp.NextUpdate.Value
            };

            if (itstatus == CertificateStatus.Good)
                status.Status = OcspRevocationStatus.Good;
            else if (itstatus is RevokedStatus revokedStatus)
            {
                status.Status = OcspRevocationStatus.Revoked;
                status.RevocationTime = revokedStatus.RevocationTime;
                try
                {
                    status.RevocationReason = revokedStatus.RevocationReason;
                }
                catch (InvalidOperationException)
                {
                    status.RevocationReason = -1;
                }
            }
            else
                status.Status = OcspRevocationStatus.Unknown;

            return status;
        }

        #endregion

    }
}
