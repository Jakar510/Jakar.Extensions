// Jakar.Extensions :: Jakar.Extensions
// 09/18/2024  21:09

namespace Jakar.Extensions;


public interface IBuffer<TValue> : ICollection<TValue>, IDisposable
{
    int  Capacity   { [Pure, MethodImpl(                              MethodImplOptions.AggressiveInlining )] get; }
    bool IsEmpty    { [Pure, MethodImpl(                              MethodImplOptions.AggressiveInlining )] get; }
    bool IsNotEmpty { [Pure, MethodImpl(                              MethodImplOptions.AggressiveInlining )] get; }
    ref TValue this[ int     index ] { [Pure, MethodImpl(             MethodImplOptions.AggressiveInlining )] get; }
    ref TValue this[ Index   index ] { [Pure, MethodImpl(             MethodImplOptions.AggressiveInlining )] get; }
    Span<TValue> this[ Range range ] { [Pure, MethodImpl(             MethodImplOptions.AggressiveInlining )] get; }
    Span<TValue> this[ int   start, int length ] { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    Span<TValue> Next { [Pure] get; }
    Span<TValue> Span { [Pure] get; }


    ref TValue           GetPinnableReference();
    ref TValue           GetPinnableReference( TValue terminate );
    TValue[]             ToArray();
    Span<TValue>         Slice( int                     start );
    Span<TValue>         Slice( int                     start, int length );
    TValue               Get( int                       index );
    void                 Set( int                       index, TValue value );
    void                 Fill( TValue                   value );
    bool                 TryCopyTo( scoped Span<TValue> destination, out int length );
    void                 CopyTo( TValue[]               array );
    void                 CopyTo( int                    sourceStartIndex, TValue[] array,                 int length, int destinationStartIndex );
    void                 CopyTo( TValue[]               array,            int      destinationStartIndex, int length, int sourceStartIndex );
    ReadOnlySpan<TValue> AsSpan();
    ReadOnlySpan<TValue> AsSpan( TValue?      terminate );
    void                 EnsureCapacity( int  additionalCapacityBeyondPos );
    void                 EnsureCapacity( uint additionalCapacityBeyondPos );
}



public interface ICollectionRemove<TValue>
{
    bool RemoveAt( int                               index );
    void Replace( int                                index, TValue                      value, int count = 1 );
    void Replace( int                                index, params ReadOnlySpan<TValue> values );
    void RemoveRange( int                            start, int                         count );
    int  Remove( Func<TValue, bool>                  match );
    int  Remove( IEnumerable<TValue>                 values );
    int  Remove( params       ReadOnlySpan<TValue>   values );
    int  Remove( ref readonly ReadOnlyMemory<TValue> values );
    int  Remove( ref readonly ImmutableArray<TValue> values );
    bool RemoveAt( int                               index, [NotNullWhen( true )] out TValue? value );
}



public interface ICollectionAdd<TValue>
{
    void Add( TValue                                                          value );
    void Add( TValue                                                          value, int count );
    void Add( params       ReadOnlySpan<TValue>                               values );
    void Add( ref readonly SpanEnumerable<TValue, EnumerableProducer<TValue>> values );
    void Add( ref readonly ReadOnlyMemory<TValue>                             values );
    void Add( ref readonly ImmutableArray<TValue>                             values );
    void Add( IEnumerable<TValue>                                             enumerable );
    bool TryAdd( TValue                                                       value );
    void AddOrUpdate( TValue                                                  value );
    void AddOrUpdate( IEnumerable<TValue>                                     values );
    void AddOrUpdate( ref readonly ReadOnlyMemory<TValue>                     values );
    void AddOrUpdate( ref readonly ImmutableArray<TValue>                     values );
    void AddOrUpdate( params       ReadOnlySpan<TValue>                       values );
    void AddRange( TValue                                                     value, int count );
    void AddRange( IEnumerable<TValue>                                        enumerable );
    void Insert( int                                                          index, TValue                              value, int count = 1 );
    void Insert( int                                                          index, IEnumerable<TValue>                 collection );
    void Insert( int                                                          index, params       ReadOnlySpan<TValue>   collection );
    void Insert( int                                                          index, ref readonly ReadOnlyMemory<TValue> collection );
    void Insert( int                                                          index, ref readonly ImmutableArray<TValue> collection );
    bool Exists( Func<TValue, bool>                                           match );
}



public interface ICollectionSort<out TValue>
{
    void Sort();
    void Sort( IComparer<TValue>  comparer );
    void Sort( Comparison<TValue> comparer );
    void Sort( int                start, int count );
    void Sort( int                start, int length, IComparer<TValue> comparer );
}



public interface ICollectionSearch<TValue>
{
    void     Reverse();
    void     Reverse( int                                start, int length );
    int      IndexOf( TValue                             value, int start );
    int      IndexOf( TValue                             value, int start, int endInclusive );
    int      FindIndex( Func<TValue, bool>               match, int start );
    int      FindIndex( Func<TValue, bool>               match, int start, int endInclusive );
    int      LastIndexOf( TValue                         value );
    int      LastIndexOf( TValue                         value, int endInclusive );
    int      LastIndexOf( TValue                         value, int start, int endInclusive );
    int      FindLastIndex( Func<TValue, bool>           match, int endInclusive );
    int      FindLastIndex( Func<TValue, bool>           match, int start, int endInclusive );
    TValue?  FindLast( Func<TValue, bool>                match );
    TValue?  FindLast( Func<TValue, bool>                match, int endInclusive );
    TValue?  FindLast( Func<TValue, bool>                match, int start, int endInclusive );
    TValue?  Find( Func<TValue, bool>                    match );
    TValue?  Find( Func<TValue, bool>                    match, int start );
    TValue?  Find( Func<TValue, bool>                    match, int start, int endInclusive );
    TValue[] FindAll( Func<TValue, bool>                 match );
    TValue[] FindAll( Func<TValue, bool>                 match, int start );
    TValue[] FindAll( Func<TValue, bool>                 match, int start, int endInclusive );
    int      FindCount( Func<TValue, bool>               match );
    bool     Contains( TValue                            value );
    bool     Contains( ref readonly ReadOnlySpan<TValue> value );
}



public interface IIObservableCollection<TValue> : ICollectionAlerts, ICollection<TValue>, IReadOnlyCollection<TValue>, ICollectionSearch<TValue>, ICollectionSort<TValue>, ICollectionAdd<TValue>, ICollectionRemove<TValue>, IDisposable;
