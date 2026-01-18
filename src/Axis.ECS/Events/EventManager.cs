using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Axis.ECS.Events;


public sealed class EventManager : IEventManager
{
    private const int InitialStreamCapacity = 8;
    private readonly Dictionary<Type, IEventStream> _streams;
    private readonly Dictionary<Type, Dictionary<Id, IEventStream>> _entityStreams;

    public EventManager()
    {
        _streams = new Dictionary<Type, IEventStream>();
        _entityStreams = new Dictionary<Type, Dictionary<Id, IEventStream>>();
    }

    public void AddEvent<TEvent>(ref TEvent @event)
    {
        var stream = GetEventStream<TEvent>();
        stream.AddEvent(@event);
    }

    public void AddEvent<TEvent>(Id id, ref TEvent @event)
    {
        var stream = GetEventStream<TEvent>(id);
        stream.AddEvent(@event);
    }

    public bool TryGetEventStream<TEvent>([MaybeNullWhen(false)] out EventStream<TEvent> stream)
    {
        Type eventType = typeof(TEvent);

        if (_streams.TryGetValue(eventType, out var existingStream))
        {
            stream = (EventStream<TEvent>)existingStream;
            return true;
        }

        stream = null!;
        return false;
    }

    public bool TryGetEventStream<TEvent>(Id id, [MaybeNullWhen(false)] out EventStream<TEvent> stream)
    {
        Type eventType = typeof(TEvent);

        if (_entityStreams.TryGetValue(eventType, out var entityStreams))
        {
            if (entityStreams.TryGetValue(id, out var existingStream))
            {
                stream = (EventStream<TEvent>)existingStream;
                return true;
            }
        }

        stream = null!;
        return false;
    }

    public EventStream<TEvent> GetEventStream<TEvent>(Id id)
    {
        Type eventType = typeof(TEvent);

        // todo: investigate CollectionsMarshal.GetValueRefOrAddDefault
        if (!_entityStreams.TryGetValue(eventType, out var entityStreams))
        {
            entityStreams = new Dictionary<Id, IEventStream>();
            _entityStreams[eventType] = entityStreams;
        }

        if (entityStreams.TryGetValue(id, out var stream))
        {
            return (EventStream<TEvent>)stream;
        }

        EventStream<TEvent> newStream = new EventStream<TEvent>(InitialStreamCapacity);
        entityStreams[id] = newStream;
        return newStream;
    }

    public EventStream<TEvent> GetEventStream<TEvent>()
    {
        Type eventType = typeof(TEvent);

        if (_streams.TryGetValue(eventType, out var stream))
        {
            return (EventStream<TEvent>)stream;
        }

        EventStream<TEvent> newStream = new EventStream<TEvent>(InitialStreamCapacity);
        _streams[eventType] = newStream;
        return newStream;
    }

    public void ClearAllEvents()
    {
        // todo: investigate performance improvements
        // todo: investigate cleaning up unused streams (e.g. for entities that have been deleted)

        if (_streams.Count != 0)
        {
            foreach (var stream in _streams.Values)
            {
                stream.Clear();
            }
        }

        if (_entityStreams.Count == 0)
        {
            return;
        }

        foreach (var entityStreams in _entityStreams.Values)
        {
            if (entityStreams.Count == 0)
            {
                continue;
            }

            foreach (var stream in entityStreams.Values)
            {
                stream.Clear();
            }
        }
    }
}