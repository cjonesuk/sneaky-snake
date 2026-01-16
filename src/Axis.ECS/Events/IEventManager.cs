namespace Axis.ECS.Events;

public interface IEventManager
{
    void ClearAllEvents();
    void Debug<T>();
    void DebugAll();
    EventStream<TEvent> GetEventStream<TEvent>();
    void Raise<TEvent>(TEvent @event);
}
