namespace Jakar.Extensions;


/// <summary>
/// Weak event manager that allows for garbage collection when the EventHandler is still subscribed
/// </summary>
/// <typeparam name="TEventArgs">Event args type.</typeparam>
public sealed class WeakEventManager<TEventArgs>
{
    private readonly Dictionary<string, List<EventManagerService.Subscription<TEventArgs>>> _eventHandlers = new();


    public void AddEventHandler( in EventHandler<TEventArgs> handler, [ CallerMemberName ] in string eventName = EventManagerService.EMPTY )
    {
        if ( string.IsNullOrWhiteSpace( eventName ) ) { throw new ArgumentNullException( nameof(eventName) ); }

        _eventHandlers.AddEventHandler( eventName, handler.Target, handler );
    }
    public void AddEventHandler( in Action<TEventArgs> action, [ CallerMemberName ] in string eventName = EventManagerService.EMPTY )
    {
        if ( string.IsNullOrWhiteSpace( eventName ) ) { throw new ArgumentNullException( nameof(eventName) ); }

        _eventHandlers.AddEventHandler( eventName, action.Target, action );
    }


    public void RemoveEventHandler( in EventHandler<TEventArgs> handler, [ CallerMemberName ] in string eventName = EventManagerService.EMPTY )
    {
        if ( string.IsNullOrWhiteSpace( eventName ) ) { throw new ArgumentNullException( nameof(eventName) ); }

        _eventHandlers.RemoveEventHandler( eventName, handler.Target, handler );
    }
    public void RemoveEventHandler( in Action<TEventArgs> action, [ CallerMemberName ] in string eventName = EventManagerService.EMPTY )
    {
        if ( string.IsNullOrWhiteSpace( eventName ) ) { throw new ArgumentNullException( nameof(eventName) ); }

        _eventHandlers.RemoveEventHandler( eventName, action.Target, action );
    }


    public void RaiseEvent( in object? sender, in TEventArgs eventArgs, [ CallerMemberName ] in string eventName = EventManagerService.EMPTY )
    {
        Trace.Assert( string.IsNullOrWhiteSpace( eventName ) is false );
        Trace.Assert( _eventHandlers.ContainsKey( eventName ) );
        _eventHandlers.HandleEvent( eventName, eventArgs, sender );
    }
    public void RaiseEvent( in TEventArgs eventArgs, [ CallerMemberName ] in string eventName = EventManagerService.EMPTY )
    {
        Trace.Assert( string.IsNullOrWhiteSpace( eventName ) is false );
        Trace.Assert( _eventHandlers.ContainsKey( eventName ) );
        _eventHandlers.HandleEvent( eventName, eventArgs );
    }
}



/// <summary>
/// Weak event manager that allows for garbage collection when the EventHandler is still subscribed
/// </summary>
public sealed class WeakEventManager
{
    private readonly Dictionary<string, List<EventManagerService.Subscription>> _eventHandlers = new();


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
    public void RaiseEvent( in string  eventName ) => _eventHandlers.HandleEvent( eventName );
}
