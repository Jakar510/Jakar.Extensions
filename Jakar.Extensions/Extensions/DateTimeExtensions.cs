﻿// Jakar.Extensions :: Jakar.Extensions
// 09/29/2022  11:35 PM

namespace Jakar.Extensions;


#if NET6_0_OR_GREATER
public static class DateTimeExtensions
{
    public static DateOnly AsDateOnly( this DateTime       date ) => new(date.Year, date.Month, date.Day);
    public static DateOnly AsDateOnly( this DateTimeOffset date ) => new(date.Year, date.Month, date.Day);
    public static TimeOnly AsTimeOnly( this DateTime       date ) => new(date.Hour, date.Minute, date.Second);
    public static TimeOnly AsTimeOnly( this DateTimeOffset date ) => new(date.Hour, date.Minute, date.Second);
}
#endif
