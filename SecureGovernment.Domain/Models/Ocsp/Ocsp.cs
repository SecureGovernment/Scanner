using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace SecureGovernment.Domain.Models.Ocsp
{
    public class Ocsp
    {
        private X509Certificate _Issuer { get; set; }
        private X509Certificate _Certificate { get; set; }
        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public Ocsp(X509Certificate issuer, X509Certificate certificate)
        {
            this._Issuer = issuer;
            this._Certificate = certificate;
        }

        public OcspReq CreateOcspReq()
        {
            CertificateID id = new CertificateID(CertificateID.HashSha1, _Issuer, _Certificate.SerialNumber);
            var gen = new OcspReqGenerator();
            gen.AddRequest(id);

            BigInteger.TryParse(Math.Floor((DateTime.UtcNow - Jan1st1970).TotalSeconds).ToString(), out BigInteger nonce);
            var ext = new Dictionary<DerObjectIdentifier, X509Extension>
            {
                { OcspObjectIdentifiers.PkixOcspNonce, new X509Extension(false, new DerOctetString(nonce.ToByteArray())) }
            };
            gen.SetRequestExtensions(new X509Extensions(ext));

            return gen.Generate();
        }

        public virtual IList<Uri> GetOcspUris()
        {
            List<Uri> ocspUris;
            try
            {
                byte[] bytes = _Certificate.GetExtensionValue(new DerObjectIdentifier(X509Extensions.AuthorityInfoAccess.Id)).GetOctets();
                Asn1InputStream aIn = new Asn1InputStream(bytes);
                var Asn1 = aIn.ReadObject();
                AuthorityInformationAccess authorityInformationAccess = AuthorityInformationAccess.GetInstance(Asn1);
                var ocspValues = authorityInformationAccess.GetAccessDescriptions().Where(x => x.AccessMethod.Id.Equals("1.3.6.1.5.5.7.48.1"));
                ocspUris = ocspValues.Select(x => new Uri(x.AccessLocation.Name.ToString())).ToList();
            }
            catch
            {
                ocspUris = new List<Uri>();
            }
            return ocspUris;
        }
    }
}
