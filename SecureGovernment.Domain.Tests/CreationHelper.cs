using Org.BouncyCastle.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SecureGovernment.Domain.Tests
{
    public static class CreationHelper
    {
        public static X509Certificate2 CreateCertificate()
        {
            var certificateBytes = Encoding.UTF8.GetBytes(
                @"-----BEGIN CERTIFICATE-----
                MIIDgjCCAmqgAwIBAgIJAOszhx3HIXpCMA0GCSqGSIb3DQEBCwUAMFYxCzAJBgNV
                BAYTAkFVMRMwEQYDVQQIDApTb21lLVN0YXRlMSEwHwYDVQQKDBhJbnRlcm5ldCBX
                aWRnaXRzIFB0eSBMdGQxDzANBgNVBAsMBklzc3VlcjAeFw0xOTA3MjcxOTU3NDRa
                Fw0yMDA3MjYxOTU3NDRaMFYxCzAJBgNVBAYTAkFVMRMwEQYDVQQIDApTb21lLVN0
                YXRlMSEwHwYDVQQKDBhJbnRlcm5ldCBXaWRnaXRzIFB0eSBMdGQxDzANBgNVBAsM
                Bklzc3VlcjCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAMOHN9cDtucG
                mWqS6NVCbnNF5ZrQgiZOxvdiZvPmBWgEGnacQOOgRoodOvH5aiU28hlgpmKij263
                QoDG8NDfH4H0n0bBKDDXfP1lUlONTuIGlBFIr5rSMEVJuIJaMZ1IkWBdhhXf1Td8
                rmOms94401HHKBIlPxL2ftFySN2NWiUMZsBLftu3l/nRFdNd4JBdDGL1RFh2bSjh
                TvczZ1jSOlCFkojf7FwlCmYZBtfn6bwSBswN5g+cTtZIo3XAlvGzvDV0J8gtVtM2
                cyIZ2RWww1Cl3zW73QiY9C4dAl8F7kTcyyxfMrNTd4Aco46AsoTaL7Dw1YrP0Ous
                tVjjiqhV/W8CAwEAAaNTMFEwHQYDVR0OBBYEFN4+/Esd576UMX0ICJpB2zft6Slj
                MB8GA1UdIwQYMBaAFN4+/Esd576UMX0ICJpB2zft6SljMA8GA1UdEwEB/wQFMAMB
                Af8wDQYJKoZIhvcNAQELBQADggEBAGbMQ+LAJdyZzJgQ1BLHiM8fUIICHX8YvCVq
                zx6Cs/xR3KcWib5x9yJVhMiof376JHMWaL3srkmXLLi8Be0a8JTRfVTYjnKhFzJ/
                /KY0ndCFD/CUJrHMfZwOdhRj4cEdJTlwHaNYEiR2B/aLWZD+jJPao5pyp6GiN1ld
                7SxnbKK8f6y2p+Wg+4wGFNJjJao9OkfqEi7q5ABE2ii7RwxjzA+kmABx7TWtaA8e
                sYJjKV0bPb6raUNamu+h3FciTGCHxO2KJIXrOE7e+OmtuQN0ymFTERzTo0FcougC
                gjEcCpROYwQx8QBud8u547k2XlbeNVfEGKXdJUFsubvWB9BdIhY=
                -----END CERTIFICATE-----");

            return new X509Certificate2(certificateBytes);
        }

        public static Org.BouncyCastle.X509.X509Certificate CreateBCCertificate() => DotNetUtilities.FromX509Certificate(CreateCertificate());
        public static Org.BouncyCastle.X509.X509Certificate CreateBCIssuer() => DotNetUtilities.FromX509Certificate(CreateIssuer());

        public static X509Certificate2 CreateIssuer()
        {
            var issuerBytes = Encoding.UTF8.GetBytes(
                @"-----BEGIN CERTIFICATE-----
                MIIDjDCCAnSgAwIBAgIJAKiIi7g48RPBMA0GCSqGSIb3DQEBCwUAMFsxCzAJBgNV
                BAYTAkFVMRMwEQYDVQQIDApTb21lLVN0YXRlMSEwHwYDVQQKDBhJbnRlcm5ldCBX
                aWRnaXRzIFB0eSBMdGQxFDASBgNVBAsMC0NlcnRpZmljYXRlMB4XDTE5MDcyNzE5
                NTk0NFoXDTIwMDcyNjE5NTk0NFowWzELMAkGA1UEBhMCQVUxEzARBgNVBAgMClNv
                bWUtU3RhdGUxITAfBgNVBAoMGEludGVybmV0IFdpZGdpdHMgUHR5IEx0ZDEUMBIG
                A1UECwwLQ2VydGlmaWNhdGUwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIB
                AQDVElfwwuEY+ZwHXGYdLbAPQcfTxT6MKOgSp0qxv/f57kY2ZFqu+hcK/14zIl24
                R3Gaa/CRawEH+OKq+6Ste412XQHadVKhldX528ZVDrjNFm/akErwYhXJpmdOcKxt
                hkX0CBwoRtXu4QJnKN2iWC8jg7Y10RF0GKBEv455O5omMn8TpGQSkFG07MfnywR3
                l02NB6V45+558QerN9qodmgpmqzojtKv5yaNuWhP9Rx2LwTMnsbY7bJ89/rZqAtR
                fwFAalMI3Np1w8LhOjeCaT3DuXX5VfSeQ7t/MpIDSfYs2mRuPkyWLiO5yETIBPEs
                7jv8NXgDEEP+fLByErh97gnDAgMBAAGjUzBRMB0GA1UdDgQWBBTyQei/o7DdKTum
                hod96/dG3at+sjAfBgNVHSMEGDAWgBTyQei/o7DdKTumhod96/dG3at+sjAPBgNV
                HRMBAf8EBTADAQH/MA0GCSqGSIb3DQEBCwUAA4IBAQCbOO4cmYvTnBJGfvacsjrj
                65+PJ8jeDhY1W/I8xyZ3unZ7qt3p8KDUcTD/v2ekd0OOBAFMa1GD/cmWH4ghgY0J
                z4rfSj9oyYw3RIlO0qikbzHfxHajBqxTXo0JEmRKEtBG9dXcMzC1NIY4f9VY8vle
                NZyVeQ1H0glX3e6Pu5NzcL52BxLCN+nPv01xdxD8jAWUOVdOXAdG5ko3cgX7nh2q
                leKlullbsvPGHtBgyMqKj5etJBVbbTOyyij4kNaSZ0ZwP2pW9mC9IVilOyWd84yI
                aPGIZ4YGBcAL4icbPLQXWbKObKOQUJ16KUvGCHDirbH0U28kob3HZG+L0RSpctkz
                -----END CERTIFICATE-----");

            return new X509Certificate2(issuerBytes);
        }
    }
}
