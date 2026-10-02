namespace Jakar.Extensions;


public static partial class Spans
{
    /// <param name="memory"> The source memory from which the element is removed. </param>
    extension<TValue>( scoped ref readonly Memory<TValue> memory )
        where TValue : IEquatable<TValue>
    {
        /// <summary> Removes all leading and trailing occurrences of a specified element from the memory. </summary>
        /// <param name="trimElement"> The specified element to look for and remove. </param>
        public Memory<TValue> Trim( TValue trimElement )
        {
            ReadOnlySpan<TValue> span   = memory.Span;
            int                  start  = span.ClampStart(trimElement);
            int                  length = span.ClampEnd(start, trimElement);
            return memory.Slice(start, length);
        }
        /// <summary> Removes all leading occurrences of a specified element from the memory. </summary>
        /// <param name="trimElement"> The specified element to look for and remove. </param>
        public Memory<TValue> TrimStart( TValue trimElement )
        {
            ReadOnlySpan<TValue> span = memory.Span;
            return memory[span.ClampStart(trimElement)..];
        }
        /// <summary> Removes all trailing occurrences of a specified element from the memory. </summary>
        /// <param name="trimElement"> The specified element to look for and remove. </param>
        public Memory<TValue> TrimEnd( TValue trimElement )
        {
            ReadOnlySpan<TValue> span = memory.Span;
            return memory[..span.ClampEnd(0, trimElement)];
        }
    }



    /// <param name="memory"> The source memory from which the element is removed. </param>
    extension<TValue>( scoped ref readonly ReadOnlyMemory<TValue> memory )
        where TValue : IEquatable<TValue>
    {
        /// <summary> Removes all leading and trailing occurrences of a specified element from the memory. </summary>
        /// <param name="trimElement"> The specified element to look for and remove. </param>
        public ReadOnlyMemory<TValue> Trim( TValue trimElement )
        {
            ReadOnlySpan<TValue> span   = memory.Span;
            int                  start  = span.ClampStart(trimElement);
            int                  length = span.ClampEnd(start, trimElement);
            return memory.Slice(start, length);
        }
        /// <summary> Removes all leading occurrences of a specified element from the memory. </summary>
        /// <param name="trimElement"> The specified element to look for and remove. </param>
        public ReadOnlyMemory<TValue> TrimStart( TValue trimElement )
        {
            ReadOnlySpan<TValue> span = memory.Span;
            return memory[span.ClampStart(trimElement)..];
        }
        /// <summary> Removes all trailing occurrences of a specified element from the memory. </summary>
        /// <param name="trimElement"> The specified element to look for and remove. </param>
        public ReadOnlyMemory<TValue> TrimEnd( TValue trimElement )
        {
            ReadOnlySpan<TValue> span = memory.Span;
            return memory[..span.ClampEnd(0, trimElement)];
        }
    }



    /// <param name="span"> The source span from which the element is removed. </param>
    extension<TValue>( scoped ref readonly Span<TValue> span )
        where TValue : IEquatable<TValue>
    {
        /// <summary> Removes all leading and trailing occurrences of a specified element from the span. </summary>
        /// <param name="trimElement"> The specified element to look for and remove. </param>
        public Span<TValue> Trim( TValue trimElement )
        {
            ReadOnlySpan<TValue> values = span;
            int                  start  = values.ClampStart(trimElement);
            int                  length = values.ClampEnd(start, trimElement);
            return span.Slice(start, length);
        }
        /// <summary> Removes all leading occurrences of a specified element from the span. </summary>
        /// <param name="trimElement"> The specified element to look for and remove. </param>
        public Span<TValue> TrimStart( TValue trimElement )
        {
            ReadOnlySpan<TValue> temp = span;
            return span[temp.ClampStart(trimElement)..];
        }
        /// <summary> Removes all trailing occurrences of a specified element from the span. </summary>
        /// <param name="trimElement"> The specified element to look for and remove. </param>
        public Span<TValue> TrimEnd( TValue trimElement )
        {
            ReadOnlySpan<TValue> values = span;
            return span[..values.ClampEnd(0, trimElement)];
        }
    }



    /// <param name="span"> The source span from which the element is removed. </param>
    extension<TValue>( scoped ref readonly ReadOnlySpan<TValue> span )
        where TValue : IEquatable<TValue>
    {
        /// <summary> Removes all leading and trailing occurrences of a specified element from the span. </summary>
        /// <param name="trimElement"> The specified element to look for and remove. </param>
        public ReadOnlySpan<TValue> Trim( TValue trimElement )
        {
            int start  = span.ClampStart(trimElement);
            int length = span.ClampEnd(start, trimElement);
            return span.Slice(start, length);
        }
        /// <summary> Removes all leading occurrences of a specified element from the span. </summary>
        /// <param name="trimElement"> The specified element to look for and remove. </param>
        public ReadOnlySpan<TValue> TrimStart( TValue trimElement ) =>
            span[span.ClampStart(trimElement)..];
        /// <summary> Removes all trailing occurrences of a specified element from the span. </summary>
        /// <param name="trimElement"> The specified element to look for and remove. </param>
        public ReadOnlySpan<TValue> TrimEnd( TValue trimElement )
        {
            ReadOnlySpan<TValue> values = span;
            return span[..values.ClampEnd(0, trimElement)];
        }
        /// <summary> Delimits all leading occurrences of a specified element from the span. </summary>
        /// <param name="trimElement"> The specified element to look for and remove. </param>
        public int ClampStart( TValue trimElement )
        {
            int start = 0;

            if ( trimElement != null )
            {
                for ( ; start < span.Length; start++ )
                {
                    if ( !trimElement.Equals(span[start]) ) { break; }
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
        /// <summary> Delimits all trailing occurrences of a specified element from the span. </summary>
        /// <param name="start"> The start index from which to being searching. </param>
        /// <param name="trimElement"> The specified element to look for and remove. </param>
        public int ClampEnd( int start, TValue trimElement )
        {
            // Initially, start==len==0. If ClampStart trims all, start==len
            Debug.Assert((uint)start <= span.Length);

            int end = span.Length - 1;

            if ( trimElement != null )
            {
                for ( ; end >= start; end-- )
                {
                    if ( !trimElement.Equals(span[end]) ) { break; }
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
    }



    /// <param name="memory"> The source memory from which the elements are removed. </param>
    extension<TValue>( scoped ref readonly Memory<TValue> memory )
        where TValue : IEquatable<TValue>
    {
        /// <summary> Removes all leading and trailing occurrences of a set of elements specified in a readonly span from the memory. </summary>
        /// <param name="trimElements"> The span which contains the set of elements to remove. </param>
        /// <remarks> If <paramref name="trimElements"/> is empty, the memory is returned unaltered. </remarks>
        public Memory<TValue> Trim( ReadOnlySpan<TValue> trimElements )
        {
            switch ( trimElements.Length )
            {
                case > 1:
                {
                    ReadOnlySpan<TValue> span   = memory.Span;
                    int                  start  = span.ClampStart(trimElements);
                    int                  length = span.ClampEnd(start, trimElements);
                    return memory.Slice(start, length);
                }

                case 1:
                    return memory.Trim(trimElements[0]);

                default:
                    return memory;
            }
        }
        /// <summary> Removes all leading occurrences of a set of elements specified in a readonly span from the memory. </summary>
        /// <param name="trimElements"> The span which contains the set of elements to remove. </param>
        /// <remarks> If <paramref name="trimElements"/> is empty, the memory is returned unaltered. </remarks>
        public Memory<TValue> TrimStart( params ReadOnlySpan<TValue> trimElements )
        {
            switch ( trimElements.Length )
            {
                case > 1:
                {
                    ReadOnlySpan<TValue> values = memory.Span;
                    return memory[values.ClampStart(trimElements)..];
                }

                case 1:
                    return memory.TrimStart(trimElements[0]);

                default:
                    return memory;
            }
        }
        /// <summary> Removes all trailing occurrences of a set of elements specified in a readonly span from the memory. </summary>
        /// <param name="trimElements"> The span which contains the set of elements to remove. </param>
        /// <remarks> If <paramref name="trimElements"/> is empty, the memory is returned unaltered. </remarks>
        public Memory<TValue> TrimEnd( params ReadOnlySpan<TValue> trimElements )
        {
            switch ( trimElements.Length )
            {
                case > 1:
                {
                    ReadOnlySpan<TValue> values = memory.Span;
                    return memory[..values.ClampEnd(0, trimElements)];
                }

                case 1:
                    return memory.TrimEnd(trimElements[0]);

                default:
                    return memory;
            }
        }
    }



    /// <param name="memory"> The source memory from which the elements are removed. </param>
    extension<TValue>( scoped ref readonly ReadOnlyMemory<TValue> memory )
        where TValue : IEquatable<TValue>
    {
        /// <summary> Removes all leading and trailing occurrences of a set of elements specified in a readonly span from the memory. </summary>
        /// <param name="trimElements"> The span which contains the set of elements to remove. </param>
        /// <remarks> If <paramref name="trimElements"/> is empty, the memory is returned unaltered. </remarks>
        public ReadOnlyMemory<TValue> Trim( params ReadOnlySpan<TValue> trimElements )
        {
            switch ( trimElements.Length )
            {
                case > 1:
                {
                    ReadOnlySpan<TValue> span   = memory.Span;
                    int                  start  = span.ClampStart(trimElements);
                    int                  length = span.ClampEnd(start, trimElements);
                    return memory.Slice(start, length);
                }

                case 1:
                    return memory.Trim(trimElements[0]);

                default:
                    return memory;
            }
        }
        /// <summary> Removes all leading occurrences of a set of elements specified in a readonly span from the memory. </summary>
        /// <param name="trimElements"> The span which contains the set of elements to remove. </param>
        /// <remarks> If <paramref name="trimElements"/> is empty, the memory is returned unaltered. </remarks>
        public ReadOnlyMemory<TValue> TrimStart( params ReadOnlySpan<TValue> trimElements )
        {
            switch ( trimElements.Length )
            {
                case > 1:
                {
                    ReadOnlySpan<TValue> values = memory.Span;
                    return memory[values.ClampStart(trimElements)..];
                }

                case 1:
                    return memory.TrimStart(trimElements[0]);

                default:
                    return memory;
            }
        }
        /// <summary> Removes all trailing occurrences of a set of elements specified in a readonly span from the memory. </summary>
        /// <param name="trimElements"> The span which contains the set of elements to remove. </param>
        /// <remarks> If <paramref name="trimElements"/> is empty, the memory is returned unaltered. </remarks>
        public ReadOnlyMemory<TValue> TrimEnd( params ReadOnlySpan<TValue> trimElements )
        {
            switch ( trimElements.Length )
            {
                case > 1:
                {
                    ReadOnlySpan<TValue> values = memory.Span;
                    return memory[..values.ClampEnd(0, trimElements)];
                }

                case 1:
                    return memory.TrimEnd(trimElements[0]);

                default:
                    return memory;
            }
        }
    }



    /// <param name="span"> The source span from which the elements are removed. </param>
    extension<TValue>( scoped ref readonly Span<TValue> span )
        where TValue : IEquatable<TValue>
    {
        /// <summary> Removes all leading and trailing occurrences of a set of elements specified in a readonly span from the span. </summary>
        /// <param name="trimElements"> The span which contains the set of elements to remove. </param>
        /// <remarks> If <paramref name="trimElements"/> is empty, the span is returned unaltered. </remarks>
        public Span<TValue> Trim( params ReadOnlySpan<TValue> trimElements )
        {
            switch ( trimElements.Length )
            {
                case > 1:
                {
                    ReadOnlySpan<TValue> values = span;
                    int                  start  = values.ClampStart(trimElements);
                    int                  length = values.ClampEnd(start, trimElements);
                    return span.Slice(start, length);
                }

                case 1:
                    return span.Trim(trimElements[0]);

                default:
                    return span;
            }
        }
        /// <summary> Removes all leading occurrences of a set of elements specified in a readonly span from the span. </summary>
        /// <param name="trimElements"> The span which contains the set of elements to remove. </param>
        /// <remarks> If <paramref name="trimElements"/> is empty, the span is returned unaltered. </remarks>
        public Span<TValue> TrimStart( params ReadOnlySpan<TValue> trimElements )
        {
            switch ( trimElements.Length )
            {
                case > 1:
                {
                    ReadOnlySpan<TValue> values = span;
                    return span[values.ClampStart(trimElements)..];
                }

                case 1:
                    return span.TrimStart(trimElements[0]);

                default:
                    return span;
            }
        }
        /// <summary> Removes all trailing occurrences of a set of elements specified in a readonly span from the span. </summary>
        /// <param name="trimElements"> The span which contains the set of elements to remove. </param>
        /// <remarks> If <paramref name="trimElements"/> is empty, the span is returned unaltered. </remarks>
        public Span<TValue> TrimEnd( params ReadOnlySpan<TValue> trimElements )
        {
            switch ( trimElements.Length )
            {
                case > 1:
                {
                    ReadOnlySpan<TValue> values = span;
                    return span[..values.ClampEnd(0, trimElements)];
                }

                case 1:
                    return span.TrimEnd(trimElements[0]);

                default:
                    return span;
            }
        }
    }



    /// <param name="span"> The source span from which the elements are removed. </param>
    extension<TValue>( scoped ref readonly ReadOnlySpan<TValue> span )
        where TValue : IEquatable<TValue>
    {
        /// <summary> Removes all leading and trailing occurrences of a set of elements specified in a readonly span from the span. </summary>
        /// <param name="trimElements"> The span which contains the set of elements to remove. </param>
        /// <remarks> If <paramref name="trimElements"/> is empty, the span is returned unaltered. </remarks>
        public ReadOnlySpan<TValue> Trim( params ReadOnlySpan<TValue> trimElements )
        {
            switch ( trimElements.Length )
            {
                case > 1:
                {
                    ReadOnlySpan<TValue> values = span;
                    int                  start  = values.ClampStart(trimElements);
                    int                  length = values.ClampEnd(start, trimElements);
                    return span.Slice(start, length);
                }

                case 1:
                    return span.Trim(trimElements[0]);

                default:
                    return span;
            }
        }
        /// <summary> Removes all leading occurrences of a set of elements specified in a readonly span from the span. </summary>
        /// <param name="trimElements"> The span which contains the set of elements to remove. </param>
        /// <remarks> If <paramref name="trimElements"/> is empty, the span is returned unaltered. </remarks>
        public ReadOnlySpan<TValue> TrimStart( params ReadOnlySpan<TValue> trimElements ) =>
            trimElements.Length switch
            {
                > 1 => span[span.ClampStart(trimElements)..],
                1   => span.TrimStart(trimElements[0]),
                _   => span
            };
        /// <summary> Removes all trailing occurrences of a set of elements specified in a readonly span from the span. </summary>
        /// <param name="trimElements"> The span which contains the set of elements to remove. </param>
        /// <remarks> If <paramref name="trimElements"/> is empty, the span is returned unaltered. </remarks>
        public ReadOnlySpan<TValue> TrimEnd( params ReadOnlySpan<TValue> trimElements ) =>
            trimElements.Length switch
            {
                > 1 => span[..span.ClampEnd(0, trimElements)],
                1   => span.TrimStart(trimElements[0]),
                _   => span
            };
        /// <summary> Delimits all leading occurrences of a set of elements specified in a readonly span from the span. </summary>
        /// <param name="trimElements"> The span which contains the set of elements to remove. </param>
        public int ClampStart( params ReadOnlySpan<TValue> trimElements )
        {
            int start = 0;

            for ( ; start < span.Length; start++ )
            {
                if ( !trimElements.Contains(span[start]) ) { break; }
            }

            return start;
        }
        /// <summary> Delimits all trailing occurrences of a set of elements specified in a readonly span from the span. </summary>
        /// <param name="start"> The start index from which to being searching. </param>
        /// <param name="trimElements"> The span which contains the set of elements to remove. </param>
        public int ClampEnd( int start, params ReadOnlySpan<TValue> trimElements )
        {
            // Initially, start==len==0. If ClampStart trims all, start==len
            Debug.Assert((uint)start <= span.Length);

            int end = span.Length - 1;

            for ( ; end >= start; end-- )
            {
                if ( !trimElements.Contains(span[end]) ) { break; }
            }

            return end - start + 1;
        }
    }



    /// <param name="memory"> The source memory from which the characters are removed. </param>
    extension( scoped ref readonly Memory<char> memory )
    {
        /// <summary> Removes all leading and trailing white-space characters from the memory. </summary>
        public Memory<char> Trim()
        {
            ReadOnlySpan<char> values = memory.Span;
            int                start  = values.ClampStart();
            int                length = values.ClampEnd(start);
            return memory.Slice(start, length);
        }
        /// <summary> Removes all leading white-space characters from the memory. </summary>
        public Memory<char> TrimStart()
        {
            ReadOnlySpan<char> values = memory.Span;
            return memory[values.ClampStart()..];
        }
        /// <summary> Removes all trailing white-space characters from the memory. </summary>
        public Memory<char> TrimEnd()
        {
            ReadOnlySpan<char> values = memory.Span;
            return memory[..values.ClampEnd(0)];
        }
    }



    /// <param name="memory"> The source memory from which the characters are removed. </param>
    extension( scoped ref readonly ReadOnlyMemory<char> memory )
    {
        /// <summary> Removes all leading and trailing white-space characters from the memory. </summary>
        public ReadOnlyMemory<char> Trim()
        {
            ReadOnlySpan<char> values = memory.Span;
            int                start  = values.ClampStart();
            int                length = values.ClampEnd(start);
            return memory.Slice(start, length);
        }
        /// <summary> Removes all leading white-space characters from the memory. </summary>
        public ReadOnlyMemory<char> TrimStart()
        {
            ReadOnlySpan<char> values = memory.Span;
            return memory[values.ClampStart()..];
        }
        /// <summary> Removes all trailing white-space characters from the memory. </summary>
        public ReadOnlyMemory<char> TrimEnd()
        {
            ReadOnlySpan<char> values = memory.Span;
            return memory[..values.ClampEnd(0)];
        }
    }



    /// <param name="span"> The source span from which the characters are removed. </param>
    extension( scoped ref readonly ReadOnlySpan<char> span )
    {
        /// <summary> Removes all leading and trailing white-space characters from the span. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ReadOnlySpan<char> Trim()
        {
            // Assume that in most cases input doesn't need trimming
            if ( span.Length == 0 || ( !char.IsWhiteSpace(span[0]) && !char.IsWhiteSpace(span[^1]) ) ) { return span; }

            return trimFallback(in span);

            [MethodImpl(MethodImplOptions.NoInlining)]
            static ReadOnlySpan<char> trimFallback( scoped ref readonly ReadOnlySpan<char> span )
            {
                int start = 0;

                for ( ; start < span.Length; start++ )
                {
                    if ( !char.IsWhiteSpace(span[start]) ) { break; }
                }

                int end = span.Length - 1;

                for ( ; end > start; end-- )
                {
                    if ( !char.IsWhiteSpace(span[end]) ) { break; }
                }

                return span.Slice(start, end - start + 1);
            }
        }
        /// <summary> Removes all leading white-space characters from the span. </summary>
        public ReadOnlySpan<char> TrimStart()
        {
            int start = 0;

            for ( ; start < span.Length; start++ )
            {
                if ( !char.IsWhiteSpace(span[start]) ) { break; }
            }

            return span[start..];
        }
        /// <summary> Removes all trailing white-space characters from the span. </summary>
        public ReadOnlySpan<char> TrimEnd()
        {
            int end = span.Length - 1;

            for ( ; end >= 0; end-- )
            {
                if ( !char.IsWhiteSpace(span[end]) ) { break; }
            }

            return span[..( end + 1 )];
        }
        /// <summary> Removes all leading and trailing occurrences of a specified character from the span. </summary>
        /// <param name="trimChar"> The specified character to look for and remove. </param>
        public ReadOnlySpan<char> Trim( char trimChar )
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

            return span.Slice(start, end - start + 1);
        }
        /// <summary> Removes all leading occurrences of a specified character from the span. </summary>
        /// <param name="trimChar"> The specified character to look for and remove. </param>
        public ReadOnlySpan<char> TrimStart( char trimChar )
        {
            int start = 0;

            for ( ; start < span.Length; start++ )
            {
                if ( span[start] != trimChar ) { break; }
            }

            return span[start..];
        }
        /// <summary> Removes all trailing occurrences of a specified character from the span. </summary>
        /// <param name="trimChar"> The specified character to look for and remove. </param>
        public ReadOnlySpan<char> TrimEnd( char trimChar )
        {
            int end = span.Length - 1;

            for ( ; end >= 0; end-- )
            {
                if ( span[end] != trimChar ) { break; }
            }

            return span[..( end + 1 )];
        }
        /// <summary> Removes all leading and trailing occurrences of a set of characters specified in a readonly span from the span. </summary>
        /// <param name="trimChars"> The span which contains the set of characters to remove. </param>
        /// <remarks> If <paramref name="trimChars"/> is empty, white-space characters are removed instead. </remarks>
        public ReadOnlySpan<char> Trim( params ReadOnlySpan<char> trimChars )
        {
            ReadOnlySpan<char> values = span.TrimStart(trimChars);
            return values.TrimEnd(trimChars);
        }
        /// <summary> Removes all leading occurrences of a set of characters specified in a readonly span from the span. </summary>
        /// <param name="trimChars"> The span which contains the set of characters to remove. </param>
        /// <remarks> If <paramref name="trimChars"/> is empty, white-space characters are removed instead. </remarks>
        public ReadOnlySpan<char> TrimStart( params ReadOnlySpan<char> trimChars )
        {
            if ( trimChars.IsEmpty ) { return span.TrimStart(); }

            int start = 0;

            for ( ; start < span.Length; start++ )
            {
                if ( !trimChars.Contains(span[start]) ) { break; }

                /*
            foreach ( char c in trimChars )
            {
                if ( span[start] == c ) { goto Next; }
            }

            break;
            Next: ;
            */
            }

            return span[start..];
        }
        /// <summary> Removes all trailing occurrences of a set of characters specified in a readonly span from the span. </summary>
        /// <param name="trimChars"> The span which contains the set of characters to remove. </param>
        /// <remarks> If <paramref name="trimChars"/> is empty, white-space characters are removed instead. </remarks>
        public ReadOnlySpan<char> TrimEnd( params ReadOnlySpan<char> trimChars )
        {
            if ( trimChars.IsEmpty ) { return span.TrimEnd(); }

            int end = span.Length - 1;

            for ( ; end >= 0; end-- )
            {
                if ( !trimChars.Contains(span[span[end]]) ) { break; }

                /*
            for ( int i = 0; i < trimChars.Length; i++ )
            {
                if ( span[end] == trimChars[i] ) { goto Next; }
            }

            break;
            Next: ;
            */
            }

            return span[..( end + 1 )];
        }
    }



    /// <param name="span"> The source span from which the characters are removed. </param>
    extension( scoped ref readonly Span<char> span )
    {
        /// <summary> Removes all leading and trailing white-space characters from the span. </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public Span<char> Trim()
        {
            // Assume that in most cases input doesn't need trimming
            if ( span.Length == 0 || ( !char.IsWhiteSpace(span[0]) && !char.IsWhiteSpace(span[^1]) ) ) { return span; }

            return trimFallback(span);

            [MethodImpl(MethodImplOptions.NoInlining)]
            static Span<char> trimFallback( Span<char> span )
            {
                int start = 0;

                for ( ; start < span.Length; start++ )
                {
                    if ( !char.IsWhiteSpace(span[start]) ) { break; }
                }

                int end = span.Length - 1;

                for ( ; end > start; end-- )
                {
                    if ( !char.IsWhiteSpace(span[end]) ) { break; }
                }

                return span.Slice(start, end - start + 1);
            }
        }
        /// <summary> Removes all leading white-space characters from the span. </summary>
        public Span<char> TrimStart()
        {
            ReadOnlySpan<char> values = span;
            return span[values.ClampStart()..];
        }
        /// <summary> Removes all trailing white-space characters from the span. </summary>
        public Span<char> TrimEnd()
        {
            ReadOnlySpan<char> values = span;
            return span[..values.ClampEnd(0)];
        }
    }



    /// <param name="span"> The source span from which the characters are removed. </param>
    extension( scoped ref readonly ReadOnlySpan<char> span )
    {
        /// <summary> Delimits all leading occurrences of whitespace characters from the span. </summary>
        public int ClampStart()
        {
            int start = 0;

            for ( ; start < span.Length; start++ )
            {
                if ( !char.IsWhiteSpace(span[start]) ) { break; }
            }

            return start;
        }
        /// <summary> Delimits all trailing occurrences of whitespace characters from the span. </summary>
        /// <param name="start"> The start index from which to being searching. </param>
        public int ClampEnd( int start )
        {
            // Initially, start==len==0. If ClampStart trims all, start==len
            Debug.Assert((uint)start <= span.Length);

            int end = span.Length - 1;

            for ( ; end >= start; end-- )
            {
                if ( !char.IsWhiteSpace(span[end]) ) { break; }
            }

            return end - start + 1;
        }
    }
}
