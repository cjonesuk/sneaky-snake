using System.Numerics;
using Axis.ECS;

namespace Remnant.PlayGame.Collision;


/// <summary>
/// Relates an entity to a horizontal position and a radius for broadphase collision detection
/// </summary> 
public readonly record struct ColliderProxy(Entity Entity, Vector2 Position, float Radius);

public record struct Bounds(Vector2 Min, Vector2 Max);

public interface IBroadphase
{
    Bounds Bounds { get; }

    /// <summary>
    /// Builds the broadphase data structure from a set of collider proxies. This is called once per frame before collision detection.
    /// </summary> 
    void Build(in ReadOnlySpan<ColliderProxy> items);

    void QueryAabb(in Vector2 min, in Vector2 max, List<Entity> results);
    void QueryCircle(in Vector2 center, float radius, List<Entity> results);

    void QueryRay(in Vector2 origin, in Vector2 direction, float maxDistance, List<Entity> results);
}


public sealed class UniformGridBroadphase : IBroadphase
{
    private const int InitialCapacity = 64;
    private readonly Bounds _bounds;
    private readonly Vector2 _cellSize;
    private readonly int _columns;
    private readonly int _rows;
    private readonly int _cellCount;

    private int[] _counts; // Number of items in each cell
    private int[] _cellStart; // Starting index of each cell
    private int[] _cursor; // Next index per cell for inserting items into the final arrays


    private int _itemCount; // Total number of items in the grid
    private Entity[] _entities; // Final array of entities, sorted by cell
    private Vector2[] _positions; // Final array of positions, sorted by cell

    public UniformGridBroadphase(Bounds bounds, int columns, int rows)
    {
        _bounds = bounds;
        _cellSize = new Vector2((bounds.Max.X - bounds.Min.X) / columns, (bounds.Max.Y - bounds.Min.Y) / rows);
        _columns = columns;
        _rows = rows;

        _cellCount = _columns * _rows;
        _counts = new int[_cellCount + 1];
        _cellStart = new int[_cellCount];
        _cursor = new int[_cellCount];

        _itemCount = 0;
        _entities = new Entity[InitialCapacity];
        _positions = new Vector2[InitialCapacity];
    }

    public Bounds Bounds => _bounds;


    public void Build(in ReadOnlySpan<ColliderProxy> items)
    {
        int itemCount = items.Length;
        int cellCount = _columns * _rows;

        // Allocate enough space for all items
        if (_entities.Length < itemCount)
        {
            Array.Resize(ref _entities, itemCount);
            Array.Resize(ref _positions, itemCount);
        }

        // 1. Build a histogram of how many items are in each cell
        for (int index = 0; index < itemCount; index++)
        {
            int cellIndex = CellIndex(items[index].Position);
            _counts[cellIndex]++;
        }

        // 2. Compute the prefix sum of the counts to determine the starting index of each cell in the output arrays
        _cellStart[0] = 0;
        for (int cellIndex = 0; cellIndex < cellCount; cellIndex++)
        {
            _cellStart[cellIndex + 1] = _cellStart[cellIndex] + _counts[cellIndex];
        }

        // 3. Copy the entities and positions into the final output arrays
        Array.Copy(_cellStart, _cursor, cellCount);

        for (int itemIndex = 0; itemIndex < itemCount; itemIndex++)
        {
            int cellIndex = CellIndex(items[itemIndex].Position);
            int outputIndex = _cursor[cellIndex]++;
            _entities[outputIndex] = items[itemIndex].Entity;
            _positions[outputIndex] = items[itemIndex].Position;
        }

        _itemCount = itemCount;
    }

    public void QueryAabb(in Vector2 min, in Vector2 max, List<Entity> results)
    {
        int cx0 = ClampX(min.X);
        int cy0 = ClampY(min.Y);
        int cx1 = ClampX(max.X);
        int cy1 = ClampY(max.Y);

        for (int cy = cy0; cy <= cy1; cy++) // For every row of cells that intersects the AABB
        {
            for (int cx = cx0; cx <= cx1; cx++) // For every column of cells that intersects the AABB
            {
                int cellIndex = cy * _columns + cx;

                // Iterate over all items in this cell
                for (int itemIndex = _cellStart[cellIndex]; itemIndex < _cellStart[cellIndex + 1]; itemIndex++)
                {
                    Vector2 position = _positions[itemIndex];
                    if (position.X >= min.X && position.X <= max.X &&
                       position.Y >= min.Y && position.Y <= max.Y)
                    {
                        results.Add(_entities[itemIndex]);
                    }
                }
            }
        }
    }

    public void QueryCircle(in Vector2 center, float radius, List<Entity> results)
    {
        float radiusSquared = radius * radius;

        int cx0 = ClampX(center.X - radius);
        int cy0 = ClampY(center.Y - radius);
        int cx1 = ClampX(center.X + radius);
        int cy1 = ClampY(center.Y + radius);

        for (int cy = cy0; cy <= cy1; cy++) // For every row of cells that intersects the circle
        {
            for (int cx = cx0; cx <= cx1; cx++) // For every column of cells that intersects the circle 
            {
                int cellIndex = cy * _columns + cx;
                for (int itemIndex = _cellStart[cellIndex]; itemIndex < _cellStart[cellIndex + 1]; itemIndex++)
                {
                    Vector2 position = _positions[itemIndex] - center;

                    if (position.X * position.X + position.Y * position.Y <= radiusSquared)
                    {
                        results.Add(_entities[itemIndex]);
                    }
                }
            }
        }
    }

    public void QueryRay(in Vector2 origin, in Vector2 direction, float maxDistance, List<Entity> results)
    {
        throw new NotImplementedException();
    }

    private int ClampX(float wx) => Math.Clamp((int)((wx - _bounds.Min.X) / _cellSize.X), 0, _columns - 1);
    private int ClampY(float wz) => Math.Clamp((int)((wz - _bounds.Min.Y) / _cellSize.Y), 0, _rows - 1);

    private int CellIndex(Vector2 position)
    {
        int y = ClampY(position.Y);
        int x = ClampX(position.X);

        return y * _columns + x;
    }
}