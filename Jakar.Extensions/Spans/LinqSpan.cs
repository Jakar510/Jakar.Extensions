// Jakar.Extensions :: Jakar.Extensions
// 08/28/2023  9:54 PM

namespace Jakar.Extensions;



/*
public interface ILinqSpanHandler<out TIn>
{
    void Handle( Predicate<TIn> selector );
}



public interface ILinqSpanHandler<TIn, TOut>
{
    void Handle( scoped in ReadOnlySpan<TIn> input, ref Span<TOut> output );
}



public readonly ref struct SpanLinq<TIn, THandler>( scoped in ReadOnlySpan<TIn> span, THandler handler )
    where THandler : ILinqSpanHandler<TIn>
{
    private readonly ReadOnlySpan<TIn> _span    = span;
    private readonly THandler          _handler = handler;


    [Pure]
    public SpanLinq<TIn, THandler> Where( Predicate<TIn> selector )
    {
        if ( _span.Length == 0 ) { return default; }

        TIn[]     buffer = AsyncLinq.GetArray<TIn>( _span.Length );
        Span<TIn> span   = buffer;
        Spans.Where( _span, ref span, selector, out int length );

        return new SpanLinq<TIn, THandler>( span[..length], _handler );
    }
}



public readonly ref struct SpanLinq<TIn, TOut, THandler>( scoped in ReadOnlySpan<TIn> span, THandler handler )
    where THandler : ILinqSpanHandler<TIn, TOut>
{
    private readonly ReadOnlySpan<TIn> _span    = span;
    private readonly THandler          _handler = handler;


    [Pure]
    public SpanLinq<TOut, THandler> Select( Func<TIn, TOut> selector )
    {
        if ( _span.Length == 0 ) { return default; }

        TOut[]     buffer = AsyncLinq.GetArray<TOut>( _span.Length );
        Span<TOut> span   = buffer;
        Spans.Where( _span, ref span, selector, out int length );

        return new SpanLinq<TOut, THandler>( span[..length], _handler );
    }
}
*/
