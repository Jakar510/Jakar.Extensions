// Jakar.Extensions :: Jakar.Extensions.Serilog
// 02/11/2025  11:02

namespace Jakar.Extensions;


public sealed class ScopeProvider : IExternalScopeProvider
{
    private static readonly Lazy<ScopeProvider> _service      = new(static () => new ScopeProvider());
    private readonly        AsyncLocal<Scope?>  _currentScope = new();


    public void ForEachScope<TState>( Action<object?, TState> callback, TState state )
    {
        Report( _currentScope.Value );
        return;

        void Report( Scope? current )
        {
            if ( current is null ) { return; }

            Report( current.parent );
            callback( current.state, state );
        }
    }

    public IDisposable Push( object? state )
    {
        Scope? parent   = _currentScope.Value;
        Scope  newScope = new(this, state, parent);
        _currentScope.Value = newScope;

        return newScope;
    }



    private sealed class Scope( ScopeProvider provider, object? state, Scope? parent ) : IDisposable
    {
        public readonly  object?       state     = state;
        public readonly  Scope?        parent    = parent;
        private readonly ScopeProvider _provider = provider;
        private          bool          _isDisposed;

        public override string? ToString() => state?.ToString();
        public void Dispose()
        {
            if ( _isDisposed ) { return; }

            _provider._currentScope.Value = parent;
            _isDisposed                   = true;
        }
    }
}



public sealed class ScopeProvider<TValue> : IExternalScopeProvider
{
    private static readonly Lazy<ScopeProvider<TValue>> _service      = new(static () => new ScopeProvider<TValue>());
    private readonly        AsyncLocal<Scope?>          _currentScope = new();


    public void ForEachScope<TState>( Action<object?, TState> callback, TState state )
    {
        Report( _currentScope.Value );
        return;

        void Report( Scope? current )
        {
            if ( current is null ) { return; }

            Report( current.parent );
            callback( current.state, state );
        }
    }

    IDisposable IExternalScopeProvider.Push( object? state ) => Push( (TValue?)state );
    public IDisposable Push( TValue? state )
    {
        Scope? parent   = _currentScope.Value;
        Scope  newScope = new(this, state, parent);
        _currentScope.Value = newScope;

        return newScope;
    }



    private sealed class Scope( ScopeProvider<TValue> provider, TValue? state, Scope? parent ) : IDisposable
    {
        private readonly ScopeProvider<TValue> _provider = provider;
        public readonly  Scope?                parent    = parent;
        public readonly  TValue?               state     = state;
        private          bool                  _isDisposed;


        public override string? ToString() => state?.ToString();
        public void Dispose()
        {
            if ( _isDisposed ) { return; }

            _provider._currentScope.Value = parent;
            _isDisposed                   = true;
        }
    }
}
