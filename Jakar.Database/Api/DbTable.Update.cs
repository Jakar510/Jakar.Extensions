﻿// Jakar.Extensions :: Jakar.Database
// 03/12/2023  1:09 PM

namespace Jakar.Database;


public partial class DbTable<TRecord>
{
    public ValueTask Update( TRecord                   record,  CancellationToken token = default ) => this.TryCall( Update, record,  token );
    public ValueTask Update( IEnumerable<TRecord>      records, CancellationToken token = default ) => this.TryCall( Update, records, token );
    public ValueTask Update( IAsyncEnumerable<TRecord> records, CancellationToken token = default ) => this.TryCall( Update, records, token );


    public virtual async ValueTask Update( DbConnection connection, DbTransaction? transaction, IEnumerable<TRecord> records, CancellationToken token = default )
    {
        foreach ( TRecord record in records ) { await Update( connection, transaction, record, token ); }
    }


    public virtual async ValueTask Update( DbConnection connection, DbTransaction? transaction, IAsyncEnumerable<TRecord> records, CancellationToken token = default )
    {
        await foreach ( TRecord record in records.WithCancellation( token ) ) { await Update( connection, transaction, record, token ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveOptimization )]
    public virtual async ValueTask Update( DbConnection connection, DbTransaction? transaction, TRecord record, CancellationToken token = default )
    {
        Guid              id         = record.ID;
        DynamicParameters parameters = GetParameters( id, record );
        string            sql        = $"UPDATE {SchemaTableName} SET {string.Join( ',', KeyValuePairs )} WHERE {ID} = @{nameof(id)};";

        if ( token.IsCancellationRequested ) { return; }

        try { await connection.ExecuteScalarAsync( sql, parameters, transaction ); }
        catch ( Exception e ) { throw new SqlException( sql, record, e ); }
    }
}
