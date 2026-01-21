// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 06/25/2024  10:06

using System.Diagnostics;



namespace Jakar.Extensions.Telemetry;


[method: SetsRequiredMembers]
public sealed class TelemetryActivitySource( in AppInformation appContext ) : IValueEnumerable<TelemetryActivitySource.Enumerator, TelemetryActivity>, IDisposable
{
    internal readonly AppInformation                                  appContext   = appContext;
    private readonly  ConcurrentDictionary<string, TelemetryActivity> __activities = new(Environment.ProcessorCount, DEFAULT_CAPACITY, StringComparer.Ordinal);


    public          IEnumerable<TelemetryActivity> Activities  => __activities.Values;
    public required AppInformation                 AppContext  { get => appContext; init => appContext = value; }
    public          List<FileData>                 Attachments { get;               init; } = [];
    public          int                            Count       => __activities.Count;
    public TelemetryActivity this[ string operationName, TelemetryActivityContext context ] { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => GetOrAddActivity(operationName, context); }
    public TelemetryActivity this[ string operationName ] { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => GetOrAddActivity(operationName); }
    public TelemetryActivity? this[ ReadOnlySpan<char> operationName ] => TryGetValue(operationName, out TelemetryActivity? value)
                                                                              ? value
                                                                              : null;


    [JsonConstructor][SetsRequiredMembers]
    public TelemetryActivitySource( AppInformation appContext, IEnumerable<TelemetryActivity> activities ) : this(appContext)
    {
        AppContext = appContext;
        foreach ( TelemetryActivity value in activities ) { __activities.TryAdd(value.OperationName, value); }
    }
    [SetsRequiredMembers]
    public TelemetryActivitySource( AppInformation appContext, params ReadOnlySpan<TelemetryActivity> values ) : this(appContext)
    {
        AppContext = appContext;
        foreach ( TelemetryActivity value in values ) { __activities.TryAdd(value.OperationName, value); }
    }
    public void Dispose()
    {
        foreach ( TelemetryActivity activity in __activities.Values ) { activity.Dispose(); }

        __activities.Clear();
    }
    public static TelemetryActivitySource Create<TApp>( string? packageName = null )
        where TApp : IAppID => new(new AppInformation(TApp.AppVersion, TApp.AppID, TApp.AppName, packageName));
    public static TelemetryActivitySource Create( AppInformation context ) => new(context);


    public bool              ContainsKey( string             key )                                                             => __activities.ContainsKey(key);
    public bool              TryGetValue( string             operationName, [NotNullWhen(true)] out TelemetryActivity? value ) => __activities.TryGetValue(operationName, out value);
    public bool              TryGetValue( ReadOnlySpan<char> operationName, [NotNullWhen(true)] out TelemetryActivity? value ) => __activities.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue(operationName, out value);
    public TelemetryActivity GetOrAddActivity( string        operationName ) => GetOrAddActivity(operationName, TelemetryActivityContext.Create(operationName));
    public TelemetryActivity GetOrAddActivity( string operationName, in TelemetryActivityContext context )
    {
        ArgumentException.ThrowIfNullOrEmpty(operationName);
        return __activities.GetOrAdd(operationName, TelemetryActivity.Create, context);
    }


    public bool HasListeners() => false;


    public ValueEnumerable<Enumerator, TelemetryActivity> AsValueEnumerable() => new(new Enumerator(__activities));



    [StructLayout(LayoutKind.Auto)]
    public struct Enumerator : IValueEnumerator<TelemetryActivity>
    {
        private readonly ConcurrentDictionary<string, TelemetryActivity> _source;
        private readonly SortedSet<string>                               _keys = new(StringComparer.Ordinal);
        private          SortedSet<string>.Enumerator?                   _enumerator;


        public Enumerator( ConcurrentDictionary<string, TelemetryActivity> source )
        {
            _source     = source;
            _enumerator = _keys.GetEnumerator();
        }


        public bool TryGetNonEnumeratedCount( out int count )
        {
            count = _source.Count;
            return true;
        }
        public bool TryGetSpan( out ReadOnlySpan<TelemetryActivity> span )
        {
            span = ReadOnlySpan<TelemetryActivity>.Empty;
            return false;
        }
        public bool TryCopyTo( Span<TelemetryActivity> destination, Index offset )
        {
            if ( destination.Length < _source.Count ) { return false; }

            int i = 0;
            foreach ( TelemetryActivity activity in _source.Values ) { destination[i++] = activity; }

            return true;
        }
        public bool TryGetNext( out TelemetryActivity current )
        {
            _keys.Add(_source.Keys);
            SortedSet<string>.Enumerator enumerator = _enumerator ??= _keys.GetEnumerator();

            if ( enumerator.MoveNext() )
            {
                current = _source[enumerator.Current];
                return true;
            }

            current     = null!;
            _enumerator = null;
            return false;
        }


        public void Dispose() => _keys.Clear();
    }
}
