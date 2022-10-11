// Jakar.Extensions :: Jakar.Extensions
// 06/09/2022  3:00 PM

#nullable enable
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

    public int Length => _args.Length;

    public object? this[ int index ] => index switch
                                        {
                                            0 => _arg0,
                                            1 => _arg1,
                                            2 => _arg2,
                                            _ => _args[index]
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
    public ParamsArray( in ReadOnlySpan<object?> args )
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


    public static ParamsArray Create( params object?[] args )
    {
        ReadOnlySpan<object?> span = args;
        return new ParamsArray( span );
    }

#if NET6_0_OR_GREATER
    public static ParamsArray Create( List<object?> args )
    {
        ReadOnlySpan<object?> span = CollectionsMarshal.AsSpan( args );
        return new ParamsArray( span );
    }
#endif
}
