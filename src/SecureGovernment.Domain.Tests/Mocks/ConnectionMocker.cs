using SecureGovernment.Domain.Models;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace SecureGovernment.Domain.Tests.Mocks
{
    public static class ConnectionMocker
    {
        public static Connection MockConnection(string url = "", (X509Certificate2, Chain)? loadCertificate = null, bool throwLoadCertificateException = false)
        {
            var connection = Mocker.CreateMock<Connection>(url);

            if (loadCertificate.HasValue && !throwLoadCertificateException)
                connection.Setup(x => x.LoadCertificates()).Returns(loadCertificate.Value);
            if (throwLoadCertificateException)
                connection.Setup(x => x.LoadCertificates()).Throws(new SocketException());

            return connection.Object;
        }
    }
}
