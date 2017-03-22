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
        Console.Title = "Samples.PubSub.Publisher";
        var endpointConfiguration = new EndpointConfiguration("Samples.PubSub.Publisher");
        endpointConfiguration.UseSerialization<JsonSerializer>();
        endpointConfiguration.UsePersistence<InMemoryPersistence>();
        endpointConfiguration.UseTransport<MsmqTransport>();

        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.Conventions().DefiningEventsAs(t => t.Namespace != null && t.Namespace.StartsWith("Event"));


        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
        try
        {
            await Start(endpointInstance)
                .ConfigureAwait(false);
        }
        finally
        {
            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }

    static async Task Start(IEndpointInstance endpointInstance)
    {
        Console.WriteLine("Press '1' to publish the IBaseEvent event");
        Console.WriteLine("Press '2' to publish the IInheritedEvent event");
        Console.WriteLine("Press any other key to exit");

        #region PublishLoop

        while (true)
        {
            var key = Console.ReadKey();
            Console.WriteLine();

            var id = Guid.NewGuid();
            if (key.Key == ConsoleKey.D1)
            {
                await endpointInstance.Publish<IBaseEvent>(received => received.Id = id)
                    .ConfigureAwait(false);
                Console.WriteLine($"Published IBaseEvent with Id {id}.");
            }
            else if (key.Key == ConsoleKey.D2)
            {
                await endpointInstance.Publish<IInheritedEvent>(received => received.Id = id)
                    .ConfigureAwait(false);
                Console.WriteLine($"Published IInheritedEvent with Id {id}.");
            }
            else
            {
                return;
            }
        }

        #endregion
    }
}