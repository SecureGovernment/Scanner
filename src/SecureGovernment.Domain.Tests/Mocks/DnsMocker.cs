using DnsClient;
using DnsClient.Protocol;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecureGovernment.Domain.Tests.Mocks
{
    public static class DnsMocker
    {
        public static Mock<IDnsQueryResponse> MockDnsQueryResponse(List<DnsResourceRecord> records = null)
        {
            if (records == null)
                records = new List<DnsResourceRecord>();

            var dnsResponse = new Mock<IDnsQueryResponse>();
            dnsResponse.Setup(x => x.Answers).Returns(records);

            return dnsResponse;
        }

        public static Mock<ILookupClient> MockLookupClient(List<(string hostname, QueryType queryType, IDnsQueryResponse response)> responses)
        {
            var lookupClientMock = Utils.CreateMock<ILookupClient>();
            foreach (var (hostname, queryType, response) in responses)
                lookupClientMock.Setup(x => x.QueryAsync(hostname, queryType, QueryClass.IN, null, default)).Returns(Task.FromResult(response));

            return lookupClientMock;
        }
    }
}
