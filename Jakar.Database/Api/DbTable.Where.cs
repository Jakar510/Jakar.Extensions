// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:08 PM


namespace Jakar.Database;


[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public partial class DbTable<TClass>
{
    public IAsyncEnumerable<TClass> Where( bool           matchAll,   DynamicParameters  parameters, [EnumeratorCancellation] CancellationToken token = default ) => this.Call(Where, matchAll,   parameters, token);
    public IAsyncEnumerable<TClass> Where( string         sql,        DynamicParameters? parameters, [EnumeratorCancellation] CancellationToken token = default ) => this.Call(Where, sql,        parameters, token);
    public IAsyncEnumerable<TClass> Where<TValue>( string columnName, TValue?            value,      [EnumeratorCancellation] CancellationToken token = default ) => this.Call(Where, columnName, value,      token);


    public virtual async IAsyncEnumerable<TClass> Where( NpgsqlConnection connection, DbTransaction? transaction, string sql, DynamicParameters parameters, [EnumeratorCancellation] CancellationToken token = default )
    {
        SqlCommand               command = new(sql, parameters);
        await using DbDataReader reader  = await _database.ExecuteReaderAsync(connection, transaction, command, token);
        await foreach ( TClass record in reader.CreateAsync<TClass>(token) ) { yield return record; }
    }


    public virtual async IAsyncEnumerable<TClass> Where( NpgsqlConnection connection, DbTransaction? transaction, bool matchAll, DynamicParameters parameters, [EnumeratorCancellation] CancellationToken token = default )
    {
        SqlCommand               command = SQLCache.Get(matchAll, parameters);
        await using DbDataReader reader  = await _database.ExecuteReaderAsync(connection, transaction, command, token);
        await foreach ( TClass record in reader.CreateAsync<TClass>(token) ) { yield return record; }
    }


    public virtual async IAsyncEnumerable<TClass> Where( NpgsqlConnection connection, DbTransaction? transaction, SqlCommand sql, [EnumeratorCancellation] CancellationToken token = default )
    {
        await using DbDataReader reader = await _database.ExecuteReaderAsync(connection, transaction, sql, token);
        await foreach ( TClass record in reader.CreateAsync<TClass>(token) ) { yield return record; }
    }
    public virtual async IAsyncEnumerable<TClass> Where( SqlCommand.Definition definition, [EnumeratorCancellation] CancellationToken token = default )
    {
        await using DbDataReader reader = await _database.ExecuteReaderAsync(definition);
        await foreach ( TClass record in reader.CreateAsync<TClass>(token) ) { yield return record; }
    }


    public virtual IAsyncEnumerable<TClass> Where<TValue>( NpgsqlConnection connection, DbTransaction? transaction, string columnName, TValue? value, [EnumeratorCancellation] CancellationToken token = default )
    {
        DynamicParameters parameters = new();
        parameters.Add(nameof(value), value);

        SqlCommand sql = new($"SELECT * FROM {TClass.TableName} WHERE {columnName} = @{nameof(value)};", parameters);
        return Where(connection, transaction, sql, token);
    }
}
