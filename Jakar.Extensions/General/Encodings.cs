#nullable enable
namespace Jakar.Extensions;


public static class Encodings
{
    public static Encoding Latin            { get; } = Encoding.GetEncoding("ISO-8859-1");
    public static Encoding Windows1252      { get; } = Encoding.GetEncoding("Windows-1252");
    public static Encoding Ascii            { get; } = Encoding.ASCII;
    public static Encoding Unicode          { get; } = Encoding.Unicode;
    public static Encoding BigEndianUnicode { get; } = Encoding.BigEndianUnicode;
    public static Encoding Utf32            { get; } = Encoding.UTF32;
    public static Encoding Utf8             { get; } = Encoding.UTF8;
#if !NETSTANDARD2_1
    [Obsolete]
#endif
    public static Encoding Utf7 { get; } = Encoding.UTF7;
}
