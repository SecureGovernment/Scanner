using SecureGovernment.Domain.Models.DnsRecords.Results;
using SecureGovernment.Domain.Models.DnsReponse.Parsed;
using System.Threading.Tasks;

namespace SecureGovernment.Domain.Interfaces.Services
{
    public interface IDnsScannerService
    {
        Task<ParsedDmarcResponse> ScanDmarcAsync(WorkerInformation workerInformation);
        Task<ParsedSpfResponse> ScanSpfAsync(WorkerInformation workerInformation);
        Task<ParsedDnssecResponse> ScanDnssecAsync(WorkerInformation workerInformation);
        Task<ParsedCaaResponse> ScanCaaAsync(WorkerInformation workerInformation);
        Task<ParsedMxResponse> ScanMxAsync(WorkerInformation workerInformation);
    }
}
