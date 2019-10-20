using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SecureGovernment.Domain.Interfaces.Facades;
using SecureGovernment.Domain.Models;
using SecureGovernment.Domain.Models.DnsRecords.Results;
using SecureGovernment.Domain.Models.DnsReponse.Parsed;

namespace SecureGovernment.Controllers
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

            return new JsonResult(MapToScanViewModel(scanResults), new Newtonsoft.Json.JsonSerializerSettings() { NullValueHandling = NullValueHandling.Include, });
        }

        public ScanViewModel MapToScanViewModel(List<ScanResult> scanResults){
            var typesAlreadySeen = new List<Type>();
            var scanViewModel = new ScanViewModel();

            foreach (var scanResult in scanResults)
            {
                switch (scanResult)
                {
                    case ParsedCaaResponse parsedCaaResponse:
                        scanViewModel.HasCaa = parsedCaaResponse.HasCaaRecords;
                        scanViewModel.IssueCas = parsedCaaResponse.IssueCas;
                        scanViewModel.IssueWildcardCas = parsedCaaResponse.IssueWildCas;
                        scanViewModel.Iodef = parsedCaaResponse.Iodef;
                        scanViewModel.RawCaaRecords = parsedCaaResponse.Records;
                        break;
                    case ParsedDmarcResponse parsedDmarcResponse:
                        scanViewModel.HasDmarc = parsedDmarcResponse.HasDmarcRecords;
                        scanViewModel.RawDmarcRecords = parsedDmarcResponse.Records;
                        break;
                    case ParsedDnssecResponse parsedDnssecResponse:
                        scanViewModel.HasDnssec = parsedDnssecResponse.HasDnssec;
                        scanViewModel.RawDnssecRecords = parsedDnssecResponse.Records;
                        break;
                    case ParsedMxResponse parsedMxResponse:
                        scanViewModel.HasMx = parsedMxResponse.HasMxRecords;
                        scanViewModel.RawMxRecords = parsedMxResponse.Records;
                        break;
                    case ParsedSpfResponse parsedSpfResponse:
                        scanViewModel.HasSpf = parsedSpfResponse.HasSpfRecords;
                        scanViewModel.HasOldSpf = parsedSpfResponse.HasOldSpf;
                        scanViewModel.RawOldSpfRecords = parsedSpfResponse.OldSpfRecords;
                        scanViewModel.RawSpfRecords = parsedSpfResponse.Records;
                        break;
                    case ParsedDkimResponse parsedDkimResponse:
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            return scanViewModel;
        }
    }
}