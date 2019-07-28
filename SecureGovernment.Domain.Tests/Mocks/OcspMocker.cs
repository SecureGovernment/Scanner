using SecureGovernment.Domain.Models.Ocsp;
using System;
using System.Collections.Generic;

namespace SecureGovernment.Domain.Tests.Mocks
{
    public static class OcspMocker
    {
        public static Ocsp MockOcsp(Org.BouncyCastle.X509.X509Certificate certificate = null, Org.BouncyCastle.X509.X509Certificate issuer = null, IList<Uri> ocspUris = null)
        {
            var ocspMock = Mocker.CreateMock<Ocsp>(certificate, issuer);
            ocspMock.Setup(x => x.GetOcspUris()).Returns(ocspUris);

            return ocspMock.Object;
        }
    }
}
