// Jakar.Extensions :: Jakar.Shapes
// 09/29/2025  08:43

using System.Globalization;
using System.Numerics;



namespace Jakar.Shapes;


public readonly struct TestShape : INumber<TestShape>
{
    public static TestShape MultiplicativeIdentity { get; }
    public static TestShape AdditiveIdentity       { get; }
    public static TestShape One                    { get; }
    public static int       Radix                  { get; }
    public static TestShape Zero                   { get; }


    public int    CompareTo( object?   obj )                                     => 0;
    public int    CompareTo( TestShape other )                                   => 0;
    public bool   Equals( TestShape    other )                                   => false;
    public string ToString( string?    format, IFormatProvider? formatProvider ) => null;
    public bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider )
    {
        charsWritten = 0;
        return false;
    }


    public static TestShape operator +( TestShape  left, TestShape right ) => default;
    public static bool operator ==( TestShape      left, TestShape right ) => false;
    public static bool operator !=( TestShape      left, TestShape right ) => false;
    public static bool operator >( TestShape       left, TestShape right ) => false;
    public static bool operator >=( TestShape      left, TestShape right ) => false;
    public static bool operator <( TestShape       left, TestShape right ) => false;
    public static bool operator <=( TestShape      left, TestShape right ) => false;
    public static TestShape operator --( TestShape value )                 => default;
    public static TestShape operator /( TestShape  left, TestShape right ) => default;
    public static TestShape operator ++( TestShape value )                 => default;
    public static TestShape operator %( TestShape  left, TestShape right ) => default;
    public static TestShape operator *( TestShape  left, TestShape right ) => default;
    public static TestShape operator -( TestShape  left, TestShape right ) => default;
    public static TestShape operator -( TestShape  value ) => default;
    public static TestShape operator +( TestShape  value ) => default;


    public static TestShape Abs( TestShape                value )                                            => default;
    public static bool      IsCanonical( TestShape        value )                                            => false;
    public static bool      IsComplexNumber( TestShape    value )                                            => false;
    public static bool      IsEvenInteger( TestShape      value )                                            => false;
    public static bool      IsFinite( TestShape           value )                                            => false;
    public static bool      IsImaginaryNumber( TestShape  value )                                            => false;
    public static bool      IsInfinity( TestShape         value )                                            => false;
    public static bool      IsInteger( TestShape          value )                                            => false;
    public static bool      IsNaN( TestShape              value )                                            => false;
    public static bool      IsNegative( TestShape         value )                                            => false;
    public static bool      IsNegativeInfinity( TestShape value )                                            => false;
    public static bool      IsNormal( TestShape           value )                                            => false;
    public static bool      IsOddInteger( TestShape       value )                                            => false;
    public static bool      IsPositive( TestShape         value )                                            => false;
    public static bool      IsPositiveInfinity( TestShape value )                                            => false;
    public static bool      IsRealNumber( TestShape       value )                                            => false;
    public static bool      IsSubnormal( TestShape        value )                                            => false;
    public static bool      IsZero( TestShape             value )                                            => false;
    public static TestShape MaxMagnitude( TestShape       x, TestShape    y )                                => default;
    public static TestShape MaxMagnitudeNumber( TestShape x, TestShape    y )                                => default;
    public static TestShape MinMagnitude( TestShape       x, TestShape    y )                                => default;
    public static TestShape MinMagnitudeNumber( TestShape x, TestShape    y )                                => default;
    public static TestShape Parse( ReadOnlySpan<char>     s, NumberStyles style, IFormatProvider? provider ) => default;
    public static TestShape Parse( string                 s, NumberStyles style, IFormatProvider? provider ) => default;
    public static bool TryParse( ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, out TestShape result )
    {
        result = default;
        return false;
    }
    public static bool TryParse( [NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, out TestShape result )
    {
        result = default;
        return false;
    }
    public static TestShape Parse( string s, IFormatProvider? provider ) => default;
    public static bool TryParse( [NotNullWhen(true)] string? s, IFormatProvider? provider, out TestShape result )
    {
        result = default;
        return false;
    }
    public static TestShape Parse( ReadOnlySpan<char> s, IFormatProvider? provider ) => default;
    public static bool TryParse( ReadOnlySpan<char> s, IFormatProvider? provider, out TestShape result )
    {
        result = default;
        return false;
    }


    public static bool TryConvertFromChecked<TOther>( TOther value, out TestShape result )
        where TOther : INumberBase<TOther>
    {
        result = default;
        return false;
    }
    public static bool TryConvertFromSaturating<TOther>( TOther value, out TestShape result )
        where TOther : INumberBase<TOther>
    {
        result = default;
        return false;
    }
    public static bool TryConvertFromTruncating<TOther>( TOther value, out TestShape result )
        where TOther : INumberBase<TOther>
    {
        result = default;
        return false;
    }
    public static bool TryConvertToChecked<TOther>( TestShape value, [MaybeNullWhen(false)] out TOther result )
        where TOther : INumberBase<TOther>
    {
        result = default;
        return false;
    }
    public static bool TryConvertToSaturating<TOther>( TestShape value, [MaybeNullWhen(false)] out TOther result )
        where TOther : INumberBase<TOther>
    {
        result = default;
        return false;
    }
    public static bool TryConvertToTruncating<TOther>( TestShape value, [MaybeNullWhen(false)] out TOther result )
        where TOther : INumberBase<TOther>
    {
        result = default;
        return false;
    }
}
