
namespace Axis.ECS.Events;

public interface IEventStream
{
    void Clear();
    void Debug();
}

public sealed class EventStream<TEvent> : IEventStream
{
    private const int InitialCapacity = 16;
    private TEvent[] _events;
    private int _count;

    public EventStream()
    {
        _events = new TEvent[InitialCapacity];
        _count = 0;
    }

    public void AddEvent(TEvent @event)
    {
        if (_count == _events.Length)
        {
            Array.Resize(ref _events, _events.Length * 2);
        }

        _events[_count++] = @event;
    }

    public Span<TEvent> AsSpan()
    {
        return _events.AsSpan(0, _count);
    }

    public void Clear()
    {
        _count = 0;
    }

    public void Debug()
    {
        for (int i = 0; i < _count; i++)
        {
            Console.WriteLine($"Event: {_events[i]}");
        }
    }
}


public sealed class EventManager : IEventManager
{
    private readonly Dictionary<Type, IEventStream> _streams = new();

    public void Raise<TEvent>(TEvent @event)
    {
        EventStream<TEvent> stream = GetEventStream<TEvent>();
        stream.AddEvent(@event);
    }

    public EventStream<TEvent> GetEventStream<TEvent>()
    {
        Type eventType = typeof(TEvent);

        if (_streams.TryGetValue(eventType, out var stream))
        {
            return (EventStream<TEvent>)stream;
        }

        EventStream<TEvent> newStream = new EventStream<TEvent>();
        _streams[eventType] = newStream;
        return newStream;
    }

    public void ClearAllEvents()
    {
        foreach (var stream in _streams.Values)
        {
            stream.Clear();
        }
    }

    public void Debug<T>()
    {
        Type eventType = typeof(T);
        LogEventsForType(eventType);
    }

    public void DebugAll()
    {
        foreach (var stream in _streams.Values)
        {
            stream.Debug();
        }
    }

    private void LogEventsForType(Type eventType)
    {
        if (_streams.TryGetValue(eventType, out var stream))
        {
            stream.Debug();
        }
    }
}