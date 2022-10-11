namespace Jakar.Extensions;


public static partial class AsyncLinq
{
    public static async ValueTask<double> Max( this IAsyncEnumerable<int> source, CancellationToken token = default )
    {
        double value = double.MinValue;

        await foreach (int item in source.WithCancellation( token )) { value = Math.Max( value, item ); }

        return value;
    }
    public static async ValueTask<double> Max<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, int> selector, CancellationToken token = default )
    {
        double value = double.MinValue;

        await foreach (TSource item in source.WithCancellation( token )) { value = Math.Max( value, selector( item ) ); }

        return value;
    }


    public static async ValueTask<double> Max( this IAsyncEnumerable<long> source, CancellationToken token = default )
    {
        double value = double.MinValue;

        await foreach (long item in source.WithCancellation( token )) { value = Math.Max( value, item ); }

        return value;
    }
    public static async ValueTask<double> Max<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, long> selector, CancellationToken token = default )
    {
        double value = double.MinValue;

        await foreach (TSource item in source.WithCancellation( token )) { value = Math.Max( value, selector( item ) ); }

        return value;
    }


    public static async ValueTask<float> Max( this IAsyncEnumerable<float> source, CancellationToken token = default )
    {
        double value = double.MinValue;

        await foreach (float item in source.WithCancellation( token )) { value = Math.Max( value, item ); }

        return (float)value;
    }
    public static async ValueTask<float> Max<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, float> selector, CancellationToken token = default )
    {
        double value = double.MinValue;

        await foreach (TSource item in source.WithCancellation( token )) { value = Math.Max( value, selector( item ) ); }

        return (float)value;
    }


    public static async ValueTask<double> Max( this IAsyncEnumerable<double> source, CancellationToken token = default )
    {
        double value = double.MinValue;

        await foreach (double item in source.WithCancellation( token )) { value = Math.Max( value, item ); }

        return value;
    }
    public static async ValueTask<double> Max<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, double> selector, CancellationToken token = default )
    {
        double value = double.MinValue;

        await foreach (TSource item in source.WithCancellation( token )) { value = Math.Max( value, selector( item ) ); }

        return value;
    }


    public static async ValueTask<decimal> Max( this IAsyncEnumerable<decimal> source, CancellationToken token = default )
    {
        decimal value = decimal.MinValue;

        await foreach (decimal item in source.WithCancellation( token )) { value = Math.Max( value, item ); }

        return value;
    }
    public static async ValueTask<decimal> Max<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, decimal> selector, CancellationToken token = default )
    {
        decimal value = 0;

        await foreach (TSource item in source.WithCancellation( token )) { value = Math.Max( value, selector( item ) ); }

        return value;
    }


    public static async ValueTask<double?> Max( this IAsyncEnumerable<int?> source, CancellationToken token = default )
    {
        double value = double.MinValue;

        await foreach (int? item in source.WithCancellation( token ))
        {
            if (item.HasValue) { value = Math.Max( value, item.Value ); }
        }

        return value;
    }
    public static async ValueTask<double?> Max<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, int?> selector, CancellationToken token = default )
    {
        double value = double.MinValue;

        await foreach (TSource element in source.WithCancellation( token ))
        {
            int? item = selector( element );
            if (item.HasValue) { value = Math.Max( value, item.Value ); }
        }

        return value;
    }


    public static async ValueTask<double?> Max( this IAsyncEnumerable<long?> source, CancellationToken token = default )
    {
        double value = double.MinValue;

        await foreach (long? item in source.WithCancellation( token ))
        {
            if (item.HasValue) { value = Math.Max( value, item.Value ); }
        }

        return value;
    }
    public static async ValueTask<double?> Max<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, long?> selector, CancellationToken token = default )
    {
        double value = double.MinValue;

        await foreach (TSource element in source.WithCancellation( token ))
        {
            long? item = selector( element );
            if (item.HasValue) { value = Math.Max( value, item.Value ); }
        }

        return value;
    }


    public static async ValueTask<float?> Max( this IAsyncEnumerable<float?> source, CancellationToken token = default )
    {
        double value = double.MinValue;

        await foreach (float? item in source.WithCancellation( token ))
        {
            if (item.HasValue) { value = Math.Max( value, item.Value ); }
        }

        return (float?)value;
    }
    public static async ValueTask<float?> Max<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, float?> selector, CancellationToken token = default )
    {
        double value = double.MinValue;

        await foreach (TSource element in source.WithCancellation( token ))
        {
            float? item = selector( element );
            if (item.HasValue) { value = Math.Max( value, item.Value ); }
        }

        return (float?)value;
    }


    public static async ValueTask<double?> Max( this IAsyncEnumerable<double?> source, CancellationToken token = default )
    {
        double value = double.MinValue;

        await foreach (double? item in source.WithCancellation( token ))
        {
            if (item.HasValue) { value = Math.Max( value, item.Value ); }
        }

        return value;
    }
    public static async ValueTask<double?> Max<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, double?> selector, CancellationToken token = default )
    {
        double value = double.MinValue;

        await foreach (TSource element in source.WithCancellation( token ))
        {
            double? item = selector( element );
            if (item.HasValue) { value = Math.Max( value, item.Value ); }
        }

        return value;
    }


    public static async ValueTask<decimal?> Max( this IAsyncEnumerable<decimal?> source, CancellationToken token = default )
    {
        decimal value = decimal.MinValue;

        await foreach (decimal? item in source.WithCancellation( token ))
        {
            if (item.HasValue) { value = Math.Max( value, item.Value ); }
        }

        return value;
    }
    public static async ValueTask<decimal?> Max<TSource>( this IAsyncEnumerable<TSource> source, Func<TSource, decimal?> selector, CancellationToken token = default )
    {
        decimal value = decimal.MinValue;

        await foreach (TSource element in source.WithCancellation( token ))
        {
            decimal? item = selector( element );
            if (item.HasValue) { value = Math.Max( value, item.Value ); }
        }

        return value;
    }
}
