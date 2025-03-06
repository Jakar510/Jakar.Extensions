// Jakar.Extensions :: Jakar.Extensions
// 06/09/2022  3:00 PM


namespace Jakar.Extensions;


/// <summary>
///     <para> Based on System.ParamsArray </para>
/// </summary>
public readonly ref struct ParamsArray<T>( IMemoryOwner<T> owner, int length )
{
    private readonly IMemoryOwner<T> _owner  = owner;
    public readonly  ReadOnlySpan<T> Span    = owner.Memory.Span[..length];
    public readonly  int             Length  = length;
    public readonly  bool            IsEmpty = length <= 0;


    public ref readonly T this[ int index ] { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ref Span[index]; }
    public          void                       Dispose()       => _owner.Dispose();
    public override string                     ToString()      => $"ParamsArray<{typeof(T).Name}>({nameof(Length)}: {Length})";
    [Pure] public   ReadOnlySpan<T>.Enumerator GetEnumerator() => Span.GetEnumerator();


    public static implicit operator ParamsArray<T>( Span<T>         args ) => Create( args );
    public static implicit operator ParamsArray<T>( ReadOnlySpan<T> args ) => Create( args );
    public static implicit operator ParamsArray<T>( T               args ) => Create( args );
    public static implicit operator ParamsArray<T>( T[]             args ) => Create( args );
    public static implicit operator ParamsArray<T>( List<T>         args ) => Create( args );
    public static implicit operator ReadOnlySpan<T>( ParamsArray<T> args ) => args.Span;


    [Pure]
    public static ParamsArray<T> Create( params ReadOnlySpan<T> args )
    {
        IMemoryOwner<T> owner = MemoryPool<T>.Shared.Rent( args.Length );
        args.CopyTo( owner.Memory.Span );
        return new ParamsArray<T>( owner, args.Length );
    }
    [Pure]
    public static ParamsArray<T> Create( List<T> args )
    {
        IMemoryOwner<T> owner = MemoryPool<T>.Shared.Rent( args.Count );
        CollectionsMarshal.AsSpan( args ).CopyTo( owner.Memory.Span );
        return new ParamsArray<T>( owner, args.Count );
    }
}
