using Moq;
using SecureGovernment.Domain.Models;
using SecureGovernment.Domain.Models.DnsRecords.Results;

namespace SecureGovernment.Domain.Tests.Mocks
{
    public static class ConnectionMocker
    {
        public static Connection MockConnection(string url = "", WorkerInformation workerInformation = null)
        {
            var connection = Mocker.CreateMock<Connection>(url);

            if (workerInformation != null)
                connection.Setup(x => x.Connect()).ReturnsAsync(workerInformation);

            return connection.Object;
        }
    }
}
