using SecureGovernment.Domain.Models;
using System.Security.Cryptography.X509Certificates;

namespace SecureGovernment.Domain.Tests.Mocks
{
    public static class ChainMocker
    {
        public static Chain MockChain(X509Certificate2 certificate = null)
        {
            if (certificate == null)
                certificate = CreationHelper.CreateCertificate();

            var chain = new X509Chain();
            chain.Build(certificate);

            var chainMock = Mocker.CreateMock<Chain>(chain);

            return chainMock.Object;
        }
    }
}
