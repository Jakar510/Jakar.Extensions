// Jakar.Extensions :: Jakar.Extensions
// 08/28/2023  9:54 PM

namespace Jakar.Extensions;


public readonly ref struct SpanLinq<T>( ReadOnlySpan<T> span )
{
    private readonly ReadOnlySpan<T> _span = span;


    [ Pure ]
    public ReadOnlySpan<T> Where( Func<T, bool> selector )
    {
        if ( _span.Length == 0 ) { return default; }

        T[]     buffer = AsyncLinq.GetArray<T>( _span.Length );
        Span<T> span   = buffer;
        Spans.Where( _span, ref span, selector, out int length );

        return new ReadOnlySpan<T>( buffer, 0, length );
    }
}
