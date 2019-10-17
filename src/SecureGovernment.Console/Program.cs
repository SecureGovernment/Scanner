using Autofac;
using DnsClient;
using Serilog;
using System;
using System.Net;
using System.Reflection;

namespace SecureGovernment
{
    class Program
    {
        static void Main(string[] args)
        {
            var logConfiguration = new LoggerConfiguration();
            var logger = logConfiguration.CreateLogger();
            AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();

            var builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly(), Assembly.Load("SecureGovernment.Domain"))
                   .AsImplementedInterfaces().PropertiesAutowired();
            builder.RegisterInstance(new LookupClient(IPAddress.Parse("8.8.8.8"), 53)).As<ILookupClient>();
            builder.RegisterInstance(logger).As<ILogger>();

            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var app = scope.Resolve<IApplication>();
                app.Run(args);
            }
        }
    }
}
