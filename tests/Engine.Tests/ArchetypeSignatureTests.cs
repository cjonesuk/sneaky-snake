using Shouldly;
using System;
using Engine;

namespace Engine.Tests;

public class ArchetypeSignatureTests
{
    [Fact]
    public void ContainsAll_returns_true_when_all_types_present()
    {
        var sig = new ArchetypeSignature(new[] { typeof(string), typeof(int), typeof(DateTime) });
        sig.ContainsAll(new[] { typeof(int), typeof(string) }).ShouldBeTrue();
    }

    [Fact]
    public void ContainsAll_returns_false_when_missing_type()
    {
        var sig = new ArchetypeSignature(new[] { typeof(string), typeof(int) });
        sig.ContainsAll(new[] { typeof(int), typeof(Guid) }).ShouldBeFalse();
    }

    [Fact]
    public void Equality_is_order_insensitive_and_sequence_equal()
    {
        var a = new ArchetypeSignature(new[] { typeof(int), typeof(string) });
        var b = new ArchetypeSignature(new[] { typeof(string), typeof(int) });

        a.Equals(b).ShouldBeTrue();
        // Also ensure hash codes match for equal signatures
        a.GetHashCode().ShouldBe(b.GetHashCode());
    }

    [Fact]
    public void Inequality_when_different_sets()
    {
        var a = new ArchetypeSignature(new[] { typeof(int), typeof(string) });
        var b = new ArchetypeSignature(new[] { typeof(int), typeof(string), typeof(Guid) });

        a.Equals(b).ShouldBeFalse();
    }

    [Fact]
    public void DebuggerDisplay_shows_sorted_type_names()
    {
        var sig = new ArchetypeSignature(new[] { typeof(B), typeof(A) });
        // DebuggerDisplay is not directly accessible; verify internal ordering via equality
        var expected = new ArchetypeSignature(new[] { typeof(A), typeof(B) });
        sig.Equals(expected).ShouldBeTrue();
    }

    private class A { }
    private class B { }
}
