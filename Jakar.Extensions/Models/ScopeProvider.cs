// Jakar.Extensions :: Jakar.Extensions.Serilog
// 02/11/2025  11:02

using Microsoft.Extensions.Logging;



namespace Jakar.Extensions.Serilog;


public sealed class ScopeProvider : IExternalScopeProvider
{
    private readonly AsyncLocal<Scope?> _currentScope = new();


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
        private readonly ScopeProvider _provider = provider;
        private          bool          _isDisposed;
        public readonly  Scope?        parent = parent;
        public readonly  object?       state  = state;

        public override string? ToString() => state?.ToString();
        public void Dispose()
        {
            if ( _isDisposed ) { return; }

            _provider._currentScope.Value = parent;
            _isDisposed                   = true;
        }
    }
}



public sealed class ScopeProvider<T> : IExternalScopeProvider
{
    private readonly AsyncLocal<Scope?> _currentScope = new();


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

    IDisposable IExternalScopeProvider.Push( object? state ) => Push( (T?)state );
    public IDisposable Push( T? state )
    {
        Scope? parent   = _currentScope.Value;
        Scope  newScope = new(this, state, parent);
        _currentScope.Value = newScope;

        return newScope;
    }



    private sealed class Scope( ScopeProvider<T> provider, T? state, Scope? parent ) : IDisposable
    {
        private readonly ScopeProvider<T> _provider = provider;
        private          bool             _isDisposed;
        public readonly  Scope?           parent = parent;
        public readonly  T?               state  = state;


        public override string? ToString() => state?.ToString();
        public void Dispose()
        {
            if ( _isDisposed ) { return; }

            _provider._currentScope.Value = parent;
            _isDisposed                   = true;
        }
    }
}
