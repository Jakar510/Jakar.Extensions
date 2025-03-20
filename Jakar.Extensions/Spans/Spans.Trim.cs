namespace Jakar.Extensions;


public static partial class Spans
{
    public static Span<T> Trim<T>( scoped ref readonly Span<T> span, T trimElement )
        where T : IEquatable<T> => span.Trim( trimElement );
    public static ReadOnlySpan<T> Trim<T>( scoped ref readonly ReadOnlySpan<T> span, T trimElement )
        where T : IEquatable<T> => span.Trim( trimElement );
    public static Span<T> Trim<T>( scoped ref readonly Span<T> span, scoped ref readonly ReadOnlySpan<T> trimElement )
        where T : IEquatable<T> => span.Trim( trimElement );
    public static ReadOnlySpan<T> Trim<T>( scoped ref readonly ReadOnlySpan<T> span, scoped ref readonly ReadOnlySpan<T> trimElement )
        where T : IEquatable<T> => span.Trim( trimElement );


    public static Span<T> TrimEnd<T>( scoped ref readonly Span<T> span, T trimElement )
        where T : IEquatable<T> => span.TrimEnd( trimElement );
    public static ReadOnlySpan<T> TrimEnd<T>( scoped ref readonly ReadOnlySpan<T> span, T trimElement )
        where T : IEquatable<T> => span.TrimEnd( trimElement );
    public static Span<T> TrimEnd<T>( scoped ref readonly Span<T> span, scoped ref readonly ReadOnlySpan<T> trimElement )
        where T : IEquatable<T> => span.TrimEnd( trimElement );
    public static ReadOnlySpan<T> TrimEnd<T>( scoped ref readonly ReadOnlySpan<T> span, scoped ref readonly ReadOnlySpan<T> trimElement )
        where T : IEquatable<T> => span.TrimEnd( trimElement );


    public static Span<T> TrimStart<T>( scoped ref readonly Span<T> span, T trimElement )
        where T : IEquatable<T> => span.TrimStart( trimElement );
    public static ReadOnlySpan<T> TrimStart<T>( scoped ref readonly ReadOnlySpan<T> span, T trimElement )
        where T : IEquatable<T> => span.TrimStart( trimElement );
    public static Span<T> TrimStart<T>( scoped ref readonly Span<T> span, scoped ref readonly ReadOnlySpan<T> trimElement )
        where T : IEquatable<T> => span.TrimStart( trimElement );
    public static ReadOnlySpan<T> TrimStart<T>( scoped ref readonly ReadOnlySpan<T> span, scoped ref readonly ReadOnlySpan<T> trimElement )
        where T : IEquatable<T> => span.TrimStart( trimElement );


    public static int ClampStart<T>( this scoped ref readonly ReadOnlySpan<T> span, T trimElement )
        where T : IEquatable<T>
    {
        int index = 0;
        while ( index < span.Length && trimElement.Equals( span[index] ) ) { ++index; }

        return index;
    }
    public static int ClampStart<T>( this scoped ref readonly ReadOnlySpan<T> span, scoped ref readonly ReadOnlySpan<T> trimElement )
        where T : IEquatable<T>
    {
        int index = 0;
        while ( index < span.Length && span[index].IsOneOf( trimElement ) ) { ++index; }

        return index;
    }
    public static int ClampEnd<T>( this scoped ref readonly ReadOnlySpan<T> span, T trimElement, int start = 0 )
        where T : IEquatable<T>
    {
        int index = span.Length - 1;
        while ( index >= start && trimElement.Equals( span[index] ) ) { --index; }

        return index - start + 1;
    }
    public static int ClampEnd<T>( this scoped ref readonly ReadOnlySpan<T> span, scoped ref readonly ReadOnlySpan<T> trimElement, int start = 0 )
        where T : IEquatable<T>
    {
        int index = span.Length - 1;
        while ( index >= start && span[index].IsOneOf( trimElement ) ) { --index; }

        return index - start + 1;
    }
}
