// Jakar.Extensions :: Jakar.Extensions
// 09/25/2025  19:15

namespace Jakar.Extensions;


public class Observer<TValue> : IObservable<TValue>
{
    private ImmutableArray<ObserverWrapper> __observers = ImmutableArray<ObserverWrapper>.Empty;


    public virtual IDisposable Subscribe( IObserver<TValue> observer )
    {
        ObserverWrapper listener = new(observer, this);
        ImmutableInterlocked.Update(ref __observers, list => list.Add(listener));
        return listener;
    }
    protected virtual void Remove( ObserverWrapper listener ) => ImmutableInterlocked.Update(ref __observers, list => list.Remove(listener));


    protected void OnNext( TValue value )
    {
        ImmutableArray<ObserverWrapper> observers = __observers;
        foreach ( ObserverWrapper listener in observers.AsSpan() ) { listener.OnNext(value); }
    }



    protected class ObserverWrapper( IObserver<TValue> observer, Observer<TValue> parent ) : IObserver<TValue>, IDisposable
    {
        protected readonly Observer<TValue>  __parent   = parent;
        protected readonly IObserver<TValue> __observer = observer;


        public virtual void Dispose()                  => __parent.Remove(this);
        public virtual void OnCompleted()              => __observer.OnCompleted();
        public virtual void OnError( Exception error ) => __observer.OnError(error);
        public virtual void OnNext( TValue     value ) => __observer.OnNext(value);
    }
}
