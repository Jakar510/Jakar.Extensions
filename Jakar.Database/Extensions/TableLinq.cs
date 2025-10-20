// Jakar.Extensions :: Jakar.Database
// 09/06/2022  4:54 PM

namespace Jakar.Database;


public static class TableLinq
{
    public static async ValueTask<ErrorOrResult<TSelf>> FirstAsync<TSelf>( this DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
        where TSelf : ITableRecord<TSelf>
    {
        await foreach ( TSelf self in reader.CreateAsync<TSelf>(token) ) { return self; }

        throw new InvalidOperationException("Sequence contains no elements");
    }
    public static async ValueTask<ErrorOrResult<TSelf>> FirstOrDefaultAsync<TSelf>( this DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
        where TSelf : ITableRecord<TSelf>
    {
        await foreach ( TSelf self in reader.CreateAsync<TSelf>(token) ) { return self; }

        return Error.NotFound();
    }


    public static async ValueTask<ErrorOrResult<TSelf>> SingleAsync<TSelf>( this DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
        where TSelf : ITableRecord<TSelf>
    {
        TSelf? record = default;

        await foreach ( TSelf self in reader.CreateAsync<TSelf>(token) )
        {
            if ( record is not null ) { throw new InvalidOperationException("Sequence contains more than one element"); }

            record = self;
        }

        return record is null
                   ? Error.NotFound()
                   : record;
    }
    public static async ValueTask<ErrorOrResult<TSelf>> SingleOrDefaultAsync<TSelf>( this DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
        where TSelf : ITableRecord<TSelf>
    {
        TSelf? record = default;

        await foreach ( TSelf self in reader.CreateAsync<TSelf>(token) )
        {
            if ( record is not null )
            {
                record = default;
                break;
            }

            record = self;
        }

        return record is null
                   ? Error.NotFound()
                   : record;
    }


    public static async ValueTask<ErrorOrResult<TSelf>> LastAsync<TSelf>( this DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
        where TSelf : ITableRecord<TSelf>
    {
        TSelf? record = default;
        await foreach ( TSelf self in reader.CreateAsync<TSelf>(token) ) { record = self; }

        return record is null
                   ? Error.NotFound()
                   : record;
    }
    public static async ValueTask<ErrorOrResult<TSelf>> LastOrDefaultAsync<TSelf>( this DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
        where TSelf : ITableRecord<TSelf>
    {
        TSelf? record = default;
        await foreach ( TSelf self in reader.CreateAsync<TSelf>(token) ) { record = self; }

        return record is null
                   ? Error.NotFound()
                   : record;
    }


    public static async IAsyncEnumerable<TSelf> CreateAsync<TSelf>( this DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
        where TSelf : ITableRecord<TSelf>
    {
        while ( await reader.ReadAsync(token) ) { yield return TSelf.Create(reader); }
    }


    public static async ValueTask<TSelf[]> CreateAsync<TSelf>( this DbDataReader reader, int initialCapacity, [EnumeratorCancellation] CancellationToken token = default )
        where TSelf : ITableRecord<TSelf>
    {
        List<TSelf> list = new(initialCapacity);
        while ( await reader.ReadAsync(token) ) { list.Add(TSelf.Create(reader)); }

        return list.ToArray();
    }


    public static async IAsyncEnumerable<TSelf> Where<TSelf>( this IAsyncEnumerable<TSelf> source, Func<NpgsqlConnection, NpgsqlTransaction?, TSelf, CancellationToken, bool> func, NpgsqlConnection connection, NpgsqlTransaction? transaction, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TSelf record in source.WithCancellation(token) )
        {
            if ( func(connection, transaction, record, token) ) { yield return record; }
        }
    }
    public static async IAsyncEnumerable<TSelf> Where<TSelf>( this IAsyncEnumerable<TSelf> source, Func<NpgsqlConnection, NpgsqlTransaction?, TSelf, CancellationToken, ValueTask<bool>> func, NpgsqlConnection connection, NpgsqlTransaction? transaction, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TSelf record in source.WithCancellation(token) )
        {
            if ( await func(connection, transaction, record, token) ) { yield return record; }
        }
    }
    public static async IAsyncEnumerable<TResult> Select<TSelf, TResult>( this IAsyncEnumerable<TSelf> source, Func<NpgsqlConnection, NpgsqlTransaction?, TSelf, CancellationToken, TResult> func, NpgsqlConnection connection, NpgsqlTransaction? transaction, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TSelf record in source.WithCancellation(token) ) { yield return func(connection, transaction, record, token); }
    }
    public static async IAsyncEnumerable<TResult> Select<TSelf, TResult>( this IAsyncEnumerable<TSelf> source, Func<NpgsqlConnection, NpgsqlTransaction?, TSelf, CancellationToken, ValueTask<TResult>> func, NpgsqlConnection connection, NpgsqlTransaction? transaction, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TSelf record in source.WithCancellation(token) ) { yield return await func(connection, transaction, record, token); }
    }
}
