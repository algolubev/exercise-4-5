using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using System;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseNServiceBus(context =>
                {
                    var endpointConfiguration = new EndpointConfiguration("Samples.AsyncPages.WebApplication");
                    endpointConfiguration.EnableInstallers();
                    endpointConfiguration.UseSerialization<NewtonsoftSerializer>();

                    endpointConfiguration.MakeInstanceUniquelyAddressable("1");
                    endpointConfiguration.EnableCallbacks();
                    var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();

                    var connectionString = Environment.GetEnvironmentVariable("AzureServiceBus_ConnectionString");
                    if (string.IsNullOrWhiteSpace(connectionString))
                    {
                        throw new Exception("Could not read the 'AzureServiceBus_ConnectionString' environment variable. Check the sample prerequisites.");
                    }

                    transport.ConnectionString(connectionString);

                    return endpointConfiguration;
                });
    }
}
