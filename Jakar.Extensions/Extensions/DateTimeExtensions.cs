// Jakar.Extensions :: Jakar.Extensions
// 09/29/2022  11:35 PM

namespace Jakar.Extensions;


public static class DateTimeExtensions
{
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static DateOnly AsDateOnly( this DateTime       date ) => new(date.Year, date.Month, date.Day);
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static DateOnly AsDateOnly( this DateTimeOffset date ) => new(date.Year, date.Month, date.Day);
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TimeOnly AsTimeOnly( this DateTime       date ) => new(date.Hour, date.Minute, date.Second);
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static TimeOnly AsTimeOnly( this DateTimeOffset date ) => new(date.Hour, date.Minute, date.Second);
}
