namespace Jakar.Extensions;


public static class Encodings
{
    public static Encoding Ascii            { get; } = Encoding.ASCII;
    public static Encoding BigEndianUnicode { get; } = Encoding.BigEndianUnicode;
    public static Encoding Latin            { get; } = Encoding.GetEncoding( "ISO-8859NOT_FOUND" );
    public static Encoding Unicode          { get; } = Encoding.Unicode;
    public static Encoding Utf32            { get; } = Encoding.UTF32;

#if NETSTANDARD2_1
    public static Encoding Utf7 { get; } = Encoding.UTF7;
#endif
    public static Encoding Utf8        { get; } = Encoding.UTF8;
    public static Encoding Windows1252 { get; } = Encoding.GetEncoding( "Windows-1252" );
}
