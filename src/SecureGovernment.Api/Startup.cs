using Autofac;
using DnsClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SecureGovernment.Api.Controllers;
using SecureGovernment.Domain.Infastructure.Settings;
using SecureGovernment.Domain.Interfaces.Infastructure;
using System.Net;
using System.Reflection;

namespace SecureGovernment.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddControllers()
                .AddNewtonsoftJson().AddControllersAsServices();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {

            var settings = new Settings();
            this.Configuration.Bind(settings);

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly(), Assembly.Load("SecureGovernment.Domain"))
                   .AsImplementedInterfaces().PropertiesAutowired();
            builder.RegisterType<ScanController>().PropertiesAutowired();
            builder.RegisterInstance(settings).As<ISettings>().SingleInstance();
            builder.RegisterInstance(new LookupClient(IPAddress.Parse("8.8.8.8"), 53)).As<ILookupClient>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
