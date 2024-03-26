namespace Jakar.Extensions;


public static partial class Spans
{
    public static Span<T> Trim<T>(

    #if NETSTANDARD2_1
        this
        #endif
            Span<T> span,
        T trimElement
    )
        where T : IEquatable<T>
    {
    #if NETSTANDARD2_1
        int start  = ClampStart( span, trimElement );
        int length = ClampEnd( span, trimElement );
        return span.Slice( start, length );
    #else
        return span.Trim( trimElement );
    #endif
    }
    public static ReadOnlySpan<T> Trim<T>(

    #if NETSTANDARD2_1
        this
        #endif
            ReadOnlySpan<T> span,
        T trimElement
    )
        where T : IEquatable<T>
    {
    #if NETSTANDARD2_1
        int start  = ClampStart( span, trimElement );
        int length = ClampEnd( span, trimElement );
        return span.Slice( start, length );
    #else
        return span.Trim( trimElement );
    #endif
    }
    public static Span<T> Trim<T>(

    #if NETSTANDARD2_1
        this
        #endif
            Span<T> span,
        scoped in ReadOnlySpan<T> trimElement
    )
        where T : IEquatable<T>
    {
    #if NETSTANDARD2_1
        int start  = ClampStart( span, trimElement );
        int length = ClampEnd( span, trimElement );
        return span.Slice( start, length );
    #else
        return span.Trim( trimElement );
    #endif
    }
    public static ReadOnlySpan<T> Trim<T>(

    #if NETSTANDARD2_1
        this
        #endif
            ReadOnlySpan<T> span,
        scoped in ReadOnlySpan<T> trimElement
    )
        where T : IEquatable<T>
    {
    #if NETSTANDARD2_1
        int start  = ClampStart( span, trimElement );
        int length = ClampEnd( span, trimElement );
        return span.Slice( start, length );
    #else
        return span.Trim( trimElement );
    #endif
    }


    public static Span<T> TrimEnd<T>(

    #if NETSTANDARD2_1
        this
        #endif
            Span<T> span,
        T trimElement
    )
        where T : IEquatable<T>
    {
    #if NETSTANDARD2_1
        int length = ClampEnd( span, trimElement );
        return span[.. length];
    #else
        return span.TrimEnd( trimElement );
    #endif
    }
    public static ReadOnlySpan<T> TrimEnd<T>(

    #if NETSTANDARD2_1
        this
        #endif
            ReadOnlySpan<T> span,
        T trimElement
    )
        where T : IEquatable<T>
    {
    #if NETSTANDARD2_1
        int length = ClampEnd( span, trimElement );
        return span[.. length];
    #else
        return span.TrimEnd( trimElement );
    #endif
    }
    public static Span<T> TrimEnd<T>(

    #if NETSTANDARD2_1
        this
        #endif
            Span<T> span,
        scoped in ReadOnlySpan<T> trimElement
    )
        where T : IEquatable<T>
    {
    #if NETSTANDARD2_1
        int length = ClampEnd( span, trimElement );
        return span[.. length];
    #else
        return span.TrimEnd( trimElement );
    #endif
    }
    public static ReadOnlySpan<T> TrimEnd<T>(

    #if NETSTANDARD2_1
        this
        #endif
            ReadOnlySpan<T> span,
        scoped in ReadOnlySpan<T> trimElement
    )
        where T : IEquatable<T>
    {
    #if NETSTANDARD2_1
        int length = ClampEnd( span, trimElement );
        return span[.. length];
    #else
        return span.TrimEnd( trimElement );
    #endif
    }


    public static Span<T> TrimStart<T>(

    #if NETSTANDARD2_1
        this
        #endif
            Span<T> span,
        T trimElement
    )
        where T : IEquatable<T>
    {
    #if NETSTANDARD2_1
        int start = ClampStart( span, trimElement );
        return span[start..];
    #else
        return span.TrimStart( trimElement );
    #endif
    }
    public static ReadOnlySpan<T> TrimStart<T>(

    #if NETSTANDARD2_1
        this
        #endif
            ReadOnlySpan<T> span,
        T trimElement
    )
        where T : IEquatable<T>
    {
    #if NETSTANDARD2_1
        int start = ClampStart( span, trimElement );
        return span[start..];
    #else
        return span.TrimStart( trimElement );
    #endif
    }
    public static Span<T> TrimStart<T>(

    #if NETSTANDARD2_1
        this
        #endif
            Span<T> span,
        scoped in ReadOnlySpan<T> trimElement
    )
        where T : IEquatable<T>
    {
    #if NETSTANDARD2_1
        int start = ClampStart( span, trimElement );
        return span[start..];
    #else
        return span.TrimStart( trimElement );
    #endif
    }
    public static ReadOnlySpan<T> TrimStart<T>(

    #if NETSTANDARD2_1
        this
        #endif
            ReadOnlySpan<T> span,
        scoped in ReadOnlySpan<T> trimElement
    )
        where T : IEquatable<T>
    {
    #if NETSTANDARD2_1
        int start = ClampStart( span, trimElement );
        return span[start..];
    #else
        return span.TrimStart( trimElement );
    #endif
    }


    public static int ClampStart<T>( this ReadOnlySpan<T> span, T trimElement )
        where T : IEquatable<T>
    {
        int index = 0;
        while ( index < span.Length && trimElement.Equals( span[index] ) ) { ++index; }

        return index;
    }
    public static int ClampStart<T>( this ReadOnlySpan<T> span, scoped in ReadOnlySpan<T> trimElement )
        where T : IEquatable<T>
    {
        int index = 0;
        while ( index < span.Length && span[index].IsOneOf( trimElement ) ) { ++index; }

        return index;
    }
    public static int ClampEnd<T>( this ReadOnlySpan<T> span, T trimElement, int start = 0 )
        where T : IEquatable<T>
    {
        int index = span.Length - 1;
        while ( index >= start && trimElement.Equals( span[index] ) ) { --index; }

        return index - start + 1;
    }
    public static int ClampEnd<T>( this ReadOnlySpan<T> span, scoped in ReadOnlySpan<T> trimElement, int start = 0 )
        where T : IEquatable<T>
    {
        int index = span.Length - 1;
        while ( index >= start && span[index].IsOneOf( trimElement ) ) { --index; }

        return index - start + 1;
    }
}
