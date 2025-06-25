// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 01/09/2025  16:01

namespace Jakar.Extensions.Telemetry;


[Serializable, StructLayout( LayoutKind.Auto )]
public readonly struct TelemetryActivityTraceID : IEquatable<TelemetryActivityTraceID>
{
    public const            int                      SIZE       = sizeof(ulong) * 2;
    public const            int                      LENGTH     = sizeof(ulong) * 4;
    private static readonly string                   _empty     = new('0', LENGTH);
    public static readonly  TelemetryActivityTraceID Empty      = new(_empty);
    private readonly        string                   _hexString = _empty;
    private readonly        int                      _hash      = _empty.GetHashCode();


    internal TelemetryActivityTraceID( string hexString ) => _hexString = hexString;
    private TelemetryActivityTraceID( ReadOnlySpan<byte> idData )
    {
        if ( idData.Length != 32 ) { throw new ArgumentOutOfRangeException( nameof(idData) ); }

        Span<ulong> span = stackalloc ulong[2];

        if ( Utf8Parser.TryParse( idData[..16], out span[0], out _, 'x' ) is false )
        {
            // Invalid Id, use random https://github.com/dotnet/runtime/issues/29859
            _hexString = CreateRandom()._hexString;
            return;
        }

        if ( Utf8Parser.TryParse( idData.Slice( 16, 16 ), out span[1], out _, 'x' ) is false )
        {
            // Invalid Id, use random https://github.com/dotnet/runtime/issues/29859
            _hexString = CreateRandom()._hexString;
            return;
        }

        if ( BitConverter.IsLittleEndian )
        {
            span[0] = BinaryPrimitives.ReverseEndianness( span[0] );
            span[1] = BinaryPrimitives.ReverseEndianness( span[1] );
        }

        _hexString = Convert.ToHexStringLower( MemoryMarshal.AsBytes( span ) );
    }

    public static TelemetryActivityTraceID CreateRandom()                                    => new(CreateRandom( SIZE ));
    public static TelemetryActivityTraceID CreateFromUtf8String( ReadOnlySpan<byte> idData ) => new(idData);
    public static TelemetryActivityTraceID CreateFromBytes( ReadOnlySpan<byte> idData ) => idData.Length != SIZE
                                                                                               ? throw new ArgumentOutOfRangeException( nameof(idData) )
                                                                                               : new TelemetryActivityTraceID( Convert.ToHexStringLower( idData ) );
    public static TelemetryActivityTraceID CreateFromString( ReadOnlySpan<char> idData ) => idData.Length != LENGTH || IsLowerCaseHexAndNotAllZeros( idData ) is false
                                                                                                ? throw new ArgumentOutOfRangeException( nameof(idData) )
                                                                                                : new TelemetryActivityTraceID( idData.ToString() );
    public override string ToString() => _hexString;

    public bool Equals( TelemetryActivityTraceID traceId ) => _hexString == traceId._hexString;
    public override bool Equals( [NotNullWhen( true )] object? obj )
    {
        if ( obj is TelemetryActivityTraceID traceId ) { return _hexString == traceId._hexString; }

        return false;
    }
    public override int GetHashCode() => _hash;

    public bool CopyTo( Span<byte> destination )                       => Convert.FromHexString( _hexString, destination, out _, out _ ) is OperationStatus.Done;
    public bool CopyTo( Span<byte> destination, out int bytesWritten ) => Convert.FromHexString( _hexString, destination, out _, out bytesWritten ) is OperationStatus.Done;

    internal static string CreateRandom( int size )
    {
        Span<byte> span = stackalloc byte[size];
        RandomNumberGenerator.Fill( span );
        return Convert.ToHexStringLower( span );
    }
    internal static bool IsLowerCaseHexAndNotAllZeros( ReadOnlySpan<char> idData )
    {
        bool isNonZero = false;
        int  i         = 0;

        for ( ; i < idData.Length; i++ )
        {
            char c = idData[i];
            if ( char.IsLower( c ) ) { return false; }

            if ( c != '0' ) { isNonZero = true; }
        }

        return isNonZero;
    }


    public static string Collate( IEnumerable<TelemetryActivityTraceID> spans ) => $"|{string.Join( '|', spans.Select( static x => x._hexString ) )}";


    public static bool operator ==( TelemetryActivityTraceID traceId1, TelemetryActivityTraceID traceId2 ) => traceId1._hexString == traceId2._hexString;
    public static bool operator !=( TelemetryActivityTraceID traceId1, TelemetryActivityTraceID traceId2 ) => traceId1._hexString != traceId2._hexString;
}
