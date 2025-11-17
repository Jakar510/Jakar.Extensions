namespace Jakar.Extensions;


public static partial class AsyncLinq
{
    public static async ValueTask<decimal?> Min( this IAsyncEnumerable<decimal?> source )
    {
        decimal value = decimal.MaxValue;

        await foreach ( decimal? item in source )
        {
            if ( item.HasValue ) { value = Math.Min(value, item.Value); }
        }

        return value;
    }
    public static async ValueTask<decimal> Min( this IAsyncEnumerable<decimal> source )
    {
        decimal value = decimal.MaxValue;

        await foreach ( decimal item in source ) { value = Math.Min(value, item); }

        return value;
    }
    public static async ValueTask<double?> Min( this IAsyncEnumerable<int?> source )
    {
        double value = double.MaxValue;

        await foreach ( int? item in source )
        {
            if ( item.HasValue ) { value = Math.Min(value, item.Value); }
        }

        return value;
    }
    public static async ValueTask<double?> Min( this IAsyncEnumerable<long?> source )
    {
        double value = double.MaxValue;

        await foreach ( long? item in source )
        {
            if ( item.HasValue ) { value = Math.Min(value, item.Value); }
        }

        return value;
    }
    public static async ValueTask<double?> Min( this IAsyncEnumerable<double?> source )
    {
        double value = double.MaxValue;

        await foreach ( double? item in source )
        {
            if ( item.HasValue ) { value = Math.Min(value, item.Value); }
        }

        return value;
    }
    public static async ValueTask<double> Min( this IAsyncEnumerable<int> source )
    {
        double value = double.MaxValue;

        await foreach ( int item in source ) { value = Math.Min(value, item); }

        return value;
    }
    public static async ValueTask<double> Min( this IAsyncEnumerable<long> source )
    {
        double value = double.MaxValue;

        await foreach ( long item in source ) { value = Math.Min(value, item); }

        return value;
    }
    public static async ValueTask<double> Min( this IAsyncEnumerable<double> source )
    {
        double value = double.MaxValue;

        await foreach ( double item in source ) { value = Math.Min(value, item); }

        return value;
    }
    public static async ValueTask<float?> Min( this IAsyncEnumerable<float?> source )
    {
        double value = double.MaxValue;

        await foreach ( float? item in source )
        {
            if ( item.HasValue ) { value = Math.Min(value, item.Value); }
        }

        return (float?)value;
    }
    public static async ValueTask<float> Min( this IAsyncEnumerable<float> source )
    {
        double value = double.MaxValue;

        await foreach ( float item in source ) { value = Math.Min(value, item); }

        return (float)value;
    }



    extension<TSource>( IAsyncEnumerable<TSource> source )
    {
        public async ValueTask<decimal?> Min( Func<TSource, decimal?> selector )
        {
            decimal value = decimal.MaxValue;

            await foreach ( TSource element in source )
            {
                decimal? item = selector(element);
                if ( item.HasValue ) { value = Math.Min(value, item.Value); }
            }

            return value;
        }
        public async ValueTask<decimal> Min( Func<TSource, decimal> selector )
        {
            decimal value = 0;

            await foreach ( TSource item in source ) { value = Math.Min(value, selector(item)); }

            return value;
        }
        public async ValueTask<double?> Min( Func<TSource, int?> selector )
        {
            double value = double.MaxValue;

            await foreach ( TSource element in source )
            {
                int? item = selector(element);
                if ( item.HasValue ) { value = Math.Min(value, item.Value); }
            }

            return value;
        }
        public async ValueTask<double?> Min( Func<TSource, long?> selector )
        {
            double value = double.MaxValue;

            await foreach ( TSource element in source )
            {
                long? item = selector(element);
                if ( item.HasValue ) { value = Math.Min(value, item.Value); }
            }

            return value;
        }
        public async ValueTask<double?> Min( Func<TSource, double?> selector )
        {
            double value = double.MaxValue;

            await foreach ( TSource element in source )
            {
                double? item = selector(element);
                if ( item.HasValue ) { value = Math.Min(value, item.Value); }
            }

            return value;
        }
        public async ValueTask<double> Min( Func<TSource, int> selector )
        {
            double value = double.MaxValue;

            await foreach ( TSource item in source ) { value = Math.Min(value, selector(item)); }

            return value;
        }
        public async ValueTask<double> Min( Func<TSource, long> selector )
        {
            double value = double.MaxValue;

            await foreach ( TSource item in source ) { value = Math.Min(value, selector(item)); }

            return value;
        }
        public async ValueTask<double> Min( Func<TSource, double> selector )
        {
            double value = double.MaxValue;

            await foreach ( TSource item in source ) { value = Math.Min(value, selector(item)); }

            return value;
        }
        public async ValueTask<float?> Min( Func<TSource, float?> selector )
        {
            double value = double.MaxValue;

            await foreach ( TSource element in source )
            {
                float? item = selector(element);
                if ( item.HasValue ) { value = Math.Min(value, item.Value); }
            }

            return (float?)value;
        }
        public async ValueTask<float> Min( Func<TSource, float> selector )
        {
            double value = double.MaxValue;

            await foreach ( TSource item in source ) { value = Math.Min(value, selector(item)); }

            return (float)value;
        }
    }
}
