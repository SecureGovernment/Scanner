using Org.BouncyCastle.Pkix;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Collections;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SecureGovernment.Domain.Models
{
    public class Verification
    {
        public static bool BuildCertificateChainBC(X509Certificate2 cert, List<X509Certificate2> cas)
        {
            PkixCertPathBuilder builder = new PkixCertPathBuilder();

            // Create chain for this certificate
            X509CertStoreSelector holder = new X509CertStoreSelector();
            holder.Certificate = DotNetUtilities.FromX509Certificate(cert);

            ISet caCertsSet = new HashSet();
            var caCerts = cas.Select(x => DotNetUtilities.FromX509Certificate(x)).ToList();
            foreach (var caCert in caCerts)
            {
                caCertsSet.Add(new TrustAnchor(caCert, null));
            }
            // WITHOUT THIS LINE BUILDER CANNOT BEGIN BUILDING THE CHAIN
            caCerts.Add(holder.Certificate);

            PkixBuilderParameters builderParams = new PkixBuilderParameters(caCertsSet, holder);
            builderParams.IsRevocationEnabled = false;

            X509CollectionStoreParameters intermediateStoreParameters =
                new X509CollectionStoreParameters(caCerts);

            builderParams.AddStore(X509StoreFactory.Create(
                "Certificate/Collection", intermediateStoreParameters));

            try
            {
                PkixCertPathBuilderResult result = builder.Build(builderParams);
                return true;
            } catch
            {
                return false;
            }
        }
    }
}
