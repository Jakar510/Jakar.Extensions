namespace Jakar.Extensions;


public static partial class AsyncLinq
{
    public static async ValueTask<double> Average( this IAsyncEnumerable<int> source, CancellationToken token = default )
    {
        long   count = 0;
        double value = 0;

        await foreach (int item in source.WithCancellation( token ))
        {
            value += item;
            count++;
        }

        return value / count;
    }
    public static async ValueTask<double> Average<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, int> selector, CancellationToken token = default )
    {
        long   count = 0;
        double value = 0;

        await foreach (TSource item in source.WithCancellation( token ))
        {
            value += selector( item );
            count++;
        }

        return value / count;
    }


    public static async ValueTask<double> Average( this IAsyncEnumerable<long> source, CancellationToken token = default )
    {
        long   count = 0;
        double value = 0;

        await foreach (long item in source.WithCancellation( token ))
        {
            value += item;
            count++;
        }

        return value / count;
    }
    public static async ValueTask<double> Average<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, long> selector, CancellationToken token = default )
    {
        long   count = 0;
        double value = 0;

        await foreach (TSource item in source.WithCancellation( token ))
        {
            value += selector( item );
            count++;
        }

        return value / count;
    }


    public static async ValueTask<float> Average( this IAsyncEnumerable<float> source, CancellationToken token = default )
    {
        long   count = 0;
        double value = 0;

        await foreach (float item in source.WithCancellation( token ))
        {
            value += item;
            count++;
        }

        return (float)(value / count);
    }
    public static async ValueTask<float> Average<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, float> selector, CancellationToken token = default )
    {
        long   count = 0;
        double value = 0;

        await foreach (TSource item in source.WithCancellation( token ))
        {
            value += selector( item );
            count++;
        }

        return (float)(value / count);
    }


    public static async ValueTask<double> Average( this IAsyncEnumerable<double> source, CancellationToken token = default )
    {
        long   count = 0;
        double value = 0;

        await foreach (double item in source.WithCancellation( token ))
        {
            value += item;
            count++;
        }

        return value / count;
    }
    public static async ValueTask<double> Average<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, double> selector, CancellationToken token = default )
    {
        long   count = 0;
        double value = 0;

        await foreach (TSource item in source.WithCancellation( token ))
        {
            value += selector( item );
            count++;
        }

        return value / count;
    }


    public static async ValueTask<decimal> Average( this IAsyncEnumerable<decimal> source, CancellationToken token = default )
    {
        long    count = 0;
        decimal value = 0;

        await foreach (decimal item in source.WithCancellation( token ))
        {
            value += item;
            count++;
        }

        return value / count;
    }
    public static async ValueTask<decimal> Average<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, decimal> selector, CancellationToken token = default )
    {
        long    count = 0;
        decimal value = 0;

        await foreach (TSource item in source.WithCancellation( token ))
        {
            value += selector( item );
            count++;
        }

        return value / count;
    }


    public static async ValueTask<double?> Average( this IAsyncEnumerable<int?> source, CancellationToken token = default )
    {
        long    count = 0;
        double? value = 0;

        await foreach (int? item in source.WithCancellation( token ))
        {
            value += item;
            count++;
        }

        return value / count;
    }
    public static async ValueTask<double?> Average<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, int?> selector, CancellationToken token = default )
    {
        long    count = 0;
        double? value = 0;

        await foreach (TSource item in source.WithCancellation( token ))
        {
            value += selector( item );
            count++;
        }

        return value / count;
    }


    public static async ValueTask<double?> Average( this IAsyncEnumerable<long?> source, CancellationToken token = default )
    {
        long    count = 0;
        double? value = 0;

        await foreach (long? item in source.WithCancellation( token ))
        {
            value += item;
            count++;
        }

        return value / count;
    }
    public static async ValueTask<double?> Average<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, long?> selector, CancellationToken token = default )
    {
        long    count = 0;
        double? value = 0;

        await foreach (TSource item in source.WithCancellation( token ))
        {
            value += selector( item );
            count++;
        }

        return value / count;
    }


    public static async ValueTask<float?> Average( this IAsyncEnumerable<float?> source, CancellationToken token = default )
    {
        long    count = 0;
        double? value = 0;

        await foreach (float? item in source.WithCancellation( token ))
        {
            value += item;
            count++;
        }

        return (float?)(value / count);
    }
    public static async ValueTask<float?> Average<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, float?> selector, CancellationToken token = default )
    {
        long    count = 0;
        double? value = 0;

        await foreach (TSource item in source.WithCancellation( token ))
        {
            value += selector( item );
            count++;
        }

        return (float?)(value / count);
    }


    public static async ValueTask<double?> Average( this IAsyncEnumerable<double?> source, CancellationToken token = default )
    {
        long    count = 0;
        double? value = 0;

        await foreach (double? item in source.WithCancellation( token ))
        {
            value += item;
            count++;
        }

        return value / count;
    }
    public static async ValueTask<double?> Average<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, double?> selector, CancellationToken token = default )
    {
        long    count = 0;
        double? value = 0;

        await foreach (TSource item in source.WithCancellation( token ))
        {
            value += selector( item );
            count++;
        }

        return value / count;
    }


    public static async ValueTask<decimal?> Average( this IAsyncEnumerable<decimal?> source, CancellationToken token = default )
    {
        long     count = 0;
        decimal? value = 0;

        await foreach (decimal? item in source.WithCancellation( token ))
        {
            value += item;
            count++;
        }

        return value / count;
    }
    public static async ValueTask<decimal?> Average<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, decimal?> selector, CancellationToken token = default )
    {
        long     count = 0;
        decimal? value = 0;

        await foreach (TSource item in source.WithCancellation( token ))
        {
            value += selector( item );
            count++;
        }

        return value / count;
    }
}
