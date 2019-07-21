using McMaster.Extensions.CommandLineUtils;
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
