// Jakar.Extensions :: Jakar.Extensions
// 06/18/2025  14:22

namespace Jakar.Extensions;


/// <summary>
///     <para>
///         <see href="https://devblogs.microsoft.com/dotnet/performance_improvements_in_net_7/#vectorization"/>
///     </para>
/// </summary>
[Experimental( nameof(Vectors) )]
public static class Vectors
{
    public static bool Contains( scoped ref readonly ReadOnlySpan<nuint> source, nuint value )
    {
        if ( Vector.IsHardwareAccelerated && source.Length >= Vector<nuint>.Count )
        {
            Vector<nuint> target = new(value);
            Vector<nuint> values = new(source);
            return Vector.EqualsAny( values, target );
        }

        return MemoryExtensions.Contains( source, value );
    }
    public static bool Contains( scoped ref readonly ReadOnlySpan<uint> source, uint value )
    {
        if ( Vector.IsHardwareAccelerated && source.Length >= Vector<uint>.Count )
        {
            Vector<uint> target = new(value);
            Vector<uint> values = new(source);
            return Vector.EqualsAny( values, target );
        }

        return MemoryExtensions.Contains( source, value );
    }
    public static bool Contains( scoped ref readonly ReadOnlySpan<byte> source, byte value )
    {
        if ( Vector.IsHardwareAccelerated && source.Length >= Vector<byte>.Count )
        {
            Vector<byte> target = new(value);
            Vector<byte> values = new(source);
            return Vector.EqualsAny( values, target );
        }

        return MemoryExtensions.Contains( source, value );
    }
    public static bool Contains( scoped ref readonly ReadOnlySpan<byte> source, sbyte value )
    {
        if ( Vector.IsHardwareAccelerated && source.Length >= Vector<sbyte>.Count )
        {
            Vector<sbyte> target = new(value);
            Vector<sbyte> values = new(source);
            return Vector.EqualsAny( values, target );
        }

        foreach ( byte x in source )
        {
            if ( x == value ) { return true; }
        }

        return false;
    }
    public static bool Contains( scoped ref readonly ReadOnlySpan<short> source, short value )
    {
        if ( Vector.IsHardwareAccelerated && source.Length >= Vector<short>.Count )
        {
            Vector<short> target = new(value);
            Vector<short> values = new(source);
            return Vector.EqualsAny( values, target );
        }

        return MemoryExtensions.Contains( source, value );
    }
    public static bool Contains( scoped ref readonly ReadOnlySpan<int> source, int value )
    {
        if ( Vector.IsHardwareAccelerated && source.Length >= Vector<int>.Count )
        {
            Vector<int> target = new(value);
            Vector<int> values = new(source);
            return Vector.EqualsAny( values, target );
        }

        return MemoryExtensions.Contains( source, value );
    }
    public static bool Contains( scoped ref readonly ReadOnlySpan<long> source, long value )
    {
        if ( Vector.IsHardwareAccelerated && source.Length >= Vector<long>.Count )
        {
            Vector<long> target = new(value);
            Vector<long> values = new(source);
            return Vector.EqualsAny( values, target );
        }

        return MemoryExtensions.Contains( source, value );
    }
    public static bool Contains( scoped ref readonly ReadOnlySpan<ushort> source, ushort value )
    {
        if ( Vector.IsHardwareAccelerated && source.Length >= Vector<ushort>.Count )
        {
            Vector<ushort> target = new(value);
            Vector<ushort> values = new(source);
            return Vector.EqualsAny( values, target );
        }

        return MemoryExtensions.Contains( source, value );
    }
    public static bool Contains( scoped ref readonly ReadOnlySpan<nint> source, nint value )
    {
        if ( Vector.IsHardwareAccelerated && source.Length >= Vector<nint>.Count )
        {
            Vector<nint> target = new(value);
            Vector<nint> values = new(source);
            return Vector.EqualsAny( values, target );
        }

        return MemoryExtensions.Contains( source, value );
    }
    public static bool Contains( scoped ref readonly ReadOnlySpan<ulong> source, ulong value )
    {
        if ( Vector.IsHardwareAccelerated && source.Length >= Vector<ulong>.Count )
        {
            Vector<ulong> target = new(value);
            Vector<ulong> values = new(source);
            return Vector.EqualsAny( values, target );
        }

        return MemoryExtensions.Contains( source, value );
    }
    public static bool Contains( scoped ref readonly ReadOnlySpan<float> source, float value )
    {
        if ( Vector.IsHardwareAccelerated && source.Length >= Vector<float>.Count )
        {
            Vector<float> target = new(value);
            Vector<float> values = new(source);
            return Vector.EqualsAny( values, target );
        }

        return MemoryExtensions.Contains( source, value );
    }
    public static bool Contains( scoped ref readonly ReadOnlySpan<double> source, double value )
    {
        if ( Vector.IsHardwareAccelerated && source.Length >= Vector<double>.Count )
        {
            Vector<double> target = new(value);
            Vector<double> values = new(source);
            return Vector.EqualsAny( values, target );
        }

        return MemoryExtensions.Contains( source, value );
    }
}
