namespace Jakar.Extensions;


public static partial class AsyncLinq
{
    public static async ValueTask<double> Min( this IAsyncEnumerable<int> source, CancellationToken token = default )
    {
        var value = double.MaxValue;

        await foreach ( int item in source.WithCancellation(token) ) { value = Math.Min(value, item); }

        return value;
    }
    public static async ValueTask<double> Min<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, int> selector, CancellationToken token = default )
    {
        var value = double.MaxValue;

        await foreach ( TSource item in source.WithCancellation(token) ) { value = Math.Min(value, selector(item)); }

        return value;
    }


    public static async ValueTask<double> Min( this IAsyncEnumerable<long> source, CancellationToken token = default )
    {
        var value = double.MaxValue;

        await foreach ( long item in source.WithCancellation(token) ) { value = Math.Min(value, item); }

        return value;
    }
    public static async ValueTask<double> Min<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, long> selector, CancellationToken token = default )
    {
        var value = double.MaxValue;

        await foreach ( TSource item in source.WithCancellation(token) ) { value = Math.Min(value, selector(item)); }

        return value;
    }


    public static async ValueTask<float> Min( this IAsyncEnumerable<float> source, CancellationToken token = default )
    {
        var value = double.MaxValue;

        await foreach ( float item in source.WithCancellation(token) ) { value = Math.Min(value, item); }

        return (float)value;
    }
    public static async ValueTask<float> Min<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, float> selector, CancellationToken token = default )
    {
        var value = double.MaxValue;

        await foreach ( TSource item in source.WithCancellation(token) ) { value = Math.Min(value, selector(item)); }

        return (float)value;
    }


    public static async ValueTask<double> Min( this IAsyncEnumerable<double> source, CancellationToken token = default )
    {
        var value = double.MaxValue;

        await foreach ( double item in source.WithCancellation(token) ) { value = Math.Min(value, item); }

        return value;
    }
    public static async ValueTask<double> Min<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, double> selector, CancellationToken token = default )
    {
        var value = double.MaxValue;

        await foreach ( TSource item in source.WithCancellation(token) ) { value = Math.Min(value, selector(item)); }

        return value;
    }


    public static async ValueTask<decimal> Min( this IAsyncEnumerable<decimal> source, CancellationToken token = default )
    {
        var value = decimal.MaxValue;

        await foreach ( decimal item in source.WithCancellation(token) ) { value = Math.Min(value, item); }

        return value;
    }
    public static async ValueTask<decimal> Min<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, decimal> selector, CancellationToken token = default )
    {
        decimal value = 0;

        await foreach ( TSource item in source.WithCancellation(token) ) { value = Math.Min(value, selector(item)); }

        return value;
    }


    public static async ValueTask<double?> Min( this IAsyncEnumerable<int?> source, CancellationToken token = default )
    {
        var value = double.MaxValue;

        await foreach ( int? item in source.WithCancellation(token) )
        {
            if ( item.HasValue ) { value = Math.Min(value, item.Value); }
        }

        return value;
    }
    public static async ValueTask<double?> Min<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, int?> selector, CancellationToken token = default )
    {
        var value = double.MaxValue;

        await foreach ( TSource element in source.WithCancellation(token) )
        {
            int? item = selector(element);
            if ( item.HasValue ) { value = Math.Min(value, item.Value); }
        }

        return value;
    }


    public static async ValueTask<double?> Min( this IAsyncEnumerable<long?> source, CancellationToken token = default )
    {
        var value = double.MaxValue;

        await foreach ( long? item in source.WithCancellation(token) )
        {
            if ( item.HasValue ) { value = Math.Min(value, item.Value); }
        }

        return value;
    }
    public static async ValueTask<double?> Min<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, long?> selector, CancellationToken token = default )
    {
        var value = double.MaxValue;

        await foreach ( TSource element in source.WithCancellation(token) )
        {
            long? item = selector(element);
            if ( item.HasValue ) { value = Math.Min(value, item.Value); }
        }

        return value;
    }


    public static async ValueTask<float?> Min( this IAsyncEnumerable<float?> source, CancellationToken token = default )
    {
        var value = double.MaxValue;

        await foreach ( float? item in source.WithCancellation(token) )
        {
            if ( item.HasValue ) { value = Math.Min(value, item.Value); }
        }

        return (float?)value;
    }
    public static async ValueTask<float?> Min<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, float?> selector, CancellationToken token = default )
    {
        var value = double.MaxValue;

        await foreach ( TSource element in source.WithCancellation(token) )
        {
            float? item = selector(element);
            if ( item.HasValue ) { value = Math.Min(value, item.Value); }
        }

        return (float?)value;
    }


    public static async ValueTask<double?> Min( this IAsyncEnumerable<double?> source, CancellationToken token = default )
    {
        var value = double.MaxValue;

        await foreach ( double? item in source.WithCancellation(token) )
        {
            if ( item.HasValue ) { value = Math.Min(value, item.Value); }
        }

        return value;
    }
    public static async ValueTask<double?> Min<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, double?> selector, CancellationToken token = default )
    {
        var value = double.MaxValue;

        await foreach ( TSource element in source.WithCancellation(token) )
        {
            double? item = selector(element);
            if ( item.HasValue ) { value = Math.Min(value, item.Value); }
        }

        return value;
    }


    public static async ValueTask<decimal?> Min( this IAsyncEnumerable<decimal?> source, CancellationToken token = default )
    {
        var value = decimal.MaxValue;

        await foreach ( decimal? item in source.WithCancellation(token) )
        {
            if ( item.HasValue ) { value = Math.Min(value, item.Value); }
        }

        return value;
    }
    public static async ValueTask<decimal?> Min<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, decimal?> selector, CancellationToken token = default )
    {
        var value = decimal.MaxValue;

        await foreach ( TSource element in source.WithCancellation(token) )
        {
            decimal? item = selector(element);
            if ( item.HasValue ) { value = Math.Min(value, item.Value); }
        }

        return value;
    }
}
