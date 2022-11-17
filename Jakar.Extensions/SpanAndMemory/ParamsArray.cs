// Jakar.Extensions :: Jakar.Extensions
// 06/09/2022  3:00 PM


namespace Jakar.Extensions;
#nullable enable



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

    public int  Length  => _args.Length;
    public bool IsEmpty => _args.IsEmpty;

    public object? this[ int index ] => index switch
                                        {
                                            0 => _arg0,
                                            1 => _arg1,
                                            2 => _arg2,
                                            _ => _args[index],
                                        };


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
    public ParamsArray( ReadOnlySpan<object?> args )
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


    public static implicit operator ParamsArray( ReadOnlySpan<object?> args ) => new(args);
    public static implicit operator ReadOnlySpan<object?>( ParamsArray args ) => args._args;
    public static implicit operator ParamsArray( object?[]             args ) => new(args.AsSpan());


    public override string ToString() => $"{nameof(ParamsArray)}<{nameof(Length)}: {Length}>";


    [Pure] public ReadOnlySpan<object?>.Enumerator GetEnumerator() => _args.GetEnumerator();

    [Pure]
    public static ParamsArray Create( params object?[] args )
    {
        ReadOnlySpan<object?> span = args;
        return MemoryMarshal.CreateReadOnlySpan( ref MemoryMarshal.GetReference( span ), span.Length );
    }

#if NET6_0_OR_GREATER
    public static implicit operator ParamsArray( List<object?> args ) => new(args);
    [Pure]
    public static ParamsArray Create( List<object?> args )
    {
        ReadOnlySpan<object?> span = CollectionsMarshal.AsSpan( args );
        return MemoryMarshal.CreateReadOnlySpan( ref MemoryMarshal.GetReference( span ), span.Length );
    }
#endif
}



/// <summary>
///     <para> Based on System.ParamsArray </para>
/// </summary>
public readonly ref struct ParamsArray<T> where T : unmanaged, IEquatable<T>
{
    public ReadOnlySpan<T> Span    { get; }
    public int             Length  => Span.Length;
    public bool            IsEmpty => Span.IsEmpty;


    public T this[ int index ] => Span[index];
    public int this[ T value ] => Span.IndexOf( value );


    public ParamsArray( ReadOnlySpan<T> args ) => Span = args;


    public static implicit operator ParamsArray<T>( ReadOnlySpan<T> args ) => new(args);
    public static implicit operator ParamsArray<T>( T               args ) => Create( args );
    public static implicit operator ParamsArray<T>( T[]             args ) => new(args);
    public static implicit operator ReadOnlySpan<T>( ParamsArray<T> args ) => args.Span;


    [Pure] public ReadOnlySpan<T>.Enumerator GetEnumerator() => Span.GetEnumerator();


    public override string ToString() => $"{nameof(ParamsArray<T>)}<{nameof(Length)}: {Length}>";
    private bool TryFormat( Span<T> destination, out int charsWritten, ReadOnlySpan<T> format, IFormatProvider? provider )
    {
        if ( Span.TryCopyTo( destination ) )
        {
            charsWritten = Length;
            return true;
        }

        charsWritten = 0;
        return false;
    }


    [Pure]
    public static ParamsArray<T> Create( T arg0 )
    {
        Span<T> span = stackalloc T[1];
        span[0] = arg0;
        return MemoryMarshal.CreateReadOnlySpan( ref span.GetPinnableReference(), span.Length );
    }
    [Pure]
    public static ParamsArray<T> Create( T arg0, T arg1 )
    {
        Span<T> span = stackalloc T[2];
        span[0] = arg0;
        span[1] = arg1;
        return MemoryMarshal.CreateReadOnlySpan( ref span.GetPinnableReference(), span.Length );
    }
    [Pure]
    public static ParamsArray<T> Create( T arg0, T arg1, T arg2 )
    {
        Span<T> span = stackalloc T[3];
        span[0] = arg0;
        span[1] = arg1;
        span[2] = arg2;
        return MemoryMarshal.CreateReadOnlySpan( ref span.GetPinnableReference(), span.Length );
    }
    [Pure]
    public static ParamsArray<T> Create( params T[] args )
    {
        ReadOnlySpan<T> span = args;
        return MemoryMarshal.CreateReadOnlySpan( ref MemoryMarshal.GetReference( span ), span.Length );
    }


#if NET6_0_OR_GREATER
    public static implicit operator ParamsArray<T>( List<T> args ) => Create( args );

    [Pure]
    public static ParamsArray<T> Create( List<T> args )
    {
        ReadOnlySpan<T> span = CollectionsMarshal.AsSpan( args );
        return MemoryMarshal.CreateReadOnlySpan( ref MemoryMarshal.GetReference( span ), span.Length );
    }
#endif
}
