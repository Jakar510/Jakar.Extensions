#nullable enable
namespace Jakar.Extensions;


public static class TimeSpanExtensions
{
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TimeSpan FromDays( this         int    value ) => TimeSpan.FromDays( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TimeSpan FromDays( this         long   value ) => TimeSpan.FromDays( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TimeSpan FromDays( this         double value ) => TimeSpan.FromDays( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TimeSpan FromHours( this        int    value ) => TimeSpan.FromHours( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TimeSpan FromHours( this        long   value ) => TimeSpan.FromHours( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TimeSpan FromHours( this        double value ) => TimeSpan.FromHours( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TimeSpan FromMilliseconds( this int    value ) => TimeSpan.FromMilliseconds( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TimeSpan FromMilliseconds( this long   value ) => TimeSpan.FromMilliseconds( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TimeSpan FromMilliseconds( this double value ) => TimeSpan.FromMilliseconds( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TimeSpan FromMinutes( this      int    value ) => TimeSpan.FromMinutes( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TimeSpan FromMinutes( this      long   value ) => TimeSpan.FromMinutes( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TimeSpan FromMinutes( this      double value ) => TimeSpan.FromMinutes( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TimeSpan FromSeconds( this      int    value ) => TimeSpan.FromSeconds( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TimeSpan FromSeconds( this      long   value ) => TimeSpan.FromSeconds( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TimeSpan FromSeconds( this      double value ) => TimeSpan.FromSeconds( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TimeSpan FromTicks( this        int    value ) => TimeSpan.FromTicks( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TimeSpan FromTicks( this        long   value ) => TimeSpan.FromTicks( value );
}
