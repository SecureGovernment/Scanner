using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SecureGovernment.Domain.Interfaces.Facades;
using SecureGovernment.Domain.Models.DnsRecords.Results;
using System.Threading.Tasks;

namespace SecureGovernment.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScanController : ControllerBase
    {
        public IScannerFacade ScannerFacade { get; set; }

        [HttpGet]
        public async Task<JsonResult> Get(string domain)
        {
            var workerInformation = new WorkerInformation() { Hostname = domain };
            var scanResults = await ScannerFacade.ScanDns(workerInformation);

            return new JsonResult(scanResults, new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = NullValueHandling.Include });
        }
    }
}
