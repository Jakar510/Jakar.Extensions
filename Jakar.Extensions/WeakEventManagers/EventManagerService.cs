// Jakar.Extensions :: Jakar.Extensions
// 11/10/2023  1:46 PM

namespace Jakar.Extensions;


[ SuppressMessage( "ReSharper", "ForCanBeConvertedToForeach" ), SuppressMessage( "ReSharper", "SuggestBaseTypeForParameter" ) ]
public static class EventManagerService
{
    public const    string EMPTY = "";
    static readonly object _lock = new();


    private static bool TryGetDynamicMethod( this MethodInfo rtDynamicMethod, [ NotNullWhen( true ) ] out DynamicMethod? method )
    {
        TypeInfo? typeInfoRTDynamicMethod = typeof(DynamicMethod).GetTypeInfo().GetDeclaredNestedType( "RTDynamicMethod" );
        Type?     typeRTDynamicMethod     = typeInfoRTDynamicMethod?.AsType();

        method = typeInfoRTDynamicMethod?.IsAssignableFrom( rtDynamicMethod.GetType().GetTypeInfo() ) is true
                     ? (DynamicMethod?)typeRTDynamicMethod?.GetRuntimeFields().First( f => f.Name is "m_owner" ).GetValue( rtDynamicMethod )
                     : null;

        return method is not null;
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
        eventHandlers.AddRemoveEvents( eventName, out List<SubscriptionTarget> toRaise );

        for ( int i = 0; i < toRaise.Count; i++ )
        {
            try
            {
                (object? instance, MethodInfo eventHandler) = toRaise[i];

                if ( eventHandler.TryGetDynamicMethod( out DynamicMethod? method ) )
                {
                    method.Invoke( instance,
                                   new[]
                                   {
                                       sender,
                                       eventArgs
                                   } );
                }
                else
                {
                    eventHandler.Invoke( instance,
                                         new[]
                                         {
                                             sender,
                                             eventArgs
                                         } );
                }
            }
            catch ( TargetParameterCountException e )
            {
                const string MSG = "Parameter count mismatch. If invoking an `event Action` use `HandleEvent(string eventName)` or if invoking an `event Action<T>` use `HandleEvent(object eventArgs, string eventName)`instead.";
                throw new InvalidHandleEventException( MSG, e );
            }
        }
    }

    public static void HandleEvent( this Dictionary<string, List<Subscription>> eventHandlers, in string eventName, in object? actionEventArgs )
    {
        eventHandlers.AddRemoveEvents( eventName, out List<SubscriptionTarget> toRaise );

        for ( int i = 0; i < toRaise.Count; i++ )
        {
            try
            {
                (object? instance, MethodInfo eventHandler) = toRaise[i];

                if ( eventHandler.TryGetDynamicMethod( out DynamicMethod? method ) )
                {
                    method.Invoke( instance,
                                   new[]
                                   {
                                       actionEventArgs
                                   } );
                }
                else
                {
                    eventHandler.Invoke( instance,
                                         new[]
                                         {
                                             actionEventArgs
                                         } );
                }
            }
            catch ( TargetParameterCountException e )
            {
                const string MSG = "Parameter count mismatch. If invoking an `event EventHandler` use `HandleEvent(object sender, TEventArgs eventArgs, string eventName)` or if invoking an `event Action` use `HandleEvent(string eventName)`instead.";
                throw new InvalidHandleEventException( MSG, e );
            }
        }
    }

    public static void HandleEvent( this Dictionary<string, List<Subscription>> eventHandlers, in string eventName )
    {
        eventHandlers.AddRemoveEvents( eventName, out List<SubscriptionTarget> toRaise );

        for ( int i = 0; i < toRaise.Count; i++ )
        {
            try
            {
                (object? instance, MethodInfo eventHandler) = toRaise[i];

                if ( eventHandler.TryGetDynamicMethod( out DynamicMethod? method ) ) { method.Invoke( instance, null ); }
                else { eventHandler.Invoke( instance, null ); }
            }
            catch ( TargetParameterCountException e )
            {
                const string MSG =
                    "Parameter count mismatch. If invoking an `event EventHandler` use `HandleEvent(object sender, TEventArgs eventArgs, string eventName)` or if invoking an `event Action<T>` use `HandleEvent(object eventArgs, string eventName)`instead.";

                throw new InvalidHandleEventException( MSG, e );
            }
        }
    }

    private static void AddRemoveEvents( this Dictionary<string, List<Subscription>> eventHandlers, in string eventName, out List<SubscriptionTarget> toRaise )
    {
        using var toRemove = new Buffer<Subscription>();
        toRaise = new List<SubscriptionTarget>();

        if ( eventHandlers.TryGetValue( eventName, out List<Subscription>? target ) is false ) { return; }

        for ( int i = 0; i < target.Count; i++ )
        {
            Subscription subscription = target[i];

            if ( subscription.Subscriber is null )
            {
                toRaise.Add( new SubscriptionTarget( null, subscription.Handler ) );
                continue;
            }

            object? subscriber = subscription.Subscriber?.Target;

            if ( subscriber is null ) { toRemove.Append( subscription ); }
            else { toRaise.Add( new SubscriptionTarget( subscriber, subscription.Handler ) ); }
        }

        foreach ( Subscription subscription in toRemove.Span ) { target.Remove( subscription ); }
    }
}



public readonly record struct Subscription( in WeakReference? Subscriber, in MethodInfo Handler );



public readonly record struct Subscription<T>( in WeakReference? Subscriber, in OneOf<EventHandler<T>, Action<T>> Handler );



public readonly record struct SubscriptionTarget( object? Instance, MethodInfo EventHandler );
