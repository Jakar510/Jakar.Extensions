// Jakar.Extensions :: Jakar.Extensions
// 04/18/2022  10:02 AM


#nullable enable
using System.Globalization;
using System.Runtime.InteropServices;



namespace Jakar.Mapper;


#if NET6_0



public readonly ref struct MConfig
{
    public const    int         MINIMUM_ARRAY_POOL_LENGTH = 256;
    public readonly char        positiveOffsetChar        = '+';
    public readonly char        negativeOffsetChar        = '-';
    public readonly char        formatDelimiter           = '|';
    public readonly char        defaultValueDelimiter     = ':';
    public readonly char        openOffsetDelimiter       = '(';
    public readonly char        closeOffsetDelimiter      = ')';
    public readonly char        startTermDelimiter        = '[';
    public readonly char        endTermDelimiter          = ']';
    public readonly CultureInfo culture;


    public static MConfig Default => new();
    public MConfig() : this(CultureInfo.CurrentCulture) { }
    public MConfig( in CultureInfo culture ) => this.culture = culture;
    public MConfig( in CultureInfo culture,
                    in char        positiveOffsetChar,
                    in char        negativeOffsetChar,
                    in char        formatDelimiter,
                    in char        defaultValueDelimiter,
                    in char        openOffsetDelimiter,
                    in char        closeOffsetDelimiter,
                    in char        startTermDelimiter,
                    in char        endTermDelimiter
    ) : this(culture)
    {
        this.positiveOffsetChar    = positiveOffsetChar;
        this.negativeOffsetChar    = negativeOffsetChar;
        this.formatDelimiter       = formatDelimiter;
        this.defaultValueDelimiter = defaultValueDelimiter;
        this.openOffsetDelimiter   = openOffsetDelimiter;
        this.closeOffsetDelimiter  = closeOffsetDelimiter;
        this.startTermDelimiter    = startTermDelimiter;
        this.endTermDelimiter      = endTermDelimiter;
    }


    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ReadOnlySpan<char> RemovedOffsets()
    {
        Span<char> buffer = stackalloc char[2];
        buffer[0] = positiveOffsetChar;
        buffer[1] = negativeOffsetChar;
        return MemoryMarshal.CreateReadOnlySpan(ref buffer.GetPinnableReference(), 2);
    }
}



#endif
