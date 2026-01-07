namespace AnECS;

public delegate void QueryAllEntitiesAction<T1>(Span<Id> ids, Span<T1> col1) where T1 : unmanaged;

public delegate void QueryAllEntitiesAction<T1, T2>(Span<Id> ids, Span<T1> col1, Span<T2> col2)
    where T1 : unmanaged
    where T2 : unmanaged;

public delegate void QueryEachEntityAction<T1>(ref Id id, ref T1 arg1)
    where T1 : unmanaged;

public delegate void QueryEachEntityAction<T1, T2>(ref Id id, ref T1 arg1, ref T2 arg2)
    where T1 : unmanaged
    where T2 : unmanaged;

