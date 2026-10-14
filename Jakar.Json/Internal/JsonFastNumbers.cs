// Jakar.Json
// 10/13/2026

namespace Jakar.Json;


/// <summary>
///     Exact fast paths for the common shapes of JSON numbers (a mantissa that fits a <see cref="ulong"/>, a short fraction, no exponent), before the BCL's general parsers.
///     Every fast path returns exactly what <c>T.TryParse(text, NumberStyles.Float, InvariantCulture)</c> would; anything else returns <see langword="false"/> so the caller falls back to it.
/// </summary>
internal static class JsonFastNumbers
{
    /// <summary> 10^0 … 10^22: exactly representable as <see cref="double"/>, so one IEEE division is correctly rounded (Clinger's fast path). </summary>
    private static ReadOnlySpan<double> __DoublePowers => [1e0, 1e1, 1e2, 1e3, 1e4, 1e5, 1e6, 1e7, 1e8, 1e9, 1e10, 1e11, 1e12, 1e13, 1e14, 1e15, 1e16, 1e17, 1e18, 1e19, 1e20, 1e21, 1e22];

    /// <summary> 10^0 … 10^10, exact as <see cref="float"/>. </summary>
    private static ReadOnlySpan<float> __SinglePowers => [1e0f, 1e1f, 1e2f, 1e3f, 1e4f, 1e5f, 1e6f, 1e7f, 1e8f, 1e9f, 1e10f];


    /// <param name="number"> A validated JSON number token (<c>-?digits(.digits)?</c>, possibly with an exponent). </param>
    /// <returns> <see langword="false"/>: use the general parser (the value isn't set). </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParse<TChar, T>( ReadOnlySpan<TChar> number, out T value )
        where TChar : unmanaged, IBinaryInteger<TChar>
        where T : struct, IFloatingPoint<T>
    {
        value = default;
        if ( typeof(T) != typeof(decimal) && typeof(T) != typeof(double) && typeof(T) != typeof(float) ) { return false; }

        if ( !TryReadMantissa(number, out bool negative, out ulong mantissa, out int digits, out int scale) ) { return false; }

        if ( typeof(T) == typeof(decimal) )
        {
            // Zero keeps the BCL's own rules for sign and scale ("-0", "0.00").
            if ( mantissa == 0 || scale > 28 ) { return false; }

            decimal result = new((int)(uint)mantissa, (int)(uint)( mantissa >> 32 ), 0, negative, (byte)scale);
            value = Unsafe.As<decimal, T>(ref result);
            return true;
        }

        if ( typeof(T) == typeof(double) )
        {
            if ( mantissa > 1UL << 53 || scale >= __DoublePowers.Length ) { return false; }

            double result = mantissa / __DoublePowers[scale];
            if ( negative ) { result = -result; }

            value = Unsafe.As<double, T>(ref result);
            return true;
        }

        // float
        if ( mantissa > 1UL << 24 || scale >= __SinglePowers.Length ) { return false; }

        float single = mantissa / __SinglePowers[scale];
        if ( negative ) { single = -single; }

        value = Unsafe.As<float, T>(ref single);
        return true;
    }


    /// <summary> <c>-?digits(.digits)?</c> with at most 19 digits in all, into a mantissa and the number of fraction digits; <see langword="false"/> for anything else (an exponent, too many digits). </summary>
    private static bool TryReadMantissa<TChar>( ReadOnlySpan<TChar> number, out bool negative, out ulong mantissa, out int digits, out int scale )
        where TChar : unmanaged, IBinaryInteger<TChar>
    {
        mantissa = 0;
        digits   = 0;
        scale    = 0;
        negative = false;

        int i = 0;

        if ( (uint)i < (uint)number.Length && uint.CreateTruncating(number[i]) == '-' )
        {
            negative = true;
            i++;
        }

        for ( ; (uint)i < (uint)number.Length; i++ )
        {
            uint d = uint.CreateTruncating(number[i]) - '0';
            if ( d > 9 ) { break; }

            if ( ++digits > 19 ) { return false; }

            mantissa = mantissa * 10 + d;
        }

        if ( (uint)i < (uint)number.Length && uint.CreateTruncating(number[i]) == '.' )
        {
            for ( i++; (uint)i < (uint)number.Length; i++ )
            {
                uint d = uint.CreateTruncating(number[i]) - '0';
                if ( d > 9 ) { break; }

                if ( ++digits > 19 ) { return false; }

                mantissa = mantissa * 10 + d;
                scale++;
            }
        }

        return i == number.Length && digits > 0; // anything left is an exponent: the general parser
    }
}
