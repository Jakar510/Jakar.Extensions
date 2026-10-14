// Jakar.Json
// 10/13/2026

namespace Jakar.Json;


/// <summary>
///     An exact fast path for the ISO 8601 shapes JSON uses (<c>yyyy-MM-ddTHH:mm:ss</c>, an optional fraction of up to 7 digits, then <c>Z</c>, <c>±HH:mm</c> or nothing), before the
///     BCL's general, culture-aware parser. Each accepted input gives exactly what the general parser would (value, <see cref="DateTime.Kind"/>, offset); anything else returns
///     <see langword="false"/> so the caller falls back to it: other layouts, leap seconds, hour 24, an 8th fraction digit, out-of-range values, and (for <see cref="DateTime"/>) offsets,
///     which the general parser converts to local time.
/// </summary>
internal static class JsonFastDates
{
    /// <summary> Like <c>DateTimeOffset.TryParse(text, InvariantCulture, DateTimeStyles.AssumeUniversal)</c>. </summary>
    public static bool TryParseDateTimeOffset( ReadOnlySpan<char> text, out DateTimeOffset value )
    {
        value = default;
        if ( !TryParseParts(text, out DateTime local, out Zone zone, out int offsetMinutes) ) { return false; }

        TimeSpan offset = zone == Zone.Offset
                              ? TimeSpan.FromMinutes(offsetMinutes)
                              : TimeSpan.Zero; // Z, or no zone with AssumeUniversal

        long utcTicks = local.Ticks - offset.Ticks;
        if ( utcTicks < DateTime.MinValue.Ticks || utcTicks > DateTime.MaxValue.Ticks ) { return false; }

        value = new DateTimeOffset(local, offset);
        return true;
    }

    /// <summary> Like <c>DateTime.TryParse(text, InvariantCulture, DateTimeStyles.RoundtripKind)</c>, for <c>Z</c> (UTC) and no zone (unspecified); offsets fall back. </summary>
    public static bool TryParseDateTime( ReadOnlySpan<char> text, out DateTime value )
    {
        value = default;
        if ( !TryParseParts(text, out DateTime local, out Zone zone, out _) || zone == Zone.Offset ) { return false; }

        value = DateTime.SpecifyKind(local,
                                     zone == Zone.Utc
                                         ? DateTimeKind.Utc
                                         : DateTimeKind.Unspecified);

        return true;
    }



    private enum Zone : byte
    {
        None,
        Utc,
        Offset
    }



    private static bool TryParseParts( ReadOnlySpan<char> text, out DateTime local, out Zone zone, out int offsetMinutes )
    {
        local         = default;
        zone          = Zone.None;
        offsetMinutes = 0;

        // yyyy-MM-ddTHH:mm:ss is 19 chars; anything shorter isn't this shape.
        if ( text.Length < 19 || text[4] != '-' || text[7] != '-' || text[10] != 'T' || text[13] != ':' || text[16] != ':' ) { return false; }

        if ( !Digits(text, 0, 4, out int year) || !Digits(text, 5, 2, out int month) || !Digits(text, 8, 2, out int day) || !Digits(text, 11, 2, out int hour) || !Digits(text, 14, 2, out int minute) || !Digits(text, 17, 2, out int second) ) { return false; }

        if ( year < 1 || month is < 1 or > 12 || day < 1 || day > DateTime.DaysInMonth(year, month) || hour > 23 || minute > 59 || second > 59 ) { return false; }

        long ticks = 0;
        int  at    = 19;

        if ( at < text.Length && text[at] == '.' )
        {
            int start = ++at;

            while ( at < text.Length && (uint)( text[at] - '0' ) <= 9 )
            {
                if ( at - start == 7 ) { return false; } // an 8th digit: the general parser rounds

                ticks = ticks * 10 + ( text[at] - '0' );
                at++;
            }

            if ( at == start ) { return false; }

            for ( int digits = at - start; digits < 7; digits++ ) { ticks *= 10; }
        }

        if ( at < text.Length )
        {
            char sign = text[at];

            if ( sign == 'Z'                         && at == text.Length - 1 ) { zone = Zone.Utc; }
            else if ( ( sign == '+' || sign == '-' ) && at                + 6 == text.Length && text[at + 3] == ':' && Digits(text, at + 1, 2, out int offsetHours) && Digits(text, at + 4, 2, out int offsetMinute) )
            {
                if ( offsetHours > 14 || offsetMinute > 59 || offsetHours == 14 && offsetMinute != 0 ) { return false; }

                zone = Zone.Offset;

                offsetMinutes = ( offsetHours * 60 + offsetMinute ) *
                                ( sign == '-'
                                      ? -1
                                      : 1 );
            }
            else { return false; }
        }

        local = new DateTime(year, month, day, hour, minute, second).AddTicks(ticks);
        return true;
    }

    private static bool Digits( ReadOnlySpan<char> text, int start, int count, out int value )
    {
        value = 0;

        for ( int i = start; i < start + count; i++ )
        {
            uint d = (uint)( text[i] - '0' );
            if ( d > 9 ) { return false; }

            value = value * 10 + (int)d;
        }

        return true;
    }
}
