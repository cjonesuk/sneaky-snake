namespace Axis.ECS;

public delegate void QueryAllEntitiesAction<TContext, T1>(ref TContext context, Span<Id> ids, Span<T1> col1) where T1 : unmanaged;

public delegate void QueryAllEntitiesAction<TContext, T1, T2>(ref TContext context, Span<Id> ids, Span<T1> col1, Span<T2> col2)
    where T1 : unmanaged
    where T2 : unmanaged;

public delegate void QueryEachEntityAction<TContext>(ref TContext context, ref Iter iter);

public delegate void QueryEachEntityAction<TContext, T1>(ref TContext context, ref Iter iter, ref T1 arg1)
    where T1 : unmanaged;

public delegate void QueryEachEntityAction<TContext, T1, T2>(ref TContext context, ref Iter iter, ref T1 arg1, ref T2 arg2)
    where T1 : unmanaged
    where T2 : unmanaged;

public readonly ref struct Iter(Archetype archetype, Span<Id> ids, int index)
{
    private readonly Archetype _archetype = archetype;
    private readonly Span<Id> _ids = ids;
    private readonly int _index = index;

    public ref Id Id => ref _ids[_index];

    public bool TryGetComponent<T>(out Ref<T> componentRef) where T : unmanaged
    {
        return _archetype.TryGetComponentRef<T>(_index, out componentRef);
    }
};