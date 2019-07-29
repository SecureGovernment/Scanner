using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace SecureGovernment.Domain.Models
{
    public abstract class ChainLink
    {
        public abstract List<ChainLink> Next { get; set; }
        public abstract List<ChainLink> Previous { get; set; }
        public abstract X509Certificate2 Certificate { get; set; }

        public void AddPrevious(ChainLink chainLink)
        {
            this.Previous.Add(chainLink);
            chainLink.Next.Add(this);
        }
    }

    public class EndCertificate : ChainLink
    {
        public EndCertificate()
        {
            this.Next = new List<ChainLink>();
        }

        public override List<ChainLink> Next { get; set; }
        public override List<ChainLink> Previous { get => null; set => throw new NotSupportedException(); }
        public override X509Certificate2 Certificate { get; set; }
    }

    public class IntermediateCertificate : ChainLink
    {
        public IntermediateCertificate()
        {
            this.Next = new List<ChainLink>();
            this.Previous = new List<ChainLink>();
        }

        public override List<ChainLink> Next { get; set; }
        public override List<ChainLink> Previous { get; set; }
        public override X509Certificate2 Certificate { get; set; }
    }

    public class RootCertificate : ChainLink
    {
        public RootCertificate()
        {
            this.Next = new List<ChainLink>();
            this.Previous = new List<ChainLink>();
        }

        public override List<ChainLink> Next { get; set; }
        public override List<ChainLink> Previous { get; set; }
        public override X509Certificate2 Certificate { get; set; }
    }
}
