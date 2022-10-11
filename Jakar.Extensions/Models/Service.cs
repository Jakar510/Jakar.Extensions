// Jakar.Extensions :: Jakar.Extensions
// 05/17/2022  4:16 PM

#nullable enable
namespace Jakar.Extensions;


public abstract class Service : ObservableClass, IDisposable, IAsyncDisposable, IValidator
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
    public         string ClassName => ClassType.Name;
    public         string FullName  { get; }
    public         Type   ClassType { get; }


    protected Service()
    {
        ClassType = GetType();
        FullName  = ClassType.AssemblyQualifiedName ?? ClassType.FullName ?? ClassName;
    }


    public static Task Delay( in double days, in CancellationToken token ) => TimeSpan.FromDays( days )
                                                                                      .Delay( token );
    public static Task Delay( in float minutes, in CancellationToken token ) => TimeSpan.FromMinutes( minutes )
                                                                                        .Delay( token );
    public static Task Delay( in long seconds, in CancellationToken token ) => TimeSpan.FromSeconds( seconds )
                                                                                       .Delay( token );
    public static Task Delay( in int ms, in CancellationToken token ) => TimeSpan.FromMilliseconds( ms )
                                                                                 .Delay( token );
    public static Task Delay( in TimeSpan delay, in CancellationToken token ) => delay.Delay( token );
    protected abstract void Dispose( bool disposing );


    // public abstract void Register( in WebApplicationBuilder builder );

#if !NETSTANDARD2_1
    [StackTraceHidden]
#endif
    [DoesNotReturn]
    protected virtual void ThrowDisabled( [CallerMemberName] string? caller = default ) => throw new InvalidOperationException( $"{ClassName}.{caller}" );

#if !NETSTANDARD2_1
    [StackTraceHidden]
#endif
    [DoesNotReturn]
    protected virtual void ThrowDisabled( Exception inner, [CallerMemberName] string? caller = default ) => throw new InvalidOperationException( $"{ClassName}.{caller}", inner );

#if !NETSTANDARD2_1
    [StackTraceHidden]
#endif
    [DoesNotReturn]
    protected void ThrowDisposed( [CallerMemberName] string? caller = default ) => throw new ObjectDisposedException( $"{ClassName}.{caller}" );

#if !NETSTANDARD2_1
    [StackTraceHidden]
#endif
    [DoesNotReturn]
    protected void ThrowDisposed( Exception inner, [CallerMemberName] string? caller = default ) => throw new ObjectDisposedException( $"{ClassName}.{caller}", inner );
    public abstract ValueTask DisposeAsync();
    public void Dispose()
    {
        Dispose( true );
        GC.SuppressFinalize( this );
    }
}
