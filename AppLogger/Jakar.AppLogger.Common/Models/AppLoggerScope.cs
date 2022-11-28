// Jakar.AppLogger :: Jakar.AppLogger.Common
// 09/08/2022  11:03 PM

namespace Jakar.AppLogger.Common;


public sealed class AppLoggerScope : IScope
{
    private readonly IScopeID _scope;


    public AppLoggerScope( IScopeID scope )
    {
        _scope         = scope;
        _scope.ScopeID = Guid.NewGuid();
    }
    public void Dispose() => _scope.ScopeID = default;
}
