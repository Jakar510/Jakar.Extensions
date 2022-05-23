// Jakar.Extensions :: Jakar.Extensions
// 05/17/2022  4:16 PM

namespace Jakar.Extensions.Models;


public abstract class Service : ObservableClass, IDisposable, IAsyncDisposable, IValidator
{
    private readonly Synchronized<bool> _isAlive = new(false);
    public           string             ClassName => ClassType.Name;
    public           string             FullName  { get; }
    public           Type               ClassType { get; }
    public virtual   bool               IsValid   => IsAlive;


    public virtual bool IsAlive
    {
        get => _isAlive.Value;
        protected set
        {
            _isAlive.Value = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsValid));
        }
    }


    protected Service()
    {
        ClassType = GetType();
        FullName  = ClassType.AssemblyQualifiedName ?? ClassType.FullName ?? ClassName;
    }
    protected abstract void Dispose( bool disposing );
    public abstract ValueTask DisposeAsync();
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }


    // StackTraceHidden
    [DoesNotReturn] protected virtual void ThrowDisabled( [CallerMemberName] string? caller                                   = default ) => throw new InvalidOperationException($"{ClassName}.{caller}");
    [DoesNotReturn] protected virtual void ThrowDisabled( Exception                  inner, [CallerMemberName] string? caller = default ) => throw new InvalidOperationException($"{ClassName}.{caller}", inner);
    [DoesNotReturn] protected void ThrowDisposed( [CallerMemberName] string?         caller                                   = default ) => throw new ObjectDisposedException($"{ClassName}.{caller}");
    [DoesNotReturn] protected void ThrowDisposed( Exception                          inner, [CallerMemberName] string? caller = default ) => throw new ObjectDisposedException($"{ClassName}.{caller}", inner);


    // public abstract void Register( in WebApplicationBuilder builder );


    public static Task Delay( in double   days,    in CancellationToken token ) => days.Delay(token);
    public static Task Delay( in float    minutes, in CancellationToken token ) => minutes.Delay(token);
    public static Task Delay( in long     seconds, in CancellationToken token ) => seconds.Delay(token);
    public static Task Delay( in int      ms,      in CancellationToken token ) => ms.Delay(token);
    public static Task Delay( in TimeSpan delay,   in CancellationToken token ) => delay.Delay(token);
}
