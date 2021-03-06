﻿using DnsClient;
using DnsClient.Protocol;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Security;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SecureGovernment.Domain.Tests
{
    public static class CreationHelpers
    {
        public static X509Certificate2 CreateCertificate()
        {
            var certificateBytes = Encoding.UTF8.GetBytes(
                @"-----BEGIN CERTIFICATE-----
MIIExDCCAqygAwIBAgICEAAwDQYJKoZIhvcNAQELBQAwMzELMAkGA1UEBhMCVVMx
JDAiBgNVBAMMG0ludGVybWVkaWF0ZSBDZXJ0aWZpY2F0ZSAjMTAeFw0xOTA3Mjkw
NDEzNThaFw0yMDA4MDcwNDEzNThaMCcxCzAJBgNVBAYTAlVTMRgwFgYDVQQDDA9F
bmQgQ2VydGlmaWNhdGUwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQDA
7KAqO9NofAMqF0ZR5h63/vzeNkV8fgLJKY3r3EWiGlRq/AWi/Bz3CjsShZWJue66
sWmKHV3c5zIIeljMT+SGJt7uiXeoLQH5VyDfBTv9GG6ndJt1eDxx5UkHuT83avo2
W1WSLb4milc5fTOpNjsSlwiKmMnAlIg48ErhPgH2IDyiFdtiZ+47dxWgTxZnx48k
XB0eZ4dyXuHCoZoaXDi452PFMe9Py8+HHi5D9zf6aV2skwW4uisT6JuLfGOYNT3j
HVivTMIkvlgxS3nRAj3UgNst7/pLFSQ6HG9Df+9IIWFdvP2RZB4IRK2P0knApOEv
HZhYBsnPLmiwUDwqTS8DAgMBAAGjge0wgeowCQYDVR0TBAIwADARBglghkgBhvhC
AQEEBAMCBkAwMwYJYIZIAYb4QgENBCYWJE9wZW5TU0wgR2VuZXJhdGVkIFNlcnZl
ciBDZXJ0aWZpY2F0ZTAdBgNVHQ4EFgQU1lROnCM0gvg3Uw0lajVpYXPqpGswUQYD
VR0jBEowSIAUimzYvjXqbe92NUCURigXkWIjmKKhLKQqMCgxCzAJBgNVBAYTAlVT
MRkwFwYDVQQDDBBSb290IENlcnRpZmljYXRlggIQADAOBgNVHQ8BAf8EBAMCBaAw
EwYDVR0lBAwwCgYIKwYBBQUHAwEwDQYJKoZIhvcNAQELBQADggIBABHF2mHk3y5d
nD3BPY4sLc5vGIuORr7jwOhmebUStnv3UscSlrUtGS/DQKDlNYuac54giyOXL/vG
fQV3gjMhhjaAOY976c3zUY2nXyALa9IjlfkbupRthd1UUgMM/JnkV8j6bqIJGwi7
wXNycEbX4dH9o3tG6rJ3hCYYXxsZoBrOJD8Lngk+5+OfqR4V8Fio7F/G1BIjlS7B
kH1+3Hb6LeejoVIIlPpSsDhczh5CjdIMwReCdTqpOuWeOqK8bYOeMYUT2A3ie8Tl
ioujhpG84BbbMTOyoJVQXbCKmlmTCeRsfLXGaTrETjUZ523C3ngGN0PftHk1xB/v
gKt9bFeek8I00ATfW576K3waRWXSDNedd7iZYs+lCRe3US8j/WdjxTFU1SM4dhGf
aOFWFDDB6zBnk+VM4pgPnYXm64ch1dkd8fl7r5Lyk2dhLjj+Df7DQqyGgVwD406x
lVV1qkrZgyJK7UjplAAwSeahY+ZT2kASwOfmK/4rpJ3sDsqVO9u2a8iS7UsFBE1s
bZ7+ur9mWCAemgK1DOTIvxbzYSEoDH3OFI5lrp+CgQlG+T0RVhonOzdCOvLWHOZH
uJAwRydFkLDvtlpYwG0o1TxdMt+6cuxUE9axoJXaTFuhkn6FsuROsYsvOIspxDtV
lxW9ksx+Dkn0EqoSiiU309pWkRpftAlA
-----END CERTIFICATE-----");

            return new X509Certificate2(certificateBytes);
        }

        public static Org.BouncyCastle.X509.X509Certificate CreateBCCertificate() => DotNetUtilities.FromX509Certificate(CreateCertificate());
        public static Org.BouncyCastle.X509.X509Certificate CreateBCIntermediate() => DotNetUtilities.FromX509Certificate(CreateIntermediate());

        public static X509Certificate2 CreateIntermediate()
        {
            var issuerBytes = Encoding.ASCII.GetBytes(
                @"-----BEGIN CERTIFICATE-----
MIIFPTCCAyWgAwIBAgICEAAwDQYJKoZIhvcNAQELBQAwKDELMAkGA1UEBhMCVVMx
GTAXBgNVBAMMEFJvb3QgQ2VydGlmaWNhdGUwHhcNMTkwNzI5MDQxMDU3WhcNMjkw
NzI2MDQxMDU3WjAzMQswCQYDVQQGEwJVUzEkMCIGA1UEAwwbSW50ZXJtZWRpYXRl
IENlcnRpZmljYXRlICMxMIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEA
9BZklqhTgjgQ2J2OvDrI6C5XSsIaVbvHUaU629K1i2fi1po9/Z6eZulbk00KcQsF
nC7vn21AcuE4soMUu3ZWBu/J8v03X7A92PQuZjQCwAQtorP3hJkemmzZDBGuiw6T
W5DYB3WQCNgtAyIE4P+S/2i17Sd5Qewm+qiwWSgMyoQAh7LZAisB1zfCCkyx80R6
lRRY7r6A0cAUBAoqbUQMRR/+t6ba2U0FpgOEhfY/HxBaBblqdkCT0OCxxYoyvEQ4
CcwZGrk7qM7D0qvP2mm4qB+UI/4Gqm0uRXPKOUqcLM+jdHP5RLvPa9x8LgN3JcZO
5ARAIvvAUXgPT9pCWyRtZEdr3lmvB9dKNOawb0RwPKnWnVHv7dbOdi8IaqOmcmMS
BTR0+O0CCTHiRtO8F74q0ZhhwQ/86T7vNNdZ8yuHKZ7QPlULBhxmmCWy3kpkHaON
J6QaqeuFZe1RIgbR37l3ayTd2Ie/iI6MlHHdLxmifRTegw2V9kz4vCUulSeXzpjF
M8q3nnWpBQPP1YNboWOlYAqE2mJNQTMBkAkKMkcSn/2voIYKF86NeQnhKu81cORh
5NerURXrgrew4Ww7354IGYlU1J+SJ9AjQUQ70F8JAbj9Ibyv0idZnR08LiBp3Orp
p3NQzJ2RYYTZ5ODDSojXNmQ5JRmjJiWk9bgqcYnqIGcCAwEAAaNmMGQwHQYDVR0O
BBYEFIps2L416m3vdjVAlEYoF5FiI5iiMB8GA1UdIwQYMBaAFKB8EJUQ2Li22xM8
8tjIu4lnJVskMBIGA1UdEwEB/wQIMAYBAf8CAQAwDgYDVR0PAQH/BAQDAgGGMA0G
CSqGSIb3DQEBCwUAA4ICAQC5GdHjLgvKdMkPvKA/z9glatByR7PwDP5VTeZksjU9
mG7UWUwwuLVJ4ahyWyoqo9B/g9Yv07SSGuHBrJ8oXfsFGqWIVvr4eECOU1aMyXCp
Fo7HY1K32UzxrBdKsZ8aUlcheDv4kk6XDNqeRPxNq2f4bSyMom00Klz4Gt6vXu1j
N5yxDXFJ0IU5vjW2dY7cx4BbfSyvSMpsa4u2rHE8dyQZ7bDpzPKhHLa9yWzVM0g1
a3V+QwoMlng1pU+1roh49y5h2vk7kevF/RbFSqbQuQIsayiRYN94WgrNw0KB+iK4
i0D0ciD4C0X7TNIWlF3nUv3JQery85DeMC4kZ+Zz2GRK9082fv+H05QYDTQYN1H3
m2uRJNXUfsJ+lgHSjpSoYbFKlV+RGq2O54mOLGU2Byb7U+I8YwqNltjQES5tLGs6
001o21WzfAnWH8RvdwLwaIqwrt8fTT1aVBuzHHLlGjRcfF9lwf74DLb1YfOoNWQ6
b4BYGlWmWGnTZ6aWno3etAi3RWRUACwcyEXb78PX4f5yfL0YNeH9WLnOi6wdpn4z
xxF8HD8/ZHNEsUC6S+IjXsBdD/tIfN3ztsqMma7MtOkVixdKBuU33m1TRwMySqmC
iC0RCXjD6t+Qynug/fAXTW2S92ZTB7FfDDbjfY1pQEX631fWSHkweHwIzwA3TaPA
HA==
-----END CERTIFICATE-----");

            return new X509Certificate2(issuerBytes);
        }

        public static X509Certificate2 CreateCaCertificate()
        {
            var caBytes = Encoding.ASCII.GetBytes(@"-----BEGIN CERTIFICATE-----
MIIFNjCCAx6gAwIBAgIJANOdM07nILuRMA0GCSqGSIb3DQEBCwUAMCgxCzAJBgNV
BAYTAlVTMRkwFwYDVQQDDBBSb290IENlcnRpZmljYXRlMB4XDTE5MDcyOTA0MDMw
MVoXDTM5MDcyNDA0MDMwMVowKDELMAkGA1UEBhMCVVMxGTAXBgNVBAMMEFJvb3Qg
Q2VydGlmaWNhdGUwggIiMA0GCSqGSIb3DQEBAQUAA4ICDwAwggIKAoICAQDtjGvd
NSWCKCmnjhp8FRJJgqFRWFZSdAiPHV9ViOjppa2+7wDuZGFrtafH0t192vRZh33w
gGKFfVQ+IScxw8Dc31wt5b+zeGtb+qJ1TRdRx+NZLcfg4DYcKcuO3sVtXAOnJb8J
dPuZ0do9Z2AOzhx68QjoJqVsoRuNlyzDXZeD+IOLZZluvy8z2SOc2xTLNkCVL3yE
+f+Hcwfo1prl6M6olRDO033mFvZGO6VDc+GCLkAzv+obxilZBEPzGMZj7MP1akfQ
fCAWe7dPVN6luGxhL7TV+2jnnMq1p4UFX+xZLtiMs28j5ILuisSDVn4wNYG6lfnk
aSXGHSf/LDdoGTF/SiU6/E9SG0yVqDzfqxUapdQeTLRBgLxiHKfQUldkIQrc7cUS
G1sr2rwlajwp+OILuChypctd1r+QnNtJSAblkXHd1fOEvGdbehF1K4lMFp1R9Ftk
cfqaCTa/OuUjI5uCGvoYRMCUe58Ln5SIU5+og2riwVx+twBGkh5SHfJFbqYg5EFk
c7601EaznuyT3lRIVJDcF5A43DOoScAUhhwGl3FRFQdkr8hmRkdXxub6uXbm+tFl
keejJdGsn7o0BDTj0MDAADXb6rgbBNsBEZ9AbJOTi12KiNd2sNchdenlXpGwfYch
6MgbaxaTUJqkqvbd7MbK32NOCGq/hx+ywNA5YwIDAQABo2MwYTAdBgNVHQ4EFgQU
oHwQlRDYuLbbEzzy2Mi7iWclWyQwHwYDVR0jBBgwFoAUoHwQlRDYuLbbEzzy2Mi7
iWclWyQwDwYDVR0TAQH/BAUwAwEB/zAOBgNVHQ8BAf8EBAMCAYYwDQYJKoZIhvcN
AQELBQADggIBACmEXU0x7yTorO5I+mNaGGNPegnGH4OPCIsAQH64V+Pu66FqZiE4
bt186+MeudGR+uCGtQoLXSUn7I+JT4GqTrQN+yMoYv5sLC3KflzuPFhgclK7o3Ov
gFD+7sZCPHAwpulxOBuSS3UBATEzkxLzUALY2eVdQHiFjqK4z9YhmZVx9VWtlc7J
sol8QF6CCBYpWsRQ/EhSmC51WhGFXk7iFPNajEM4CW8PzzyvDXV5548dtmhoPiuX
LrhGIc+ZaHsJ8pq3EgaQI0AKGCCEPMPh1kntR7Av2nMu/Yl69oE16abzSJVP+Fkc
iaiwkrHCEG4yYU4f5a9S6mm7H29kkgs54ITZv9Bm6py8BW/1I3HQD2VgfEXyF4DO
XrZnIObLZXWGuKZ0Q2hcl1jKwOlqijFiujqd39IaGwIsqb9vjw3//tiU/Wq7qrRC
OduWAXlUW6WlETlZHXwxahWAqka9yWGRjFrHmTdrD7QA12K2SdfA1SyZTdvJEh+F
dEi87K5qrj2tfryy7jiMeUidB7nQ69PYbeIixLQyaC4nA/EdwtFuRNJo4Q9FN7yJ
T1uRvvIF8CRLXXRcOeKnyAT0etA8NYP6d6q//uOOU6asL1/o4mHi6XqWdZs95kLX
EtFvGXGgbTr0FzeOLx5KPvqFKogshJSuxtN+qZDO3WQXlbbXkircmdpQ
-----END CERTIFICATE-----");

            return new X509Certificate2(caBytes);
        }

        public static DnsString CreateDnsString(string hostname) => DnsString.FromResponseQueryString(hostname);

        public static ResourceRecordInfo CreateResourceRecordInfo(DnsString hostname, ResourceRecordType resourceRecordType) 
            => new ResourceRecordInfo(hostname, resourceRecordType, QueryClass.IN, 0, 0);

        public static ResourceRecordInfo CreateResourceRecordInfo(string hostname, ResourceRecordType resourceRecordType)
            => new ResourceRecordInfo(hostname, resourceRecordType, QueryClass.IN, 0, 0);

        public static BasicOcspResp GetValidOcspResp()
        {
            var bytes = File.ReadAllBytes(@"Data\ValidOCSP.txt");
            var ocspResp = new OcspResp(bytes);
            return (BasicOcspResp)ocspResp.GetResponseObject();
        }

        public static BasicOcspResp GetInvalidOcspResp()
        {
            var bytes = File.ReadAllBytes(@"Data\InvalidOCSP.txt");
            var ocspResp = new OcspResp(bytes);
            return (BasicOcspResp)ocspResp.GetResponseObject();
        }
    }
}
