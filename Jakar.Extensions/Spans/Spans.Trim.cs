namespace Jakar.Extensions;


public static partial class Spans
{
    /// <summary>
    /// Removes all leading and trailing occurrences of a specified element from the memory.
    /// </summary>
    /// <param name="memory">The source memory from which the element is removed.</param>
    /// <param name="trimElement">The specified element to look for and remove.</param>
    public static Memory<TValue> Trim<TValue>( this scoped ref readonly Memory<TValue> memory, TValue trimElement )
        where TValue : IEquatable<TValue>
    {
        ReadOnlySpan<TValue> span   = memory.Span;
        int             start  = span.ClampStart( trimElement );
        int             length = span.ClampEnd( start, trimElement );
        return memory.Slice( start, length );
    }

    /// <summary>
    /// Removes all leading occurrences of a specified element from the memory.
    /// </summary>
    /// <param name="memory">The source memory from which the element is removed.</param>
    /// <param name="trimElement">The specified element to look for and remove.</param>
    public static Memory<TValue> TrimStart<TValue>( this scoped ref readonly Memory<TValue> memory, TValue trimElement )
        where TValue : IEquatable<TValue>
    {
        ReadOnlySpan<TValue> span = memory.Span;
        return memory.Slice( span.ClampStart( trimElement ) );
    }

    /// <summary>
    /// Removes all trailing occurrences of a specified element from the memory.
    /// </summary>
    /// <param name="memory">The source memory from which the element is removed.</param>
    /// <param name="trimElement">The specified element to look for and remove.</param>
    public static Memory<TValue> TrimEnd<TValue>( this scoped ref readonly Memory<TValue> memory, TValue trimElement )
        where TValue : IEquatable<TValue>
    {
        ReadOnlySpan<TValue> span = memory.Span;
        return memory.Slice( 0, span.ClampEnd( 0, trimElement ) );
    }

    /// <summary>
    /// Removes all leading and trailing occurrences of a specified element from the memory.
    /// </summary>
    /// <param name="memory">The source memory from which the element is removed.</param>
    /// <param name="trimElement">The specified element to look for and remove.</param>
    public static ReadOnlyMemory<TValue> Trim<TValue>( this scoped ref readonly ReadOnlyMemory<TValue> memory, TValue trimElement )
        where TValue : IEquatable<TValue>
    {
        ReadOnlySpan<TValue> span   = memory.Span;
        int             start  = span.ClampStart( trimElement );
        int             length = span.ClampEnd( start, trimElement );
        return memory.Slice( start, length );
    }

    /// <summary>
    /// Removes all leading occurrences of a specified element from the memory.
    /// </summary>
    /// <param name="memory">The source memory from which the element is removed.</param>
    /// <param name="trimElement">The specified element to look for and remove.</param>
    public static ReadOnlyMemory<TValue> TrimStart<TValue>( this scoped ref readonly ReadOnlyMemory<TValue> memory, TValue trimElement )
        where TValue : IEquatable<TValue>
    {
        ReadOnlySpan<TValue> span = memory.Span;
        return memory.Slice( span.ClampStart( trimElement ) );
    }

    /// <summary>
    /// Removes all trailing occurrences of a specified element from the memory.
    /// </summary>
    /// <param name="memory">The source memory from which the element is removed.</param>
    /// <param name="trimElement">The specified element to look for and remove.</param>
    public static ReadOnlyMemory<TValue> TrimEnd<TValue>( this scoped ref readonly ReadOnlyMemory<TValue> memory, TValue trimElement )
        where TValue : IEquatable<TValue>
    {
        ReadOnlySpan<TValue> span = memory.Span;
        return memory.Slice( 0, span.ClampEnd( 0, trimElement ) );
    }

    /// <summary>
    /// Removes all leading and trailing occurrences of a specified element from the span.
    /// </summary>
    /// <param name="span">The source span from which the element is removed.</param>
    /// <param name="trimElement">The specified element to look for and remove.</param>
    public static Span<TValue> Trim<TValue>( this scoped ref readonly Span<TValue> span, TValue trimElement )
        where TValue : IEquatable<TValue>
    {
        ReadOnlySpan<TValue> values = span;
        int             start  = values.ClampStart( trimElement );
        int             length = values.ClampEnd( start, trimElement );
        return span.Slice( start, length );
    }

    /// <summary>
    /// Removes all leading occurrences of a specified element from the span.
    /// </summary>
    /// <param name="span">The source span from which the element is removed.</param>
    /// <param name="trimElement">The specified element to look for and remove.</param>
    public static Span<TValue> TrimStart<TValue>( this scoped ref readonly Span<TValue> span, TValue trimElement )
        where TValue : IEquatable<TValue>
    {
        ReadOnlySpan<TValue> temp = span;
        return span.Slice( temp.ClampStart( trimElement ) );
    }

    /// <summary>
    /// Removes all trailing occurrences of a specified element from the span.
    /// </summary>
    /// <param name="span">The source span from which the element is removed.</param>
    /// <param name="trimElement">The specified element to look for and remove.</param>
    public static Span<TValue> TrimEnd<TValue>( this scoped ref readonly Span<TValue> span, TValue trimElement )
        where TValue : IEquatable<TValue>
    {
        ReadOnlySpan<TValue> values = span;
        return span.Slice( 0, values.ClampEnd( 0, trimElement ) );
    }

    /// <summary>
    /// Removes all leading and trailing occurrences of a specified element from the span.
    /// </summary>
    /// <param name="span">The source span from which the element is removed.</param>
    /// <param name="trimElement">The specified element to look for and remove.</param>
    public static ReadOnlySpan<TValue> Trim<TValue>( this scoped ref readonly ReadOnlySpan<TValue> span, TValue trimElement )
        where TValue : IEquatable<TValue>
    {
        int start  = span.ClampStart( trimElement );
        int length = span.ClampEnd( start, trimElement );
        return span.Slice( start, length );
    }

    /// <summary>
    /// Removes all leading occurrences of a specified element from the span.
    /// </summary>
    /// <param name="span">The source span from which the element is removed.</param>
    /// <param name="trimElement">The specified element to look for and remove.</param>
    public static ReadOnlySpan<TValue> TrimStart<TValue>( this scoped ref readonly ReadOnlySpan<TValue> span, TValue trimElement )
        where TValue : IEquatable<TValue>
    {
        return span.Slice( span.ClampStart( trimElement ) );
    }

    /// <summary>
    /// Removes all trailing occurrences of a specified element from the span.
    /// </summary>
    /// <param name="span">The source span from which the element is removed.</param>
    /// <param name="trimElement">The specified element to look for and remove.</param>
    public static ReadOnlySpan<TValue> TrimEnd<TValue>( this scoped ref readonly ReadOnlySpan<TValue> span, TValue trimElement )
        where TValue : IEquatable<TValue>
    {
        ReadOnlySpan<TValue> values = span;
        return span.Slice( 0, values.ClampEnd( 0, trimElement ) );
    }

    /// <summary>
    /// Delimits all leading occurrences of a specified element from the span.
    /// </summary>
    /// <param name="span">The source span from which the element is removed.</param>
    /// <param name="trimElement">The specified element to look for and remove.</param>
    public static int ClampStart<TValue>( this scoped ref readonly ReadOnlySpan<TValue> span, TValue trimElement )
        where TValue : IEquatable<TValue>
    {
        int start = 0;

        if ( trimElement != null )
        {
            for ( ; start < span.Length; start++ )
            {
                if ( !trimElement.Equals( span[start] ) ) { break; }
            }
        }
        else
        {
            for ( ; start < span.Length; start++ )
            {
                if ( span[start] != null ) { break; }
            }
        }

        return start;
    }

    /// <summary>
    /// Delimits all trailing occurrences of a specified element from the span.
    /// </summary>
    /// <param name="span">The source span from which the element is removed.</param>
    /// <param name="start">The start index from which to being searching.</param>
    /// <param name="trimElement">The specified element to look for and remove.</param>
    public static int ClampEnd<TValue>( this scoped ref readonly ReadOnlySpan<TValue> span, int start, TValue trimElement )
        where TValue : IEquatable<TValue>
    {
        // Initially, start==len==0. If ClampStart trims all, start==len
        Debug.Assert( (uint)start <= span.Length );

        int end = span.Length - 1;

        if ( trimElement != null )
        {
            for ( ; end >= start; end-- )
            {
                if ( !trimElement.Equals( span[end] ) ) { break; }
            }
        }
        else
        {
            for ( ; end >= start; end-- )
            {
                if ( span[end] != null ) { break; }
            }
        }

        return end - start + 1;
    }

    /// <summary>
    /// Removes all leading and trailing occurrences of a set of elements specified
    /// in a readonly span from the memory.
    /// </summary>
    /// <param name="memory">The source memory from which the elements are removed.</param>
    /// <param name="trimElements">The span which contains the set of elements to remove.</param>
    /// <remarks>If <paramref name="trimElements"/> is empty, the memory is returned unaltered.</remarks>
    public static Memory<TValue> Trim<TValue>( this scoped ref readonly Memory<TValue> memory, ReadOnlySpan<TValue> trimElements )
        where TValue : IEquatable<TValue>
    {
        switch ( trimElements.Length )
        {
            case > 1:
            {
                ReadOnlySpan<TValue> span   = memory.Span;
                int             start  = span.ClampStart( trimElements );
                int             length = span.ClampEnd( start, trimElements );
                return memory.Slice( start, length );
            }

            case 1:  return memory.Trim( trimElements[0] );
            default: return memory;
        }
    }

    /// <summary>
    /// Removes all leading occurrences of a set of elements specified
    /// in a readonly span from the memory.
    /// </summary>
    /// <param name="memory">The source memory from which the elements are removed.</param>
    /// <param name="trimElements">The span which contains the set of elements to remove.</param>
    /// <remarks>If <paramref name="trimElements"/> is empty, the memory is returned unaltered.</remarks>
    public static Memory<TValue> TrimStart<TValue>( this scoped ref readonly Memory<TValue> memory, params ReadOnlySpan<TValue> trimElements )
        where TValue : IEquatable<TValue>
    {
        switch ( trimElements.Length )
        {
            case > 1:
            {
                ReadOnlySpan<TValue> values = memory.Span;
                return memory.Slice( values.ClampStart( trimElements ) );
            }

            case 1:  return memory.TrimStart( trimElements[0] );
            default: return memory;
        }
    }

    /// <summary>
    /// Removes all trailing occurrences of a set of elements specified
    /// in a readonly span from the memory.
    /// </summary>
    /// <param name="memory">The source memory from which the elements are removed.</param>
    /// <param name="trimElements">The span which contains the set of elements to remove.</param>
    /// <remarks>If <paramref name="trimElements"/> is empty, the memory is returned unaltered.</remarks>
    public static Memory<TValue> TrimEnd<TValue>( this scoped ref readonly Memory<TValue> memory, params ReadOnlySpan<TValue> trimElements )
        where TValue : IEquatable<TValue>
    {
        switch ( trimElements.Length )
        {
            case > 1:
            {
                ReadOnlySpan<TValue> values = memory.Span;
                return memory.Slice( 0, values.ClampEnd( 0, trimElements ) );
            }

            case 1:  return memory.TrimEnd( trimElements[0] );
            default: return memory;
        }
    }

    /// <summary>
    /// Removes all leading and trailing occurrences of a set of elements specified
    /// in a readonly span from the memory.
    /// </summary>
    /// <param name="memory">The source memory from which the elements are removed.</param>
    /// <param name="trimElements">The span which contains the set of elements to remove.</param>
    /// <remarks>If <paramref name="trimElements"/> is empty, the memory is returned unaltered.</remarks>
    public static ReadOnlyMemory<TValue> Trim<TValue>( this scoped ref readonly ReadOnlyMemory<TValue> memory, params ReadOnlySpan<TValue> trimElements )
        where TValue : IEquatable<TValue>
    {
        switch ( trimElements.Length )
        {
            case > 1:
            {
                ReadOnlySpan<TValue> span   = memory.Span;
                int             start  = span.ClampStart( trimElements );
                int             length = span.ClampEnd( start, trimElements );
                return memory.Slice( start, length );
            }

            case 1:  return memory.Trim( trimElements[0] );
            default: return memory;
        }
    }

    /// <summary>
    /// Removes all leading occurrences of a set of elements specified
    /// in a readonly span from the memory.
    /// </summary>
    /// <param name="memory">The source memory from which the elements are removed.</param>
    /// <param name="trimElements">The span which contains the set of elements to remove.</param>
    /// <remarks>If <paramref name="trimElements"/> is empty, the memory is returned unaltered.</remarks>
    public static ReadOnlyMemory<TValue> TrimStart<TValue>( this scoped ref readonly ReadOnlyMemory<TValue> memory, params ReadOnlySpan<TValue> trimElements )
        where TValue : IEquatable<TValue>
    {
        switch ( trimElements.Length )
        {
            case > 1:
            {
                ReadOnlySpan<TValue> values = memory.Span;
                return memory.Slice( values.ClampStart( trimElements ) );
            }

            case 1:  return memory.TrimStart( trimElements[0] );
            default: return memory;
        }
    }

    /// <summary>
    /// Removes all trailing occurrences of a set of elements specified
    /// in a readonly span from the memory.
    /// </summary>
    /// <param name="memory">The source memory from which the elements are removed.</param>
    /// <param name="trimElements">The span which contains the set of elements to remove.</param>
    /// <remarks>If <paramref name="trimElements"/> is empty, the memory is returned unaltered.</remarks>
    public static ReadOnlyMemory<TValue> TrimEnd<TValue>( this scoped ref readonly ReadOnlyMemory<TValue> memory, params ReadOnlySpan<TValue> trimElements )
        where TValue : IEquatable<TValue>
    {
        switch ( trimElements.Length )
        {
            case > 1:
            {
                ReadOnlySpan<TValue> values = memory.Span;
                return memory.Slice( 0, values.ClampEnd( 0, trimElements ) );
            }

            case 1:  return memory.TrimEnd( trimElements[0] );
            default: return memory;
        }
    }

    /// <summary>
    /// Removes all leading and trailing occurrences of a set of elements specified
    /// in a readonly span from the span.
    /// </summary>
    /// <param name="span">The source span from which the elements are removed.</param>
    /// <param name="trimElements">The span which contains the set of elements to remove.</param>
    /// <remarks>If <paramref name="trimElements"/> is empty, the span is returned unaltered.</remarks>
    public static Span<TValue> Trim<TValue>( this scoped ref readonly Span<TValue> span, params ReadOnlySpan<TValue> trimElements )
        where TValue : IEquatable<TValue>
    {
        switch ( trimElements.Length )
        {
            case > 1:
            {
                ReadOnlySpan<TValue> values = span;
                int             start  = values.ClampStart( trimElements );
                int             length = values.ClampEnd( start, trimElements );
                return span.Slice( start, length );
            }

            case 1:  return span.Trim( trimElements[0] );
            default: return span;
        }
    }

    /// <summary>
    /// Removes all leading occurrences of a set of elements specified
    /// in a readonly span from the span.
    /// </summary>
    /// <param name="span">The source span from which the elements are removed.</param>
    /// <param name="trimElements">The span which contains the set of elements to remove.</param>
    /// <remarks>If <paramref name="trimElements"/> is empty, the span is returned unaltered.</remarks>
    public static Span<TValue> TrimStart<TValue>( this scoped ref readonly Span<TValue> span, params ReadOnlySpan<TValue> trimElements )
        where TValue : IEquatable<TValue>
    {
        switch ( trimElements.Length )
        {
            case > 1:
            {
                ReadOnlySpan<TValue> values = span;
                return span.Slice( values.ClampStart( trimElements ) );
            }

            case 1:  return span.TrimStart( trimElements[0] );
            default: return span;
        }
    }

    /// <summary>
    /// Removes all trailing occurrences of a set of elements specified
    /// in a readonly span from the span.
    /// </summary>
    /// <param name="span">The source span from which the elements are removed.</param>
    /// <param name="trimElements">The span which contains the set of elements to remove.</param>
    /// <remarks>If <paramref name="trimElements"/> is empty, the span is returned unaltered.</remarks>
    public static Span<TValue> TrimEnd<TValue>( this scoped ref readonly Span<TValue> span, params ReadOnlySpan<TValue> trimElements )
        where TValue : IEquatable<TValue>
    {
        switch ( trimElements.Length )
        {
            case > 1:
            {
                ReadOnlySpan<TValue> values = span;
                return span.Slice( 0, values.ClampEnd( 0, trimElements ) );
            }

            case 1:  return span.TrimEnd( trimElements[0] );
            default: return span;
        }
    }

    /// <summary>
    /// Removes all leading and trailing occurrences of a set of elements specified
    /// in a readonly span from the span.
    /// </summary>
    /// <param name="span">The source span from which the elements are removed.</param>
    /// <param name="trimElements">The span which contains the set of elements to remove.</param>
    /// <remarks>If <paramref name="trimElements"/> is empty, the span is returned unaltered.</remarks>
    public static ReadOnlySpan<TValue> Trim<TValue>( this scoped ref readonly ReadOnlySpan<TValue> span, params ReadOnlySpan<TValue> trimElements )
        where TValue : IEquatable<TValue>
    {
        switch ( trimElements.Length )
        {
            case > 1:
            {
                ReadOnlySpan<TValue> values = span;
                int             start  = values.ClampStart( trimElements );
                int             length = values.ClampEnd( start, trimElements );
                return span.Slice( start, length );
            }

            case 1:  return span.Trim( trimElements[0] );
            default: return span;
        }
    }

    /// <summary>
    /// Removes all leading occurrences of a set of elements specified
    /// in a readonly span from the span.
    /// </summary>
    /// <param name="span">The source span from which the elements are removed.</param>
    /// <param name="trimElements">The span which contains the set of elements to remove.</param>
    /// <remarks>If <paramref name="trimElements"/> is empty, the span is returned unaltered.</remarks>
    public static ReadOnlySpan<TValue> TrimStart<TValue>( this scoped ref readonly ReadOnlySpan<TValue> span, params ReadOnlySpan<TValue> trimElements )
        where TValue : IEquatable<TValue>
    {
        return trimElements.Length switch
               {
                   > 1 => span.Slice( span.ClampStart( trimElements ) ),
                   1   => span.TrimStart( trimElements[0] ),
                   _   => span
               };
    }

    /// <summary>
    /// Removes all trailing occurrences of a set of elements specified
    /// in a readonly span from the span.
    /// </summary>
    /// <param name="span">The source span from which the elements are removed.</param>
    /// <param name="trimElements">The span which contains the set of elements to remove.</param>
    /// <remarks>If <paramref name="trimElements"/> is empty, the span is returned unaltered.</remarks>
    public static ReadOnlySpan<TValue> TrimEnd<TValue>( this scoped ref readonly ReadOnlySpan<TValue> span, params ReadOnlySpan<TValue> trimElements )
        where TValue : IEquatable<TValue>
    {
        return trimElements.Length switch
               {
                   > 1 => span.Slice( 0, span.ClampEnd( 0, trimElements ) ),
                   1   => span.TrimStart( trimElements[0] ),
                   _   => span
               };
    }

    /// <summary>
    /// Delimits all leading occurrences of a set of elements specified
    /// in a readonly span from the span.
    /// </summary>
    /// <param name="span">The source span from which the elements are removed.</param>
    /// <param name="trimElements">The span which contains the set of elements to remove.</param>
    public static int ClampStart<TValue>( this scoped ref readonly ReadOnlySpan<TValue> span, params ReadOnlySpan<TValue> trimElements )
        where TValue : IEquatable<TValue>
    {
        int start = 0;

        for ( ; start < span.Length; start++ )
        {
            if ( trimElements.Contains( span[start] ) is false ) { break; }
        }

        return start;
    }

    /// <summary>
    /// Delimits all trailing occurrences of a set of elements specified
    /// in a readonly span from the span.
    /// </summary>
    /// <param name="span">The source span from which the elements are removed.</param>
    /// <param name="start">The start index from which to being searching.</param>
    /// <param name="trimElements">The span which contains the set of elements to remove.</param>
    public static int ClampEnd<TValue>( this scoped ref readonly ReadOnlySpan<TValue> span, int start, params ReadOnlySpan<TValue> trimElements )
        where TValue : IEquatable<TValue>
    {
        // Initially, start==len==0. If ClampStart trims all, start==len
        Debug.Assert( (uint)start <= span.Length );

        int end = span.Length - 1;

        for ( ; end >= start; end-- )
        {
            if ( !trimElements.Contains( span[end] ) ) { break; }
        }

        return end - start + 1;
    }

    /// <summary>
    /// Removes all leading and trailing white-space characters from the memory.
    /// </summary>
    /// <param name="memory">The source memory from which the characters are removed.</param>
    public static Memory<char> Trim( this scoped ref readonly Memory<char> memory )
    {
        ReadOnlySpan<char> values = memory.Span;
        int                start  = values.ClampStart();
        int                length = values.ClampEnd( start );
        return memory.Slice( start, length );
    }

    /// <summary>
    /// Removes all leading white-space characters from the memory.
    /// </summary>
    /// <param name="memory">The source memory from which the characters are removed.</param>
    public static Memory<char> TrimStart( this scoped ref readonly Memory<char> memory )
    {
        ReadOnlySpan<char> values = memory.Span;
        return memory.Slice( values.ClampStart() );
    }

    /// <summary>
    /// Removes all trailing white-space characters from the memory.
    /// </summary>
    /// <param name="memory">The source memory from which the characters are removed.</param>
    public static Memory<char> TrimEnd( this scoped ref readonly Memory<char> memory )
    {
        ReadOnlySpan<char> values = memory.Span;
        return memory.Slice( 0, values.ClampEnd( 0 ) );
    }

    /// <summary>
    /// Removes all leading and trailing white-space characters from the memory.
    /// </summary>
    /// <param name="memory">The source memory from which the characters are removed.</param>
    public static ReadOnlyMemory<char> Trim( this scoped ref readonly ReadOnlyMemory<char> memory )
    {
        ReadOnlySpan<char> values = memory.Span;
        int                start  = values.ClampStart();
        int                length = values.ClampEnd( start );
        return memory.Slice( start, length );
    }

    /// <summary>
    /// Removes all leading white-space characters from the memory.
    /// </summary>
    /// <param name="memory">The source memory from which the characters are removed.</param>
    public static ReadOnlyMemory<char> TrimStart( this scoped ref readonly ReadOnlyMemory<char> memory )
    {
        ReadOnlySpan<char> values = memory.Span;
        return memory.Slice( values.ClampStart() );
    }

    /// <summary>
    /// Removes all trailing white-space characters from the memory.
    /// </summary>
    /// <param name="memory">The source memory from which the characters are removed.</param>
    public static ReadOnlyMemory<char> TrimEnd( this scoped ref readonly ReadOnlyMemory<char> memory )
    {
        ReadOnlySpan<char> values = memory.Span;
        return memory.Slice( 0, values.ClampEnd( 0 ) );
    }

    /// <summary>
    /// Removes all leading and trailing white-space characters from the span.
    /// </summary>
    /// <param name="span">The source span from which the characters are removed.</param>
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static ReadOnlySpan<char> Trim( this scoped ref readonly ReadOnlySpan<char> span )
    {
        // Assume that in most cases input doesn't need trimming
        if ( span.Length == 0 || char.IsWhiteSpace( span[0] ) is false && char.IsWhiteSpace( span[^1] ) is false ) { return span; }

        return TrimFallback( span );

        [MethodImpl( MethodImplOptions.NoInlining )]
        static ReadOnlySpan<char> TrimFallback( ReadOnlySpan<char> span )
        {
            int start = 0;

            for ( ; start < span.Length; start++ )
            {
                if ( !char.IsWhiteSpace( span[start] ) ) { break; }
            }

            int end = span.Length - 1;

            for ( ; end > start; end-- )
            {
                if ( !char.IsWhiteSpace( span[end] ) ) { break; }
            }

            return span.Slice( start, end - start + 1 );
        }
    }

    /// <summary>
    /// Removes all leading white-space characters from the span.
    /// </summary>
    /// <param name="span">The source span from which the characters are removed.</param>
    public static ReadOnlySpan<char> TrimStart( this scoped ref readonly ReadOnlySpan<char> span )
    {
        int start = 0;

        for ( ; start < span.Length; start++ )
        {
            if ( char.IsWhiteSpace( span[start] ) is false ) { break; }
        }

        return span.Slice( start );
    }

    /// <summary>
    /// Removes all trailing white-space characters from the span.
    /// </summary>
    /// <param name="span">The source span from which the characters are removed.</param>
    public static ReadOnlySpan<char> TrimEnd( this scoped ref readonly ReadOnlySpan<char> span )
    {
        int end = span.Length - 1;

        for ( ; end >= 0; end-- )
        {
            if ( char.IsWhiteSpace( span[end] ) is false ) { break; }
        }

        return span.Slice( 0, end + 1 );
    }

    /// <summary>
    /// Removes all leading and trailing occurrences of a specified character from the span.
    /// </summary>
    /// <param name="span">The source span from which the character is removed.</param>
    /// <param name="trimChar">The specified character to look for and remove.</param>
    public static ReadOnlySpan<char> Trim( this scoped ref readonly ReadOnlySpan<char> span, char trimChar )
    {
        int start = 0;

        for ( ; start < span.Length; start++ )
        {
            if ( span[start] != trimChar ) { break; }
        }

        int end = span.Length - 1;

        for ( ; end > start; end-- )
        {
            if ( span[end] != trimChar ) { break; }
        }

        return span.Slice( start, end - start + 1 );
    }

    /// <summary>
    /// Removes all leading occurrences of a specified character from the span.
    /// </summary>
    /// <param name="span">The source span from which the character is removed.</param>
    /// <param name="trimChar">The specified character to look for and remove.</param>
    public static ReadOnlySpan<char> TrimStart( this scoped ref readonly ReadOnlySpan<char> span, char trimChar )
    {
        int start = 0;

        for ( ; start < span.Length; start++ )
        {
            if ( span[start] != trimChar ) { break; }
        }

        return span.Slice( start );
    }

    /// <summary>
    /// Removes all trailing occurrences of a specified character from the span.
    /// </summary>
    /// <param name="span">The source span from which the character is removed.</param>
    /// <param name="trimChar">The specified character to look for and remove.</param>
    public static ReadOnlySpan<char> TrimEnd( this scoped ref readonly ReadOnlySpan<char> span, char trimChar )
    {
        int end = span.Length - 1;

        for ( ; end >= 0; end-- )
        {
            if ( span[end] != trimChar ) { break; }
        }

        return span.Slice( 0, end + 1 );
    }

    /// <summary>
    /// Removes all leading and trailing occurrences of a set of characters specified
    /// in a readonly span from the span.
    /// </summary>
    /// <param name="span">The source span from which the characters are removed.</param>
    /// <param name="trimChars">The span which contains the set of characters to remove.</param>
    /// <remarks>If <paramref name="trimChars"/> is empty, white-space characters are removed instead.</remarks>
    public static ReadOnlySpan<char> Trim( this scoped ref readonly ReadOnlySpan<char> span, params ReadOnlySpan<char> trimChars )
    {
        ReadOnlySpan<char> values = span.TrimStart( trimChars );
        return values.TrimEnd( trimChars );
    }

    /// <summary>
    /// Removes all leading occurrences of a set of characters specified
    /// in a readonly span from the span.
    /// </summary>
    /// <param name="span">The source span from which the characters are removed.</param>
    /// <param name="trimChars">The span which contains the set of characters to remove.</param>
    /// <remarks>If <paramref name="trimChars"/> is empty, white-space characters are removed instead.</remarks>
    public static ReadOnlySpan<char> TrimStart( this scoped ref readonly ReadOnlySpan<char> span, params ReadOnlySpan<char> trimChars )
    {
        if ( trimChars.IsEmpty ) { return span.TrimStart(); }

        int start = 0;

        for ( ; start < span.Length; start++ )
        {
            if ( trimChars.Contains( span[start] ) is false ) { break; }

            /*
            foreach ( char c in trimChars )
            {
                if ( span[start] == c ) { goto Next; }
            }

            break;
            Next: ;
            */
        }

        return span.Slice( start );
    }

    /// <summary>
    /// Removes all trailing occurrences of a set of characters specified
    /// in a readonly span from the span.
    /// </summary>
    /// <param name="span">The source span from which the characters are removed.</param>
    /// <param name="trimChars">The span which contains the set of characters to remove.</param>
    /// <remarks>If <paramref name="trimChars"/> is empty, white-space characters are removed instead.</remarks>
    public static ReadOnlySpan<char> TrimEnd( this scoped ref readonly ReadOnlySpan<char> span, params ReadOnlySpan<char> trimChars )
    {
        if ( trimChars.IsEmpty ) { return span.TrimEnd(); }

        int end = span.Length - 1;

        for ( ; end >= 0; end-- )
        {
            if ( trimChars.Contains( span[span[end]] ) is false ) { break; }

            /*
            for ( int i = 0; i < trimChars.Length; i++ )
            {
                if ( span[end] == trimChars[i] ) { goto Next; }
            }

            break;
            Next: ;
            */
        }

        return span.Slice( 0, end + 1 );
    }

    /// <summary>
    /// Removes all leading and trailing white-space characters from the span.
    /// </summary>
    /// <param name="span">The source span from which the characters are removed.</param>
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static Span<char> Trim( this scoped ref readonly Span<char> span )
    {
        // Assume that in most cases input doesn't need trimming
        if ( span.Length == 0 || char.IsWhiteSpace( span[0] ) is false && char.IsWhiteSpace( span[^1] ) is false ) { return span; }

        return TrimFallback( span );

        [MethodImpl( MethodImplOptions.NoInlining )]
        static Span<char> TrimFallback( Span<char> span )
        {
            int start = 0;

            for ( ; start < span.Length; start++ )
            {
                if ( char.IsWhiteSpace( span[start] ) is false ) { break; }
            }

            int end = span.Length - 1;

            for ( ; end > start; end-- )
            {
                if ( char.IsWhiteSpace( span[end] ) is false ) { break; }
            }

            return span.Slice( start, end - start + 1 );
        }
    }

    /// <summary>
    /// Removes all leading white-space characters from the span.
    /// </summary>
    /// <param name="span">The source span from which the characters are removed.</param>
    public static Span<char> TrimStart( this scoped ref readonly Span<char> span )
    {
        ReadOnlySpan<char> values = span;
        return span.Slice( values.ClampStart() );
    }

    /// <summary>
    /// Removes all trailing white-space characters from the span.
    /// </summary>
    /// <param name="span">The source span from which the characters are removed.</param>
    public static Span<char> TrimEnd( this scoped ref readonly Span<char> span )
    {
        ReadOnlySpan<char> values = span;
        return span.Slice( 0, values.ClampEnd( 0 ) );
    }

    /// <summary>
    /// Delimits all leading occurrences of whitespace characters from the span.
    /// </summary>
    /// <param name="span">The source span from which the characters are removed.</param>
    public static int ClampStart( this scoped ref readonly ReadOnlySpan<char> span )
    {
        int start = 0;

        for ( ; start < span.Length; start++ )
        {
            if ( char.IsWhiteSpace( span[start] ) is false ) { break; }
        }

        return start;
    }

    /// <summary>
    /// Delimits all trailing occurrences of whitespace characters from the span.
    /// </summary>
    /// <param name="span">The source span from which the characters are removed.</param>
    /// <param name="start">The start index from which to being searching.</param>
    public static int ClampEnd( this scoped ref readonly ReadOnlySpan<char> span, int start )
    {
        // Initially, start==len==0. If ClampStart trims all, start==len
        Debug.Assert( (uint)start <= span.Length );

        int end = span.Length - 1;

        for ( ; end >= start; end-- )
        {
            if ( char.IsWhiteSpace( span[end] ) is false ) { break; }
        }

        return end - start + 1;
    }
}
