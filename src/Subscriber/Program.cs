using System;
using System.Threading.Tasks;
using Event;
using NServiceBus;

static class Program
{
    static void Main()
    {
        AsyncMain().GetAwaiter().GetResult();
    }

    static async Task AsyncMain()
    {
        Console.Title = "Samples.PubSub.Subscriber";
        var endpointConfiguration = new EndpointConfiguration("Samples.PubSub.Subscriber");
        endpointConfiguration.UseSerialization<JsonSerializer>();
        endpointConfiguration.UsePersistence<InMemoryPersistence>();
        endpointConfiguration.Conventions().DefiningEventsAs(t => t.Namespace != null && t.Namespace.StartsWith("Event"));
        var transport = endpointConfiguration.UseTransport<MsmqTransport>();
        var routing = transport.Routing();
        routing.RegisterPublisher(
            eventType: typeof(IBaseEvent), 
            publisherEndpoint: "Samples.PubSub.Publisher");

        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.EnableInstallers();

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
        try
        {
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
        finally
        {
            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}