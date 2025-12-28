using System.Runtime.InteropServices;

namespace Engine.Collision;

internal readonly struct WorldSpaceLists
{
    public readonly List<WorldAabb> Aabbs;
    public readonly List<WorldObb> Obbs;
    public readonly List<WorldCircle> Circles;

    private WorldSpaceLists(
        List<WorldAabb> aabbs,
        List<WorldObb> obbs,
        List<WorldCircle> circles)
    {
        Aabbs = aabbs;
        Obbs = obbs;
        Circles = circles;
    }

    public static WorldSpaceLists Create()
    {
        return new WorldSpaceLists(
            new List<WorldAabb>(),
            new List<WorldObb>(),
            new List<WorldCircle>()
        );
    }

    public Span<WorldAabb> GetAabbsSpan()
    {
        return CollectionsMarshal.AsSpan(Aabbs);
    }

    public Span<WorldObb> GetObbsSpan()
    {
        return CollectionsMarshal.AsSpan(Obbs);
    }

    public Span<WorldCircle> GetCirclesSpan()
    {
        return CollectionsMarshal.AsSpan(Circles);
    }

    public void Clear()
    {
        Aabbs.Clear();
        Obbs.Clear();
        Circles.Clear();
    }
}