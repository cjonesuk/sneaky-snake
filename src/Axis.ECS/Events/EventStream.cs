namespace Axis.ECS.Events;

public sealed class EventStream<TEvent> : IEventStream
{
    private TEvent[] _events;
    private int _count;

    public EventStream(int initialCapacity)
    {
        _events = new TEvent[initialCapacity];
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
