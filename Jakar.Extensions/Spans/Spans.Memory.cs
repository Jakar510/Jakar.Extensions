// Jakar.Extensions :: Jakar.Extensions
// 04/15/2022  4:46 PM

using System.Runtime.InteropServices;



namespace Jakar.Extensions.Spans;


[SuppressMessage("ReSharper", "OutParameterValueIsAlwaysDiscarded.Global")]
public static partial class Spans
{
    public static string? ConvertToString( this Memory<char> value ) => MemoryMarshal.TryGetString(value, out string? defaultValue, out _, out _)
                                                                            ? defaultValue
                                                                            : default;
    public static string? ConvertToString( this ReadOnlyMemory<char> value ) => MemoryMarshal.TryGetString(value, out string? defaultValue, out _, out _)
                                                                                    ? defaultValue
                                                                                    : default;


    public static ReadOnlySpan<char> AsReadOnlySpan( this ReadOnlyMemory<char> value )
    {
        var temp = new Memory<char>();
        value.CopyTo(temp);
        return temp.Span;
    }
    public static Span<char> AsSpan( this ReadOnlyMemory<char> value )
    {
        var temp = new Memory<char>();
        value.CopyTo(temp);
        return temp.Span;
    }


    public static bool AsSegment( this ReadOnlyMemory<char> value, out ArraySegment<char> result ) => MemoryMarshal.TryGetArray(value, out result);
    public static bool AsSegment( this Memory<char>         value, out ArraySegment<char> result ) => MemoryMarshal.TryGetArray(value, out result);


    public static bool AsSegment( this ReadOnlyMemory<byte> value, out ArraySegment<byte> result ) => MemoryMarshal.TryGetArray(value, out result);
    public static bool AsSegment( this Memory<byte>         value, out ArraySegment<byte> result ) => MemoryMarshal.TryGetArray(value, out result);


    public static bool AsSpan( this ReadOnlyMemory<char> value, ref Span<char> result )
    {
        if ( !value.AsSegment(out ArraySegment<char> segment) ) { return false; }

        Span<char> temp = segment;
        temp.CopyTo(result);
        return true;
    }
    public static bool AsSpan( this Memory<char> value, ref Span<char> result )
    {
        if ( !value.AsSegment(out ArraySegment<char> segment) ) { return false; }

        Span<char> temp = segment;
        temp.CopyTo(result);
        return true;
    }


    public static bool AsSpan( this ReadOnlyMemory<byte> value, ref Span<byte> result )
    {
        if ( value.AsSegment(out ArraySegment<byte> segment) )
        {
            Span<byte> temp = segment;
            temp.CopyTo(result);
            return true;
        }

        return false;
    }
    public static bool AsSpan( this Memory<byte> value, ref Span<byte> result )
    {
        if ( value.AsSegment(out ArraySegment<byte> segment) )
        {
            Span<byte> temp = segment;
            temp.CopyTo(result);
            return true;
        }

        return false;
    }

    // public static ReadOnlySpan<char> Copy( this Span<char> value )
    // {
    //     var result = new char[value.Length];
    //     value.CopyTo(result);
    //     return result;
    // }
    // public static ReadOnlySpan<char> Copy( this ReadOnlySpan<char> value )
    // {
    //     var result = new char[value.Length];
    //     value.CopyTo(result);
    //     return result;
    // }


    public static Memory<char> AsMemory( this Span<char> value )
    {
        MemoryMarshal.TryRead(MemoryMarshal.AsBytes(value), out Memory<char> result);
        return result;
    }
    public static Memory<char> AsMemory( this ReadOnlySpan<char> value )
    {
        MemoryMarshal.TryRead(MemoryMarshal.AsBytes(value), out Memory<char> result);
        return result;
    }


    public static Memory<byte> AsMemory( this Span<byte> value )
    {
        MemoryMarshal.TryRead(MemoryMarshal.AsBytes(value), out Memory<byte> result);
        return result;
    }
    public static Memory<byte> AsMemory( this ReadOnlySpan<byte> value )
    {
        MemoryMarshal.TryRead(MemoryMarshal.AsBytes(value), out Memory<byte> result);
        return result;
    }
}
