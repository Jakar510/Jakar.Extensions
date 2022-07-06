namespace Jakar.AppLogger;


/// <summary>
/// <para><see href="https://stackoverflow.com/a/67048259/9530917">How to protect strings without SecureString?</see></para>
/// <para><see href="https://stackoverflow.com/a/69327347/9530917">How to convert SecureString to System.String?</see></para>
/// </summary>
public sealed class AppLoggerApi : Service
{
    public static readonly string EmptyGuid                     = Guid.Empty.ToString();
    public const           string DEFAULT_OUTPUT_TEMPLATE       = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";
    public const           long   DEFAULT_FILE_SIZE_LIMIT_BYTES = 1L * 1024 * 1024 * 1024; // 1GB
    public const           string INSTALL_ID                    = nameof(INSTALL_ID);


    public           IAppLoggerConfig Config   { get; init; }
    internal         string           ApiToken { get; init; }
    private readonly MultiQueue<Log>  _logs = new();


    public AppLoggerApi( string apiToken, IAppLoggerConfig config )
    {
        ApiToken = apiToken;
        Config   = config;
    }
    protected override void Dispose( bool disposing ) { }
    public override async ValueTask DisposeAsync()
    {
        Dispose(true);
        await ValueTask.CompletedTask;
    }


    public void Add( Log log ) => _logs.Add(log);


    public async Task StartAsync( CancellationToken token )
    {
        while ( !token.IsCancellationRequested )
        {
            if ( _logs.Remove(out Log? log) ) { await Handle(log, token); }

            await Delay(50, token);
        }
    }
    public async Task StopAsync( CancellationToken token )
    {
        while ( _logs.Remove(out Log? log) ) { await Handle(log, token); }
    }

    private async Task Handle( Log log, CancellationToken token ) { }
}
