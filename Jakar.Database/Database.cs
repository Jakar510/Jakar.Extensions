// Jakar.Extensions :: Jakar.Database
// 08/14/2022  8:39 PM

using Microsoft.Extensions.Diagnostics.HealthChecks;



namespace Jakar.Database;


public abstract class Database : ObservableClass, IConnectableDb, IAsyncDisposable, IHealthCheck
{
    protected readonly ConcurrentBag<IAsyncDisposable> _disposables = new();


    public DbTable<UserRecord> Users { get; }


    static Database()
    {
        EnumSqlHandler<SupportedLanguage>.Register();
        EnumSqlHandler<MimeType>.Register();
        EnumSqlHandler<Status>.Register();
        EnumSqlHandler<AppVersion.Format>.Register();
        DateTimeOffsetHandler.Register();
        DateTimeHandler.Register();
        DateOnlyHandler.Register();
        TimeOnlyHandler.Register();
        AppVersionHandler.Register();
    }
    protected Database() : base() => Users = Create<UserRecord>();
    public virtual async ValueTask DisposeAsync()
    {
        foreach (IAsyncDisposable disposable in _disposables) { await disposable.DisposeAsync(); }

        _disposables.Clear();
    }


    protected DbTable<TRecord> Create<TRecord>() where TRecord : TableRecord<TRecord> => AddDisposable( new DbTable<TRecord>( this ) );
    protected TValue AddDisposable<TValue>( TValue value ) where TValue : IAsyncDisposable
    {
        _disposables.Add( value );
        return value;
    }


    public virtual async Task<HealthCheckResult> CheckHealthAsync( HealthCheckContext context, CancellationToken token = default )
    {
        try
        {
            await using var connection = await ConnectAsync( token );

            return connection.State switch
                   {
                       ConnectionState.Broken     => HealthCheckResult.Unhealthy(),
                       ConnectionState.Closed     => HealthCheckResult.Degraded(),
                       ConnectionState.Open       => HealthCheckResult.Healthy(),
                       ConnectionState.Connecting => HealthCheckResult.Healthy(),
                       ConnectionState.Executing  => HealthCheckResult.Healthy(),
                       ConnectionState.Fetching   => HealthCheckResult.Healthy(),
                       _                          => throw new ArgumentOutOfRangeException()
                   };
        }
        catch (Exception e) { return HealthCheckResult.Unhealthy( e.Message ); }
    }


    protected abstract DbConnection CreateConnection();
    public DbConnection Connect()
    {
        DbConnection connection = CreateConnection();
        connection.Open();
        return connection;
    }
    public async ValueTask<DbConnection> ConnectAsync( CancellationToken token )
    {
        DbConnection connection = CreateConnection();
        await connection.OpenAsync( token );
        return connection;
    }
}
