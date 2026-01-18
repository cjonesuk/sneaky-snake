using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Axis.ECS.Events;

public interface IEventManager
{
    void AddEvent<TEvent>(ref TEvent @event);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddEvent<TEvent>(TEvent @event) => AddEvent(ref @event);

    void AddEvent<TEvent>(Id id, ref TEvent @event);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddEvent<TEvent>(Id id, TEvent @event) => AddEvent(id, ref @event);

    void ClearAllEvents();

    EventStream<TEvent> GetEventStream<TEvent>();
    EventStream<TEvent> GetEventStream<TEvent>(Id id);


    bool TryGetEventStream<TEvent>([MaybeNullWhen(false)] out EventStream<TEvent> stream);
    bool TryGetEventStream<TEvent>(Id id, [MaybeNullWhen(false)] out EventStream<TEvent> stream);

}
