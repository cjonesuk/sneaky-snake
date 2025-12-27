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

    public void Debug()
    {
        foreach (var kvp in _eventQueues)
        {
            Type eventType = kvp.Key;
            IList queue = (IList)kvp.Value;

            if (queue.Count == 0)
            {
                continue;
            }

            Console.WriteLine($"Event Type: {eventType.Name}, Count: {queue.Count}");
            foreach (var evt in queue)
            {
                Console.WriteLine($"  Event: {evt}");
            }
        }
    }
}