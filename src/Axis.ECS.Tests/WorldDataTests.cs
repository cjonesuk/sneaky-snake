using Shouldly;

namespace Axis.ECS.Tests;

public class WorldDataTests
{
    [Fact]
    public void SetData_ThenGetData_RoundTripsValue()
    {
        var world = World.Create();
        world.SetData(new Health(42));

        world.GetData<Health>().ShouldBe(new Health(42));
    }

    [Fact]
    public void GetData_ReturnsByRef_MutationPersists()
    {
        var world = World.Create();
        world.SetData(new Health(10));

        ref var data = ref world.GetData<Health>();
        data.Value = 99;

        world.GetData<Health>().Value.ShouldBe(99);
    }

    [Fact]
    public void HasData_TracksLifecycle()
    {
        var world = World.Create();
        world.HasData<Health>().ShouldBeFalse();

        world.SetData(new Health(1));
        world.HasData<Health>().ShouldBeTrue();

        world.RemoveData<Health>();
        world.HasData<Health>().ShouldBeFalse();
    }

    [Fact]
    public void SetData_Twice_Updates()
    {
        var world = World.Create();
        world.SetData(new Health(1));
        world.SetData(new Health(2));

        world.GetData<Health>().Value.ShouldBe(2);
    }

    [Fact]
    public void GetData_OnMissingType_Throws()
    {
        var world = World.Create();

        Should.Throw<InvalidOperationException>(() => world.GetData<Health>());
    }

    [Fact]
    public void RemoveAllEntities_PreservesDataApi()
    {
        var world = World.Create();
        world.SetData(new Health(10));

        world.RemoveAllEntities();

        // Old data is gone (entity was wiped), but the API should still work.
        world.HasData<Health>().ShouldBeFalse();
        world.SetData(new Health(20));
        world.GetData<Health>().Value.ShouldBe(20);
    }
}
