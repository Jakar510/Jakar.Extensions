// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:07 PM

namespace Jakar.Database;


[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public partial class DbTable<TClass>
{
    public ValueTask<long>                  Count( CancellationToken                token = default )                                                   => this.Call(Count, token);
    public ValueTask<bool>                  Exists( bool                            matchAll,   DynamicParameters parameters, CancellationToken token ) => this.TryCall(Exists, matchAll, parameters, token);
    public IAsyncEnumerable<TClass>         Get( IEnumerable<RecordID<TClass>>      ids,        CancellationToken token                               = default ) => this.Call(Get, ids,        token);
    public IAsyncEnumerable<TClass>         Get( IAsyncEnumerable<RecordID<TClass>> ids,        CancellationToken token                               = default ) => this.Call(Get, ids,        token);
    public ValueTask<ErrorOrResult<TClass>> Get( bool                               matchAll,   DynamicParameters parameters, CancellationToken token = default ) => this.Call(Get, matchAll,   parameters, token);
    public ValueTask<ErrorOrResult<TClass>> Get( string                             columnName, object?           value,      CancellationToken token = default ) => this.Call(Get, columnName, value,      token);
    public ValueTask<ErrorOrResult<TClass>> Get( RecordID<TClass>                   id,         CancellationToken token = default ) => this.Call(Get, id, token);
    public ValueTask<ErrorOrResult<TClass>> Get( RecordID<TClass>?                  id,         CancellationToken token = default ) => this.Call(Get, id, token);


    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async ValueTask<long> Count( NpgsqlConnection connection, DbTransaction? transaction, CancellationToken token = default )
    {
        SqlCommand sql = SQLCache.GetCount();

        try { return await connection.QueryFirstAsync<long>(sql.sql, sql.parameters, transaction); }
        catch ( Exception e ) { throw new SqlException(sql, e); }
    }


    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async ValueTask<bool> Exists( NpgsqlConnection connection, DbTransaction transaction, bool matchAll, DynamicParameters parameters, CancellationToken token )
    {
        SqlCommand sql = SQLCache.GetExists(matchAll, parameters);

        try
        {
            CommandDefinition   command = _database.GetCommand(in sql, transaction, token);
            IEnumerable<string> results = await connection.QueryAsync<string>(command);
            return results.Any();
        }
        catch ( Exception e ) { throw new SqlException(sql.sql, parameters, e); }
    }


    public virtual async IAsyncEnumerable<TClass> Get( NpgsqlConnection connection, DbTransaction? transaction, IAsyncEnumerable<RecordID<TClass>> ids, [EnumeratorCancellation] CancellationToken token = default )
    {
        HashSet<RecordID<TClass>> set = await ids.ToHashSet(token);
        await foreach ( TClass record in Get(connection, transaction, set, token) ) { yield return record; }
    }


    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual IAsyncEnumerable<TClass> Get( NpgsqlConnection connection, DbTransaction? transaction, IEnumerable<RecordID<TClass>> ids, [EnumeratorCancellation] CancellationToken token = default )
    {
        SqlCommand sql = SQLCache.Get(ids);
        return Where(connection, transaction, sql, token);
    }


    public async ValueTask<ErrorOrResult<TClass>> Get( NpgsqlConnection connection, DbTransaction? transaction, RecordID<TClass>? id, CancellationToken token = default ) => id.HasValue
                                                                                                                                                                             ? await Get(connection, transaction, id.Value, token)
                                                                                                                                                                             : Error.NotFound();
    public async ValueTask<ErrorOrResult<TClass>> Get( NpgsqlConnection connection, DbTransaction? transaction, RecordID<TClass> id, CancellationToken token = default )
    {
        SqlCommand            command    = SQLCache.Get(in id);
        SqlCommand.Definition definition = _database.GetCommand(in command, connection, transaction, token);
        return await _cache.GetOrCreateAsync(id.key, definition, factory, Options, token);

        async ValueTask<ErrorOrResult<TClass>> factory( SqlCommand.Definition sql, CancellationToken cancellationToken )
        {
            try
            {
                TClass? result = null;

                await foreach ( TClass record in Where(sql, cancellationToken) )
                {
                    if ( result is not null ) { return Error.Conflict(sql.command.CommandText); }

                    result = record;
                }

                return result is null
                           ? Error.NotFound(sql.command.CommandText)
                           : result;
            }
            catch ( Exception e ) { throw new SqlException(definition.command.CommandText, definition.command.Parameters as DynamicParameters, string.Empty, e); }
        }
    }


    public virtual async ValueTask<ErrorOrResult<TClass>> Get( NpgsqlConnection connection, DbTransaction? transaction, string columnName, object? value, CancellationToken token = default ) => await Get(connection, transaction, true, Database.GetParameters(value, null, columnName), token);


    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public virtual async ValueTask<ErrorOrResult<TClass>> Get( NpgsqlConnection connection, DbTransaction? transaction, bool matchAll, DynamicParameters parameters, CancellationToken token = default )
    {
        SqlCommand sql = SQLCache.Get(matchAll, parameters);

        try
        {
            SqlCommand.Definition definition = _database.GetCommand(in sql, connection, transaction, token);
            TClass?               result     = null;

            await foreach ( TClass record in Where(definition, token) )
            {
                if ( result is not null ) { return Error.Conflict(definition.command.CommandText); }

                result = record;
            }

            return result is null
                       ? Error.NotFound(definition.command.CommandText)
                       : result;
        }
        catch ( Exception e ) { throw new SqlException(sql, e); }
    }
}
