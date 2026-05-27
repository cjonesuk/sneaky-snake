using Shouldly;

namespace Axis.ECS.Tests;

public class EntityIdRecyclingTests
{
    [Fact]
    public void RemovedEntity_IndexIsReused_WithBumpedGeneration()
    {
        var world = World.Create();
        var first = world.SpawnEntity();

        world.RemoveEntity(first.Id);
        var second = world.SpawnEntity();

        second.Id.Index.ShouldBe(first.Id.Index);
        second.Id.Generation.ShouldBe((byte)(first.Id.Generation + 1));
    }

    [Fact]
    public void StaleIdReference_ReportsNotAlive_EvenAfterIndexReuse()
    {
        var world = World.Create();
        var first = world.SpawnEntity();
        var staleId = first.Id;

        world.RemoveEntity(first.Id);
        var second = world.SpawnEntity();

        second.IsAlive().ShouldBeTrue();
        world.IsEntityAlive(staleId).ShouldBeFalse();
    }

    [Fact]
    public void RepeatedSpawnAndRemove_StaysWithinGenerationBudget()
    {
        var world = World.Create();
        // Same index reused until generation hits 255, then a fresh index is allocated
        // (the worn-out slot is retired to avoid wrap collision). 254 cycles fits.
        var entity = world.SpawnEntity();
        uint firstIndex = entity.Id.Index;

        for (int i = 0; i < 254; i++)
        {
            world.RemoveEntity(entity.Id);
            entity = world.SpawnEntity();
            entity.Id.Index.ShouldBe(firstIndex, $"index drifted on iteration {i}");
        }
    }

    [Fact]
    public void GenerationWrap_RetiresSlot_NoCollisionWithOriginal()
    {
        var world = World.Create();
        var first = world.SpawnEntity();
        uint firstIndex = first.Id.Index;

        // Spawn-remove until the generation can no longer be bumped without wrap (256 cycles).
        for (int i = 0; i < 256; i++)
        {
            world.RemoveEntity(first.Id);
            first = world.SpawnEntity();
        }

        // The 257th cycle should now allocate a NEW index because the slot retired.
        world.RemoveEntity(first.Id);
        var afterWrap = world.SpawnEntity();

        afterWrap.Id.Index.ShouldNotBe(firstIndex);
    }

    [Fact]
    public void RemoveAllEntities_FreesAllIndices()
    {
        var world = World.Create();
        var a = world.SpawnEntity();
        var b = world.SpawnEntity();
        var c = world.SpawnEntity();

        world.RemoveAllEntities();

        // After RemoveAllEntities, the next spawn should land in one of the freed slots
        // (with a bumped generation) rather than allocating a new index.
        var next = world.SpawnEntity();
        var possibleReusedIndices = new[] { a.Id.Index, b.Id.Index, c.Id.Index };
        possibleReusedIndices.ShouldContain(next.Id.Index);
    }
}
