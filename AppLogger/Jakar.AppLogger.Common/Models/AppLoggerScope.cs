// Jakar.AppLogger :: Jakar.AppLogger.Common
// 09/08/2022  11:03 PM

namespace Jakar.AppLogger.Common;


public sealed class AppLoggerScope<TState> : IScope<TState>
{
    private readonly IScopeID _scope;
    public           TState   State { get; }


    public AppLoggerScope( IScopeID scope, TState state )
    {
        State          = state;
        _scope         = scope;
        _scope.ScopeID = Guid.NewGuid();
    }
    public void Dispose() => _scope.ScopeID = default;


    public static implicit operator TState( AppLoggerScope<TState> scope ) => scope.State;
}
