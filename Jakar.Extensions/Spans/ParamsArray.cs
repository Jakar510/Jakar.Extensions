// Jakar.Extensions :: Jakar.Extensions
// 06/09/2022  3:00 PM


namespace Jakar.Extensions;


/// <summary>
///     <para> Based on System.ParamsArray </para>
/// </summary>
public readonly ref struct ParamsArray
{
    // Sentinel fixed-length arrays eliminate the need for a "count" field keeping this
    // struct down to just 4 fields. These are only used for their "Length" property,
    // that is, their elements are never set or referenced.
    private static readonly object?[] _oneArgArray   = new object?[1];
    private static readonly object?[] _twoArgArray   = new object?[2];
    private static readonly object?[] _threeArgArray = new object?[3];


    private readonly object? _arg0;
    private readonly object? _arg1;
    private readonly object? _arg2;

    // After construction, the first three elements of this array will never be accessed because the indexer will retrieve those values from arg0, arg1, and arg2.
    private readonly ReadOnlySpan<object?> _args;

    public int  Length  { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _args.Length; }
    public bool IsEmpty { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _args.IsEmpty; }

    public object? this[ int index ]
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        get => index switch
               {
                   0 => _arg0,
                   1 => _arg1,
                   2 => _arg2,
                   _ => _args[index]
               };
    }


    public ParamsArray( object? arg0 )
    {
        _arg0 = arg0;
        _arg1 = default;
        _arg2 = default;

        // Always assign this.args to make use of its "Length" property
        _args = _oneArgArray;
    }
    public ParamsArray( object? arg0, object? arg1 )
    {
        _arg0 = arg0;
        _arg1 = arg1;
        _arg2 = default;

        // Always assign this.args to make use of its "Length" property
        _args = _twoArgArray;
    }
    public ParamsArray( object? arg0, object? arg1, object? arg2 )
    {
        _arg0 = arg0;
        _arg1 = arg1;
        _arg2 = arg2;

        // Always assign this.args to make use of its "Length" property
        _args = _threeArgArray;
    }
    public ParamsArray( scoped in ReadOnlySpan<object?> args )
    {
        int len = args.Length;

        _arg0 = len > 0
                    ? args[0]
                    : default;

        _arg1 = len > 1
                    ? args[1]
                    : default;

        _arg2 = len > 2
                    ? args[2]
                    : default;

        _args = args;
    }


    public static implicit operator ParamsArray( Span<object?>         args ) => new(args);
    public static implicit operator ParamsArray( ReadOnlySpan<object?> args ) => new(args);
    public static implicit operator ReadOnlySpan<object?>( ParamsArray args ) => args._args;
    public static implicit operator ParamsArray( object?[]             args ) => Create( args );


    public override string ToString() => $"{nameof(ParamsArray)}<{nameof(Length)}: {Length}>";


    [Pure] public ReadOnlySpan<object?>.Enumerator GetEnumerator() => _args.GetEnumerator();

    [Pure] public static ParamsArray Create( params object?[] args ) => new ReadOnlySpan<object?>( args );

    public static implicit operator ParamsArray( List<object?> args ) => new(args);
    [Pure]
    public static ParamsArray Create( List<object?> args )
    {
        ReadOnlySpan<object?> span = CollectionsMarshal.AsSpan( args );
        return MemoryMarshal.CreateReadOnlySpan( ref MemoryMarshal.GetReference( span ), span.Length );
    }
}



/// <summary>
///     <para> Based on System.ParamsArray </para>
/// </summary>
public readonly ref struct ParamsArray<T>
    where T : IEquatable<T>
{
    public ReadOnlySpan<T> Span    { [MethodImpl(          MethodImplOptions.AggressiveInlining )] get; }
    public int             Length  { [MethodImpl(          MethodImplOptions.AggressiveInlining )] get => Span.Length; }
    public bool            IsEmpty { [MethodImpl(          MethodImplOptions.AggressiveInlining )] get => Span.IsEmpty; }
    public ref readonly T this[ int index ] { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ref Span[index]; }
    public int this[ T              value ] { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Span.IndexOf( value ); }


    public ParamsArray( T[]                       args ) => Span = args;
    public ParamsArray( scoped in ReadOnlySpan<T> args ) => Span = args;


    public static implicit operator ParamsArray<T>( Span<T>         args ) => new(args);
    public static implicit operator ParamsArray<T>( ReadOnlySpan<T> args ) => new(args);
    public static implicit operator ParamsArray<T>( T               args ) => Create( args );
    public static implicit operator ParamsArray<T>( T[]             args ) => Create( args );
    public static implicit operator ReadOnlySpan<T>( ParamsArray<T> args ) => args.Span;


    [Pure] public ReadOnlySpan<T>.Enumerator GetEnumerator() => Span.GetEnumerator();


    public override string ToString() => $"{nameof(ParamsArray<T>)}<{nameof(Length)}: {Length}>";


    [Pure]
    public static ParamsArray<T> Create( T arg0 )
    {
        Span<T> span = [arg0];
        return MemoryMarshal.CreateReadOnlySpan( ref span.GetPinnableReference(), span.Length );
    }
    [Pure]
    public static ParamsArray<T> Create( T arg0, T arg1 )
    {
        Span<T> span = [arg0, arg1];
        return MemoryMarshal.CreateReadOnlySpan( ref span.GetPinnableReference(), span.Length );
    }
    [Pure]
    public static ParamsArray<T> Create( T arg0, T arg1, T arg2 )
    {
        Span<T> span = [arg0, arg1, arg2];
        return MemoryMarshal.CreateReadOnlySpan( ref span.GetPinnableReference(), span.Length );
    }
    [Pure] public static ParamsArray<T> Create( params T[] args ) => new(args);


    public static implicit operator ParamsArray<T>( List<T> args ) => Create( args );

    [Pure] public static ParamsArray<T> Create( List<T> args ) => new(CollectionsMarshal.AsSpan( args ));
}
