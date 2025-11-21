namespace Jakar.Extensions;


public static partial class AsyncLinq
{
    public static async ValueTask<decimal?> Max( this IAsyncEnumerable<decimal?> source )
    {
        decimal value = decimal.MinValue;

        await foreach ( decimal? item in source.ConfigureAwait(false) )
        {
            if ( item.HasValue ) { value = Math.Max(value, item.Value); }
        }

        return value;
    }
    public static async ValueTask<double?> Max( this IAsyncEnumerable<int?> source )
    {
        double value = double.MinValue;

        await foreach ( int? item in source.ConfigureAwait(false) )
        {
            if ( item.HasValue ) { value = Math.Max(value, item.Value); }
        }

        return value;
    }
    public static async ValueTask<double?> Max( this IAsyncEnumerable<long?> source )
    {
        double value = double.MinValue;

        await foreach ( long? item in source.ConfigureAwait(false) )
        {
            if ( item.HasValue ) { value = Math.Max(value, item.Value); }
        }

        return value;
    }
    public static async ValueTask<float?> Max( this IAsyncEnumerable<float?> source )
    {
        double value = double.MinValue;

        await foreach ( float? item in source.ConfigureAwait(false) )
        {
            if ( item.HasValue ) { value = Math.Max(value, item.Value); }
        }

        return (float?)value;
    }
    public static async ValueTask<double?> Max( this IAsyncEnumerable<double?> source )
    {
        double value = double.MinValue;

        await foreach ( double? item in source.ConfigureAwait(false) )
        {
            if ( item.HasValue ) { value = Math.Max(value, item.Value); }
        }

        return value;
    }


    public static async ValueTask<T> Max<T>( this IAsyncEnumerable<T> source, T minValue )
        where T : INumber<T>
    {
        T value = minValue;
        await foreach ( T item in source.ConfigureAwait(false) ) { value = T.Max(value, item); }

        return value;
    }



    extension<TSource>( IAsyncEnumerable<TSource> source )
    {
        public async ValueTask<T> Max<T>( Func<TSource, T> selector, T minValue )
            where T : INumber<T>
        {
            T value = minValue;
            await foreach ( TSource item in source.ConfigureAwait(false) ) { value = T.Max(value, selector(item)); }

            return value;
        }


        public async ValueTask<decimal?> Max( Func<TSource, decimal?> selector )
        {
            decimal value = decimal.MinValue;

            await foreach ( TSource element in source.ConfigureAwait(false) )
            {
                decimal? item = selector(element);
                if ( item.HasValue ) { value = Math.Max(value, item.Value); }
            }

            return value;
        }
        public async ValueTask<double?> Max( Func<TSource, int?> selector )
        {
            double value = double.MinValue;

            await foreach ( TSource element in source.ConfigureAwait(false) )
            {
                int? item = selector(element);
                if ( item.HasValue ) { value = Math.Max(value, item.Value); }
            }

            return value;
        }
        public async ValueTask<double?> Max( Func<TSource, long?> selector )
        {
            double value = double.MinValue;

            await foreach ( TSource element in source.ConfigureAwait(false) )
            {
                long? item = selector(element);
                if ( item.HasValue ) { value = Math.Max(value, item.Value); }
            }

            return value;
        }
        public async ValueTask<double?> Max( Func<TSource, double?> selector )
        {
            double value = double.MinValue;

            await foreach ( TSource element in source.ConfigureAwait(false) )
            {
                double? item = selector(element);
                if ( item.HasValue ) { value = Math.Max(value, item.Value); }
            }

            return value;
        }
        public async ValueTask<float?> Max( Func<TSource, float?> selector )
        {
            double value = double.MinValue;

            await foreach ( TSource element in source.ConfigureAwait(false) )
            {
                float? item = selector(element);
                if ( item.HasValue ) { value = Math.Max(value, item.Value); }
            }

            return (float?)value;
        }
    }
}
