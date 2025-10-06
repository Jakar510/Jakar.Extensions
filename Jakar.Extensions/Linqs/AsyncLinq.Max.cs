namespace Jakar.Extensions;


public static partial class AsyncLinq
{
    public static async ValueTask<decimal?> Max( this IAsyncEnumerable<decimal?> source )
    {
        decimal value = decimal.MinValue;

        await foreach ( decimal? item in source )
        {
            if ( item.HasValue ) { value = Math.Max(value, item.Value); }
        }

        return value;
    }
    public static async ValueTask<decimal?> Max<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, decimal?> selector )
    {
        decimal value = decimal.MinValue;

        await foreach ( TSource element in source )
        {
            decimal? item = selector(element);
            if ( item.HasValue ) { value = Math.Max(value, item.Value); }
        }

        return value;
    }


    public static async ValueTask<decimal> Max( this IAsyncEnumerable<decimal> source )
    {
        decimal value = decimal.MinValue;

        await foreach ( decimal item in source ) { value = Math.Max(value, item); }

        return value;
    }
    public static async ValueTask<decimal> Max<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, decimal> selector )
    {
        decimal value = 0;

        await foreach ( TSource item in source ) { value = Math.Max(value, selector(item)); }

        return value;
    }


    public static async ValueTask<double?> Max( this IAsyncEnumerable<int?> source )
    {
        double value = double.MinValue;

        await foreach ( int? item in source )
        {
            if ( item.HasValue ) { value = Math.Max(value, item.Value); }
        }

        return value;
    }
    public static async ValueTask<double?> Max<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, int?> selector )
    {
        double value = double.MinValue;

        await foreach ( TSource element in source )
        {
            int? item = selector(element);
            if ( item.HasValue ) { value = Math.Max(value, item.Value); }
        }

        return value;
    }


    public static async ValueTask<double?> Max( this IAsyncEnumerable<long?> source )
    {
        double value = double.MinValue;

        await foreach ( long? item in source )
        {
            if ( item.HasValue ) { value = Math.Max(value, item.Value); }
        }

        return value;
    }
    public static async ValueTask<double?> Max<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, long?> selector )
    {
        double value = double.MinValue;

        await foreach ( TSource element in source )
        {
            long? item = selector(element);
            if ( item.HasValue ) { value = Math.Max(value, item.Value); }
        }

        return value;
    }


    public static async ValueTask<double?> Max( this IAsyncEnumerable<double?> source )
    {
        double value = double.MinValue;

        await foreach ( double? item in source )
        {
            if ( item.HasValue ) { value = Math.Max(value, item.Value); }
        }

        return value;
    }
    public static async ValueTask<double?> Max<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, double?> selector )
    {
        double value = double.MinValue;

        await foreach ( TSource element in source )
        {
            double? item = selector(element);
            if ( item.HasValue ) { value = Math.Max(value, item.Value); }
        }

        return value;
    }
    public static async ValueTask<double> Max( this IAsyncEnumerable<int> source )
    {
        double value = double.MinValue;

        await foreach ( int item in source ) { value = Math.Max(value, item); }

        return value;
    }
    public static async ValueTask<double> Max<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, int> selector )
    {
        double value = double.MinValue;

        await foreach ( TSource item in source ) { value = Math.Max(value, selector(item)); }

        return value;
    }


    public static async ValueTask<double> Max( this IAsyncEnumerable<long> source )
    {
        double value = double.MinValue;

        await foreach ( long item in source ) { value = Math.Max(value, item); }

        return value;
    }
    public static async ValueTask<double> Max<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, long> selector )
    {
        double value = double.MinValue;

        await foreach ( TSource item in source ) { value = Math.Max(value, selector(item)); }

        return value;
    }


    public static async ValueTask<double> Max( this IAsyncEnumerable<double> source )
    {
        double value = double.MinValue;

        await foreach ( double item in source ) { value = Math.Max(value, item); }

        return value;
    }
    public static async ValueTask<double> Max<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, double> selector )
    {
        double value = double.MinValue;

        await foreach ( TSource item in source ) { value = Math.Max(value, selector(item)); }

        return value;
    }


    public static async ValueTask<float?> Max( this IAsyncEnumerable<float?> source )
    {
        double value = double.MinValue;

        await foreach ( float? item in source )
        {
            if ( item.HasValue ) { value = Math.Max(value, item.Value); }
        }

        return (float?)value;
    }
    public static async ValueTask<float?> Max<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, float?> selector )
    {
        double value = double.MinValue;

        await foreach ( TSource element in source )
        {
            float? item = selector(element);
            if ( item.HasValue ) { value = Math.Max(value, item.Value); }
        }

        return (float?)value;
    }


    public static async ValueTask<float> Max( this IAsyncEnumerable<float> source )
    {
        double value = double.MinValue;

        await foreach ( float item in source ) { value = Math.Max(value, item); }

        return (float)value;
    }
    public static async ValueTask<float> Max<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, float> selector )
    {
        double value = double.MinValue;

        await foreach ( TSource item in source ) { value = Math.Max(value, selector(item)); }

        return (float)value;
    }
}
