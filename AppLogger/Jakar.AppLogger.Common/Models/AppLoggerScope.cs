// Jakar.AppLogger :: Jakar.AppLogger.Common
// 09/08/2022  11:03 PM

namespace Jakar.AppLogger.Common;


public readonly record struct AppLoggerScope<TState> : IScope<TState>, IScopeID
{
    private readonly LoggingSettings _settings;
    public           Guid            ScopeID { get; } = Guid.NewGuid();
    public           TState          State   { get; }


    public AppLoggerScope( LoggingSettings settings, TState state )
    {
        _settings = settings;
        State     = state;
        _settings.ScopeIDs.Add( ScopeID );
    }
    public void Dispose() => _settings.ScopeIDs.Remove( ScopeID );


    public static implicit operator TState( AppLoggerScope<TState> scope ) => scope.State;
}
