// Jakar.Extensions :: Jakar.Extensions
// 06/09/2022  3:00 PM


namespace Jakar.Extensions;


/// <summary>
///     <para> Based on System.ParamsArray </para>
/// </summary>
public readonly ref struct ParamsArray<TValue>( IMemoryOwner<TValue> owner, int length )
{
    private readonly IMemoryOwner<TValue> _owner  = owner;
    public readonly  ReadOnlySpan<TValue> Span    = owner.Memory.Span[..length];
    public readonly  int             Length  = length;
    public readonly  bool            IsEmpty = length <= 0;


    public ref readonly TValue this[ int index ] { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ref Span[index]; }
    public          void                       Dispose()       => _owner.Dispose();
    public override string                     ToString()      => $"ParamsArray<{typeof(TValue).Name}>({nameof(Length)}: {Length})";
    [Pure] public   ReadOnlySpan<TValue>.Enumerator GetEnumerator() => Span.GetEnumerator();


    public static implicit operator ParamsArray<TValue>( Span<TValue>         args ) => Create( args );
    public static implicit operator ParamsArray<TValue>( ReadOnlySpan<TValue> args ) => Create( args );
    public static implicit operator ParamsArray<TValue>( TValue               args ) => Create( args );
    public static implicit operator ParamsArray<TValue>( TValue[]             args ) => Create( args );
    public static implicit operator ParamsArray<TValue>( List<TValue>         args ) => Create( args );
    public static implicit operator ReadOnlySpan<TValue>( ParamsArray<TValue> args ) => args.Span;


    [Pure]
    public static ParamsArray<TValue> Create( params ReadOnlySpan<TValue> args )
    {
        IMemoryOwner<TValue> owner = MemoryPool<TValue>.Shared.Rent( args.Length );
        args.CopyTo( owner.Memory.Span );
        return new ParamsArray<TValue>( owner, args.Length );
    }
    [Pure]
    public static ParamsArray<TValue> Create( List<TValue> args )
    {
        IMemoryOwner<TValue> owner = MemoryPool<TValue>.Shared.Rent( args.Count );
        CollectionsMarshal.AsSpan( args ).CopyTo( owner.Memory.Span );
        return new ParamsArray<TValue>( owner, args.Count );
    }
}
