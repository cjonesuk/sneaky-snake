using Axis.ECS.Generators;

namespace Axis.ECS.Tests;

public static partial class Simulation
{
    [SystemSource]
    public static void Integrate()
    {
        Console.WriteLine("Integrating simulation step...");
        Simulation.Generated_Integrate();
    }
}

public class WorldQueryTests
{
    record struct Position(float X, float Y);
    record struct Velocity(float DX, float DY);
    record struct Health(int Current, int Max);
    record struct Ammo(int Count, int Capacity);

    class TestWorld
    {
        private readonly World _world;

        public TestWorld()
        {
            _world = new World();
        }
    }

    [Fact]
    public void TestGenerator()
    {

    }

    [Fact]
    public void Query_Experiments()
    {
        var world = new World();
        var player = world.CreateEntity(new Position(0, 0), new Velocity(1, 1), new Health(100, 100));
        var car = world.CreateEntity(new Position(10, 10), new Velocity(5, 0));
        var gun = world.CreateEntity(new Position(15, 15), new Ammo(30, 30));

        var query = world.CreateQuery();

        Assert.NotNull(query);

        query.Add<Position>();
        query.Add<Velocity>();

        var result = query.Run();

        foreach (Archetype archetype in result)
        {

        }


    }
}