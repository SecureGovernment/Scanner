using Autofac;
using Autofac.Extensions.DependencyInjection;
using DnsClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SecureGovernment.Domain.Infastructure.Settings;
using SecureGovernment.Domain.Interfaces.Infastructure;
using Serilog;
using System;
using System.IO;
using System.Net;
using System.Reflection;

namespace SecureGovernment
{
    class Program
    {
        public static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();

            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true);

            var configuration = configBuilder.Build();
            serviceCollection.Configure<Settings>(configuration);

            if(!IPAddress.TryParse(configuration["dnsip"], out var dnsIp))
                throw new InvalidDataException("Please enter a valid IP address for a DNS resolver!");

            var logConfiguration = new LoggerConfiguration();
            var logger = logConfiguration.CreateLogger();
            AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();

            var builder = new ContainerBuilder();

            builder.Populate(serviceCollection);
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly(), Assembly.Load("SecureGovernment.Domain"))
                   .AsImplementedInterfaces().PropertiesAutowired();
            builder.RegisterInstance(new LookupClient(dnsIp, 53)).As<ILookupClient>();
            builder.RegisterInstance(logger).As<ILogger>();
            builder.Register(x => x.Resolve<IOptionsSnapshot<Settings>>().Value).As<ISettings>().InstancePerLifetimeScope();


            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var app = scope.Resolve<IApplication>();
                app.Run(args);
            }
        }
    }
}
