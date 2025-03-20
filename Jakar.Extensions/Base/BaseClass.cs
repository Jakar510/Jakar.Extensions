namespace Jakar.Extensions;


[Serializable]
public class BaseClass
{
    public const int    ANSI_CAPACITY    = BaseRecord.ANSI_CAPACITY;
    public const int    BINARY_CAPACITY  = BaseRecord.BINARY_CAPACITY;
    public const int    MAX_STRING_SIZE  = BaseRecord.MAX_STRING_SIZE; // 1GB
    public const int    UNICODE_CAPACITY = BaseRecord.UNICODE_CAPACITY;
    public const string EMPTY            = BaseRecord.EMPTY;
    public const string NULL             = BaseRecord.NULL;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected static void ClearAndDispose<T>( ref T? field )
        where T : IDisposable => Disposables.CastAndDispose( ref field );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected static ValueTask ClearAndDisposeAsync<T>( ref T? resource )
        where T : class, IDisposable => Disposables.CastAndDisposeAsync( ref resource );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] protected static ValueTask CastAndDispose( IDisposable? resource ) => Disposables.CastAndDisposeAsync( resource );
}
