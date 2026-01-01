using System.Collections;

namespace Engine.WorldManagement.Events;

internal sealed class EventManager : IEventManager
{
    private readonly Dictionary<Type, object> _eventQueues = new();

    public void Raise<TEvent>(TEvent @event)
    {
        List<TEvent> queue = GetEventList<TEvent>();
        queue.Add(@event);
    }

    public List<TEvent> GetEventList<TEvent>()
    {
        Type eventType = typeof(TEvent);
        if (!_eventQueues.TryGetValue(eventType, out var queueObj))
        {
            queueObj = new List<TEvent>();
            _eventQueues[eventType] = queueObj;
        }

        return (List<TEvent>)queueObj;
    }

    public void ClearAllEvents()
    {
        foreach (var queue in _eventQueues.Values)
        {
            ((IList)queue).Clear();
        }
    }

    public void Debug<T>()
    {
        Type eventType = typeof(T);
        LogEventsForType(eventType);
    }

    public void DebugAll()
    {
        foreach (var keys in _eventQueues.Keys)
        {
            LogEventsForType(keys);
        }
    }

    private void LogEventsForType(Type eventType)
    {
        if (_eventQueues.TryGetValue(eventType, out var queueObj))
        {
            IList queue = (IList)queueObj;

            if (queue.Count == 0)
            {
                return;
            }

            foreach (var evt in queue)
            {
                Console.WriteLine($"Event: {evt}");
            }
        }
    }
}