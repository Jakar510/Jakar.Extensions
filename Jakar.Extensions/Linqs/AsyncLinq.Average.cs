namespace Jakar.Extensions;


public static partial class AsyncLinq
{
    public static async ValueTask<decimal?> Average( this IAsyncEnumerable<decimal?> source )
    {
        long     count = 0;
        decimal? value = 0;

        await foreach ( decimal? item in source )
        {
            value += item;
            count++;
        }

        return value / count;
    }
    public static async ValueTask<decimal?> Average<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, decimal?> selector )
    {
        long     count = 0;
        decimal? value = 0;

        await foreach ( TSource item in source )
        {
            value += selector( item );
            count++;
        }

        return value / count;
    }


    public static async ValueTask<decimal> Average( this IAsyncEnumerable<decimal> source )
    {
        long    count = 0;
        decimal value = 0;

        await foreach ( decimal item in source )
        {
            value += item;
            count++;
        }

        return value / count;
    }
    public static async ValueTask<decimal> Average<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, decimal> selector )
    {
        long    count = 0;
        decimal value = 0;

        await foreach ( TSource item in source )
        {
            value += selector( item );
            count++;
        }

        return value / count;
    }


    public static async ValueTask<double?> Average( this IAsyncEnumerable<int?> source )
    {
        long    count = 0;
        double? value = 0;

        await foreach ( int? item in source )
        {
            value += item;
            count++;
        }

        return value / count;
    }
    public static async ValueTask<double?> Average<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, int?> selector )
    {
        long    count = 0;
        double? value = 0;

        await foreach ( TSource item in source )
        {
            value += selector( item );
            count++;
        }

        return value / count;
    }


    public static async ValueTask<double?> Average( this IAsyncEnumerable<long?> source )
    {
        long    count = 0;
        double? value = 0;

        await foreach ( long? item in source )
        {
            value += item;
            count++;
        }

        return value / count;
    }
    public static async ValueTask<double?> Average<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, long?> selector )
    {
        long    count = 0;
        double? value = 0;

        await foreach ( TSource item in source )
        {
            value += selector( item );
            count++;
        }

        return value / count;
    }


    public static async ValueTask<double?> Average( this IAsyncEnumerable<double?> source )
    {
        long    count = 0;
        double? value = 0;

        await foreach ( double? item in source )
        {
            value += item;
            count++;
        }

        return value / count;
    }
    public static async ValueTask<double?> Average<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, double?> selector )
    {
        long    count = 0;
        double? value = 0;

        await foreach ( TSource item in source )
        {
            value += selector( item );
            count++;
        }

        return value / count;
    }
    public static async ValueTask<double> Average( this IAsyncEnumerable<int> source )
    {
        long   count = 0;
        double value = 0;

        await foreach ( int item in source )
        {
            value += item;
            count++;
        }

        return value / count;
    }
    public static async ValueTask<double> Average<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, int> selector )
    {
        long   count = 0;
        double value = 0;

        await foreach ( TSource item in source )
        {
            value += selector( item );
            count++;
        }

        return value / count;
    }


    public static async ValueTask<double> Average( this IAsyncEnumerable<long> source )
    {
        long   count = 0;
        double value = 0;

        await foreach ( long item in source )
        {
            value += item;
            count++;
        }

        return value / count;
    }
    public static async ValueTask<double> Average<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, long> selector )
    {
        long   count = 0;
        double value = 0;

        await foreach ( TSource item in source )
        {
            value += selector( item );
            count++;
        }

        return value / count;
    }


    public static async ValueTask<double> Average( this IAsyncEnumerable<double> source )
    {
        long   count = 0;
        double value = 0;

        await foreach ( double item in source )
        {
            value += item;
            count++;
        }

        return value / count;
    }
    public static async ValueTask<double> Average<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, double> selector )
    {
        long   count = 0;
        double value = 0;

        await foreach ( TSource item in source )
        {
            value += selector( item );
            count++;
        }

        return value / count;
    }


    public static async ValueTask<float?> Average( this IAsyncEnumerable<float?> source )
    {
        long    count = 0;
        double? value = 0;

        await foreach ( float? item in source )
        {
            value += item;
            count++;
        }

        return (float?)(value / count);
    }
    public static async ValueTask<float?> Average<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, float?> selector )
    {
        long    count = 0;
        double? value = 0;

        await foreach ( TSource item in source )
        {
            value += selector( item );
            count++;
        }

        return (float?)(value / count);
    }


    public static async ValueTask<float> Average( this IAsyncEnumerable<float> source )
    {
        long   count = 0;
        double value = 0;

        await foreach ( float item in source )
        {
            value += item;
            count++;
        }

        return (float)(value / count);
    }
    public static async ValueTask<float> Average<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, float> selector )
    {
        long   count = 0;
        double value = 0;

        await foreach ( TSource item in source )
        {
            value += selector( item );
            count++;
        }

        return (float)(value / count);
    }
}
