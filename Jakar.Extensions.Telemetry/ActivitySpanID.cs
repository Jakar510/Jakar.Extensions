// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 01/09/2025  16:01

using System.Buffers;
using System.Buffers.Binary;
using System.Buffers.Text;
using System.Linq;



namespace Jakar.Extensions.Telemetry;


public readonly struct ActivitySpanID : IEquatable<ActivitySpanID>
{
    public const            int            SIZE       = sizeof(ulong);
    public const            int            LENGTH     = sizeof(ulong) * 2;
    private static readonly string         _empty     = new('0', LENGTH);
    private readonly        string         _hexString = _empty;
    private readonly        int            _hash      = _empty.GetHashCode();
    public static readonly  ActivitySpanID Empty      = new(_empty);


    internal ActivitySpanID( string hexString )
    {
        _hexString = hexString;
        _hash      = hexString.GetHashCode();
    }
    private ActivitySpanID( ReadOnlySpan<byte> idData )
    {
        if ( idData.Length != LENGTH ) { throw new ArgumentOutOfRangeException( nameof(idData) ); }

        if ( Utf8Parser.TryParse( idData, out ulong id, out _, 'x' ) is false )
        {
            _hexString = CreateRandom()._hexString;
            return;
        }

        if ( BitConverter.IsLittleEndian ) { id = BinaryPrimitives.ReverseEndianness( id ); }

        Span<byte> span = stackalloc byte[SIZE];
        id.TryFormat( span, out _ );
        _hexString = Convert.ToHexStringLower( span );
    }

    public static ActivitySpanID CreateRandom()                                    => new(ActivityTraceID.CreateRandom( SIZE ));
    public static ActivitySpanID CreateFromUtf8String( ReadOnlySpan<byte> idData ) => new(idData);
    public static ActivitySpanID CreateFromBytes( ReadOnlySpan<byte> idData ) => idData.Length != SIZE
                                                                                     ? throw new ArgumentOutOfRangeException( nameof(idData) )
                                                                                     : new ActivitySpanID( Convert.ToHexStringLower( idData ) );
    public static ActivitySpanID CreateFromString( string idData ) => idData.Length != LENGTH || ActivityTraceID.IsLowerCaseHexAndNotAllZeros( idData ) is false
                                                                          ? throw new ArgumentOutOfRangeException( idData )
                                                                          : new ActivitySpanID( idData );
    public override string ToString() => _hexString;

    public bool Equals( ActivitySpanID spanId ) => _hexString == spanId._hexString;
    public override bool Equals( [NotNullWhen( true )] object? obj )
    {
        if ( obj is ActivitySpanID spanId ) { return _hexString == spanId._hexString; }

        return false;
    }
    public override int GetHashCode() => _hash;

    public bool CopyTo( Span<byte> destination )                       => Convert.FromHexString( _hexString, destination, out _, out _ ) is OperationStatus.Done;
    public bool CopyTo( Span<byte> destination, out int bytesWritten ) => Convert.FromHexString( _hexString, destination, out _, out bytesWritten ) is OperationStatus.Done;

    public static string Collate( IEnumerable<ActivitySpanID> spans ) => string.Join( '|', spans.Select( static x => x._hexString ) );

    public static bool operator ==( ActivitySpanID left, ActivitySpanID right ) => left._hexString == right._hexString;
    public static bool operator !=( ActivitySpanID left, ActivitySpanID right ) => left._hexString != right._hexString;
}
