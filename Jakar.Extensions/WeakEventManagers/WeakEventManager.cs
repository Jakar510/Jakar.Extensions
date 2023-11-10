namespace Jakar.Extensions;


/// <summary>
/// Weak event manager that allows for garbage collection when the EventHandler is still subscribed
/// </summary>
/// <typeparam name="TEventArgs">Event args type.</typeparam>
public sealed class WeakEventManager<TEventArgs>
{
    readonly Dictionary<string, List<Subscription>> _eventHandlers = new();


    public void AddEventHandler( in EventHandler<TEventArgs> handler, [ CallerMemberName ] in string eventName = EventManagerService.EMPTY )
    {
        if ( string.IsNullOrWhiteSpace( eventName ) ) { throw new ArgumentNullException( nameof(eventName) ); }

        _eventHandlers.AddEventHandler( eventName, handler.Target, handler.GetMethodInfo() );
    }

    public void AddEventHandler( in Action<TEventArgs> action, [ CallerMemberName ] in string eventName = EventManagerService.EMPTY )
    {
        if ( string.IsNullOrWhiteSpace( eventName ) ) { throw new ArgumentNullException( nameof(eventName) ); }

        _eventHandlers.AddEventHandler( eventName, action.Target, action.GetMethodInfo() );
    }

    public void RemoveEventHandler( in EventHandler<TEventArgs> handler, [ CallerMemberName ] in string eventName = EventManagerService.EMPTY )
    {
        if ( string.IsNullOrWhiteSpace( eventName ) ) { throw new ArgumentNullException( nameof(eventName) ); }

        _eventHandlers.RemoveEventHandler( eventName, handler.Target, handler.GetMethodInfo() );
    }

    public void RemoveEventHandler( in Action<TEventArgs> action, [ CallerMemberName ] in string eventName = EventManagerService.EMPTY )
    {
        if ( string.IsNullOrWhiteSpace( eventName ) ) { throw new ArgumentNullException( nameof(eventName) ); }

        _eventHandlers.RemoveEventHandler( eventName, action.Target, action.GetMethodInfo() );
    }

    public void RaiseEvent( in object? sender, in TEventArgs eventArgs, in string eventName ) => _eventHandlers.HandleEvent( eventName, sender, eventArgs );

    public void RaiseEvent( in TEventArgs eventArgs, in string eventName ) => _eventHandlers.HandleEvent( eventName, eventArgs );
}



/// <summary>
/// Weak event manager that allows for garbage collection when the EventHandler is still subscribed
/// </summary>
public sealed class WeakEventManager
{
    readonly Dictionary<string, List<Subscription>> _eventHandlers = new();


    public void AddEventHandler( in Delegate handler, [ CallerMemberName ] in string eventName = EventManagerService.EMPTY )
    {
        if ( string.IsNullOrWhiteSpace( eventName ) ) { throw new ArgumentNullException( nameof(eventName) ); }

        _eventHandlers.AddEventHandler( eventName, handler.Target, handler.GetMethodInfo() );
    }

    public void RemoveEventHandler( in Delegate handler, [ CallerMemberName ] in string eventName = EventManagerService.EMPTY )
    {
        if ( string.IsNullOrWhiteSpace( eventName ) ) { throw new ArgumentNullException( nameof(eventName) ); }

        _eventHandlers.RemoveEventHandler( eventName, handler.Target, handler.GetMethodInfo() );
    }

    public void RaiseEvent( in object? sender, in object? eventArgs, in string eventName ) => _eventHandlers.HandleEvent( eventName, sender, eventArgs );

    public void RaiseEvent( in string eventName ) => _eventHandlers.HandleEvent( eventName );
}