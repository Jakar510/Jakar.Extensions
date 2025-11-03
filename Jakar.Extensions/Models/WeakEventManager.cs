// Jakar.Extensions :: Jakar.Extensions
// 11/03/2025  10:57

using System;



namespace Jakar.Extensions;


/// <summary> Weak event manager that allows for garbage collection when the EventHandler is still subscribed </summary>
/// <typeparam name="TEventArgs">Event args type.</typeparam>
public sealed class WeakEventManager : BaseClass
{
    private readonly Dictionary<string, List<Subscription>> _eventHandlers = new();
    private readonly Lock                                   __lock         = new();


    private List<Subscription> Subscriptions( string eventName )
    {
        ArgumentNullException.ThrowIfNull(eventName);

        return _eventHandlers.GetOrAdd<string, List<Subscription>>(eventName, CreateList);
        static List<Subscription> CreateList( string x ) => [];
    }
    public void AddEventHandler( EventHandler handler, [CallerMemberName] string eventName = EMPTY )
    {
        ArgumentNullException.ThrowIfNull(eventName);

        lock ( __lock )
        {
            Subscriptions(eventName)
               .Add(Subscription.Create(handler));
        }
    }
    public void AddEventHandler( Action handler, [CallerMemberName] string eventName = EMPTY )
    {
        ArgumentNullException.ThrowIfNull(eventName);

        lock ( __lock )
        {
            Subscriptions(eventName)
               .Add(Subscription.Create(handler));
        }
    }
    public void AddEventHandler<TEventArgs>( EventHandler<TEventArgs> handler, [CallerMemberName] string eventName = EMPTY )
    {
        ArgumentNullException.ThrowIfNull(eventName);

        lock ( __lock )
        {
            Subscriptions(eventName)
               .Add(Subscription.Create(handler));
        }
    }
    public void AddEventHandler<TEventArgs>( Action<TEventArgs> handler, [CallerMemberName] string eventName = EMPTY )
    {
        ArgumentNullException.ThrowIfNull(eventName);

        lock ( __lock )
        {
            Subscriptions(eventName)
               .Add(Subscription.Create(handler));
        }
    }


    public void RemoveEventHandler( EventHandler handler, [CallerMemberName] string eventName = EMPTY )
    {
        ArgumentNullException.ThrowIfNull(eventName);

        lock ( __lock )
        {
            Subscriptions(eventName)
               .Remove(Subscription.Create(handler));
        }
    }
    public void RemoveEventHandler( Action handler, [CallerMemberName] string eventName = EMPTY )
    {
        ArgumentNullException.ThrowIfNull(eventName);

        lock ( __lock )
        {
            Subscriptions(eventName)
               .Remove(Subscription.Create(handler));
        }
    }
    public void RemoveEventHandler<TEventArgs>( EventHandler<TEventArgs> handler, [CallerMemberName] string eventName = EMPTY )
    {
        ArgumentNullException.ThrowIfNull(eventName);

        lock ( __lock )
        {
            Subscriptions(eventName)
               .Remove(Subscription.Create(handler));
        }
    }
    public void RemoveEventHandler<TEventArgs>( Action<TEventArgs> handler, [CallerMemberName] string eventName = EMPTY )
    {
        ArgumentNullException.ThrowIfNull(eventName);

        lock ( __lock )
        {
            Subscriptions(eventName)
               .Remove(Subscription.Create(handler));
        }
    }


    public void RaiseEvent<TEventArgs>( TEventArgs eventArgs, object? sender = null, [CallerMemberName] string eventName = EMPTY )
    {
        ArgumentNullException.ThrowIfNull(eventName);

        lock ( __lock )
        {
            ReadOnlySpan<Subscription> subs = CollectionsMarshal.AsSpan(_eventHandlers[eventName]);
            foreach ( ref readonly Subscription sub in subs ) { sub.Execute(sender, eventArgs); }
        }
    }
    public void RaiseEvent( object? sender = null, [CallerMemberName] string eventName = EMPTY )
    {
        ArgumentNullException.ThrowIfNull(eventName);

        lock ( __lock )
        {
            ReadOnlySpan<Subscription> subs = CollectionsMarshal.AsSpan(_eventHandlers[eventName]);
            foreach ( ref readonly Subscription sub in subs ) { sub.Execute(sender, EventArgs.Empty); }
        }
    }



    private readonly struct Subscription : IEquatable<Subscription>
    {
        private readonly WeakReference<Delegate>? Handler;
        private Subscription( Delegate handler ) => Handler = new WeakReference<Delegate>(handler);


        public static Subscription Create( Action                               handler ) => new(handler);
        public static Subscription Create<TEventArgs>( Action<TEventArgs>       handler ) => new(handler);
        public static Subscription Create( EventHandler                         handler ) => new(handler);
        public static Subscription Create<TEventArgs>( EventHandler<TEventArgs> handler ) => new(handler);


        public void Execute<TEventArgs>( object? sender, TEventArgs args )
        {
            if ( Handler is null || !Handler.TryGetTarget(out Delegate? target) ) { return; }

            switch ( target )
            {
                case EventHandler<TEventArgs> handler:
                    handler(sender, args);
                    return;

                case Action<TEventArgs> handler:
                    handler(args);
                    return;

                case EventHandler handler when args is EventArgs eventArgs:
                    handler(sender, eventArgs);
                    return;

                case EventHandler handler:
                    handler(sender, EventArgs.Empty);
                    return;

                case Action handler:
                    handler();
                    return;
            }
        }


        public          bool Equals( Subscription other )                         => ReferenceEquals(Handler, other.Handler);
        public override bool Equals( object?      obj )                           => obj is Subscription other && Equals(other);
        public override int  GetHashCode()                                        => HashCode.Combine(Handler);
        public static   bool operator ==( Subscription left, Subscription right ) => left.Equals(right);
        public static   bool operator !=( Subscription left, Subscription right ) => !left.Equals(right);
    }
}
