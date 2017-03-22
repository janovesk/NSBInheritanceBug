using System.Threading.Tasks;
using Event;
using NServiceBus;
using NServiceBus.Logging;

public class BaseEventHandler :
    IHandleMessages<IBaseEvent>
{
    static ILog log = LogManager.GetLogger<BaseEventHandler>();

    public Task Handle(IBaseEvent message, IMessageHandlerContext context)
    {
        log.Info($"Subscriber has received IBaseEvent event with Id {message.Id}.");
        return Task.CompletedTask;
    }
}