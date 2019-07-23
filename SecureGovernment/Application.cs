using McMaster.Extensions.CommandLineUtils;
using SecureGovernment.Domain.Interfaces.Facades;
using SecureGovernment.Domain.Interfaces.Services;
using SecureGovernment.Domain.Models.DnsRecords.Results;
using Serilog;
using System;

namespace SecureGovernment
{
    public interface IApplication
    {
        void Run(string[] args);
    }
    public class Application : IApplication
    {
        public ILogger Logger { get; set; }
        public IScannerFacade ScannerFacade { get; set; }

        public void Run(string[] args)
        {
            var app = new CommandLineApplication()
            {
                Name = "scscanner",
                FullName = "SecureGovernment Scan Runner",
            };

            app.HelpOption("-?|-h|--help");

            app.OnExecute(() =>
            {
                app.ShowHelp();
            });

            app.Command("scan", (command) =>
            {
                command.Description = "Starts a scan";

                command.OnExecute(() =>
                {
                    var result = ScannerFacade.ScanDns(new WorkerInformation() { Hostname = "whitehouse.gov" });
                    result.Wait();
                    var dnsResult = result.Result;
                });
            });

            try
            {
                app.Execute(args);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(ex.Message);
                Logger.Error(ex, string.Empty);
                Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
