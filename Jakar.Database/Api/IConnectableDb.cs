// Jakar.Extensions :: Jakar.Database
// 08/18/2022  10:38 PM

namespace Jakar.Database;


public interface IDbOptions
{
    public int?           CommandTimeout { get; }
    public DbTypeInstance DbTypeInstance { get; }

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



public interface IDbTable : IAsyncDisposable
{
    public void ResetCaches();
}



public interface IConnectableDb : IDbTable, IDbOptions
{
    public IsolationLevel TransactionIsolationLevel { get; }

    public ValueTask<DbConnection> ConnectAsync( CancellationToken token );
}



public interface IConnectableDbRoot : IConnectableDb, ITableCacheFactory
{
    public IAsyncEnumerable<T> Where<T>( DbConnection connection, DbTransaction? transaction, Activity? activity, string sql, DynamicParameters? parameters, [EnumeratorCancellation] CancellationToken token = default )
        where T : IDbReaderMapping<T>;
    public IAsyncEnumerable<T> WhereValue<T>( DbConnection connection, DbTransaction? transaction, Activity? activity, string sql, DynamicParameters? parameters, [EnumeratorCancellation] CancellationToken token = default )
        where T : struct;


    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public CommandDefinition GetCommand( Activity? activity, in SqlCommand sql, DbTransaction? transaction, CancellationToken token );


    public ValueTask<DbDataReader> ExecuteReaderAsync( DbConnection connection, DbTransaction? transaction, Activity? activity, SqlCommand sql, CancellationToken token );
}



public readonly record struct SqlCommand( string SQL, DynamicParameters? Parameters = null, CommandType? CommandType = null, CommandFlags Flags = CommandFlags.None )
{
    public static implicit operator SqlCommand( string sql ) => new(sql);

    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public CommandDefinition ToCommandDefinition( DbTransaction? transaction, CancellationToken token, int? timeout = null ) => new(SQL, Parameters, transaction, timeout, CommandType, Flags, token);
}
