using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SecureGovernment.Api.ViewModel;
using SecureGovernment.Domain.Interfaces.Facades;
using SecureGovernment.Domain.Models.DnsRecords.Results;
using System;
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

            var scanResultsViewModel = new ScanResultViewModel() { Date = DateTime.Now, Domain = domain, Results = scanResults };

            return new JsonResult(scanResultsViewModel, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Include });
        }
    }
}
