// Jakar.Extensions :: Jakar.Database
// 08/18/2022  10:38 PM

namespace Jakar.Database;


public interface IDbOptions
{
    public int? CommandTimeout { get; }

    // public string                                                  AppName                  { get; }
    // public string                                                  AuthenticationType       { get; }
    // public TimeSpan                                                ClockSkew                { get; }
    // public SecuredStringResolverOptions                            ConnectionStringResolver { get; }
    // public Uri                                                     Domain                   { get; }
    // public string                                                  JWTAlgorithm             { get; }
    // public string                                                  JWTKey                   { get; }
    // public PasswordRequirements                                    PasswordRequirements     { get; }
    // public (LocalFile Pem, SecuredStringResolverOptions Password)? DataProtectorKey         { get; }
    // public string                                                  TokenAudience            { get; }
    // public string                                                  TokenIssuer              { get; }
    // public string                                                  UserExists               { get; }
    // public AppVersion                                              AppVersion                  { get; }
}



public interface IDbTable : IAsyncDisposable;



public interface IConnectableDb : IDbTable, IDbOptions
{
    public IsolationLevel TransactionIsolationLevel { get; }

    public ValueTask<DbConnection> ConnectAsync( CancellationToken token );
}



public interface IConnectableDbRoot : IConnectableDb
{
    public IAsyncEnumerable<T> Where<T>( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, [EnumeratorCancellation] CancellationToken token = default )
        where T : IDbReaderMapping<T>, IRecordPair;
    public IAsyncEnumerable<T> WhereValue<T>( DbConnection connection, DbTransaction? transaction, string sql, DynamicParameters? parameters, [EnumeratorCancellation] CancellationToken token = default )
        where T : struct;


    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )]
    public CommandDefinition GetCommand<T>( T command, DbTransaction? transaction, CancellationToken token, CommandType? commandType = null )
        where T : IDapperSqlCommand;
    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public CommandDefinition     GetCommand( SqlCommand sql, DbTransaction? transaction, CancellationToken token );
    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public SqlCommand.Definition GetCommand( SqlCommand sql, DbConnection   connection,  DbTransaction?    transaction, CancellationToken token );


    public ValueTask<DbDataReader> ExecuteReaderAsync<T>( DbConnection connection, DbTransaction? transaction, T command, CancellationToken token )
        where T : IDapperSqlCommand;
    public ValueTask<DbDataReader> ExecuteReaderAsync( DbConnection          connection, DbTransaction? transaction, SqlCommand sql, CancellationToken token );
    public ValueTask<DbDataReader> ExecuteReaderAsync( SqlCommand.Definition definition );
}
