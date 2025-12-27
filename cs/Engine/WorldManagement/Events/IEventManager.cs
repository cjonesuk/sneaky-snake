namespace Engine.WorldManagement.Events;

public interface IEventManager
{
    void Raise<TEvent>(TEvent @event);
    List<TEvent> GetEventList<TEvent>();
    void ClearAllEvents();

    void Debug();
}
