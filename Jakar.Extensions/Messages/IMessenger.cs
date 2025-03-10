/*
using System.Runtime;



namespace Jakar.Extensions;


/// <summary>
///     An interface for a type providing the ability to exchange messages between different objects. This can be useful to decouple different modules of an application without having to keep strong references to types being referenced. It is also possible to send messages to specific channels, uniquely identified by a token, and to have different messengers in different sections of an applications. In order to use the
///     <see
///         cref="IMessenger"/>
///     functionalities, first define a message type, like so:
///     <code>
/// public sealed class LoginCompletedMessage { }
/// </code>
///     Then, register a recipient for this message:
///     <code>
/// Messenger.Default.Register&lt;MyRecipientType, LoginCompletedMessage&gt;(this, (r, m) =>
/// {
///     // Handle the message here...
/// });
/// </code>
///     The message handler here is a lambda expression taking two parameters: the recipient and the message. This is done to avoid the allocations for the closures that would've been generated if the expression had captured the current instance. The recipient type parameter is used so that the recipient can be directly accessed within the handler without the need to manually perform type casts. This allows the code to be less verbose and more reliable, as all the checks are done just at build time. If the handler is defined within the same type as the recipient, it is also possible to directly access private members. This allows the message handler to be a static method, which enables the C# compiler to perform a number of additional memory optimizations (such as caching the delegate, avoiding unnecessary memory allocations). Finally, send a message when needed, like so:
///     <code>
/// Messenger.Default.Send&lt;LoginCompletedMessage&gt;();
/// </code>
///     Additionally, the method group syntax can also be used to specify the message handler to invoke when receiving a message, if a method with the right signature is available in the current scope. This is helpful to keep the registration and handling logic separate. Following up from the previous example, consider a class having this method:
///     <code>
/// private static void Receive(MyRecipientType recipient, LoginCompletedMessage message)
/// {
///     // Handle the message there
/// }
/// </code>
///     The registration can then be performed in a single line like so:
///     <code>
/// Messenger.Default.Register(this, Receive);
/// </code>
///     The C# compiler will automatically convert that expression to a <see cref="MessageHandler{TRecipient,TMessage}"/> instance compatible with
///     <see
///         cref="IMessengerExtensions.Register{TRecipient,TMessage}(IMessenger,TRecipient,MessageHandler{TRecipient,TMessage})"/>
///     . This will also work if multiple overloads of that method are available, each handling a different message type: the C# compiler will automatically pick the right one for the current message type. It is also possible to register message handlers explicitly using the
///     <see
///         cref="IRecipient{TMessage}"/>
///     interface. To do so, the recipient just needs to implement the interface and then call the
///     <see
///         cref="IMessengerExtensions.RegisterAll(IMessenger,object)"/>
///     extension, which will automatically register all the handlers that are declared by the recipient type. Registration for individual handlers is supported as well.
/// </summary>
public interface IMessenger<TRecipient, TMessage, in TToken>
    where TRecipient : class, IRecipient<TMessage>
    where TMessage : class
    where TToken : IEquatable<TToken>
{
    ConditionalWeakTable2<TRecipient, Action<TRecipient, TMessage>> Handlers { get; }

    /// <summary> Checks whether a given recipient has already been registered for a message. </summary>
    /// <typeparam name="TMessage"> The type of message to check for the given recipient. </typeparam>
    /// <typeparam name="TToken"> The type of token to check the channel for. </typeparam>
    /// <param name="recipient"> The target recipient to check the registration for. </param>
    /// <param name="token"> The token used to identify the target channel to check. </param>
    /// <returns> Whether <paramref name="recipient"/> has already been registered for the specified message. </returns>
    /// <exception cref="System.ArgumentNullException"> Thrown if <paramref name="recipient"/> or <paramref name="token"/> are <see langword="null"/>. </exception>
    bool IsRegistered( TRecipient recipient, TToken token );

    /// <summary> Registers a recipient for a given type of message. </summary>
    /// <typeparam name="TRecipient"> The type of recipient for the message. </typeparam>
    /// <typeparam name="TMessage"> The type of message to receive. </typeparam>
    /// <typeparam name="TToken"> The type of token to use to pick the messages to receive. </typeparam>
    /// <param name="recipient"> The recipient that will receive the messages. </param>
    /// <param name="token"> A token used to determine the receiving channel to use. </param>
    /// <param name="handler"> The <see cref="MessageHandler{TRecipient,TMessage}"/> to invoke when a message is received. </param>
    /// <exception cref="System.ArgumentNullException"> Thrown if <paramref name="recipient"/>, <paramref name="token"/> or <paramref name="handler"/> are <see langword="null"/>. </exception>
    /// <exception cref="InvalidOperationException"> Thrown when trying to register the same message twice. </exception>
    void Register( TRecipient recipient, TToken token, Action<TRecipient, TMessage> handler );

    /// <summary> Unregisters a recipient from all registered messages. </summary>
    /// <param name="recipient"> The recipient to unregister. </param>
    /// <remarks> This method will unregister the target recipient across all channels. Use this method as an easy way to lose all references to a target recipient. If the recipient has no registered handler, this method does nothing. </remarks>
    /// <exception cref="System.ArgumentNullException"> Thrown if <paramref name="recipient"/> is <see langword="null"/>. </exception>
    void UnregisterAll( object recipient );

    /// <summary> Unregisters a recipient from all messages on a specific channel. </summary>
    /// <typeparam name="TToken"> The type of token to identify what channel to unregister from. </typeparam>
    /// <param name="recipient"> The recipient to unregister. </param>
    /// <param name="token"> The token to use to identify which handlers to unregister. </param>
    /// <remarks> If the recipient has no registered handler, this method does nothing. </remarks>
    /// <exception cref="System.ArgumentNullException"> Thrown if <paramref name="recipient"/> or <paramref name="token"/> are <see langword="null"/>. </exception>
    void UnregisterAll( object recipient, TToken token );

    /// <summary> Unregisters a recipient from messages of a given type. </summary>
    /// <typeparam name="TMessage"> The type of message to stop receiving. </typeparam>
    /// <typeparam name="TToken"> The type of token to identify what channel to unregister from. </typeparam>
    /// <param name="recipient"> The recipient to unregister. </param>
    /// <param name="token"> The token to use to identify which handlers to unregister. </param>
    /// <remarks> If the recipient has no registered handler, this method does nothing. </remarks>
    /// <exception cref="System.ArgumentNullException"> Thrown if <paramref name="recipient"/> or <paramref name="token"/> are <see langword="null"/>. </exception>
    void Unregister( object recipient, TToken token );

    /// <summary> Sends a message of the specified type to all registered recipients. </summary>
    /// <typeparam name="TMessage"> The type of message to send. </typeparam>
    /// <typeparam name="TToken"> The type of token to identify what channel to use to send the message. </typeparam>
    /// <param name="message"> The message to send. </param>
    /// <param name="token"> The token indicating what channel to use. </param>
    /// <returns> The message that was sent (ie. <paramref name="message"/>). </returns>
    /// <exception cref="System.ArgumentNullException"> Thrown if <paramref name="message"/> or <paramref name="token"/> are <see langword="null"/>. </exception>
    TMessage Send( TMessage message, TToken token );

    /// <summary> Performs a cleanup on the current messenger. Invoking this method does not unregister any of the currently registered recipient, and it can be used to perform cleanup operations such as trimming the internal data structures of a messenger implementation. </summary>
    void Cleanup();

    /// <summary> Resets the instance and unregisters all the existing recipients. </summary>
    void Reset();
}



/// <summary> An interface for a recipient that declares a registration for a specific message type. </summary>
/// <typeparam name="TMessage"> The type of message to receive. </typeparam>
public interface IRecipient<in TMessage>
    where TMessage : class
{
    /// <summary> Receives a given <typeparamref name="TMessage"/> message instance. </summary>
    /// <param name="message"> The message being received. </param>
    void Receive( TMessage message );
}



/// <summary> A custom <see cref="ConditionalWeakTable{TKey, TValue}"/> instance that is specifically optimized to be used by <see cref="WeakReferenceMessenger"/>. In particular, it offers zero-allocation enumeration of stored items. </summary>
/// <typeparam name="TKey"> Tke key of items to store in the table. </typeparam>
/// <typeparam name="TValue"> The values to store in the table. </typeparam>
public sealed class ConditionalWeakTable2<TKey, TValue>
    where TKey : class
    where TValue : class?
{
    /// <summary> This lock protects all mutation of data in the table. Readers do not take this lock. </summary>
    private readonly Lock _lockObject;

    /// <summary> The actual storage for the table; swapped out as the table grows. </summary>
    private volatile Container _container;

    /// <summary> Initializes a new instance of the <see cref="ConditionalWeakTable2{TKey, TValue}"/> class. </summary>
    public ConditionalWeakTable2()
    {
        _lockObject = new Lock();
        _container  = new Container( this );
    }

    /// <inheritdoc cref="ConditionalWeakTable{TKey, TValue}.TryGetValue(TKey, out TValue)"/>
    public bool TryGetValue( TKey key, [MaybeNullWhen( false )] out TValue value ) => _container.TryGetValueWorker( key, out value );

    /// <summary> Tries to add a new pair to the table. </summary>
    /// <param name="key"> The key to add. </param>
    /// <param name="value"> The value to associate with key. </param>
    public bool TryAdd( TKey key, TValue value )
    {
        lock (_lockObject)
        {
            int entryIndex = _container.FindEntry( key, out _ );

            if ( entryIndex != -1 ) { return false; }

            CreateEntry( key, value );

            return true;
        }
    }

    /// <inheritdoc cref="ConditionalWeakTable{TKey, TValue}.Remove(TKey)"/>
    public bool Remove( TKey key )
    {
        lock (_lockObject) { return _container.Remove( key ); }
    }

    /// <inheritdoc cref="ConditionalWeakTable{TKey, TValue}.GetValue(TKey, ConditionalWeakTable{TKey, TValue}.CreateValueCallback)"/>
    [UnconditionalSuppressMessage( "ReflectionAnalysis", "IL2091", Justification = "ConditionalWeakTable<TKey, TValue> is only referenced to reuse the callback delegate type, but no value is ever created through reflection." )]
    public TValue GetValue( TKey key, ConditionalWeakTable<TKey, TValue>.CreateValueCallback createValueCallback ) =>
        TryGetValue( key, out TValue? existingValue )
            ? existingValue
            : GetValueLocked( key, createValueCallback );

    /// <summary> Implements the functionality for <see cref="GetValue(TKey, ConditionalWeakTable{TKey, TValue}.CreateValueCallback)"/> under a lock. </summary>
    /// <param name="key"> The input key. </param>
    /// <param name="createValueCallback"> The callback to use to create a new item. </param>
    /// <returns> The new <typeparamref name="TValue"/> item to store. </returns>
    [UnconditionalSuppressMessage( "ReflectionAnalysis", "IL2091", Justification = "ConditionalWeakTable<TKey, TValue> is only referenced to reuse the callback delegate type, but no value is ever created through reflection." )]
    private TValue GetValueLocked( TKey key, ConditionalWeakTable<TKey, TValue>.CreateValueCallback createValueCallback )
    {
        // If we got here, the key was not in the table. Invoke the callback (outside the lock) to generate the new value for the key.
        TValue newValue = createValueCallback( key );

        lock (_lockObject)
        {
            // Now that we've taken the lock, must recheck in case we lost a race to add the key
            if ( _container.TryGetValueWorker( key, out TValue? existingValue ) ) { return existingValue; }

            // Verified in-lock that we won the race to add the key. Add it now
            CreateEntry( key, newValue );

            return newValue;
        }
    }

    public Enumerator GetEnumerator()
    {
        // This is an optimization specific for this custom table that relies on the way the enumerator is being used within the messenger type.
        // Specifically, enumerators are always used in a using block, meaning Dispose() is always guaranteed to be executed.
        // Given we cannot remove the internal lock for the table as it's needed to ensure consistency in case a container is resurrected (see below), the solution to speedup iteration is to avoid taking and releasing a lock repeatedly every single time MoveNext() is invoked.
        // This is fine in this specific scenario because we're the only users of the enumerators so there's no concern about blocking other threads while enumerating.
        // So here we just preemptively take a lock for the entire lifetime of the enumerator, and just release it once we're done.
        _lockObject.Enter();

        return new Enumerator( this );
    }


    /// <summary> Worker for adding a new key/value pair. Will resize the container if it is full. </summary>
    /// <param name="key"> The key for the new entry. </param>
    /// <param name="value"> The value for the new entry. </param>
    private void CreateEntry( TKey key, TValue value )
    {
        Container container = _container;

        if ( container.HasCapacity is false ) { _container = container = container.Resize(); }

        container.CreateEntryNoResize( key, value );
    }



    /// <summary> Container holds the actual data for the table. A given instance of Container always has the same capacity. When we need more capacity, we create a new Container, copy the old one into the new one, and discard the old one. This helps enable lock-free reads from the table, as readers never need to deal with motion of entries due to rehashing. </summary>
    private sealed class Container
    {
        /// <summary> The <see cref="ConditionalWeakTable2{TKey, TValue}"/> with which this container is associated. </summary>
        private readonly ConditionalWeakTable2<TKey, TValue> _parent;

        /// <summary> Set to true when initially finalized </summary>
        private bool _finalized;

        /// <summary> Flag detects if OOM or other background exception threw us out of the lock. </summary>
        private bool _invalid;

        /// <summary> The table entries containing the stored dependency handles </summary>
        private Entry[]? _entries;

        /// <summary> <c> firstFreeEntry &lt; entries.Length => table </c> has capacity, entries grow from the bottom of the table. </summary>
        private int _firstFreeEntry;

        /// <summary> <c> buckets[hashcode &amp; (buckets.Length - 1)] </c> contains index of the first entry in bucket (-1 if empty). </summary>
        private int[]? _buckets;

        /// <summary> Used to ensure the next allocated container isn't finalized until this one is GC'd. </summary>
        private volatile object? _oldKeepAlive;

        /// <summary> Gets the index of the first free entry. </summary>
        internal int FirstFreeEntry => _firstFreeEntry;

        /// <summary> Gets the capacity of the current container. </summary>
        internal bool HasCapacity => _firstFreeEntry < _entries?.Length;

        /// <summary> Initializes a new instance of the <see cref="Container"/> class. </summary>
        /// <param name="parent"> The input <see cref="ConditionalWeakTable2{TKey, TValue}"/> object associated with the current instance. </param>
        internal Container( ConditionalWeakTable2<TKey, TValue> parent )
        {
            _buckets = new int[DEFAULT_CAPACITY];
            for ( int i = 0; i < _buckets.Length; i++ ) { _buckets[i] = -1; }

            _entries = new Entry[DEFAULT_CAPACITY];

            // Only store the parent after all of the allocations have happened successfully.
            // Otherwise, as part of growing or clearing the container, we could end up allocating a new Container that fails (OOMs) part way through construction but that gets finalized and ends up clearing out some other container present in the associated CWT.
            _parent = parent;
        }

        /// <summary> Initializes a new instance of the <see cref="Container"/> class. </summary>
        /// <param name="parent"> The input <see cref="ConditionalWeakTable2{TKey, TValue}"/> object associated with the current instance. </param>
        /// <param name="buckets"> The array of buckets. </param>
        /// <param name="entries"> The array of entries. </param>
        /// <param name="firstFreeEntry"> The index of the first free entry. </param>
        private Container( ConditionalWeakTable2<TKey, TValue> parent, int[] buckets, Entry[] entries, int firstFreeEntry )
        {
            _parent         = parent;
            _buckets        = buckets;
            _entries        = entries;
            _firstFreeEntry = firstFreeEntry;
        }

        /// <summary> Worker for adding a new key/value pair. Container must NOT be full. </summary>
        internal void CreateEntryNoResize( TKey key, TValue value )
        {
            VerifyIntegrity();

            _invalid = true;

            int hashCode = RuntimeHelpers.GetHashCode( key ) & int.MaxValue;
            int newEntry = _firstFreeEntry++;
            
            _buckets                    ??= new int[DEFAULT_CAPACITY];
            _entries                    ??= new Entry[Math.Max( DEFAULT_CAPACITY, newEntry )];
            _entries[newEntry].hashCode =   hashCode;
            _entries[newEntry].depHnd   =   new DependentHandle( key, value );

            int bucket = hashCode & (_buckets.Length - 1);
            _entries[newEntry].next = _buckets[bucket];

            // This write must be volatile, as we may be racing with concurrent readers.
            // If they see the new entry, they must also see all of the writes earlier in this method.
            Volatile.Write( ref _buckets[bucket], newEntry );

            _invalid = false;
        }

        /// <summary> Worker for finding a key/value pair. Must hold lock. </summary>
        internal bool TryGetValueWorker( TKey key, [MaybeNullWhen( false )] out TValue value )
        {
            int entryIndex = FindEntry( key, out object? secondary );

            value = Unsafe.As<TValue>( secondary );

            return entryIndex != -1;
        }

        /// <summary> Returns -1 if not found (if key expires during FindEntry, this can be treated as "not found."). Must hold lock, or be prepared to retry the search while holding lock. </summary>
        /// <remarks> This method requires <paramref name="value"/> to be on the stack to be properly tracked. </remarks>
        internal int FindEntry( TKey key, out object? value )
        {
            _buckets ??= new int[DEFAULT_CAPACITY];
            int hashCode = RuntimeHelpers.GetHashCode( key ) & int.MaxValue;
            int bucket   = hashCode                          & (_buckets.Length - 1);
            _entries ??= new Entry[DEFAULT_CAPACITY];

            for ( int entriesIndex = Volatile.Read( ref _buckets[bucket] ); entriesIndex != -1; entriesIndex = _entries[entriesIndex].next )
            {
                if ( _entries[entriesIndex].hashCode == hashCode )
                {
                    // if (_entries[entriesIndex].depHnd.UnsafeGetTargetAndDependent(out value) == key)
                    (object? oKey, value) = _entries[entriesIndex].depHnd.TargetAndDependent;

                    if ( oKey == key )
                    {
                        // Ensure we don't get finalized while accessing DependentHandle
                        GC.KeepAlive( this );

                        return entriesIndex;
                    }
                }
            }

            // Ensure we don't get finalized while accessing DependentHandle
            GC.KeepAlive( this );

            value = null;

            return -1;
        }

        /// <summary> Gets the entry at the specified entry index. </summary>
        internal bool TryGetEntry( int index, [NotNullWhen( true )] out TKey? key, [MaybeNullWhen( false )] out TValue value )
        {
            _entries ??= new Entry[DEFAULT_CAPACITY];

            if ( index < _entries.Length )
            {
                // object? oKey = entries[index].depHnd.UnsafeGetTargetAndDependent(out object? oValue);
                (object? oKey, object? oValue) = _entries[index].depHnd.TargetAndDependent;

                // Ensure we don't get finalized while accessing DependentHandle
                GC.KeepAlive( this );

                if ( oKey != null )
                {
                    key   = Unsafe.As<TKey>( oKey );
                    value = Unsafe.As<TValue>( oValue )!;

                    return true;
                }
            }

            key   = null;
            value = null;

            return false;
        }

        /// <summary> Removes the specified key from the table, if it exists. </summary>
        internal bool Remove( TKey key )
        {
            VerifyIntegrity();

            int entryIndex = FindEntry( key, out _ );

            if ( entryIndex != -1 )
            {
                RemoveIndex( entryIndex );

                return true;
            }

            return false;
        }

        /// <summary> Removes a given entry at a specified index. </summary>
        /// <param name="entryIndex"> The index of the entry to remove. </param>
        private void RemoveIndex( int entryIndex )
        {
            _entries ??= new Entry[DEFAULT_CAPACITY];
            ref Entry entry = ref _entries[entryIndex];

            // We do not free the handle here, as we may be racing with readers who already saw the hash code.
            // Instead, we simply overwrite the entry's hash code, so subsequent reads will ignore it.
            // The handle will be free'd in Container's finalizer, after the table is resized or discarded.
            Volatile.Write( ref entry.hashCode, -1 );

            // Also, clear the key to allow GC to collect objects pointed to by the entry
            // entry.depHnd.UnsafeSetTargetToNull();
            entry.depHnd.Target = null;
        }

        /// <summary> Resize, and scrub expired keys off bucket lists. Must hold <see cref="ConditionalWeakTable2{TKey,TValue}._lockObject"/>. </summary>
        /// <remarks> <see cref="_firstFreeEntry"/> is less than <c> entries.Length </c> on exit, that is, the table has at least one free entry. </remarks>
        [Pure]
        internal Container Resize()
        {
            _entries ??= new Entry[DEFAULT_CAPACITY];
            _buckets ??= new int[DEFAULT_CAPACITY];
            bool hasExpiredEntries = false;
            int  newSize           = _buckets.Length;

            // If any expired or removed keys exist, we won't resize
            for ( int entriesIndex = 0; entriesIndex < _entries.Length; entriesIndex++ )
            {
                ref Entry entry = ref _entries[entriesIndex];

                if ( entry.hashCode == -1 )
                {
                    // the entry was removed
                    hasExpiredEntries = true;

                    break;
                }

                if ( entry.depHnd is { IsAllocated: true, Target: null } ) // entry.depHnd.UnsafeGetTarget() is null)
                {
                    // the entry has expired
                    hasExpiredEntries = true;

                    break;
                }
            }

            if ( !hasExpiredEntries )
            {
                // Not necessary to check for overflow here, the attempt to allocate new arrays will throw
                newSize = _buckets.Length * 2;
            }

            return Resize( newSize );
        }

        /// <summary> Creates a new <see cref="Container"/> of a specified size with the current items. </summary>
        /// <param name="newSize"> The new requested size. </param>
        /// <returns> The new <see cref="Container"/> instance with the requested size. </returns>
        [Pure]
        private Container Resize( int newSize )
        {
            // Reallocate both buckets and entries and rebuild the bucket and entries from scratch.
            // This serves both to scrub entries with expired keys and to put the new entries in the proper bucket.
            int[] newBuckets = new int[newSize];
            _entries ??= new Entry[DEFAULT_CAPACITY];

            for ( int bucketIndex = 0; bucketIndex < newBuckets.Length; bucketIndex++ ) { newBuckets[bucketIndex] = -1; }

            Entry[] newEntries      = new Entry[newSize];
            int     newEntriesIndex = 0;

            // There are no active enumerators, which means we want to compact by removing expired/removed entries
            for ( int entriesIndex = 0; entriesIndex < _entries.Length; entriesIndex++ )
            {
                ref Entry       oldEntry = ref _entries[entriesIndex];
                int             hashCode = oldEntry.hashCode;
                DependentHandle depHnd   = oldEntry.depHnd;

                if ( hashCode != -1 && depHnd.IsAllocated )
                {
                    // if (depHnd.UnsafeGetTarget() is not null)
                    if ( depHnd.Target is not null )
                    {
                        ref Entry newEntry = ref newEntries[newEntriesIndex];

                        // Entry is used and has not expired. Link it into the appropriate bucket list
                        newEntry.hashCode = hashCode;
                        newEntry.depHnd   = depHnd;

                        int bucket = hashCode & (newBuckets.Length - 1);

                        newEntry.next      = newBuckets[bucket];
                        newBuckets[bucket] = newEntriesIndex;
                        newEntriesIndex++;
                    }
                    else
                    {
                        // Pretend the item was removed, so that this container's finalizer will clean up this dependent handle
                        Volatile.Write( ref oldEntry.hashCode, -1 );
                    }
                }
            }

            // Create the new container. We want to transfer the responsibility of freeing the handles from the old container to the new container, and also ensure that the new container isn't finalized while the old container may still be in use.
            // As such, we store a reference from the old container to the new one, which will keep the new container alive as long as the old one is.
            Container newContainer = new(_parent!, newBuckets, newEntries, newEntriesIndex);

            // Once this is set, the old container's finalizer will not free transferred dependent handles
            _oldKeepAlive = newContainer;

            // Ensure we don't get finalized while accessing DependentHandles
            GC.KeepAlive( this );

            return newContainer;
        }

        /// <summary> Verifies that the current instance is valid. </summary>
        /// <exception cref="InvalidOperationException"> Thrown if the current instance is invalid. </exception>
        private void VerifyIntegrity()
        {
            if ( _invalid ) { throw new InvalidOperationException( "The current collection is in a corrupted state." ); }
        }

        /// <summary> Finalizes the current <see cref="Container"/> instance. </summary>
        ~Container()
        {
            // Skip doing anything if the container is invalid, including if somehow the container object was allocated but its associated table never set.
            if ( _invalid ) { return; }

            // It's possible that the ConditionalWeakTable2 could have been resurrected, in which case code could be accessing this Container as it's being finalized.
            // We don't support usage after finalization, but we also don't want to potentially corrupt state by allowing dependency handles to be used as or after they've been freed.
            // To avoid that, if it's at all possible that another thread has a reference to this container via the CWT, we remove such a reference and then re-register for finalization: the next time around, we can be sure that no references remain to this and we can clean up the dependency handles without fear of corruption.
            if ( !_finalized )
            {
                _finalized = true;

                lock (_parent._lockObject)
                {
                    if ( _parent._container == this ) { _parent._container = null!; }
                }

                // Next time it's finalized, we'll be sure there are no remaining refs
                GC.ReRegisterForFinalize( this );

                return;
            }

            Entry[]? entries = _entries;

            _invalid = true;
            _entries  = null;
            _buckets = null;

            if ( entries is not null )
            {
                for ( int entriesIndex = 0; entriesIndex < entries.Length; entriesIndex++ )
                {
                    // We need to free handles in two cases:
                    // - If this container still owns the dependency handle (meaning ownership hasn't been transferred to another container that replaced this one), then it should be freed.
                    // - If this container had the entry removed, then even if in general ownership was transferred to another container, removed entries are not, therefore this container must free them.
                    if ( _oldKeepAlive is null || entries[entriesIndex].hashCode == -1 ) { entries[entriesIndex].depHnd.Dispose(); }
                }
            }
        }
    }



    /// <summary> A single entry within a <see cref="ConditionalWeakTable2{TKey, TValue}"/> instance. </summary>
    private struct Entry
    {
        /// <summary> Holds key and value using a weak reference for the key and a strong reference for the value that is traversed only if the key is reachable without going through the value. </summary>
        public DependentHandle depHnd;

        /// <summary> Cached copy of key's hashcode. </summary>
        public int hashCode;

        /// <summary> Index of next entry, -1 if last. </summary>
        public int next;
    }



    /// <summary> Provides an enumerator for the current <see cref="ConditionalWeakTable2{TKey, TValue}"/> instance. </summary>
    public ref struct Enumerator
    {
        /// <summary> Parent table, set to null when disposed. </summary>
        private ConditionalWeakTable2<TKey, TValue> _table;

        /// <summary> Last index in the container that should be enumerated. </summary>
        private readonly int _maxIndexInclusive;

        /// <summary> The current index into the container. </summary>
        private readonly int _currentIndex;

        /// <summary> The current key, if available. </summary>
        private TKey? _key;

        /// <summary> The current value, if available. </summary>
        private TValue? _value;

        /// <summary> Initializes a new instance of the <see cref="Enumerator"/> class. </summary>
        /// <param name="table"> The input <see cref="ConditionalWeakTable2{TKey, TValue}"/> instance being enumerated. </param>
        public Enumerator( ConditionalWeakTable2<TKey, TValue> table )
        {
            // Store a reference to the parent table and increase its active enumerator count
            _table = table;

            Container? container = table._container;

            if ( container is null || container.FirstFreeEntry == 0 )
            {
                // The max index is the same as the current to prevent enumeration
                _maxIndexInclusive = -1;
            }
            else
            {
                // Store the max index to be enumerated
                _maxIndexInclusive = container.FirstFreeEntry - 1;
            }

            _currentIndex = -1;
            _key          = null;
            _value        = null;
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            // Release the lock
            Monitor.Exit( _table._lockObject );

            _table = null!;

            // Ensure we don't keep the last current alive unnecessarily
            _key   = null;
            _value = null;
        }

        /// <inheritdoc cref="IEnumerator.MoveNext"/>
        public bool MoveNext()
        {
            // From the table, we have to get the current container. This could have changed
            // since we grabbed the enumerator, but the index-to-pair mapping should not have
            // due to there being at least one active enumerator. If the table (or rather its
            // container at the time) has already been finalized, this will be null.
            Container c = _table._container;

            int currentIndex      = _currentIndex;
            int maxIndexInclusive = _maxIndexInclusive;

            // We have the container. Find the next entry to return, if there is one. We need to loop as we
            // may try to get an entry that's already been removed or collected, in which case we try again.
            while ( currentIndex < maxIndexInclusive )
            {
                currentIndex++;

                if ( c.TryGetEntry( currentIndex, out _key, out _value ) )
                {
                    currentIndex = currentIndex;

                    return true;
                }
            }

            currentIndex = currentIndex;

            return false;
        }

        /// <summary> Gets the current key. </summary>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public TKey GetKey() => _key!;

        /// <summary> Gets the current value. </summary>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public TValue GetValue() => _value!;
    }
}
*/
