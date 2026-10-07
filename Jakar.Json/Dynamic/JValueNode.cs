// Jakar.Json
// 10/07/2026

namespace Jakar.Json;


/// <summary>
///     An immutable JSON string, number or boolean. Numbers are either the exact text they were parsed from (so <c>1.10</c> stays <c>1.10</c>),
///     or a typed CLR value stored as bits and written with the writer's canonical formatting (§5.2). Nothing is boxed.
/// </summary>
public sealed class JValueNode : JNode
{
    private enum NumberKind : byte
    {
        None,
        Text,
        Int64,
        UInt64,
        Int128,
        UInt128,
        Single,
        Double,
        Decimal
    }



    private readonly JNodeKind  __kind;
    private readonly NumberKind __number;
    private readonly string?    __text; // String: the value; NumberKind.Text: the validated JSON number text
    private readonly Int128     __bits; // Boolean: 0/1; typed numbers: the value's bits


    public JValueNode( string value )
    {
        ArgumentNullException.ThrowIfNull(value);
        __kind = JNodeKind.String;
        __text = value;
    }

    public JValueNode( bool value )
    {
        __kind = JNodeKind.Boolean;

        __bits = value
                     ? 1
                     : 0;
    }

    // int and uint are spelled out: without them, int/byte/ushort/uint arguments are ambiguous between the long and UInt128 overloads.
    public JValueNode( int     value ) : this(NumberKind.Int64, value) { }
    public JValueNode( uint    value ) : this(NumberKind.Int64, value) { }
    public JValueNode( long    value ) : this(NumberKind.Int64, value) { }
    public JValueNode( ulong   value ) : this(NumberKind.UInt64, value) { }
    public JValueNode( Int128  value ) : this(NumberKind.Int128, value) { }
    public JValueNode( UInt128 value ) : this(NumberKind.UInt128, unchecked ((Int128)value)) { }
    public JValueNode( float   value ) : this(NumberKind.Single, BitConverter.SingleToInt32Bits(Finite(value))) { }
    public JValueNode( double  value ) : this(NumberKind.Double, BitConverter.DoubleToInt64Bits(Finite(value))) { }
    public JValueNode( decimal value ) : this(NumberKind.Decimal, Unsafe.BitCast<decimal, Int128>(value)) { }

    // §5.3 canonical strings.
    public JValueNode( Guid           value ) : this(value.ToString("D")) { } // lowercase
    public JValueNode( DateTimeOffset value ) : this(value.ToString("O",                CultureInfo.InvariantCulture)) { }
    public JValueNode( DateOnly       value ) : this(value.ToString("yyyy-MM-dd",       CultureInfo.InvariantCulture)) { }
    public JValueNode( TimeOnly       value ) : this(value.ToString("HH:mm:ss.fffffff", CultureInfo.InvariantCulture)) { }
    public JValueNode( TimeSpan       value ) : this(value.ToString("c",                CultureInfo.InvariantCulture)) { }

    /// <summary> <see cref="DateTimeKind.Local"/> values are converted to UTC first (<c>LocalDateTimes = ConvertToUtc</c>, §5.4). </summary>
    public JValueNode( DateTime value ) : this(( value.Kind == DateTimeKind.Local
                                                     ? value.ToUniversalTime()
                                                     : value ).ToString("O", CultureInfo.InvariantCulture)) { }


    private JValueNode( NumberKind number, Int128 bits )
    {
        __kind   = JNodeKind.Number;
        __number = number;
        __bits   = bits;
    }

    private JValueNode( string text, NumberKind number )
    {
        __kind   = JNodeKind.Number;
        __number = number;
        __text   = text;
    }

    private JValueNode( JValueNode other )
    {
        __kind   = other.__kind;
        __number = other.__number;
        __text   = other.__text;
        __bits   = other.__bits;
    }


    /// <summary> A number from its JSON text, kept verbatim. </summary>
    /// <exception cref="FormatException"> <paramref name="text"/> isn't exactly one JSON number. </exception>
    public static JValueNode FromNumberText( string text )
    {
        ArgumentNullException.ThrowIfNull(text);

        return JsonTape.IsValidNumber(text)
                   ? new JValueNode(text, NumberKind.Text)
                   : throw new FormatException($"'{text}' isn't a JSON number.");
    }

    internal static JValueNode FromValidatedNumberText( string text ) => new(text, NumberKind.Text);


    public override JNodeKind Kind => __kind;

    /// <summary> A number with no fraction or exponent. </summary>
    public bool IsInteger => __number is NumberKind.Int64 or NumberKind.UInt64 or NumberKind.Int128 or NumberKind.UInt128 || ( __number == NumberKind.Text && __text.AsSpan().IndexOfAny('.', 'e', 'E') < 0 );


    // ─── Strings and booleans ────────────────────────────────────────────────

    public string GetString() => __kind == JNodeKind.String
                                     ? __text!
                                     : throw Mismatch(JNodeKind.String);

    public bool GetBoolean() => __kind       == JNodeKind.Boolean
                                    ? __bits != 0
                                    : throw Mismatch(JNodeKind.Boolean);

    public Guid           GetGuid()           => Guid.ParseExact(GetString(), "D");
    public DateTime       GetDateTime()       => DateTime.ParseExact(GetString(), "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind); // never shifts to local time
    public DateTimeOffset GetDateTimeOffset() => DateTimeOffset.ParseExact(GetString(), "O", CultureInfo.InvariantCulture);


    // ─── Numbers ─────────────────────────────────────────────────────────────

    public int     GetInt32()   => GetInteger<int>();
    public long    GetInt64()   => GetInteger<long>();
    public ulong   GetUInt64()  => GetInteger<ulong>();
    public float   GetSingle()  => GetFloat<float>();
    public double  GetDouble()  => GetFloat<double>();
    public decimal GetDecimal() => GetFloat<decimal>();

    /// <exception cref="OverflowException"> The value doesn't fit <typeparamref name="T"/>. </exception>
    /// <exception cref="FormatException"> The number text has a fraction or exponent. </exception>
    /// <exception cref="InvalidOperationException"> The value isn't a number, or is a typed floating-point number. </exception>
    public T GetInteger<T>()
        where T : IBinaryInteger<T> => __number switch
                                       {
                                           NumberKind.Int64   => T.CreateChecked((long)__bits),
                                           NumberKind.UInt64  => T.CreateChecked((ulong)__bits),
                                           NumberKind.Int128  => T.CreateChecked(__bits),
                                           NumberKind.UInt128 => T.CreateChecked(unchecked ((UInt128)__bits)),
                                           NumberKind.Text    => T.Parse(__text!, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture),
                                           NumberKind.None    => throw Mismatch(JNodeKind.Number),
                                           _                  => throw new InvalidOperationException("The number isn't an integer.")
                                       };

    /// <exception cref="OverflowException"> The value doesn't fit <typeparamref name="T"/>. </exception>
    public T GetFloat<T>()
        where T : IFloatingPoint<T> => __number switch
                                       {
                                           NumberKind.Single  => T.CreateChecked(BitConverter.Int32BitsToSingle((int)__bits)),
                                           NumberKind.Double  => T.CreateChecked(BitConverter.Int64BitsToDouble((long)__bits)),
                                           NumberKind.Decimal => T.CreateChecked(Unsafe.BitCast<Int128, decimal>(__bits)),
                                           NumberKind.Int64   => T.CreateChecked((long)__bits),
                                           NumberKind.UInt64  => T.CreateChecked((ulong)__bits),
                                           NumberKind.Int128  => T.CreateChecked(__bits),
                                           NumberKind.UInt128 => T.CreateChecked(unchecked ((UInt128)__bits)),
                                           NumberKind.Text    => ParseFloat<T>(__text!),
                                           _                  => throw Mismatch(JNodeKind.Number)
                                       };

    /// <summary> Any <see cref="ISpanParsable{TSelf}"/> type, parsed from the value's text with invariant culture (e.g. <see cref="DateOnly"/>, <c>Email</c>, <see cref="BigInteger"/>). </summary>
    public T GetValue<T>()
        where T : ISpanParsable<T>
    {
        switch ( __kind )
        {
            case JNodeKind.String:
                return T.Parse(__text!, CultureInfo.InvariantCulture);

            case JNodeKind.Boolean:
                return T.Parse(__bits != 0
                                   ? "true"
                                   : "false",
                               CultureInfo.InvariantCulture);
        }

        if ( __number == NumberKind.Text ) { return T.Parse(__text!, CultureInfo.InvariantCulture); }

        Span<char> buffer = stackalloc char[64];
        return T.Parse(buffer[..FormatNumber(buffer)], CultureInfo.InvariantCulture);
    }


    /// <summary> Exception-free <see cref="GetInteger{T}"/> for readers. </summary>
    internal bool TryGetInteger<T>( out T value, out JsonErrorKind error )
        where T : struct, IBinaryInteger<T>
    {
        value = default;
        error = JsonErrorKind.None;

        switch ( __number )
        {
            case NumberKind.Text:
                if ( T.TryParse(__text, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value) ) { return true; }

                error = JsonLexer<char>.HasFractionOrExponent(__text)
                            ? JsonErrorKind.InvalidNumber
                            : JsonErrorKind.NumberOverflow;

                return false;

            case NumberKind.Int64 or NumberKind.UInt64 or NumberKind.Int128 or NumberKind.UInt128:
                bool fits = __number switch
                            {
                                NumberKind.Int64  => TryNarrow((long)__bits,                out value),
                                NumberKind.UInt64 => TryNarrow((ulong)__bits,               out value),
                                NumberKind.Int128 => TryNarrow(__bits,                      out value),
                                _                 => TryNarrow(unchecked ((UInt128)__bits), out value)
                            };

                if ( fits ) { return true; }

                error = JsonErrorKind.NumberOverflow;
                return false;

            default:
                error = __kind == JNodeKind.Number
                            ? JsonErrorKind.InvalidNumber
                            : JsonErrorKind.UnexpectedToken;

                return false;
        }
    }

    /// <summary> Saturate, then check the value survives the round trip: it fits exactly when it does. </summary>
    private static bool TryNarrow<TFrom, T>( TFrom source, out T value )
        where TFrom : IBinaryInteger<TFrom>
        where T : IBinaryInteger<T>
    {
        value = T.CreateSaturating(source);
        return TFrom.CreateSaturating(value) == source;
    }

    /// <summary> Exception-free <see cref="GetFloat{T}"/> for readers. </summary>
    internal bool TryGetFloat<T>( out T value, out JsonErrorKind error )
        where T : struct, IFloatingPoint<T>
    {
        value = default;
        error = JsonErrorKind.None;

        if ( __number == NumberKind.Text )
        {
            if ( T.TryParse(__text, NumberStyles.Float, CultureInfo.InvariantCulture, out value) && !T.IsInfinity(value) ) { return true; }

            error = JsonErrorKind.NumberOverflow;
            return false;
        }

        if ( __number == NumberKind.None )
        {
            error = JsonErrorKind.UnexpectedToken;
            return false;
        }

        Span<char> buffer = stackalloc char[64];
        int        length = FormatNumber(buffer);
        if ( T.TryParse(buffer[..length], NumberStyles.Float, CultureInfo.InvariantCulture, out value) && !T.IsInfinity(value) ) { return true; }

        error = JsonErrorKind.NumberOverflow;
        return false;
    }

    /// <summary> The number's text: verbatim when parsed, invariant round-trippable otherwise. </summary>
    internal void AppendNumberText( ref ValueStringBuilder builder )
    {
        if ( __number == NumberKind.Text )
        {
            builder.Append(__text);
            return;
        }

        Span<char> buffer = stackalloc char[64];
        builder.Append(buffer[..FormatNumber(buffer)]);
    }


    private static T ParseFloat<T>( string text )
        where T : IFloatingPoint<T>
    {
        T value = T.Parse(text, NumberStyles.Float, CultureInfo.InvariantCulture);

        return T.IsInfinity(value)
                   ? throw new OverflowException($"{text} overflows the floating-point type.")
                   : value;
    }

    private static T Finite<T>( T value )
        where T : IFloatingPointIeee754<T> => T.IsFinite(value)
                                                  ? value
                                                  : throw new ArgumentOutOfRangeException(nameof(value), value, "JSON has no NaN or Infinity; store a string instead.");

    /// <summary> Invariant text of a typed number (round-trippable for floats). 64 chars fit every typed number. </summary>
    private int FormatNumber( Span<char> destination )
    {
        int written = 0;

        bool formatted = __number switch
                         {
                             NumberKind.Int64   => ( (long)__bits ).TryFormat(destination, out written, default, CultureInfo.InvariantCulture),
                             NumberKind.UInt64  => ( (ulong)__bits ).TryFormat(destination, out written, default, CultureInfo.InvariantCulture),
                             NumberKind.Int128  => __bits.TryFormat(destination, out written, default, CultureInfo.InvariantCulture),
                             NumberKind.UInt128 => unchecked ((UInt128)__bits).TryFormat(destination, out written, default, CultureInfo.InvariantCulture),
                             NumberKind.Single  => BitConverter.Int32BitsToSingle((int)__bits).TryFormat(destination, out written, "R", CultureInfo.InvariantCulture),
                             NumberKind.Double  => BitConverter.Int64BitsToDouble((long)__bits).TryFormat(destination, out written, "R", CultureInfo.InvariantCulture),
                             NumberKind.Decimal => Unsafe.BitCast<Int128, decimal>(__bits).TryFormat(destination, out written, default, CultureInfo.InvariantCulture),
                             _                  => throw Mismatch(JNodeKind.Number)
                         };

        return formatted
                   ? written
                   : throw new UnreachableException();
    }

    private ReadOnlySpan<char> NumberText( Span<char> buffer ) => __number == NumberKind.Text
                                                                      ? __text.AsSpan()
                                                                      : buffer[..FormatNumber(buffer)];

    private InvalidOperationException Mismatch( JNodeKind expected ) => new($"The value is a JSON {__kind}, not a {expected}.");


    // ─── JNode ───────────────────────────────────────────────────────────────

    public override void WriteTo<TWriter>( ref TWriter writer )
    {
        switch ( __kind )
        {
            case JNodeKind.String:
                writer.WriteString(__text!);
                return;

            case JNodeKind.Boolean:
                writer.WriteBoolean(__bits != 0);
                return;
        }

        switch ( __number )
        {
            case NumberKind.Text:
                writer.WriteRawNumber(__text!);
                break; // validated when the node was created

            case NumberKind.Int64:
                writer.WriteInteger((long)__bits);
                break;

            case NumberKind.UInt64:
                writer.WriteInteger((ulong)__bits);
                break;

            case NumberKind.Int128:
                writer.WriteInteger(__bits);
                break;

            case NumberKind.UInt128:
                writer.WriteInteger(unchecked ((UInt128)__bits));
                break;

            case NumberKind.Single:
                writer.WriteFloat(BitConverter.Int32BitsToSingle((int)__bits));
                break;

            case NumberKind.Double:
                writer.WriteFloat(BitConverter.Int64BitsToDouble((long)__bits));
                break;

            case NumberKind.Decimal:
                writer.WriteFloat(Unsafe.BitCast<Int128, decimal>(__bits));
                break;
        }
    }

    public override JValueNode DeepClone() => new(this);

    private protected override bool DeepEqualsCore( JNode other )
    {
        JValueNode value = (JValueNode)other;

        return __kind switch
               {
                   JNodeKind.String  => string.Equals(__text, value.__text, StringComparison.Ordinal),
                   JNodeKind.Boolean => __bits == value.__bits,
                   _                 => NumberEquals(value)
               };
    }

    /// <summary>
    ///     Equal by value across representations. Doubles reject most mismatches cheaply; decimals then separate values that doubles can't
    ///     (<c>0.1</c> vs <c>0.10000000000000001</c>, 2^63 vs 2^63 + 1). Integers beyond decimal's range compare by their invariant text.
    /// </summary>
    private bool NumberEquals( JValueNode other )
    {
        if ( __number == other.__number && __number is NumberKind.Int64 or NumberKind.UInt64 or NumberKind.Int128 or NumberKind.UInt128 ) { return __bits == other.__bits; }

        if ( TryGetDouble(out double left) && other.TryGetDouble(out double right) && left != right ) { return false; }

        if ( TryGetDecimal(out decimal a) && other.TryGetDecimal(out decimal b) ) { return a == b; }

        Span<char> leftText = stackalloc char[64], rightText = stackalloc char[64];
        return NumberText(leftText).SequenceEqual(other.NumberText(rightText));
    }

    private bool TryGetDouble( out double value )
    {
        if ( __number == NumberKind.Text ) { return double.TryParse(__text, NumberStyles.Float, CultureInfo.InvariantCulture, out value) && double.IsFinite(value); }

        value = GetFloat<double>();
        return true;
    }

    private bool TryGetDecimal( out decimal value )
    {
        switch ( __number )
        {
            case NumberKind.Text:
                return decimal.TryParse(__text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);

            case NumberKind.Decimal:
                value = Unsafe.BitCast<Int128, decimal>(__bits);
                return true;

            case NumberKind.Int64:
                value = (long)__bits;
                return true;

            case NumberKind.UInt64:
                value = (ulong)__bits;
                return true;

            case NumberKind.Int128 when __bits >= (Int128)decimal.MinValue && __bits <= (Int128)decimal.MaxValue:
                value = (decimal)__bits;
                return true;

            case NumberKind.UInt128 when unchecked ((UInt128)__bits) <= (UInt128)decimal.MaxValue:
                value = (decimal)unchecked ((UInt128)__bits);
                return true;

            case NumberKind.Single or NumberKind.Double:
            {
                double number = GetFloat<double>();

                if ( Math.Abs(number) <= (double)decimal.MaxValue )
                {
                    value = (decimal)number;
                    return true;
                }

                break;
            }
        }

        value = 0;
        return false;
    }

    private protected override void AppendSegment( JNode child, ref ValueStringBuilder builder ) { } // values have no children

    private protected override void RemoveChild( JNode child ) => throw new UnreachableException("A value has no children.");
}
