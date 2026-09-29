// Jakar.Extensions :: Jakar.Extensions.Serilog
// 02/11/2025  11:02

namespace Jakar.Extensions;


public sealed class ScopeProvider : IExternalScopeProvider
{
    private static readonly Lazy<ScopeProvider> __service      = new(static () => new ScopeProvider());
    private readonly        AsyncLocal<Scope?>  __currentScope = new();


    public void ForEachScope<TState>( Action<object?, TState> callback, TState state )
    {
        report(__currentScope.Value);
        return;

        void report( Scope? current )
        {
            if ( current is null ) { return; }

            report(current.parent);
            callback(current.state, state);
        }
    }

    public IDisposable Push( object? state )
    {
        Scope? parent   = __currentScope.Value;
        Scope  newScope = new(this, state, parent);
        __currentScope.Value = newScope;

        return newScope;
    }



    private sealed class Scope( ScopeProvider provider, object? state, Scope? parent ) : IDisposable
    {
        public readonly  object?       state      = state;
        public readonly  Scope?        parent     = parent;
        private readonly ScopeProvider __provider = provider;
        private          bool          __isDisposed;

        public override string? ToString() => state?.ToString();
        public void Dispose()
        {
            if ( __isDisposed ) { return; }

            __provider.__currentScope.Value = parent;
            __isDisposed                    = true;
        }
    }
}



public sealed class ScopeProvider<TValue> : IExternalScopeProvider
{
    private static readonly Lazy<ScopeProvider<TValue>> __service      = new(static () => new ScopeProvider<TValue>());
    private readonly        AsyncLocal<Scope?>          __currentScope = new();


    public void ForEachScope<TState>( Action<object?, TState> callback, TState state )
    {
        report(__currentScope.Value);
        return;

        void report( Scope? current )
        {
            if ( current is null ) { return; }

            report(current.parent);
            callback(current.state, state);
        }
    }

    IDisposable IExternalScopeProvider.Push( object? state ) => Push((TValue?)state);
    public IDisposable Push( TValue? state )
    {
        Scope? parent   = __currentScope.Value;
        Scope  newScope = new(this, state, parent);
        __currentScope.Value = newScope;

        return newScope;
    }



    private sealed class Scope( ScopeProvider<TValue> provider, TValue? state, Scope? parent ) : IDisposable
    {
        private readonly ScopeProvider<TValue> __provider = provider;
        public readonly  Scope?                parent     = parent;
        public readonly  TValue?               state      = state;
        private          bool                  __isDisposed;


        public override string? ToString() => state?.ToString();
        public void Dispose()
        {
            if ( __isDisposed ) { return; }

            __provider.__currentScope.Value = parent;
            __isDisposed                    = true;
        }
    }
}
