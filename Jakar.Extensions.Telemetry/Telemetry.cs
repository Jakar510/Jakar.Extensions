using System.Linq;
using System.Reflection;


// using static Jakar.Extensions.Telemetry.TelemetryLogger;



namespace Jakar.Extensions.Telemetry;



// OpenTelemetry TelemetryLogger


/*
public class Telemetry<TApp>( IEnumerable<ILoggerProvider> providers, IOptionsMonitor<LoggerFilterOptions> filterOption, IOptions<LoggerFactoryOptions>? options = null, IExternalScopeProvider? scopeProvider = null ) : ILoggerFactory
    where TApp : IAppID
{
    private readonly ConcurrentDictionary<string, ILogger> _loggers = new(StringComparer.Ordinal);
    public           ActivityCollection                    Collection { get; } = ActivityCollection.Create<TApp>();


    public static Telemetry<TApp> Create( IServiceProvider provider ) =>
        Create( provider.GetRequiredService<IEnumerable<ILoggerProvider>>(), provider.GetRequiredService<IOptionsMonitor<LoggerFilterOptions>>(), provider.GetService<IOptions<LoggerFactoryOptions>>(), provider.GetService<IExternalScopeProvider>() );

    public static Telemetry<TApp> Create( IEnumerable<ILoggerProvider> providers, IOptionsMonitor<LoggerFilterOptions> filterOption, IOptions<LoggerFactoryOptions>? options = null, IExternalScopeProvider? scopeProvider = null ) => new(providers, filterOption, options, scopeProvider);
    public static Telemetry<TApp> Get( IServiceProvider                provider ) => provider.GetRequiredService<Telemetry<TApp>>();
    public void Dispose()
    {
        Collection.Dispose();

        // _loggers.Values.ForEach( logger => logger.Dispose() );
        GC.SuppressFinalize( this );
    }


    public  ILogger CreateLogger( string         categoryName ) => _loggers.GetOrAdd( categoryName, DoCreateLogger );
    private ILogger DoCreateLogger( string       categoryName ) { return new TelemetryLogger( categoryName ); }
    public  void    AddProvider( ILoggerProvider provider )     { }
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



[DebuggerDisplay( "{DebuggerToString(),nq}" ), DebuggerTypeProxy( typeof(LoggerDebugView) ), SuppressMessage( "ReSharper", "ForCanBeConvertedToForeach" )]
public sealed class TelemetryLogger( string categoryName, params LoggerInformation[] loggers ) : ILogger
{
    private readonly string _categoryName = categoryName;

    public LoggerInformation[] Loggers        { get; set; } = loggers;
    public MessageLogger[]?    MessageLoggers { get; set; }
    public ScopeLogger[]?      ScopeLoggers   { get; set; }

    public void Log<TState>( LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter )
    {
        MessageLogger[]? loggers = MessageLoggers;
        if ( loggers is null ) { return; }

        List<Exception>? exceptions = null;

        for ( int i = 0; i < loggers.Length; i++ )
        {
            ref readonly MessageLogger loggerInfo = ref loggers[i];
            if ( !loggerInfo.IsEnabled( logLevel ) ) { continue; }

            LoggerLog( logLevel, eventId, loggerInfo.Logger, exception, formatter, ref exceptions, state );
        }

        if ( exceptions is not null && exceptions.Count > 0 ) { ThrowLoggingError( exceptions ); }

        static void LoggerLog( LogLevel logLevel, EventId eventId, ILogger logger, Exception? exception, Func<TState, Exception?, string> formatter, ref List<Exception>? exceptions, in TState state )
        {
            try { logger.Log( logLevel, eventId, state, exception, formatter ); }
            catch ( Exception ex )
            {
                exceptions ??= new List<Exception>();
                exceptions.Add( ex );
            }
        }
    }

    public bool IsEnabled( LogLevel logLevel )
    {
        MessageLogger[]? loggers = MessageLoggers;
        if ( loggers is null ) { return false; }

        List<Exception>? exceptions = null;
        int              i          = 0;

        for ( ; i < loggers.Length; i++ )
        {
            ref readonly MessageLogger loggerInfo = ref loggers[i];
            if ( !loggerInfo.IsEnabled( logLevel ) ) { continue; }

            if ( LoggerIsEnabled( logLevel, loggerInfo.Logger, ref exceptions ) ) { break; }
        }

        if ( exceptions is not null && exceptions.Count > 0 ) { ThrowLoggingError( exceptions ); }

        return i < loggers.Length
                   ? true
                   : false;

        static bool LoggerIsEnabled( LogLevel logLevel, ILogger logger, ref List<Exception>? exceptions )
        {
            try
            {
                if ( logger.IsEnabled( logLevel ) ) { return true; }
            }
            catch ( Exception ex )
            {
                exceptions ??= new List<Exception>();
                exceptions.Add( ex );
            }

            return false;
        }
    }

    public IDisposable? BeginScope<TState>( TState state )
        where TState : notnull
    {
        ScopeLogger[]? loggers = ScopeLoggers;

        if ( loggers is null ) { return NullScope.Instance; }

        if ( loggers.Length == 1 ) { return loggers[0].CreateScope( state ); }

        var              scope      = new Scope( loggers.Length );
        List<Exception>? exceptions = null;

        for ( int i = 0; i < loggers.Length; i++ )
        {
            ref readonly ScopeLogger scopeLogger = ref loggers[i];

            try { scope.SetDisposable( i, scopeLogger.CreateScope( state ) ); }
            catch ( Exception ex )
            {
                exceptions ??= new List<Exception>();
                exceptions.Add( ex );
            }
        }

        if ( exceptions is not null && exceptions.Count > 0 ) { ThrowLoggingError( exceptions ); }

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


        private List<object?>? GetScopes()
        {
            IExternalScopeProvider? scopeProvider = _logger.ScopeLoggers?.FirstOrDefault().externalScopeProvider;
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
                string         providerName  = ProviderAliasUtilities.GetAlias( loggerInfo.ProviderType ) ?? loggerInfo.ProviderType.Name;
                MessageLogger? messageLogger = FirstOrNull( in messages, loggerInfo.Logger );
                providers.Add( new LoggerProviderDebugView( providerName, messageLogger ) );
            }

            return providers;

            // Find message logger or return null if there is no match. FirstOrDefault isn't used because MessageLogger is a struct.
            static MessageLogger? FirstOrNull( ref readonly ReadOnlySpan<MessageLogger> messageLoggers, ILogger logger )
            {
                if ( messageLoggers.IsEmpty ) { return null; }

                foreach ( MessageLogger item in messageLoggers )
                {
                    if ( item.Logger == logger ) { return item; }
                }

                return null;
            }
        }
    }



    public readonly struct LoggerInformation( ILoggerProvider provider, string category )
    {
        public ILogger Logger        { get; } = provider.CreateLogger( category );
        public string  Category      { get; } = category;
        public Type    ProviderType  { get; } = provider.GetType();
        public bool    ExternalScope { get; } = provider is ISupportExternalScope;
    }



    [DebuggerDisplay( "{DebuggerToString(),nq}" )]
    private sealed class LoggerProviderDebugView( string providerName, MessageLogger? messageLogger )
    {
        private readonly MessageLogger? _messageLogger = messageLogger;
        public           LogLevel       LogLevel => CalculateEnabledLogLevel( _messageLogger ) ?? LogLevel.None;
        public           string         Name     { get; } = providerName;

        private static LogLevel? CalculateEnabledLogLevel( MessageLogger? logger )
        {
            if ( logger is null ) { return null; }

            ReadOnlySpan<LogLevel> logLevels = [LogLevel.Critical, LogLevel.Error, LogLevel.Warning, LogLevel.Information, LogLevel.Debug, LogLevel.Trace];

            LogLevel? minimumLevel = null;

            // Check log level from highest to lowest. Report the lowest log level.
            foreach ( LogLevel logLevel in logLevels )
            {
                if ( !logger.Value.IsEnabled( logLevel ) ) { break; }

                minimumLevel = logLevel;
            }

            return minimumLevel;
        }

        private string DebuggerToString() => $"""Name = "{Name}", LogLevel = {LogLevel}""";
    }



    public readonly struct MessageLogger( ILogger logger, string category, string? providerTypeFullName, LogLevel? minLevel, Func<string?, string?, LogLevel, bool>? filter )
    {
        public  ILogger                                 Logger                { get; } = logger;
        public  string                                  Category              { get; } = category;
        private string?                                 _ProviderTypeFullName { get; } = providerTypeFullName;
        public  LogLevel?                               MinLevel              { get; } = minLevel;
        public  Func<string?, string?, LogLevel, bool>? Filter                { get; } = filter;


        public bool IsEnabled( LogLevel level )
        {
            if ( level < MinLevel ) { return false; }

            if ( Filter is not null ) { return Filter( _ProviderTypeFullName, Category, level ); }

            return true;
        }
    }



    private sealed class Scope : IDisposable
    {
        private readonly IDisposable?[]? _disposable;
        private          bool            _isDisposed;

        private IDisposable? _disposable0;
        private IDisposable? _disposable1;

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
            if ( !_isDisposed )
            {
                _disposable0?.Dispose();
                _disposable1?.Dispose();

                if ( _disposable is not null )
                {
                    int count = _disposable.Length;
                    for ( int index = 0; index != count; ++index ) { _disposable[index]?.Dispose(); }
                }

                _isDisposed = true;
            }
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
}



/// <summary> An empty scope without any logic </summary>
public sealed class NullScope : IDisposable
{
    public static readonly NullScope Instance = new();

    private NullScope() { }

    /// <inheritdoc/>
    public void Dispose() { }
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
*/
