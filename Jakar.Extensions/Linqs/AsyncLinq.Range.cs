namespace Jakar.Extensions;


public static partial class AsyncLinq
{
#if NET7_0_OR_GREATER
    public static IEnumerable<T> Range<T>( T start, T count )
        where T : INumber<T> => Range( start, count, T.One );
    public static IEnumerable<T> Range<T>( T start, T count, T offset )
        where T : INumber<T>
    {
        for ( T i = start; i < start + count; i += offset ) { yield return i; }
    }

#else
    public static IEnumerable<sbyte> Range( sbyte start, sbyte count ) => Range( start, count, (sbyte)1 );
    public static IEnumerable<sbyte> Range( sbyte start, sbyte count, sbyte offset )
    {
        for ( sbyte i = start; i < start + count; i += offset ) { yield return i; }
    }


    public static IEnumerable<byte> Range( byte start, byte count ) => Range( start, count, (byte)1 );
    public static IEnumerable<byte> Range( byte start, byte count, byte offset )
    {
        for ( byte i = start; i < start + count; i += offset ) { yield return i; }
    }


    public static IEnumerable<short> Range( short start, short count ) => Range( start, count, (short)1 );
    public static IEnumerable<short> Range( short start, short count, short offset )
    {
        for ( short i = start; i < start + count; i += offset ) { yield return i; }
    }


    public static IEnumerable<ushort> Range( ushort start, ushort count ) => Range( start, count, (ushort)1 );
    public static IEnumerable<ushort> Range( ushort start, ushort count, ushort offset )
    {
        for ( ushort i = start; i < start + count; i += offset ) { yield return i; }
    }


    public static IEnumerable<int> Range( int start, int count ) => Range( start, count, 1 );
    public static IEnumerable<int> Range( int start, int count, int offset )
    {
        for ( int i = start; i < start + count; i += offset ) { yield return i; }
    }


    public static IEnumerable<uint> Range( uint start, uint count ) => Range( start, count, 1 );
    public static IEnumerable<uint> Range( uint start, uint count, uint offset )
    {
        for ( uint i = start; i < start + count; i += offset ) { yield return i; }
    }


    public static IEnumerable<long> Range( long start, long count ) => Range( start, count, 1 );
    public static IEnumerable<long> Range( long start, long count, long offset )
    {
        for ( long i = start; i < start + count; i += offset ) { yield return i; }
    }


    public static IEnumerable<ulong> Range( ulong start, ulong count ) => Range( start, count, 1 );
    public static IEnumerable<ulong> Range( ulong start, ulong count, ulong offset )
    {
        for ( ulong i = start; i < start + count; i += offset ) { yield return i; }
    }

    public static IEnumerable<float> Range( float start, long count ) => Range( start, count, 1 );
    public static IEnumerable<float> Range( float start, long count, float offset )
    {
        for ( float i = start; i < start + count; i += offset ) { yield return i; }
    }


    public static IEnumerable<double> Range( double start, long count ) => Range( start, count, 1 );
    public static IEnumerable<double> Range( double start, long count, double offset )
    {
        for ( double i = start; i < start + count; i += offset ) { yield return i; }
    }
#endif
}
