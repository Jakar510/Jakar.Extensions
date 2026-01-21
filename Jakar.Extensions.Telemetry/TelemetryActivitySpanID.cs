// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 01/09/2025  16:01

namespace Jakar.Extensions.Telemetry;


[Serializable][StructLayout(LayoutKind.Auto)]
public readonly struct TelemetryActivitySpanID : IEquatable<TelemetryActivitySpanID>
{
    public const            int                     SIZE       = sizeof(ulong);
    public const            int                     LENGTH     = sizeof(ulong) * 2;
    private static readonly string                  _empty     = new('0', LENGTH);
    public static readonly  TelemetryActivitySpanID Empty      = new(_empty);
    private readonly        string                  _hexString = _empty;
    private readonly        int                     _hash      = _empty.GetHashCode();


    internal TelemetryActivitySpanID( string hexString )
    {
        _hexString = hexString;
        _hash      = hexString.GetHashCode();
    }
    private TelemetryActivitySpanID( params ReadOnlySpan<byte> idData )
    {
        if ( idData.Length != LENGTH ) { throw new ArgumentOutOfRangeException(nameof(idData)); }

        if ( !Utf8Parser.TryParse(idData, out ulong id, out _, 'x') )
        {
            _hexString = CreateRandom()._hexString;
            return;
        }

        if ( BitConverter.IsLittleEndian ) { id = BinaryPrimitives.ReverseEndianness(id); }

        Span<byte> span = stackalloc byte[SIZE];
        id.TryFormat(span, out _);
        _hexString = Convert.ToHexStringLower(span);
    }


    public static TelemetryActivitySpanID CreateRandom()                                           => new(TelemetryActivityTraceID.CreateRandom(SIZE));
    public static TelemetryActivitySpanID CreateFromUtf8String( params ReadOnlySpan<byte> idData ) => new(idData);
    public static TelemetryActivitySpanID CreateFromBytes( params ReadOnlySpan<byte> idData ) => idData.Length != SIZE
                                                                                                     ? throw new ArgumentOutOfRangeException(nameof(idData))
                                                                                                     : new TelemetryActivitySpanID(Convert.ToHexStringLower(idData));
    public static TelemetryActivitySpanID CreateFromString( string idData ) => idData.Length != LENGTH || !TelemetryActivityTraceID.IsLowerCaseHexAndNotAllZeros(idData)
                                                                                   ? throw new ArgumentOutOfRangeException(idData)
                                                                                   : new TelemetryActivitySpanID(idData);
    public override string ToString() => _hexString;


    public bool Equals( TelemetryActivitySpanID spanId ) => _hexString == spanId._hexString;
    public override bool Equals( [NotNullWhen(true)] object? obj )
    {
        if ( obj is TelemetryActivitySpanID spanId ) { return _hexString == spanId._hexString; }

        return false;
    }
    public override int GetHashCode() => _hash;

    public bool CopyTo( Span<byte> destination )                       => Convert.FromHexString(_hexString, destination, out _, out _) is OperationStatus.Done;
    public bool CopyTo( Span<byte> destination, out int bytesWritten ) => Convert.FromHexString(_hexString, destination, out _, out bytesWritten) is OperationStatus.Done;


    public static string Collate( TelemetryActivity? current )
    {
        List<TelemetryActivitySpanID> values = new(DEFAULT_CAPACITY);

        while ( current is not null )
        {
            values.Add(current.context.SpanID);
            current = current.parent;
        }

        values.Reverse();
        return Collate(values.AsSpan());
    }
    public static string Collate( IEnumerable<TelemetryActivityContext>         contexts ) => Collate(contexts.AsValueEnumerable().Select(static x => x.SpanID));
    public static string Collate( params ReadOnlySpan<TelemetryActivityContext> contexts ) => Collate(contexts.AsValueEnumerable().Select(static x => x.SpanID));
    public static string Collate<TEnumerator>( ValueEnumerable<TEnumerator, TelemetryActivityContext> contexts )
        where TEnumerator : struct, IValueEnumerator<TelemetryActivityContext> => Collate(contexts.Select(static x => x.SpanID));
    public static string Collate<TEnumerator>( ValueEnumerable<TEnumerator, TelemetryActivitySpanID> spans )
        where TEnumerator : struct, IValueEnumerator<TelemetryActivitySpanID>, allows ref struct
    {
        if ( !spans.TryGetNonEnumeratedCount(out int count) ) { throw new InvalidOperationException("The enumerator does not support non-enumerated count retrieval."); }

        using IMemoryOwner<string?> owner = MemoryPool<string?>.Shared.Rent(count);
        Span<string?>               ids   = owner.Memory.Span[..count];
        int                         i     = 0;

        foreach ( TelemetryActivitySpanID context in spans ) { ids[i++] = context._hexString; }

        return $"|{string.Join('|', ids)}";
    }
    public static string Collate( params ReadOnlySpan<TelemetryActivitySpanID> spans )
    {
        int                        count = spans.Length;
        using IMemoryOwner<string> owner = MemoryPool<string>.Shared.Rent(count);
        Span<string>               span  = owner.Memory.Span[..count];
        for ( int i = 0; i < count; i++ ) { span[i] = spans[i]._hexString; }

        using ValueStringBuilder sb = new();
        sb.AppendJoin('|', span);
        return sb.ToString();

        // return $"|{string.Join('|', span)}";
    }


    public static bool operator ==( TelemetryActivitySpanID left, TelemetryActivitySpanID right ) => left._hexString == right._hexString;
    public static bool operator !=( TelemetryActivitySpanID left, TelemetryActivitySpanID right ) => left._hexString != right._hexString;
}
