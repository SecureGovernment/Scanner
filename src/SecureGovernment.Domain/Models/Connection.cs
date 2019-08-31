using RestSharp;
using SecureGovernment.Domain.Models.DnsRecords.Results;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SecureGovernment.Domain.Models
{
    public class Connection
    {
        private string _Url { get; set; }
        public Connection(string url)
        {
            this._Url = url;
        }
        private X509Certificate2 Certificate { get; set; }
        private Chain Chain { get; set; }

        public virtual EndPointInformation LoadCertificates(Uri uri)
        {
            IPAddress ip = null;
            using (TcpClient tcpClient = new TcpClient(uri.Host, uri.Port)) {
                ip = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address;
                if(uri.Port == 443)
                {
                    using (SslStream sslStream = new SslStream(tcpClient.GetStream(), true, CertificateValidationCallBack))
                    {
                        sslStream.AuthenticateAsClient(uri.Host, null, System.Security.Authentication.SslProtocols.None, false);
                    }
                }
            }

            return new EndPointInformation() { IPAddress = ip, Certificate = this.Certificate, Chain = this.Chain };
        }

        public async Task<WorkerInformation> Connect()
        {
            var uris = new List<Uri>() { new Uri($"https://{this._Url}"), new Uri($"https://www.{this._Url}"), new Uri($"http://{this._Url}"), new Uri($"http://www.{this._Url}") };
            Uri resolvedUri = null;
            foreach (var uri in uris)
            {
                resolvedUri = await ConnectToUri(uri);
                if (resolvedUri != null) break;
            }

            var workerInformation = new WorkerInformation() {
                Hostname = this._Url,
                ResolvedHostname = resolvedUri,
            };

            if (resolvedUri == null) return workerInformation;

            var certificateInformation = LoadCertificates(resolvedUri);
            workerInformation.IPAddress = certificateInformation.IPAddress;
            workerInformation.Certificate = certificateInformation.Certificate;
            workerInformation.Chain = certificateInformation.Chain;

            return workerInformation;
        }

        public async Task<Uri> ConnectToUri(Uri uri)
        {
            var restClient = new RestClient() { BaseUrl = uri, Timeout = 15000 };
            var request = new RestRequest(uri);
            var response = await restClient.ExecuteGetTaskAsync<RestRequest>(request);
            return (int)response.StatusCode >= 200 && (int)response.StatusCode < 300 ? response.ResponseUri : null;
        }

        private bool CertificateValidationCallBack(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            var newChain = new X509Chain(false);
            var newCert = new X509Certificate2(certificate);
            newChain.Build(newCert);

            Chain = new Chain(newChain);
            Certificate = newCert;

            return true;
        }
    }
}
