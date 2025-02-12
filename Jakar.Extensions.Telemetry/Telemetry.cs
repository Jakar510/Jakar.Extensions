using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using static Jakar.Extensions.Telemetry.TelemetryLogger;


// using static Jakar.Extensions.Telemetry.TelemetryLogger;



namespace Jakar.Extensions.Telemetry;


// OpenTelemetry TelemetryLogger



public class Telemetry<TApp> : ILoggerFactory, ILoggerProvider
    where TApp : IAppID
{
    protected readonly IEnumerable<ILoggerProvider>                  _providers;
    protected readonly IOptionsMonitor<LoggerFilterOptions>          _filterOption;
    protected readonly LoggerFactoryOptions?                         _options;
    protected readonly IExternalScopeProvider?                       _scopeProvider;
    protected readonly ConcurrentDictionary<string, TelemetryLogger> _telemetryLoggers = new(StringComparer.Ordinal);
    protected readonly List<ILoggerProvider>                         _loggerProviders  = new(Buffers.DEFAULT_CAPACITY);


    public ActivityCollection Collection { get; } = ActivityCollection.Create<TApp>();


    public Telemetry( IEnumerable<ILoggerProvider> providers, IOptionsMonitor<LoggerFilterOptions> filterOption, IOptions<LoggerFactoryOptions>? options = null, IExternalScopeProvider? scopeProvider = null )
    {
        _providers     = providers;
        _filterOption  = filterOption;
        _options       = options?.Value;
        _scopeProvider = scopeProvider;
        _loggerProviders.Add( this );
    }


    public static Telemetry<TApp> Create( IServiceProvider provider ) => Create( provider.GetRequiredService<IEnumerable<ILoggerProvider>>(), provider.GetRequiredService<IOptionsMonitor<LoggerFilterOptions>>(), provider.GetService<IOptions<LoggerFactoryOptions>>(), provider.GetService<IExternalScopeProvider>() );

    public static Telemetry<TApp> Create( IEnumerable<ILoggerProvider> providers, IOptionsMonitor<LoggerFilterOptions> filterOption, IOptions<LoggerFactoryOptions>? options = null, IExternalScopeProvider? scopeProvider = null ) => new(providers, filterOption, options, scopeProvider);

    public static Telemetry<TApp> Get( IServiceProvider provider ) => provider.GetRequiredService<Telemetry<TApp>>();

    public void Dispose()
    {
        Collection.Dispose();
        GC.SuppressFinalize( this );
    }


    public ILogger CreateLogger( string categoryName )
    {
        TelemetryLogger logger = _telemetryLoggers.GetOrAdd( categoryName, CreateTelemetryLogger );
        logger.Loggers = GetLoggerInformation( categoryName );
        return logger;
    }
    protected TelemetryLogger CreateTelemetryLogger( string categoryName ) => TelemetryLogger.Create( categoryName, GetLoggerInformation( categoryName ) );
    public void AddProvider( ILoggerProvider provider )
    {
        _loggerProviders.Add( provider );
        foreach ( (string categoryName, TelemetryLogger logger) in _telemetryLoggers ) { logger.Loggers = GetLoggerInformation( categoryName ); }
    }


    protected LoggerInformation[] GetLoggerInformation( string categoryName )
    {
        LoggerInformation[] loggers = GC.AllocateUninitializedArray<LoggerInformation>( _loggerProviders.Count );
        for ( int i = 0; i < _loggerProviders.Count; i++ ) { loggers[i] = new LoggerInformation( _loggerProviders[i], categoryName ); }

        return loggers;
    }
}



public readonly struct LoggerInformation( ILoggerProvider provider, string category )
{
    public readonly ILogger logger        = provider.CreateLogger( category );
    public readonly string  category      = category;
    public readonly Type    providerType  = provider.GetType();
    public readonly bool    externalScope = provider is ISupportExternalScope;
}



public readonly struct MessageLogger( ILogger logger, string category, string? providerTypeFullName, LogLevel? minLevel, Func<string?, string?, LogLevel, bool>? filter )
{
    public readonly  ILogger                                 logger                = logger;
    public readonly  string                                  category              = category;
    private readonly string?                                 _providerTypeFullName = providerTypeFullName;
    public readonly  LogLevel?                               minLevel              = minLevel;
    public readonly  Func<string?, string?, LogLevel, bool>? filter                = filter;


    public bool IsEnabled( LogLevel level )
    {
        if ( level < minLevel ) { return false; }

        if ( filter is not null ) { return filter( _providerTypeFullName, category, level ); }

        return true;
    }
}



public readonly struct ScopeLogger( ILogger logger, IExternalScopeProvider? externalScopeProvider )
{
    public readonly ILogger                 logger                = logger;
    public readonly IExternalScopeProvider? externalScopeProvider = externalScopeProvider;
    public IDisposable? CreateScope<TState>( TState state )
        where TState : notnull
    {
        if ( externalScopeProvider is not null ) { return externalScopeProvider.Push( state ); }

        return logger.BeginScope( state );
    }
}



[DebuggerDisplay( "{DebuggerToString(),nq}" ), DebuggerTypeProxy( typeof(LoggerDebugView) ), SuppressMessage( "ReSharper", "ForCanBeConvertedToForeach" )]
public sealed class TelemetryLogger( string categoryName, params LoggerInformation[] loggers ) : ILogger
{
    private readonly string _categoryName = categoryName;

    public LoggerInformation[] Loggers        { get; set; } = loggers;
    public MessageLogger[]?    MessageLoggers { get; set; }
    public ScopeLogger[]?      ScopeLoggers   { get; set; }


    public static TelemetryLogger Create( string categoryName, params LoggerInformation[] loggers ) => new(categoryName, loggers);


    public void Log<TState>( LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter )
    {
        ReadOnlySpan<MessageLogger> loggers = MessageLoggers;
        if ( loggers.IsEmpty ) { return; }

        List<Exception> exceptions = new(loggers.Length);

        for ( int i = 0; i < loggers.Length; i++ )
        {
            ref readonly MessageLogger loggerInfo = ref loggers[i];
            if ( loggerInfo.IsEnabled( logLevel ) is false ) { continue; }

            LoggerLog( logLevel, eventId, loggerInfo.logger, exception, formatter, in exceptions, in state );
        }

        if ( exceptions.Count > 0 ) { ThrowLoggingError( exceptions ); }

        return;

        static void LoggerLog( LogLevel logLevel, EventId eventId, ILogger logger, Exception? exception, Func<TState, Exception?, string> formatter, ref readonly List<Exception> exceptions, ref readonly TState state )
        {
            try { logger.Log( logLevel, eventId, state, exception, formatter ); }
            catch ( Exception ex ) { exceptions.Add( ex ); }
        }
    }

    public bool IsEnabled( LogLevel logLevel )
    {
        ReadOnlySpan<MessageLogger> loggers = MessageLoggers;
        if ( loggers.IsEmpty ) { return false; }

        List<Exception> exceptions = new(loggers.Length);
        int             i          = 0;

        for ( ; i < loggers.Length; i++ )
        {
            ref readonly MessageLogger loggerInfo = ref loggers[i];
            if ( !loggerInfo.IsEnabled( logLevel ) ) { continue; }

            if ( LoggerIsEnabled( logLevel, loggerInfo.logger, ref exceptions ) ) { break; }
        }

        if ( exceptions.Count > 0 ) { ThrowLoggingError( exceptions ); }

        return i < loggers.Length;

        static bool LoggerIsEnabled( LogLevel logLevel, ILogger logger, ref readonly List<Exception> exceptions )
        {
            try
            {
                if ( logger.IsEnabled( logLevel ) ) { return true; }
            }
            catch ( Exception ex ) { exceptions.Add( ex ); }

            return false;
        }
    }


    public IDisposable? BeginScope<TState>( TState state )
        where TState : notnull
    {
        ReadOnlySpan<ScopeLogger> loggers = ScopeLoggers;
        if ( loggers.IsEmpty ) { return NullScope.Instance; }

        if ( loggers.Length == 1 ) { return loggers[0].CreateScope( state ); }

        Scope           scope      = new(loggers.Length);
        List<Exception> exceptions = new(loggers.Length);

        for ( int i = 0; i < loggers.Length; i++ )
        {
            try { scope.SetDisposable( i, loggers[i].CreateScope( state ) ); }
            catch ( Exception ex ) { exceptions.Add( ex ); }
        }

        if ( exceptions.Count > 0 ) { ThrowLoggingError( exceptions ); }

        return scope;
    }
    private static void ThrowLoggingError( List<Exception> exceptions ) => throw new AggregateException( "An error occurred while writing to logger(s).", exceptions );


    public string DebuggerToString() => DebuggerDisplayFormatting.DebuggerToString( _categoryName, this );



    private sealed class LoggerDebugView( TelemetryLogger logger )
    {
        private readonly TelemetryLogger                _logger = logger;
        private          List<LoggerProviderDebugView>? _providers;
        private          List<object?>?                 _scopes;
        public           bool                           Enabled   => DebuggerDisplayFormatting.CalculateEnabledLogLevel( _logger ) is not null;
        public           LogLevel?                      MinLevel  => DebuggerDisplayFormatting.CalculateEnabledLogLevel( _logger );
        public           string                         Name      => _logger._categoryName;
        public           List<LoggerProviderDebugView>  Providers => _providers ??= GetProviders();
        public           List<object?>?                 Scopes    => _scopes ??= GetScopes();


        protected List<object?>? GetScopes()
        {
            ReadOnlySpan<ScopeLogger> span          = _logger.ScopeLoggers;
            IExternalScopeProvider?   scopeProvider = span.FirstOrDefault( static x => true ).externalScopeProvider;
            if ( scopeProvider is null ) { return null; }

            List<object?> scopes = [];
            scopeProvider.ForEachScope( static ( scope, scopes ) => scopes.Add( scope ), scopes );
            return scopes;
        }
        public List<LoggerProviderDebugView> GetProviders()
        {
            ReadOnlySpan<MessageLogger>     messages  = _logger.MessageLoggers;
            ReadOnlySpan<LoggerInformation> loggers   = _logger.Loggers;
            List<LoggerProviderDebugView>   providers = new(loggers.Length);

            foreach ( LoggerInformation loggerInfo in loggers )
            {
                string         providerName  = ProviderAliasUtilities.GetAlias( loggerInfo.providerType ) ?? loggerInfo.providerType.Name;
                MessageLogger? messageLogger = FirstOrNull( in messages, loggerInfo.logger );
                providers.Add( new LoggerProviderDebugView( providerName, messageLogger ) );
            }

            return providers;

            // Find message logger or return null if there is no match. FirstOrDefault isn't used because MessageLogger is a struct.
            static MessageLogger? FirstOrNull( ref readonly ReadOnlySpan<MessageLogger> messageLoggers, ILogger logger )
            {
                if ( messageLoggers.IsEmpty ) { return null; }

                foreach ( MessageLogger item in messageLoggers )
                {
                    if ( item.logger == logger ) { return item; }
                }

                return null;
            }
        }
    }



    private sealed class Scope : IDisposable
    {
        private readonly IDisposable?[]? _disposable;
        private          bool            _isDisposed;
        private          IDisposable?    _disposable0;
        private          IDisposable?    _disposable1;

        public Scope( int count )
        {
            if ( count > 2 ) { _disposable = new IDisposable[count - 2]; }
        }

        public void SetDisposable( int index, IDisposable? disposable )
        {
            switch ( index )
            {
                case 0:  _disposable0            = disposable; break;
                case 1:  _disposable1            = disposable; break;
                default: _disposable![index - 2] = disposable; break;
            }
        }

        public void Dispose()
        {
            if ( _isDisposed ) { return; }

            _disposable0?.Dispose();
            _disposable1?.Dispose();
            ReadOnlySpan<IDisposable?> span = _disposable;
            foreach ( IDisposable? disposable in span ) { disposable?.Dispose(); }

            _isDisposed = true;
        }
    }
}



[DebuggerDisplay( "{DebuggerToString(),nq}" )]
internal sealed class LoggerProviderDebugView( string providerName, MessageLogger? messageLogger )
{
    private readonly LogLevel _logLevel = CalculateEnabledLogLevel( messageLogger ) ?? LogLevel.None;
    private readonly string   _name     = providerName;

    private static LogLevel? CalculateEnabledLogLevel( MessageLogger? logger )
    {
        if ( logger is null ) { return null; }

        ReadOnlySpan<LogLevel> logLevels = [LogLevel.Critical, LogLevel.Error, LogLevel.Warning, LogLevel.Information, LogLevel.Debug, LogLevel.Trace];

        LogLevel? minimumLevel = null;

        // Check log level from highest to lowest. Report the lowest log level.
        foreach ( LogLevel logLevel in logLevels )
        {
            if ( logger.Value.IsEnabled( logLevel ) is false ) { break; }

            minimumLevel = logLevel;
        }

        return minimumLevel;
    }

    private string DebuggerToString() => $"""Name = "{_name}", LogLevel = {_logLevel}""";
}



public static class DebuggerDisplayFormatting
{
    internal static string DebuggerToString( string name, ILogger logger )
    {
        LogLevel? minimumLevel = CalculateEnabledLogLevel( logger );

        string debugText = $"""
                            Name = "{name}"
                            """;

        if ( minimumLevel != null ) { debugText += $", MinLevel = {minimumLevel}"; }
        else
        {
            // Display "Enabled = false". This makes it clear that the entire ILogger
            // is disabled and nothing is written.
            //
            // If "MinLevel = None" was displayed then someone could think that the
            // min level is disabled and everything is written.
            debugText += ", Enabled = false";
        }

        return debugText;
    }

    internal static LogLevel? CalculateEnabledLogLevel( ILogger logger )
    {
        ReadOnlySpan<LogLevel> logLevels = [LogLevel.Critical, LogLevel.Error, LogLevel.Warning, LogLevel.Information, LogLevel.Debug, LogLevel.Trace];

        LogLevel? minimumLevel = null;

        // Check log level from highest to lowest. Report the lowest log level.
        foreach ( LogLevel logLevel in logLevels )
        {
            if ( logger.IsEnabled( logLevel ) is false ) { break; }

            minimumLevel = logLevel;
        }

        return minimumLevel;
    }
}



public static class ProviderAliasUtilities
{
    private const string ALIAS_ATTRIBUTE_TYPE_FULL_NAME = "Microsoft.Extensions.Logging.ProviderAliasAttribute";

    internal static string? GetAlias( Type providerType )
    {
        IList<CustomAttributeData> attributes = CustomAttributeData.GetCustomAttributes( providerType );

        for ( int i = 0; i < attributes.Count; i++ )
        {
            CustomAttributeData attributeData = attributes[i];

            if ( attributeData.AttributeType.FullName == ALIAS_ATTRIBUTE_TYPE_FULL_NAME && attributeData.ConstructorArguments.Count > 0 )
            {
                CustomAttributeTypedArgument arg = attributeData.ConstructorArguments[0];

                Debug.Assert( arg.ArgumentType == typeof(string) );

                return arg.Value?.ToString();
            }
        }

        return null;
    }
}
