using System;
using Shouldly;
using Engine;

namespace Engine.Tests;

public class ArchetypeTests
{
    [Fact]
    public void AddEntity_and_GetComponents_return_components_by_type()
    {
        var signature = new ArchetypeSignature(new[] { typeof(int), typeof(string) });
        var archetype = new Archetype(signature);

        var e1 = new EntityId(1, 0);
        var e2 = new EntityId(2, 0);

        archetype.AddEntity(e1, 42, "alpha");
        archetype.AddEntity(e2, 7, "beta");

        var ints = archetype.GetComponents<int>();
        var strings = archetype.GetComponents<string>();

        ints.Count.ShouldBe(2);
        ints[0].ShouldBe(42);
        ints[1].ShouldBe(7);

        strings.Count.ShouldBe(2);
        strings[0].ShouldBe("alpha");
        strings[1].ShouldBe("beta");
    }

    [Fact]
    public void GetComponents_returns_empty_when_type_not_present()
    {
        var signature = new ArchetypeSignature(new[] { typeof(int) });
        var archetype = new Archetype(signature);

        var e1 = new EntityId(1, 0);
        archetype.AddEntity(e1, 123);

        archetype.GetComponents<string>().ShouldBeEmpty();
    }

    [Fact]
    public void Handles_mixed_component_sets_gracefully()
    {
        var signature = new ArchetypeSignature(new[] { typeof(int), typeof(string), typeof(DateTime) });
        var archetype = new Archetype(signature);

        var e1 = new EntityId(1, 0);
        var e2 = new EntityId(2, 0);

        archetype.AddEntity(e1, 10, "x", DateTime.UnixEpoch);
        archetype.AddEntity(e2, 20, "y", DateTime.UnixEpoch.AddDays(1));

        var dates = archetype.GetComponents<DateTime>();
        dates.Count.ShouldBe(2);
        dates[0].ShouldBe(DateTime.UnixEpoch);
        dates[1].ShouldBe(DateTime.UnixEpoch.AddDays(1));
    }
}
