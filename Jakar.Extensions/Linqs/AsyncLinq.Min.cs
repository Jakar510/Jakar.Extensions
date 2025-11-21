namespace Jakar.Extensions;


public static partial class AsyncLinq
{
    public static async ValueTask<decimal?> Min( this IAsyncEnumerable<decimal?> source )
    {
        decimal value = decimal.MaxValue;

        await foreach ( decimal? item in source.ConfigureAwait(false) )
        {
            if ( item.HasValue ) { value = Math.Min(value, item.Value); }
        }

        return value;
    }
    public static async ValueTask<double?> Min( this IAsyncEnumerable<int?> source )
    {
        double value = double.MaxValue;

        await foreach ( int? item in source.ConfigureAwait(false) )
        {
            if ( item.HasValue ) { value = Math.Min(value, item.Value); }
        }

        return value;
    }
    public static async ValueTask<double?> Min( this IAsyncEnumerable<long?> source )
    {
        double value = double.MaxValue;

        await foreach ( long? item in source.ConfigureAwait(false) )
        {
            if ( item.HasValue ) { value = Math.Min(value, item.Value); }
        }

        return value;
    }
    public static async ValueTask<double?> Min( this IAsyncEnumerable<double?> source )
    {
        double value = double.MaxValue;

        await foreach ( double? item in source.ConfigureAwait(false) )
        {
            if ( item.HasValue ) { value = Math.Min(value, item.Value); }
        }

        return value;
    }
    public static async ValueTask<float?> Min( this IAsyncEnumerable<float?> source )
    {
        double value = double.MaxValue;

        await foreach ( float? item in source.ConfigureAwait(false) )
        {
            if ( item.HasValue ) { value = Math.Min(value, item.Value); }
        }

        return (float?)value;
    }


    public static async ValueTask<T> Min<T>( this IAsyncEnumerable<T> source, T maxValue )
        where T : INumber<T>
    {
        T value = maxValue;

        await foreach ( T item in source.ConfigureAwait(false) ) { value = T.Min(value, ( item )); }

        return value;
    }



    extension<TSource>( IAsyncEnumerable<TSource> source )
    {
        public async ValueTask<T> Min<T>( Func<TSource, T> selector, T maxValue )
            where T : INumber<T>
        {
            T value = maxValue;

            await foreach ( TSource item in source.ConfigureAwait(false) ) { value = T.Min(value, selector(item)); }

            return value;
        }


        public async ValueTask<decimal?> Min( Func<TSource, decimal?> selector )
        {
            decimal value = decimal.MaxValue;

            await foreach ( TSource element in source.ConfigureAwait(false) )
            {
                decimal? item = selector(element);
                if ( item.HasValue ) { value = Math.Min(value, item.Value); }
            }

            return value;
        }
        public async ValueTask<double?> Min( Func<TSource, int?> selector )
        {
            double value = double.MaxValue;

            await foreach ( TSource element in source.ConfigureAwait(false) )
            {
                int? item = selector(element);
                if ( item.HasValue ) { value = Math.Min(value, item.Value); }
            }

            return value;
        }
        public async ValueTask<double?> Min( Func<TSource, long?> selector )
        {
            double value = double.MaxValue;

            await foreach ( TSource element in source.ConfigureAwait(false) )
            {
                long? item = selector(element);
                if ( item.HasValue ) { value = Math.Min(value, item.Value); }
            }

            return value;
        }
        public async ValueTask<double?> Min( Func<TSource, double?> selector )
        {
            double value = double.MaxValue;

            await foreach ( TSource element in source.ConfigureAwait(false) )
            {
                double? item = selector(element);
                if ( item.HasValue ) { value = Math.Min(value, item.Value); }
            }

            return value;
        }
        public async ValueTask<float?> Min( Func<TSource, float?> selector )
        {
            double value = double.MaxValue;

            await foreach ( TSource element in source.ConfigureAwait(false) )
            {
                float? item = selector(element);
                if ( item.HasValue ) { value = Math.Min(value, item.Value); }
            }

            return (float?)value;
        }
    }
}
