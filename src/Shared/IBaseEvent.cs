using System;

namespace Event
{
    public interface IBaseEvent
    {
        Guid Id { get; set; }
    }

    public interface IInheritedEvent : IBaseEvent
    {
    }
}