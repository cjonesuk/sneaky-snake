using System.Runtime.CompilerServices;

namespace Axis.ECS;

/// <summary>
/// 64 bit ID is structured as follows:
/// - Bits 1-24: Entity ID (24 bits)
/// - Bits 25-32: Generation (8 bits)
/// - Bits 33-56: Reserved for future use (24 bits)
/// - Bits 57-64: Role flags (8 bits)
/// </summary>
public static class IdSpace
{
    public const int LowerBits = 32;
    public const int EntityMaskBits = 24;
    public const int GenerationBits = 8;

    public const int UpperBits = 32;
    public const int RoleBits = 8;

    public const ulong LowerMask = 0x0000_0000_FFFF_FFFF;
    public const ulong UpperMask = 0xFFFF_FFFF_0000_0000;

    /// <summary>
    /// Mask to mask out everything except the generation number 
    /// </summary>
    public const ulong GenerationMask = 0x0000_0000_FF00_0000;

    /// <summary>
    /// Number of bits to shift right to get the generation number after masking with GenerationMask
    /// </summary>
    public const int GenerationShift = LowerBits - GenerationBits;

    /// <summary>
    /// Mask to mask out everything except the entity index
    /// </summary>
    public const ulong EntityIndexMask = 0x0000_0000_00FF_FFFF;

    /// <summary>
    /// Mask to mask out everything except the role flags
    /// </summary>
    public const ulong RoleMask = 0xFF00_0000_0000_0000;

    /// <summary>
    /// Mask to mask out everything except the relationship entity in a pair ID
    /// </summary>
    public const ulong RelationshipEntityMask = 0x00FF_FFFF_0000_0000;

    /// <summary>
    /// Flag within the Role bits to indicate that an ID is a pair ID
    /// </summary>
    public const ulong PairFlag = 1UL << 63;

    public const ulong Wildcard = 0x_0000_0000_FFFF_FFFF;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint EntityIndex(ulong id) => (uint)(id & EntityIndexMask);


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte Generation(ulong id) => (byte)((id & GenerationMask) >> GenerationShift);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Relationship(ulong pair)
    {
        return (uint)((pair & RelationshipEntityMask) >> LowerBits);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint Target(ulong pair)
    {
        return (uint)(pair & LowerMask);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong MakeEntity(uint index, byte generation)
    {
        return index | ((ulong)generation << GenerationShift);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong MakePair(ulong relationship, ulong target)
    {
        ulong upper = PairFlag | ((relationship & EntityIndexMask) << LowerBits);
        ulong lower = target & LowerMask;
        return upper | lower;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPair(ulong id)
    {
        return (id & PairFlag) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsValid(ulong id)
    {
        return id != 0;
    }
}
