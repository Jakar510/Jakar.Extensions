namespace Jakar.Extensions;


public static partial class AsyncLinq
{
    public static IEnumerable<int> Range( int start, int count ) => Range(start, count, 1);
    public static IEnumerable<int> Range( int start, int count, int offset )
    {
        for ( int i = start; i < start + count; i += offset ) { yield return i; }
    }


    public static IEnumerable<long> Range( long start, long count ) => Range(start, count, 1);
    public static IEnumerable<long> Range( long start, long count, long offset )
    {
        for ( long i = start; i < start + count; i += offset ) { yield return i; }
    }


    public static IEnumerable<float> Range( float start, long count ) => Range(start, count, 1);
    public static IEnumerable<float> Range( float start, long count, float offset )
    {
        for ( float i = start; i < start + count; i += offset ) { yield return i; }
    }


    public static IEnumerable<double> Range( double start, long count ) => Range(start, count, 1);
    public static IEnumerable<double> Range( double start, long count, double offset )
    {
        for ( double i = start; i < start + count; i += offset ) { yield return i; }
    }
}
