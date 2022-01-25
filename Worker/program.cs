using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using Repositories;
using Services;
using Worker;

#region configuration-with-function-host-builder
[assembly:NServiceBusTriggerFunction("WorkerQueue")]

/*
 *Endpoint name cannot be determined automatically. Use one of the following options to specify endpoint name: 
- Use `NServiceBusTriggerFunctionAttribute(endpointName)` to generate a trigger
- Use `functionsHostBuilder.UseNServiceBus(endpointName, configuration)` 
- Add a configuration or environment variable with the key ENDPOINT_NAME 
 * 
*/
public class Program
{
    public static void Main()
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .UseNServiceBus(hostBuilderContext => {
                hostBuilderContext.AdvancedConfiguration.EnableOutbox();
                hostBuilderContext.AdvancedConfiguration.Pipeline.Register(new OrderIdAsPartitionKeyBehaviorForNumber.Registration());
                var persistence = hostBuilderContext.AdvancedConfiguration.UsePersistence<CosmosPersistence>();
                var connection =
                    @"AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
                persistence.DatabaseName("test");
                persistence.CosmosClient(new CosmosClient(connection));
                persistence.DefaultContainer("Numbers", "/OrderId");
            })
            .ConfigureServices((hostContext, services) => {
                services.AddTransient<NumberProcessor>();
                services.AddScoped<CosmosDbNumbersRespository>();
            })

            .Build();

        host.Run();
    }
}
#endregion