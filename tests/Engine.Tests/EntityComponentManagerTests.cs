using System;
using System.Linq;
using Shouldly;
using Engine;

namespace Engine.Tests;

public class EntityComponentManagerTests
{
    [Fact]
    public void AddEntity_returns_incrementing_entity_ids()
    {
        var ecm = new EntityComponentManager();

        var e1 = ecm.AddEntity(1);
        var e2 = ecm.AddEntity(2);

        e1.Id.ShouldBeLessThan(e2.Id);
    }

    [Fact]
    public void QueryAll_returns_tuples_from_matching_archetypes_only()
    {
        var ecm = new EntityComponentManager();

        ecm.AddEntity(10, "a");
        ecm.AddEntity(20, "b");

        ecm.AddEntity(30); // different archetype: only int

        var result = ecm.QueryAll<int, string>().ToList();

        result.Count.ShouldBe(2);
        result.ShouldContain((10, "a"));
        result.ShouldContain((20, "b"));
    }

    [Fact]
    public void QueryAll_is_signature_order_insensitive()
    {
        var ecm = new EntityComponentManager();

        ecm.AddEntity(1, "x");     // (int,string)
        ecm.AddEntity("y", 2);     // (string,int) -> same signature

        var result = ecm.QueryAll<int, string>().OrderBy(t => t.Item1).ToList();

        result.ShouldBe(new[] { (1, "x"), (2, "y") });
    }

    [Fact]
    public void QueryAll_includes_archetypes_with_supersets_of_requested_components()
    {
        var ecm = new EntityComponentManager();

        ecm.AddEntity(100, "one");                            // signature: {int,string}
        ecm.AddEntity(200, "two", Guid.Parse("11111111-1111-1111-1111-111111111111")); // signature: {int,string,guid}

        var result = ecm.QueryAll<int, string>().OrderBy(t => t.Item1).ToList();

        result.ShouldBe(new[] { (100, "one"), (200, "two") });
    }
}
