// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 06/25/2024  10:06

namespace Jakar.Extensions.Telemetry;


[JsonObject]
[method: SetsRequiredMembers]
public sealed class TelemetryActivitySource( AppContext appContext ) : IDisposable
{
    internal readonly AppContext                                      appContext  = appContext;
    private readonly  ConcurrentDictionary<string, TelemetryActivity> _activities = new(Environment.ProcessorCount, Buffers.DEFAULT_CAPACITY, StringComparer.Ordinal);


    public          List<FileData>?                Attachments { get; set; }
    public          IEnumerable<TelemetryActivity> Activities  => _activities.Values;
    public required AppContext                     AppContext  { get => appContext; init => appContext = value; }
    public          int                            Count       => _activities.Count;
    public TelemetryActivity this[ string  operationName, TelemetryActivityContext context ] { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => GetOrAddActivity( operationName, context ); }
    public TelemetryActivity? this[ string operationName ] => _activities.GetValueOrDefault( operationName );
    public TelemetryActivity? this[ ReadOnlySpan<char> operationName ] => TryGetValue( operationName, out TelemetryActivity? value )
                                                                              ? value
                                                                              : null;


    [JsonConstructor, SetsRequiredMembers]
    public TelemetryActivitySource( AppContext appContext, IEnumerable<TelemetryActivity> activities ) : this( appContext )
    {
        AppContext = appContext;
        foreach ( TelemetryActivity value in activities ) { _activities.TryAdd( value.OperationName, value ); }
    }
  
    [SetsRequiredMembers]
    public TelemetryActivitySource( AppContext appContext, params ReadOnlySpan<TelemetryActivity> values ) : this( appContext )
    {
        AppContext = appContext;
        foreach ( TelemetryActivity value in values ) { _activities.TryAdd( value.OperationName, value ); }
    }
    public void Dispose()
    {
        foreach ( TelemetryActivity activity in _activities.Values ) { activity.Dispose(); }

        _activities.Clear();
    }


    public bool ContainsKey( string             operationName )                                                     => _activities.ContainsKey( operationName );
    public bool TryGetValue( string             operationName, [NotNullWhen( true )] out TelemetryActivity? value ) => _activities.TryGetValue( operationName, out value );
    public bool TryGetValue( ReadOnlySpan<char> operationName, [NotNullWhen( true )] out TelemetryActivity? value ) => _activities.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue( operationName, out value );


    public TelemetryActivity GetOrAddActivity( string operationName, TelemetryActivityContext context ) => _activities.GetOrAdd( operationName, TelemetryActivity.Create, context );


    public static TelemetryActivitySource Create<TApp>()
        where TApp : IAppID => new(AppContext.Create<TApp>());
    public static TelemetryActivitySource Create( AppContext context ) => new(context);
}
