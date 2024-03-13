// Jakar.Extensions :: Jakar.Extensions
// 11/10/2023  1:46 PM

namespace Jakar.Extensions;


/*

public static class EventManagerService
{
    private static readonly object _lock = new();
    public const            string EMPTY = "";


#if NET7_0_OR_GREATER
    [ RequiresDynamicCode( nameof(TryGetDynamicMethod) ) ]
#endif
    internal static bool TryGetDynamicMethod( this MethodInfo methodInfo, [ NotNullWhen( true ) ] out DynamicMethod? method )
    {
        method = GetMethod( methodInfo, typeof(DynamicMethod) );
        return method is not null;


    #if NET7_0_OR_GREATER
        [ RequiresDynamicCode( nameof(GetMethod) ) ]
    #endif
        static DynamicMethod? GetMethod( MethodInfo methodInfo,
                                     #if NET7_0_OR_GREATER
                                         [ DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.NonPublicNestedTypes | DynamicallyAccessedMemberTypes.PublicNestedTypes ) ]
                                     #endif
                                         Type type
        )
        {
            TypeInfo? typeInfo = type.GetTypeInfo().GetDeclaredNestedType( "RTDynamicMethod" );
            return GetDynamicMethod( methodInfo, typeInfo, typeInfo?.AsType() );
        }


    #if NET7_0_OR_GREATER
        [ RequiresDynamicCode( nameof(GetDynamicMethod) ) ]
    #endif
        static DynamicMethod? GetDynamicMethod( MethodInfo methodInfo,
                                                TypeInfo?  methodType,
                                            #if NET7_0_OR_GREATER
                                                [ DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.NonPublicFields | DynamicallyAccessedMemberTypes.PublicFields ) ]
                                            #endif
                                                Type? dynamicMethod
        )
        {
            return methodType?.IsAssignableFrom( methodInfo.GetType().GetTypeInfo() ) is true
                       ? (DynamicMethod?)dynamicMethod?.GetRuntimeFields().First( f => f.Name is "m_owner" ).GetValue( methodInfo )
                       : null;
        }
    }


    public static void AddEventHandler( this Dictionary<string, List<Subscription>> eventHandlers, in string eventName, in object? handlerTarget, in MethodInfo methodInfo )
    {
        lock (_lock)
        {
            if ( eventHandlers.TryGetValue( eventName, out List<Subscription>? targets ) is false ) { eventHandlers[eventName] = targets = new List<Subscription>(); }

            targets.Add( handlerTarget is null
                             ? new Subscription( null,                               methodInfo )
                             : new Subscription( new WeakReference( handlerTarget ), methodInfo ) );
        }
    }
    public static void RemoveEventHandler( this Dictionary<string, List<Subscription>> eventHandlers, in string eventName, in object? handlerTarget, in MemberInfo methodInfo )
    {
        lock (_lock)
        {
            if ( eventHandlers.TryGetValue( eventName, out List<Subscription>? subscriptions ) is false ) { return; }

            for ( int n = subscriptions.Count; n > 0; n-- )
            {
                Subscription current = subscriptions[n - 1];

                if ( current.Subscriber?.Target != handlerTarget || current.Handler.Name != methodInfo?.Name ) { continue; }

                subscriptions.Remove( current );
                break;
            }
        }
    }


    public static void HandleEvent( this Dictionary<string, List<Subscription>> eventHandlers, in string eventName, in object? sender, in object? eventArgs )
    {
        var toRaise = new Buffer<Target>( eventHandlers.Count );
        eventHandlers.AddRemoveEvents( eventName, ref toRaise );

        using ( toRaise )
        {
            const string MSG = "Parameter count mismatch. If invoking an `event Action` use `HandleEvent(string eventName)` or if invoking an `event Action<T>` use `HandleEvent(object eventArgs, string eventName)`instead.";

            ReadOnlySpan<Target> targets = toRaise.Span;
            targets.HandleEvent( MSG, sender, eventArgs );
        }
    }
    public static void HandleEvent( this Dictionary<string, List<Subscription>> eventHandlers, in string eventName, in object? actionEventArgs )
    {
        var toRaise = new Buffer<Target>( eventHandlers.Count );
        eventHandlers.AddRemoveEvents( eventName, ref toRaise );

        using ( toRaise )

        {
            const string MSG = "Parameter count mismatch. If invoking an `event EventHandler` use `HandleEvent(object sender, TEventArgs eventArgs, string eventName)` or if invoking an `event Action` use `HandleEvent(string eventName)`instead.";

            ReadOnlySpan<Target> targets = toRaise.Span;
            targets.HandleEvent( MSG, actionEventArgs );
        }
    }
    public static void HandleEvent( this Dictionary<string, List<Subscription>> eventHandlers, in string eventName )
    {
        var toRaise = new Buffer<Target>( eventHandlers.Count );
        eventHandlers.AddRemoveEvents( eventName, ref toRaise );

        using ( toRaise )
        {
            const string MSG =
                "Parameter count mismatch. If invoking an `event EventHandler` use `HandleEvent(object sender, TEventArgs eventArgs, string eventName)` or if invoking an `event Action<T>` use `HandleEvent(object eventArgs, string eventName)`instead.";

            ReadOnlySpan<Target> targets = toRaise.Span;
            targets.HandleEvent( MSG, null );
        }
    }
    private static void HandleEvent( this ReadOnlySpan<Target> targets, in string errorMsg, params object?[]? parameters )
    {
        foreach ( (object? instance, MethodInfo eventHandler) in targets )
        {
            try
            {
                if ( eventHandler.TryGetDynamicMethod( out DynamicMethod? method ) ) { method.Invoke( instance, parameters ); }
                else { eventHandler.Invoke( instance, parameters ); }
            }
            catch ( TargetParameterCountException e ) { throw new InvalidHandleEventException( errorMsg, e ); }
        }
    }


    private static void AddRemoveEvents<T>( this T eventHandlers, in string eventName, ref Buffer<Target> toRaise )
        where T : IReadOnlyDictionary<string, List<Subscription>>
    {
        using var toRemove = new Buffer<Subscription>( eventHandlers.Count );
        if ( eventHandlers.TryGetValue( eventName, out List<Subscription>? subscriptions ) is false ) { return; }

        // ReSharper disable once ForCanBeConvertedToForeach
        for ( int i = 0; i < subscriptions.Count; i++ )
        {
            Subscription subscription = subscriptions[i];

            if ( subscription.TryGetTarget( out Target? target ) ) { toRaise.Append( target.Value ); }
            else { toRemove.Append( subscription ); }
        }

        foreach ( Subscription subscription in toRemove.Span ) { subscriptions.Remove( subscription ); }
    }
    /*
    private static void AddRemoveEvents<T>( this T eventHandlers, in string eventName, out List<Target> toRaise )
        where T : IReadOnlyDictionary<string, List<Subscription>>
    {
        using var toRemove = new Buffer<Subscription>();
        toRaise = new List<Target>();

        if ( eventHandlers.TryGetValue( eventName, out List<Subscription>? target ) is false ) { return; }

        // ReSharper disable once ForCanBeConvertedToForeach
        for ( int i = 0; i < target.Count; i++ )
        {
            Subscription subscription = target[i];

            if ( subscription.Subscriber is null )
            {
                toRaise.Add( new Target( null, subscription.Handler ) );
                continue;
            }

            object? subscriber = subscription.Subscriber?.Target;

            if ( subscriber is null ) { toRemove.Append( subscription ); }
            else { toRaise.Add( new Target( subscriber, subscription.Handler ) ); }
        }

        foreach ( Subscription subscription in toRemove.Span ) { target.Remove( subscription ); }
    }
    #1#



    public readonly record struct Subscription( in WeakReference? Subscriber, in MethodInfo Handler )
    {
        internal bool TryGetTarget( [ NotNullWhen( true ) ] out Target? target )
        {
            if ( Subscriber is null )
            {
                target = new Target( null, Handler );
                return true;
            }

            object? subscriber = Subscriber?.Target;

            target = subscriber is null
                         ? null
                         : new Target( subscriber, Handler );

            return target is not null;
        }
    }



    internal readonly record struct Target( object? Instance, MethodInfo EventHandler )
    {
        public void HandleEvent( in string errorMsg, params object?[]? parameters )
        {
            try
            {
                if ( EventHandler.TryGetDynamicMethod( out DynamicMethod? method ) ) { method.Invoke( Instance, parameters ); }
                else { EventHandler.Invoke( Instance, parameters ); }
            }
            catch ( TargetParameterCountException e ) { throw new InvalidHandleEventException( errorMsg, e ); }
        }
    }



    public static void AddEventHandler<TEventArgs>( this Dictionary<string, List<Subscription<TEventArgs>>> eventHandlers, in string eventName, in object? handlerTarget, in OneOf<EventHandler<TEventArgs>, Action<TEventArgs>> handler )
    {
        lock (_lock)
        {
            if ( eventHandlers.TryGetValue( eventName, out List<Subscription<TEventArgs>>? targets ) is false ) { eventHandlers[eventName] = targets = new List<Subscription<TEventArgs>>(); }

            targets.Add( handlerTarget is null
                             ? new Subscription<TEventArgs>( null,                               handler )
                             : new Subscription<TEventArgs>( new WeakReference( handlerTarget ), handler ) );
        }
    }
    public static void RemoveEventHandler<TEventArgs>( this Dictionary<string, List<Subscription<TEventArgs>>> eventHandlers, in string eventName, in object? handlerTarget, in OneOf<EventHandler<TEventArgs>, Action<TEventArgs>> handler )
    {
        lock (_lock)
        {
            if ( eventHandlers.TryGetValue( eventName, out List<Subscription<TEventArgs>>? subscriptions ) is false ) { return; }

            for ( int n = subscriptions.Count; n > 0; n-- )
            {
                Subscription<TEventArgs> current = subscriptions[n - 1];
                if ( ReferenceEquals( current.Subscriber?.Target, handlerTarget ) is false ) { continue; }

                subscriptions.Remove( current );
                break;
            }
        }
    }


    public static void HandleEvent<TEventArgs>( this Dictionary<string, List<Subscription<TEventArgs>>> eventHandlers, in string eventName, TEventArgs arg, object? sender = null )
    {
        var toRaise = new Buffer<Target<TEventArgs>>( eventHandlers.Count );
        eventHandlers.AddRemoveEvents( eventName, ref toRaise );

        using ( toRaise )
        {
            const string MSG =
                "Parameter count mismatch. If invoking an `event EventHandler` use `HandleEvent(object sender, TEventArgs eventArgs, string eventName)` or if invoking an `event Action<T>` use `HandleEvent(object eventArgs, string eventName)`instead.";

            ReadOnlySpan<Target<TEventArgs>> targets = toRaise.Span;
            targets.HandleEvent( MSG, arg, sender );
        }
    }
    private static void HandleEvent<TEventArgs>( this ReadOnlySpan<Target<TEventArgs>> targets, in string errorMsg, TEventArgs arg, object? sender )
    {
        foreach ( Target<TEventArgs> target in targets ) { target.HandleEvent( errorMsg, arg, sender ); }
    }
    private static void AddRemoveEvents<T, TEventArgs>( this T eventHandlers, in string eventName, ref Buffer<Target<TEventArgs>> toRaise )
        where T : IReadOnlyDictionary<string, List<Subscription<TEventArgs>>>
    {
        using var toRemove = new Buffer<Subscription<TEventArgs>>( eventHandlers.Count );
        if ( eventHandlers.TryGetValue( eventName, out List<Subscription<TEventArgs>>? subscriptions ) is false ) { return; }

        // ReSharper disable once ForCanBeConvertedToForeach
        for ( int i = 0; i < subscriptions.Count; i++ )
        {
            Subscription<TEventArgs> subscription = subscriptions[i];

            if ( subscription.TryGetTarget( out Target<TEventArgs>? target ) ) { toRaise.Append( target.Value ); }
            else { toRemove.Append( subscription ); }
        }

        foreach ( Subscription<TEventArgs> subscription in toRemove.Span ) { subscriptions.Remove( subscription ); }
    }
    /*
    private static void AddRemoveEvents<T, TEventArgs>( this T eventHandlers, in string eventName, out List<Target<TEventArgs>> toRaise )
        where T : IReadOnlyDictionary<string, List<Subscription<TEventArgs>>>
    {
        using var toRemove = new Buffer<Subscription<TEventArgs>>();
        toRaise = new List<Target<TEventArgs>>();

        if ( eventHandlers.TryGetValue( eventName, out List<Subscription<TEventArgs>>? target ) is false ) { return; }

        // ReSharper disable once ForCanBeConvertedToForeach
        for ( int i = 0; i < target.Count; i++ )
        {
            Subscription<TEventArgs> subscription = target[i];

            if ( subscription.TryGetTarget( out OneOf<EventHandler<TEventArgs>, Action<TEventArgs>>? handler ) is false )
            {
                toRemove.Append( subscription );
                continue;
            }

            if ( subscription.Subscriber is null )
            {
                toRaise.Add( new Target<TEventArgs>( null, handler.Value ) );
                continue;
            }

            object? subscriber = subscription.Subscriber?.Target;

            if ( subscriber is null ) { toRemove.Append( subscription ); }
            else { toRaise.Add( new Target<TEventArgs>( subscriber, handler.Value ) ); }
        }

        foreach ( Subscription<TEventArgs> subscription in toRemove.Span ) { target.Remove( subscription ); }
    }
    #1#



    public readonly record struct Subscription<TEventArgs>( in WeakReference? Subscriber, in OneOf<WeakReference<EventHandler<TEventArgs>>, WeakReference<Action<TEventArgs>>> Handler )
    {
        public Subscription( in WeakReference? Subscriber, in OneOf<EventHandler<TEventArgs>, Action<TEventArgs>> Handler ) : this( in Subscriber, Handler.Match( Convert, Convert ) ) { }


        internal bool TryGetTarget( [ NotNullWhen( true ) ] out Target<TEventArgs>? target )
        {
            if ( TryGetTarget( out OneOf<EventHandler<TEventArgs>, Action<TEventArgs>>? handler ) is false )
            {
                target = null;
                return false;
            }

            if ( Subscriber is null )
            {
                target = new Target<TEventArgs>( null, handler.Value );
                return true;
            }

            object? subscriber = Subscriber?.Target;

            target = subscriber is null
                         ? null
                         : new Target<TEventArgs>( subscriber, handler.Value );

            return target is not null;
        }
        public bool TryGetTarget( [ NotNullWhen( true ) ] out OneOf<EventHandler<TEventArgs>, Action<TEventArgs>>? handler )
        {
            if ( Handler.IsT0 && Handler.AsT0.TryGetTarget( out EventHandler<TEventArgs>? target1 ) )
            {
                handler = target1;
                return true;
            }

            if ( Handler.IsT1 && Handler.AsT1.TryGetTarget( out Action<TEventArgs>? target2 ) )
            {
                handler = target2;
                return true;
            }

            handler = null;
            return false;
        }


        private static OneOf<WeakReference<EventHandler<TEventArgs>>, WeakReference<Action<TEventArgs>>> Convert( EventHandler<TEventArgs> x ) => new WeakReference<EventHandler<TEventArgs>>( x );
        private static OneOf<WeakReference<EventHandler<TEventArgs>>, WeakReference<Action<TEventArgs>>> Convert( Action<TEventArgs>       x ) => new WeakReference<Action<TEventArgs>>( x );
    }



    internal readonly record struct Target<TEventArgs>( object? Subscriber, in OneOf<EventHandler<TEventArgs>, Action<TEventArgs>> EventHandler )
    {
        public void HandleEvent( in string errorMsg, TEventArgs arg, object? sender )
        {
            try
            {
                if ( EventHandler.IsT0 ) { EventHandler.AsT0( sender, arg ); }
                else if ( EventHandler.IsT1 ) { EventHandler.AsT1( arg ); }
            }
            catch ( TargetParameterCountException e ) { throw new InvalidHandleEventException( errorMsg, e ); }
        }
    }
}
*/
