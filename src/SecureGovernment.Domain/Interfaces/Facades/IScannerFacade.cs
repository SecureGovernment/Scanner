using SecureGovernment.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SecureGovernment.Domain.Interfaces.Facades
{
    public interface IScannerFacade
    {
        Task<List<ScanResult>> Scan(string url);
    }
}
