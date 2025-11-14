// Jakar.Extensions :: Jakar.Extensions
// 11/03/2025  10:57

namespace Jakar.Extensions;


/// <summary> Weak event manager that allows for garbage collection when the EventHandler is still subscribed </summary>
public sealed class WeakEventManager : IDisposable
{
    private readonly ConcurrentDictionary<string, List<WeakReference<Delegate>>> _eventHandlers = new();


    private List<WeakReference<Delegate>> this[ string eventName ] => _eventHandlers.GetOrAdd(eventName, static x => new List<WeakReference<Delegate>>(16));
    public void Dispose() => _eventHandlers.Clear();


    private void Add_Subscription( in string eventName, Delegate? handler )
    {
        ArgumentNullException.ThrowIfNull(eventName);
        if ( handler is null ) { return; }

        List<WeakReference<Delegate>> list = this[eventName];
        lock ( list ) { list.Add(new WeakReference<Delegate>(handler)); }
    }
    private void Remove_Subscription( string eventName, Delegate? handler )
    {
        ArgumentNullException.ThrowIfNull(eventName);
        if ( handler is null ) { return; }

        List<WeakReference<Delegate>> list = this[eventName];

        lock ( list )
        {
            list.RemoveAll(reference =>
                           {
                               if ( reference.TryGetTarget(out Delegate? target) ) { return target == handler; }

                               return false;
                           });
        }
    }


    public void AddEventHandler( NotifyCollectionChangedEventHandler?  handler, [CallerMemberName] string eventName = EMPTY ) => Add_Subscription(eventName, handler);
    public void AddEventHandler( PropertyChangedEventHandler?          handler, [CallerMemberName] string eventName = EMPTY ) => Add_Subscription(eventName, handler);
    public void AddEventHandler( PropertyChangingEventHandler?         handler, [CallerMemberName] string eventName = EMPTY ) => Add_Subscription(eventName, handler);
    public void AddEventHandler( EventHandler?                         handler, [CallerMemberName] string eventName = EMPTY ) => Add_Subscription(eventName, handler);
    public void AddEventHandler( Action?                               handler, [CallerMemberName] string eventName = EMPTY ) => Add_Subscription(eventName, handler);
    public void AddEventHandler<TEventArgs>( EventHandler<TEventArgs>? handler, [CallerMemberName] string eventName = EMPTY ) => Add_Subscription(eventName, handler);
    public void AddEventHandler<TEventArgs>( Action<TEventArgs>?       handler, [CallerMemberName] string eventName = EMPTY ) => Add_Subscription(eventName, handler);


    public void RemoveEventHandler( NotifyCollectionChangedEventHandler?  handler, [CallerMemberName] string eventName = EMPTY ) => Remove_Subscription(eventName, handler);
    public void RemoveEventHandler( PropertyChangedEventHandler?          handler, [CallerMemberName] string eventName = EMPTY ) => Remove_Subscription(eventName, handler);
    public void RemoveEventHandler( PropertyChangingEventHandler?         handler, [CallerMemberName] string eventName = EMPTY ) => Remove_Subscription(eventName, handler);
    public void RemoveEventHandler( EventHandler?                         handler, [CallerMemberName] string eventName = EMPTY ) => Remove_Subscription(eventName, handler);
    public void RemoveEventHandler( Action?                               handler, [CallerMemberName] string eventName = EMPTY ) => Remove_Subscription(eventName, handler);
    public void RemoveEventHandler<TEventArgs>( EventHandler<TEventArgs>? handler, [CallerMemberName] string eventName = EMPTY ) => Remove_Subscription(eventName, handler);
    public void RemoveEventHandler<TEventArgs>( Action<TEventArgs>?       handler, [CallerMemberName] string eventName = EMPTY ) => Remove_Subscription(eventName, handler);


    public void RaiseEvent<TSender>( TSender? sender, in string eventName )
        where TSender : class => RaiseEvent(sender, EventArgs.Empty, in eventName);
    public void RaiseEvent<TSender, TEventArgs>( TSender? sender, TEventArgs args, in string eventName )
        where TSender : class
    {
        ArgumentNullException.ThrowIfNull(eventName);
        if ( !_eventHandlers.TryGetValue(eventName, out List<WeakReference<Delegate>>? list) ) { return; }

        lock ( list )
        {
            using ArrayBuffer<WeakReference<Delegate>> buffer = new(list.Count);

            foreach ( ref readonly WeakReference<Delegate> method in list.AsSpan() )
            {
                if ( method.TryGetTarget(out Delegate? target) ) { Execute(target, sender, args); }
                else { buffer.Add(in method); }
            }

            foreach ( WeakReference<Delegate> value in buffer ) { list.Remove(value); }
        }
    }


    private static void Execute<TSender, TEventArgs>( Delegate target, TSender? sender, TEventArgs args )
        where TSender : class
    {
        ArgumentNullException.ThrowIfNull(target);

        switch ( target )
        {
            case EventHandler<TEventArgs> handler:
                handler(sender, args);
                return;

            case Action<TEventArgs> handler:
                handler(args);
                return;

            case Action<TSender?, TEventArgs> handler:
                handler(sender, args);
                return;

            case Func<TEventArgs, Task> handler:
                handler(args)
                   .CallSynchronously();

                return;

            case Func<TEventArgs, ValueTask> handler:
                handler(args)
                   .CallSynchronously();

                return;

            case Func<object?, TEventArgs, Task> handler:
                handler(sender, args)
                   .CallSynchronously();

                return;

            case Func<object?, TEventArgs, ValueTask> handler:
                handler(sender, args)
                   .CallSynchronously();

                return;

            case Func<TSender?, TEventArgs, Task> handler:
                handler(sender, args)
                   .CallSynchronously();

                return;

            case Func<TSender?, TEventArgs, ValueTask> handler:
                handler(sender, args)
                   .CallSynchronously();

                return;

            case NotifyCollectionChangedEventHandler handler when args is NotifyCollectionChangedEventArgs e:
                handler(sender, e);
                return;

            case PropertyChangedEventHandler handler when args is PropertyChangedEventArgs e:
                handler(sender, e);
                return;

            case PropertyChangingEventHandler handler when args is PropertyChangingEventArgs e:
                handler(sender, e);
                return;

            case EventHandler handler when args is EventArgs e:
                handler(sender, e);
                return;

            case EventHandler handler:
                handler(sender, EventArgs.Empty);
                return;

            case Action handler:
                handler();
                return;

            default:
                invokeDynamic(target, sender, args);
                return;

                // Safe fallback for any custom delegate (nonstandard signature)
                static void invokeDynamic( Delegate target, TSender? sender, TEventArgs args )
                {
                    ParameterInfo[] parameters = target.Method.GetParameters();

                    // Try to match common parameter count patterns
                    object?[] arguments = parameters.Length switch
                                          {
                                              0 => [],
                                              1 => [args],
                                              2 => [sender, args],
                                              _ => buildDynamicArgumentArray(parameters, sender, args)
                                          };

                    object? result = target.DynamicInvoke(arguments);

                    switch ( result )
                    {
                        case Task task:
                            task.CallSynchronously();
                            return;

                        case ValueTask task:
                            task.CallSynchronously();
                            return;
                    }
                }

                static object?[] buildDynamicArgumentArray( ReadOnlySpan<ParameterInfo> parameters, TSender? sender, TEventArgs args )
                {
                    object?[] result = new object?[parameters.Length];

                    for ( int i = 0; i < parameters.Length; i++ )
                    {
                        Type type = parameters[i].ParameterType;

                        result[i] = i switch
                                    {
                                        0 when type.IsInstanceOfType(sender) => sender,
                                        1 when type.IsInstanceOfType(args)   => args,
                                        _ => type.IsValueType
                                                 ? Activator.CreateInstance(type)
                                                 : null
                                    };
                    }

                    return result;
                }
        }
    }
}



public sealed class WeakEventManagerAlternate
{
    private readonly Dictionary<string, List<Subscription>> __eventHandlers = new(StringComparer.Ordinal);


    public void AddEventHandler<TEventArgs>( EventHandler<TEventArgs> handler, [CallerMemberName] string eventName = EMPTY )
        where TEventArgs : EventArgs
    {
        if ( string.IsNullOrEmpty(eventName) ) { throw new ArgumentNullException(nameof(eventName)); }

        if ( handler is null ) { throw new ArgumentNullException(nameof(handler)); }

        AddEventHandler(eventName, handler.Target, handler.GetMethodInfo());
    }
    public void AddEventHandler( Delegate? handler, [CallerMemberName] string eventName = EMPTY )
    {
        if ( string.IsNullOrEmpty(eventName) ) { throw new ArgumentNullException(nameof(eventName)); }

        if ( handler == null ) { throw new ArgumentNullException(nameof(handler)); }

        AddEventHandler(eventName, handler.Target, handler.GetMethodInfo());
    }


    public void RemoveEventHandler<TEventArgs>( EventHandler<TEventArgs> handler, [CallerMemberName] string eventName = EMPTY )
        where TEventArgs : EventArgs
    {
        if ( string.IsNullOrEmpty(eventName) ) { throw new ArgumentNullException(nameof(eventName)); }

        if ( handler is null ) { throw new ArgumentNullException(nameof(handler)); }

        RemoveEventHandler(eventName, handler.Target, handler.GetMethodInfo());
    }
    public void RemoveEventHandler( Delegate? handler, [CallerMemberName] string eventName = EMPTY )
    {
        if ( string.IsNullOrEmpty(eventName) ) { throw new ArgumentNullException(nameof(eventName)); }

        if ( handler == null ) { throw new ArgumentNullException(nameof(handler)); }

        RemoveEventHandler(eventName, handler.Target, handler.GetMethodInfo());
    }


    private void AddEventHandler( string eventName, object? handlerTarget, MethodInfo methodInfo )
    {
        if ( !__eventHandlers.TryGetValue(eventName, out List<Subscription>? targets) ) { __eventHandlers.Add(eventName, targets = []); }

        if ( handlerTarget == null )
        {
            // This event handler is a static method
            targets.Add(new Subscription(null, methodInfo));
            return;
        }

        targets.Add(new Subscription(new WeakReference(handlerTarget), methodInfo));
    }
    private void RemoveEventHandler( string eventName, object? handlerTarget, MemberInfo methodInfo )
    {
        if ( !__eventHandlers.TryGetValue(eventName, out List<Subscription>? subscriptions) ) { return; }

        for ( int n = subscriptions.Count - 1; n >= 0; n-- )
        {
            Subscription current = subscriptions[n];

            if ( current.Subscriber is { IsAlive: false } )
            {
                // If not alive, remove and continue
                subscriptions.RemoveAt(n);
                continue;
            }

            if ( current.Subscriber?.Target == handlerTarget && current.Handler.Name == methodInfo.Name )
            {
                // Found the match, we can break
                subscriptions.RemoveAt(n);
                break;
            }
        }
    }


    public void HandleEvent( object? sender, object? args, string eventName )
    {
        List<(object? subscriber, MethodInfo handler)> toRaise  = [];
        List<Subscription>                             toRemove = [];

        if ( __eventHandlers.TryGetValue(eventName, out List<Subscription>? target) )
        {
            for ( int i = 0; i < target.Count; i++ )
            {
                Subscription subscription = target[i];
                bool         isStatic     = subscription.Subscriber is null;

                if ( isStatic )
                {
                    // For a static method, we'll just pass null as the first parameter of MethodInfo.Invoke
                    toRaise.Add(( null, subscription.Handler ));
                    continue;
                }

                object? subscriber = subscription.Subscriber?.Target;

                if ( subscriber == null )

                    // The subscriber was collected, so there's no need to keep this subscription around
                {
                    toRemove.Add(subscription);
                }
                else { toRaise.Add(( subscriber, subscription.Handler )); }
            }

            for ( int i = 0; i < toRemove.Count; i++ )
            {
                Subscription subscription = toRemove[i];
                target.Remove(subscription);
            }
        }

        for ( int i = 0; i < toRaise.Count; i++ )
        {
            ( object? subscriber, MethodInfo handler ) = toRaise[i];
            handler.Invoke(subscriber, [sender, args]);
        }
    }



    private readonly struct Subscription( WeakReference? subscriber, MethodInfo handler ) : IEquatable<Subscription>
    {
        public readonly WeakReference? Subscriber = subscriber;
        public readonly MethodInfo     Handler    = handler ?? throw new ArgumentNullException(nameof(handler));

        public          bool Equals( Subscription other ) => Subscriber == other.Subscriber && Handler == other.Handler;
        public override bool Equals( object?      obj )   => obj is Subscription other      && Equals(other);
        public override int  GetHashCode()                => Subscriber?.GetHashCode() ?? 0 ^ Handler.GetHashCode();
    }
}
