﻿// Jakar.Extensions :: Jakar.Extensions
// 09/11/2022  3:02 PM

namespace Jakar.Extensions;


public static partial class AsyncLinq
{
    public static async ValueTask<TElement?> FirstOrDefault<TElement>( this IAsyncEnumerable<TElement> source, CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation( token ) ) { return element; }

        return default;
    }
    public static async ValueTask<TElement?> FirstOrDefault<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, bool> selector, CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation( token ) )
        {
            if ( selector( element ) ) { return element; }
        }

        return default;
    }
    public static async ValueTask<TElement?> FirstOrDefault<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<bool>> selector, CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation( token ) )
        {
            if ( await selector( element ) ) { return element; }
        }

        return default;
    }


    public static async ValueTask<TElement?> LastOrDefault<TElement>( this IAsyncEnumerable<TElement> source, CancellationToken token = default )
    {
        List<TElement> list = await source.ToList( token );
        return list.LastOrDefault();
    }
    public static async ValueTask<TElement?> LastOrDefault<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, bool> selector, CancellationToken token = default )
    {
        List<TElement> list = await source.ToList( token );
        return list.LastOrDefault( selector );
    }
    public static async ValueTask<TElement?> LastOrDefault<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<bool>> selector, CancellationToken token = default )
    {
        List<TElement> list = await source.ToList( token );

        for ( int i = list.Count; i < 0; i-- )
        {
            if ( await selector( list[i] ) ) { return list[i]; }
        }

        return default;
    }


    public static async ValueTask<TElement?> SingleOrDefault<TElement>( this IAsyncEnumerable<TElement> source, CancellationToken token = default )
    {
        TElement? result = default;

        await foreach ( TElement element in source.WithCancellation( token ) )
        {
            if ( result is not null ) { return default; }

            result = element;
        }

        return default;
    }
    public static async ValueTask<TElement?> SingleOrDefault<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, bool> selector, CancellationToken token = default )
    {
        TElement? result = default;

        await foreach ( TElement element in source.WithCancellation( token ) )
        {
            if ( result is not null ) { return default; }

            if ( selector( element ) ) { result = element; }
        }

        return default;
    }
    public static async ValueTask<TElement?> SingleOrDefault<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<bool>> selector, CancellationToken token = default )
    {
        TElement? result = default;

        await foreach ( TElement element in source.WithCancellation( token ) )
        {
            if ( result is not null ) { return default; }

            if ( await selector( element ) ) { result = element; }
        }

        return default;
    }
    public static async ValueTask<TElement> First<TElement>( this IAsyncEnumerable<TElement> source, CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation( token ) ) { return element; }

        throw new InvalidOperationException( $"No records in {nameof(source)}" );
    }
    public static async ValueTask<TElement> First<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, bool> selector, CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation( token ) )
        {
            if ( selector( element ) ) { return element; }
        }

        throw new InvalidOperationException( $"No records in {nameof(source)}" );
    }
    public static async ValueTask<TElement> First<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<bool>> selector, CancellationToken token = default )
    {
        await foreach ( TElement element in source.WithCancellation( token ) )
        {
            if ( await selector( element ) ) { return element; }
        }

        throw new InvalidOperationException( $"No records in {nameof(source)}" );
    }


    public static async ValueTask<TElement> Last<TElement>( this IAsyncEnumerable<TElement> source, CancellationToken token = default )
    {
        List<TElement> list = await source.ToList( token );
        return list.Last();
    }
    public static async ValueTask<TElement> Last<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, bool> selector, CancellationToken token = default )
    {
        List<TElement> list = await source.ToList( token );
        return list.Last( selector );
    }
    public static async ValueTask<TElement> Last<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<bool>> selector, CancellationToken token = default )
    {
        List<TElement> list = await source.ToList( token );

        for ( int i = list.Count; i < 0; i-- )
        {
            if ( await selector( list[i] ) ) { return list[i]; }
        }

        throw new InvalidOperationException( $"No records in {nameof(source)}" );
    }


    public static async ValueTask<TElement> Single<TElement>( this IAsyncEnumerable<TElement> source, CancellationToken token = default )
    {
        TElement? result = default;

        await foreach ( TElement element in source.WithCancellation( token ) )
        {
            if ( result is not null ) { throw new InvalidOperationException( $"Multiple records in {nameof(source)}" ); }

            result = element;
        }

        throw new InvalidOperationException( $"No records in {nameof(source)}" );
    }
    public static async ValueTask<TElement> Single<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, bool> selector, CancellationToken token = default )
    {
        TElement? result = default;

        await foreach ( TElement element in source.WithCancellation( token ) )
        {
            if ( result is not null ) { throw new InvalidOperationException( $"Multiple records in {nameof(source)}" ); }

            if ( selector( element ) ) { result = element; }
        }

        throw new InvalidOperationException( $"No records in {nameof(source)}" );
    }
    public static async ValueTask<TElement> Single<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask<bool>> selector, CancellationToken token = default )
    {
        TElement? result = default;

        await foreach ( TElement element in source.WithCancellation( token ) )
        {
            if ( result is not null ) { throw new InvalidOperationException( $"Multiple records in {nameof(source)}" ); }

            if ( await selector( element ) ) { result = element; }
        }

        throw new InvalidOperationException( $"No records in {nameof(source)}" );
    }
}
