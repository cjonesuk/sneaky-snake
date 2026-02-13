using Axis.ECS.Generators;
using Axis.ECS.Queries;
using Shouldly;

namespace Axis.ECS.Tests;

// [EcsSystem]
// internal static partial class Simulation
// {
//     // [SystemSource]
//     public static void RunMySystem(ref WorldSystemContext context, ref Position position, in Velocity velocity)
//     {
//         position.X += velocity.DX;
//         position.Y += velocity.DY;
//     }


//     public static void SetupSystems(World world)
//     {
//         world.AddMySystem();
//     }

// }

// // To be generated
// internal static partial class Simulation
// {
//     public static void AddMySystem(this World world)
//     {
//         world.AddSystem(new MySystem(world));
//     }

//     public class MySystem : IWorldSystem
//     {
//         delegate void MyDelegate(ref WorldSystemContext context, ref Position position, in Velocity velocity);
//         private readonly MyDelegate _myDelegate;
//         private readonly QueryContainer _query;

//         public MySystem(World world)
//         {
//             _query = world.CreateQuery();
//             _query.Add<Position>(QueryTermBinding.InOut);
//             _query.Add<Velocity>(QueryTermBinding.In);

//             _myDelegate = Simulation.RunMySystem;
//         }


//         public void Execute(ref WorldSystemContext data)
//         {
//             var results = _query.Run();

//             foreach (var archetype in results)
//             {
//                 if (!archetype.TryGetColumnSpan<Position>(out var positions))
//                     continue;

//                 if (!archetype.TryGetColumnSpan<Velocity>(out var velocities))
//                     continue;

//                 for (int index = 0; index < positions.Length; index++)
//                 {
//                     ref var position = ref positions[index];
//                     ref readonly var velocity = ref velocities[index];

//                     _myDelegate(ref data, ref position, in velocity);
//                 }
//             }
//         }
//     }

// }

// internal static class MySystemWorldExtensions
// {

// }

public class WorldQueryTests
{


    class TestWorld
    {
        private readonly World _world;

        public TestWorld()
        {
            _world = new World();
        }
    }

    // [Fact]
    // public void TestGenerator()
    // {
    //     var world = World.Create();
    //     world.AddMySystem();
    // }


    [Fact]
    public void Query_Experiments()
    {
        var world = World.Create();
        var player = world.CreateEntity(); new Position(0, 0), new Velocity(1, 1), new Health(100));
        var car = world.CreateEntity(new Position(10, 10), new Velocity(5, 0));
        var chestPlate = world.CreateEntity(new Position(15, 15), new Armor(30));

        var query = world.CreateQuery()
            .Add<Position>(QueryTermBinding.InOut)
            .Add<Velocity>(QueryTermBinding.In)
            .Build();

        Assert.NotNull(query);

        var result = query.Run();

        result.Count.ShouldBe(2);
    }

    // [Fact]
    // public void Query_IsA()
    // {
    //     var world = World.Create();
    //     var prefab = world.CreateEntity();
    //     prefab.Set(new Health(100));
    //     prefab.Set(new Armor(50));
    //     prefab.Set(new Healing(5));

    //     var player = world.CreateEntity();
    //     player.Set(new Position(0, 0));
    //     player.SetPair(world.Pairs.IsA, prefab);

    //     // var prefab2 = player.GetPair(world.Pairs.IsA);
    //     // prefab2.ShouldBe(prefab);

    //     var playerHealth = player.Get<Health>();
    //     playerHealth.Value.ShouldBe(100);

    //     var playerArmor = player.Get<Armor>();
    //     playerArmor.Value.ShouldBe(50);

    //     var playerHealing = player.Get<Healing>();
    //     playerHealing.Amount.ShouldBe(5);
    // }
}