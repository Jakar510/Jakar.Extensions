// Jakar.Extensions :: Jakar.Extensions
// 08/26/2023  12:06 PM

using System;



namespace Jakar.Extensions;


public static partial class Spans
{
    public static T First<T>( this Span<T> value, Func<T, bool> selector ) where T : IEquatable<T>
    {
        foreach ( T equatable in value )
        {
            if ( selector( equatable ) ) { return equatable; }
        }

        throw new NotFoundException();
    }
    public static T First<T>( this ReadOnlySpan<T> value, Func<T, bool> selector ) where T : IEquatable<T>
    {
        foreach ( T equatable in value )
        {
            if ( selector( equatable ) ) { return equatable; }
        }

        throw new NotFoundException();
    }
    public static T? FirstOrDefault<T>( this Span<T> value, Func<T, bool> selector ) where T : IEquatable<T>
    {
        foreach ( T equatable in value )
        {
            if ( selector( equatable ) ) { return equatable; }
        }

        return default;
    }
    public static T? FirstOrDefault<T>( this ReadOnlySpan<T> value, Func<T, bool> selector ) where T : IEquatable<T>
    {
        foreach ( T equatable in value )
        {
            if ( selector( equatable ) ) { return equatable; }
        }

        return default;
    }


    public static Span<T> Where<T>( this Span<T> value, Func<T, bool> selector ) where T : IEquatable<T>
    {
        using var buffer = new Buffer<T>( value.Length );

        foreach ( T equatable in value )
        {
            if ( selector( equatable ) ) { buffer.Append( equatable ); }
        }

        return buffer.Span.ToArray();
    }
    public static ReadOnlySpan<T> Where<T>( this ReadOnlySpan<T> value, Func<T, bool> selector ) where T : IEquatable<T>
    {
        using var buffer = new Buffer<T>( value.Length );

        foreach ( T equatable in value )
        {
            if ( selector( equatable ) ) { buffer.Append( equatable ); }
        }

        return buffer.Span.ToArray();
    }
}
