// Jakar.Extensions :: Jakar.Extensions
// 05/17/2022  4:16 PM

using Microsoft.Extensions.Hosting;



namespace Jakar.Extensions;


public abstract class Service : ObservableClass, IAsyncDisposable, IValidator
{
    private readonly Synchronized<bool> _isAlive = new(false);


    public virtual bool IsAlive
    {
        get => _isAlive.Value;
        protected set
        {
            _isAlive.Value = value;
            OnPropertyChanged();
            OnPropertyChanged( nameof(IsValid) );
        }
    }
    public virtual bool   IsValid   => IsAlive;
    public         string ClassName { get; }
    public         string FullName  { get; }


    protected Service()
    {
        Type type = GetType();
        ClassName = type.Name;
        FullName  = type.AssemblyQualifiedName ?? type.FullName ?? ClassName;
    }


    public virtual ValueTask DisposeAsync() => default;


    public static Task Delay( in double   days,    in CancellationToken token ) => Delay( TimeSpan.FromDays( days ),       token );
    public static Task Delay( in float    minutes, in CancellationToken token ) => Delay( TimeSpan.FromMinutes( minutes ), token );
    public static Task Delay( in long     seconds, in CancellationToken token ) => Delay( TimeSpan.FromSeconds( seconds ), token );
    public static Task Delay( in int      ms,      in CancellationToken token ) => Delay( TimeSpan.FromMilliseconds( ms ), token );
    public static Task Delay( in TimeSpan delay,   in CancellationToken token ) => delay.Delay( token );


#if NET6_0_OR_GREATER
    [StackTraceHidden]
#endif
    [ DoesNotReturn ] protected virtual void ThrowDisabled( Exception? inner = default, [ CallerMemberName ] string? caller = default ) => throw new ApiDisabledException( $"{ClassName}.{caller}", inner );


#if NET6_0_OR_GREATER
    [StackTraceHidden]
#endif
    [ DoesNotReturn ] protected void ThrowDisposed( Exception? inner = default, [ CallerMemberName ] string? caller = default ) => throw new ObjectDisposedException( $"{ClassName}.{caller}", inner );
}



public abstract class HostedService : Service, IHostedService
{
    public abstract Task StartAsync( CancellationToken token );
    public abstract Task StopAsync( CancellationToken  token );
}
