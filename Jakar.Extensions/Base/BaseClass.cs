namespace Jakar.Extensions;


[Serializable]
public class BaseClass
{
    public const int    ANSI_CAPACITY    = BaseRecord.ANSI_CAPACITY;
    public const int    BINARY_CAPACITY  = BaseRecord.BINARY_CAPACITY;
    public const string EMPTY            = BaseRecord.EMPTY;
    public const int    MAX_STRING_SIZE  = BaseRecord.MAX_STRING_SIZE; // 1GB
    public const string NULL             = BaseRecord.NULL;
    public const int    UNICODE_CAPACITY = BaseRecord.UNICODE_CAPACITY;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected static void ClearAndDispose<TValue>( ref TValue? field )
        where TValue : IDisposable => Disposables.CastAndDispose( ref field );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected static ValueTask ClearAndDisposeAsync<TValue>( ref TValue? resource )
        where TValue : class, IDisposable => Disposables.CastAndDisposeAsync( ref resource );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] protected static ValueTask CastAndDispose( IDisposable? resource ) => Disposables.CastAndDisposeAsync( resource );
}
