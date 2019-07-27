using SecureGovernment.Domain.Models.DnsRecords.Results;
using System.Security.Cryptography.X509Certificates;

namespace SecureGovernment.Domain.Tests.Mocks
{
    public static class WorkerInformationMocker
    {
        public static WorkerInformation MockWorkerInformation(string hostname = "", X509Certificate2 certificate = null, X509Certificate2 issuer = null)
        {
            var workerInformationMock = Mocker.CreateMock<WorkerInformation>();
            workerInformationMock.Setup(x => x.Issuer).Returns(issuer);

            var workerInformation = workerInformationMock.Object;
            workerInformation.Certificate = certificate;
            workerInformation.Hostname = hostname;

            return workerInformation;
        }
    }
}
