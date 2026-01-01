using System.Runtime.InteropServices;

namespace Engine.WorldManagement.Entities;

public readonly struct EntityQueryAllResult<T1, T2>
{
    private readonly List<Archetype> _archetypes;

    internal EntityQueryAllResult(List<Archetype> matchingArchetypes)
    {
        _archetypes = matchingArchetypes;
    }

    public int PageCount => _archetypes.Count;

    public Page GetPage(int pageIndex)
    {
        var archetype = _archetypes[pageIndex];
        return new Page(archetype);
    }

    public Enumerator GetEnumerator() => new Enumerator(_archetypes);

    public struct Enumerator
    {
        private readonly List<Archetype> _archetypes;
        private int _index;

        internal Enumerator(List<Archetype> archetypes)
        {
            _archetypes = archetypes;
            _index = -1;
        }

        public Page Current => new Page(_archetypes[_index]);

        public bool MoveNext()
        {
            int next = _index + 1;
            if (_archetypes == null || next >= _archetypes.Count)
                return false;

            _index = next;
            return true;
        }
    }

    public ref struct Page
    {
        public readonly Span<EntityId> EntityIds;
        public readonly Span<T1> Components1;
        public readonly Span<T2> Components2;

        internal Page(Archetype archetype)
        {
            // todo: Investigate improving this
            EntityIds = CollectionsMarshal.AsSpan(archetype.GetEntityIds());
            Components1 = CollectionsMarshal.AsSpan(archetype.GetComponents<T1>());
            Components2 = CollectionsMarshal.AsSpan(archetype.GetComponents<T2>());
        }

        public void Deconstruct(out Span<EntityId> entityIds, out Span<T1> components1, out Span<T2> components2)
        {
            entityIds = EntityIds;
            components1 = Components1;
            components2 = Components2;
        }
    }
}


public readonly struct EntityQueryAllResult<T1, T2, T3>
{
    private readonly List<Archetype> _archetypes;

    internal EntityQueryAllResult(List<Archetype> matchingArchetypes)
    {
        _archetypes = matchingArchetypes;
    }

    public int PageCount => _archetypes.Count;

    public Page GetPage(int pageIndex)
    {
        var archetype = _archetypes[pageIndex];
        return new Page(archetype);
    }

    public Enumerator GetEnumerator() => new Enumerator(_archetypes);

    public struct Enumerator
    {
        private readonly List<Archetype> _archetypes;
        private int _index;

        internal Enumerator(List<Archetype> archetypes)
        {
            _archetypes = archetypes;
            _index = -1;
        }

        public Page Current => new Page(_archetypes[_index]);

        public bool MoveNext()
        {
            int next = _index + 1;
            if (_archetypes == null || next >= _archetypes.Count)
                return false;

            _index = next;
            return true;
        }
    }

    public ref struct Page
    {
        public readonly Span<EntityId> EntityIds;
        public readonly Span<T1> Components1;
        public readonly Span<T2> Components2;
        public readonly Span<T3> Components3;

        internal Page(Archetype archetype)
        {
            // todo: Investigate improving this
            EntityIds = CollectionsMarshal.AsSpan(archetype.GetEntityIds());
            Components1 = CollectionsMarshal.AsSpan(archetype.GetComponents<T1>());
            Components2 = CollectionsMarshal.AsSpan(archetype.GetComponents<T2>());
            Components3 = CollectionsMarshal.AsSpan(archetype.GetComponents<T3>());
        }

        public void Deconstruct(
            out Span<EntityId> entityIds,
            out Span<T1> components1,
            out Span<T2> components2,
            out Span<T3> components3)
        {
            entityIds = EntityIds;
            components1 = Components1;
            components2 = Components2;
            components3 = Components3;
        }
    }
}
