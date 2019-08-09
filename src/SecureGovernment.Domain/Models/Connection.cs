using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

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

        public virtual (X509Certificate2 Certificate, Chain Chain) LoadCertificates()
        {
            using (TcpClient tcpClient = new TcpClient(this._Url, 443))
            using (SslStream sslStream = new SslStream(tcpClient.GetStream(), true, CertificateValidationCallBack))
            {
                sslStream.AuthenticateAsClient(this._Url, null, System.Security.Authentication.SslProtocols.None, false);
            }
            return (Certificate, Chain);
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
