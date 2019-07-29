using System;
using System.Security.Cryptography.X509Certificates;

namespace SecureGovernment.Domain.Models
{
    public abstract class ChainLink
    {
        public abstract ChainLink Next { get; set; }
        public abstract ChainLink Previous { get; set; }
        public abstract X509Certificate2 Certificate { get; set; }
    }

    public class EndCertificate : ChainLink
    {
        public override ChainLink Next { get; set; }
        public override ChainLink Previous { get => null; set => throw new NotSupportedException(); }
        public override X509Certificate2 Certificate { get; set; }
    }

    public class IntermediateCertificate : ChainLink
    {
        public override ChainLink Next { get; set; }
        public override ChainLink Previous { get; set; }
        public override X509Certificate2 Certificate { get; set; }
    }

    public class RootCertificate : ChainLink
    {
        public override ChainLink Next { get; set; }
        public override ChainLink Previous { get; set; }
        public override X509Certificate2 Certificate { get; set; }
    }
}
