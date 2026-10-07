// Jakar.Json.Generator
// 10/07/2026

using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;



namespace Jakar.Json.Generator;


/// <summary> Naming policies, applied at compile time (the JSON names are constants in the generated code). </summary>
internal static class Naming
{
    public static string Apply( string name, int policy ) => policy switch
                                                             {
                                                                 Values.NAMING_CAMEL       => CamelCase(name),
                                                                 Values.NAMING_PASCAL      => PascalCase(name),
                                                                 Values.NAMING_SNAKE_LOWER => Separated(name, '_', false),
                                                                 Values.NAMING_SNAKE_UPPER => Separated(name, '_', true),
                                                                 Values.NAMING_KEBAB_LOWER => Separated(name, '-', false),
                                                                 Values.NAMING_KEBAB_UPPER => Separated(name, '-', true),
                                                                 _                         => name
                                                             };


    /// <summary> System.Text.Json's camel case: the leading run of capitals is lowered (<c>URLValue</c> → <c>urlValue</c>, <c>ID</c> → <c>id</c>). </summary>
    public static string CamelCase( string name )
    {
        if ( name.Length == 0 || !char.IsUpper(name[0]) ) { return name; }

        char[] chars = name.ToCharArray();

        for ( int i = 0; i < chars.Length; i++ )
        {
            if ( i == 1 && !char.IsUpper(chars[i]) ) { break; }

            bool hasNext = i + 1 < chars.Length;

            if ( i > 0 && hasNext && !char.IsUpper(chars[i + 1]) )
            {
                if ( chars[i + 1] == ' ' ) { chars[i] = char.ToLowerInvariant(chars[i]); }

                break;
            }

            chars[i] = char.ToLowerInvariant(chars[i]);
        }

        return new string(chars);
    }

    public static string PascalCase( string name ) => name.Length == 0 || char.IsUpper(name[0])
                                                          ? name
                                                          : char.ToUpperInvariant(name[0]) + name.Substring(1);

    /// <summary> Words split at case changes and non-alphanumerics: <c>HTTPServerURL2Name</c> → <c>http_server_url2_name</c>. </summary>
    public static string Separated( string name, char separator, bool upper )
    {
        List<string>  words   = Words(name);
        StringBuilder builder = new(name.Length + words.Count);

        for ( int i = 0; i < words.Count; i++ )
        {
            if ( i > 0 ) { builder.Append(separator); }

            builder.Append(upper
                               ? words[i].ToUpperInvariant()
                               : words[i].ToLowerInvariant());
        }

        return builder.ToString();
    }

    private static List<string> Words( string name )
    {
        List<string>  words   = [];
        StringBuilder current = new();

        for ( int i = 0; i < name.Length; i++ )
        {
            char c = name[i];

            if ( !char.IsLetterOrDigit(c) )
            {
                Flush();
                continue;
            }

            if ( current.Length > 0 && char.IsUpper(c) )
            {
                char previous  = name[i - 1];
                bool nextLower = i + 1 < name.Length && char.IsLower(name[i + 1]);

                // a new word after a lowercase letter or digit, or at the last capital of an acronym followed by lowercase (URLValue → URL, Value)
                if ( char.IsLower(previous) || char.IsDigit(previous) || ( char.IsUpper(previous) && nextLower ) ) { Flush(); }
            }

            current.Append(c);
        }

        Flush();
        return words;

        void Flush()
        {
            if ( current.Length == 0 ) { return; }

            words.Add(current.ToString());
            current.Clear();
        }
    }


    /// <summary> JSON string escaping of a name, as the writer does it with minimal escaping (no quotes). </summary>
    public static string EscapeJson( string text )
    {
        StringBuilder builder = new(text.Length);

        foreach ( char c in text )
        {
            switch ( c )
            {
                case '"':
                    builder.Append("\\\"");
                    break;

                case '\\':
                    builder.Append("\\\\");
                    break;

                case '\b':
                    builder.Append("\\b");
                    break;

                case '\f':
                    builder.Append("\\f");
                    break;

                case '\n':
                    builder.Append("\\n");
                    break;

                case '\r':
                    builder.Append("\\r");
                    break;

                case '\t':
                    builder.Append("\\t");
                    break;

                default:
                    if ( c < 0x20 ) { builder.Append("\\u").Append(( (int)c ).ToString("X4")); }
                    else { builder.Append(c); }

                    break;
            }
        }

        return builder.ToString();
    }

    /// <summary> Names that every escaping mode writes the same way, so they can be pre-encoded: printable ASCII except <c>" \ &lt; &gt; &amp; ' +</c>. </summary>
    public static bool CanPreEncode( string name )
    {
        foreach ( char c in name )
        {
            if ( c < 0x20 || c >= 0x7F || c is '"' or '\\' or '<' or '>' or '&' or '\'' or '+' ) { return false; }
        }

        return true;
    }

    public static bool IsAscii( string text )
    {
        foreach ( char c in text )
        {
            if ( c >= 0x80 ) { return false; }
        }

        return true;
    }

    public static int Utf8Length( string text ) => Encoding.UTF8.GetByteCount(text);

    /// <summary> A C# string literal. </summary>
    public static string Literal( string text ) => SymbolDisplay.FormatLiteral(text, true);
}
