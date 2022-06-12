﻿// TrueLogic :: Experiments
// 04/14/2022  2:47 PM


#nullable enable
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.Toolkit.HighPerformance.Buffers;
using Microsoft.Toolkit.HighPerformance.Enumerables;



namespace Jakar.Mapper;


#if NET6_0



public static class StringMapper
{
    public static string? MapParse<T>( [NotNullIfNotNull("pattern")] this string? pattern, in T context ) where T : notnull => StringMapper<T>.Parse(pattern,                    context);
    public static string? MapParse<T>( [NotNullIfNotNull("pattern")] this string? pattern, in T context, in MConfig config ) where T : notnull => StringMapper<T>.Parse(pattern, context, config);
}



/// <summary>
/// <para><see href="https://www.meziantou.net/split-a-string-into-lines-without-allocation.htm"/></para>
/// <para><see href="https://elegantcode.com/2010/07/30/createdelegatet-an-exercise-in-using-expressions/"/></para>
/// <para><see href="https://nickssoftwareblog.com/2013/04/28/expression-based-property-getters-and-setters/"/></para>
/// <para><see href="https://stackoverflow.com/questions/25458/how-costly-is-net-reflection"/></para>
/// <para>Below is a exhaustive list of supported Patterns</para>
/// <list type="bullet">
/// <listheader>
/// <term>Pattern</term>
/// <description> The Pattern's meaning</description>
/// </listheader>
/// 
/// <item>
/// <term>[Key]</term>
/// <description>simple key</description>
/// </item>
/// 
/// <item>
/// <term>[Key|Format]</term>
/// <description>Display the value in in Format</description>
/// </item>
/// 
/// <item>
/// <term>[Key:defaultValue]</term>
/// <description>Display the value (or the defaultValue if value is null)</description>
/// </item>
/// 
/// <item>
/// <term>[Key:DefaultValue(-)]</term>
/// <description>Display the value (or the defaultValue if value is null) while decreasing the value by 1 unit</description>
/// </item>
/// 
/// <item>
/// <term>[Key:DefaultValue(+)]</term>
/// <description>Display the value (or the defaultValue if value is null) while increasing the value by 1 unit</description>
/// </item>
/// 
/// <item>
/// <term>[Key:DefaultValue(-number)]</term>
/// <description>Display the value (or the defaultValue if value is null) while decreasing the value by n units</description>
/// </item>
/// 
/// <item>
/// <term>[Key:DefaultValue(+number)]</term>
/// <description>Display the value (or the defaultValue if value is null) while increasing the value by n units</description>
/// </item>
/// 
/// <item>
/// <term>[Key|Format:DefaultValue(-)]</term>
/// <description>Display, in in Format, the value (or the defaultValue if value is null) while decreasing the value by 1 unit</description>
/// </item>
/// 
/// <item>
/// <term>[Key|Format:DefaultValue(+)]</term>
/// <description>Display, in in Format, the value (or the defaultValue if value is null) while increasing the value by 1 unit</description>
/// </item>
/// 
/// <item>
/// <term>[Key|Format:DefaultValue(-number)]</term>
/// <description>Display, in in Format, the value (or the defaultValue if value is null) while increasing the value by n units</description>
/// </item>
/// 
/// <item>
/// <term>[Key|Format:DefaultValue(+number)]</term>
/// <description>Display, in in Format, the value (or the defaultValue if value is null) while increasing the value by n units</description>
/// </item>
/// 
/// <item>
/// <term>[Key(-)]</term>
/// <description>Subtract 1 units to the value then Display it</description>
/// </item>
/// 
/// <item>
/// <term>[Key(+)]</term>
/// <description>Add 1 units to the value then Display it</description>
/// </item>
/// 
/// <item>
/// <term>[Key(-number)]</term>
/// <description>Subtract n units to the value then Display it</description>
/// </item>
/// 
/// <item>
/// <term>[Key(+number)]</term>
/// <description>Add n units to the value then Display it</description>
/// </item>
/// 
/// </list>
/// </summary>
/// <exception cref="ArgumentException"></exception>
/// <exception cref="ArgumentOutOfRangeException"></exception>
/// <exception cref="OutOfRangeException"></exception>
/// <exception cref="FormatException"></exception>
/// <exception cref="InvalidOperationException"></exception>
public ref struct StringMapper<T> where T : notnull
{
    // ReSharper disable once NotAccessedField.Local
    private readonly MContext<T>        _context;
    private readonly MConfig            _config;
    private          Span<char>         _buffer;
    private          ReadOnlySpan<char> _result; // the result buffer

    // private readonly        ValueStringBuilder _builder; // TODO: implement this
    // private static readonly StringPool         _stringPool = new();


    private StringMapper( in ReadOnlySpan<char> span, in MContext<T> context, in MConfig config )
    {
        Span<char> buffer = stackalloc char[span.Length + 2];
        buffer[0] = ' ';
        for ( var i = 0; i < span.Length; i++ ) { buffer[i + 1] = span[i]; }

        buffer[^1] = ' ';
        ReadOnlySpan<char> originalString = MemoryMarshal.CreateReadOnlySpan(ref buffer.GetPinnableReference(), buffer.Length);
        _result  = originalString;
        _context = context;
        _config  = config;
        _buffer  = span.AsSpan();

        // _builder = new ValueStringBuilder(Math.Max(originalString.Length * 5, 64));
    }


    public static string? Parse( in ReadOnlySpan<char> input, in T context ) => Parse(input, context, MConfig.Default);
    public static string? Parse( in ReadOnlySpan<char> input, in T context, in MConfig culture )
    {
        if ( context is null ) { throw new ArgumentNullException(nameof(context)); }

        if ( input.IsEmpty ) { return default; }

        return Parse(input, MContext.GetContext(context), culture);
    }
    private static string? Parse( in ReadOnlySpan<char> input, in MContext<T> context, in MConfig culture )
    {
        if ( input.IsEmpty ) { return default; }

        var mapper = new StringMapper<T>(input, context, culture);
        return mapper.ToString();
    }


    public override string ToString()
    {
        if ( _buffer.IsNullOrWhiteSpace() ) { return string.Empty; }

        ReadOnlySpan<char> span = _buffer;

        // if ( !span.IsBalanced() ) { throw new FormatException($@"String is not balanced! ""{span}"""); }


        while ( !span.IsEmpty && span.Contains(_config.startTermDelimiter) && span.Contains(_config.endTermDelimiter) )
        {
            // _buffer.Print();

            int end   = span.IndexOf(_config.endTermDelimiter);
            int start = span.LastIndexOf(_config.startTermDelimiter, end);

            if ( start < 0 ) { throw new FormatException($"Matching '{_config.startTermDelimiter}' not found for value: '{span}'"); }


            int                length   = end - start;
            ReadOnlySpan<char> original = span.Slice(start,     length + 1);
            ReadOnlySpan<char> term     = span.Slice(start + 1, length - 1);
            ReadOnlySpan<char> result   = Convert(term);
            _result = _result.Replace(original, result, _config.startTermDelimiter, _config.endTermDelimiter);

            // ReadOnlySpan<char> first = span[..start];
            // ReadOnlySpan<char> last  = span[( end + 1 )..];
            // Spans.Join(first, last, '\0', ref _buffer, out _);

            span[( end + 1 )..].CopyTo(ref _buffer, '\0');
            span = _buffer;
        }

        return _result.ToString();
    }



    private enum MapperOptions
    {
        Greater = 1,
        Less    = 2
    }



    private ReadOnlySpan<char> Convert( in ReadOnlySpan<char> value )
    {
        if ( value.Contains(_config.startTermDelimiter) || value.Contains(_config.endTermDelimiter) ) { throw new FormatException($"'{value}' cannot contain either {_config.startTermDelimiter} or {_config.endTermDelimiter}"); }


        ParseTerm(value, out ReadOnlySpan<char> key, out ReadOnlySpan<char> format, out ReadOnlySpan<char> defaultValue, out double offset, out MapperOptions? options);


        if ( offset is 0 ) { throw new OutOfRangeException($"'{value}' failed to parse the {nameof(offset)}"); }


        if ( !_context.TryGetValue(key, out object? x) ) { throw new KeyNotFoundException(key.ToString()); }

        ReadOnlySpan<char> result = x switch
                                    {
                                        null                => defaultValue,
                                        string item         => Convert(item,                                       defaultValue),
                                        Uri item            => Convert(item,                                       defaultValue, format),
                                        bool item           => Convert(item,                                       defaultValue, format),
                                        Guid item           => Convert(item,                                       defaultValue, format),
                                        JToken item         => Convert(item,                                       defaultValue, format, offset, options),
                                        int item            => Convert(HandleOptions(item, (int)offset,  options), defaultValue, format),
                                        long item           => Convert(HandleOptions(item, (long)offset, options), defaultValue, format),
                                        float item          => Convert(HandleOptions(item, offset,       options), defaultValue, format),
                                        double item         => Convert(HandleOptions(item, offset,       options), defaultValue, format),
                                        DateTime item       => Convert(HandleOptions(item, offset,       options), defaultValue, format),
                                        DateTimeOffset item => Convert(HandleOptions(item, offset,       options), defaultValue, format),
                                        TimeSpan item       => Convert(HandleOptions(item, offset,       options), defaultValue, format),
                                        _                   => throw new OutOfRangeException(nameof(x), x),
                                    };

        return result;
    }
    private ReadOnlySpan<char> Convert<TValue>( in TValue? value, in ReadOnlySpan<char> defaultValue, in ReadOnlySpan<char> format )
    {
        ReadOnlySpan<char> result = value switch
                                    {
                                        null                  => defaultValue,
                                        ISpanFormattable span => Convert(span, defaultValue, format),
                                        IFormattable span     => Convert(span, format),
                                        IConvertible span     => Convert(span),
                                        _                     => Convert(value, defaultValue)
                                    };

        return result;
    }


    private ReadOnlySpan<char> Convert( in ISpanFormattable value, in ReadOnlySpan<char> defaultValue, in ReadOnlySpan<char> format )
    {
        Span<char> buffer = stackalloc char[1000];
        if ( !value.TryFormat(buffer, out int charsWritten, format, _config.culture) ) { throw new InvalidOperationException($"{value} format failed"); }

        if ( buffer.IsEmpty ) { return defaultValue; }

        return MemoryMarshal.CreateReadOnlySpan(ref MemoryMarshal.GetReference(buffer), charsWritten);
    }
    private ReadOnlySpan<char> Convert( in IFormattable value, in ReadOnlySpan<char> format ) => value.ToString(format.ToString(), _config.culture);
    private ReadOnlySpan<char> Convert( in IConvertible value ) => value.ToString(_config.culture);


    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="key">The <see cref="PropertyInfo.Name"/></param>
    /// <param name="format"></param>
    /// <param name="defaultValue"></param>
    /// <param name="offset"></param>
    /// <param name="options"></param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="FormatException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    private void ParseTerm( in ReadOnlySpan<char> value, out ReadOnlySpan<char> key, out ReadOnlySpan<char> format, out ReadOnlySpan<char> defaultValue, out double offset, out MapperOptions? options )
    {
        ReadOnlySpan<char> span = value;

        if ( span.Contains(_config.formatDelimiter) && span.Contains(_config.defaultValueDelimiter) ) // has both format and defaultValue
        {
            int pipeIndex  = span.IndexOf(_config.formatDelimiter);
            int colonIndex = span.IndexOf(_config.defaultValueDelimiter);
            key    = span[..pipeIndex];
            format = span.Slice(pipeIndex + 1, colonIndex - pipeIndex - 1);
            span   = span[( colonIndex + 1 )..];

            if ( span.Contains(_config.openOffsetDelimiter) )
            {
                int start = span.IndexOf(_config.openOffsetDelimiter);
                defaultValue = span[..start];
                span         = span.Slice(_config.openOffsetDelimiter, _config.closeOffsetDelimiter, false);
                ParseOffsetOptions(span, out offset, out options);
            }
            else
            {
                defaultValue = span;
                offset       = 1;
                options      = default;
            }
        }

        else if ( span.Contains(_config.formatDelimiter) ) // has only format
        {
            int pipeIndex = span.IndexOf(_config.formatDelimiter);
            key          = span[..pipeIndex];
            span         = span[( pipeIndex + 1 )..];
            defaultValue = default;

            if ( span.Contains(_config.openOffsetDelimiter) )
            {
                int start = span.IndexOf(_config.openOffsetDelimiter);
                format = span[..start];
                span   = span.Slice(_config.openOffsetDelimiter, _config.closeOffsetDelimiter, false);
                ParseOffsetOptions(span, out offset, out options);
            }
            else
            {
                format  = span;
                offset  = 1;
                options = default;
            }
        }

        else if ( span.Contains(_config.defaultValueDelimiter) ) // has only defaultValue
        {
            int colonIndex = span.IndexOf(_config.defaultValueDelimiter);
            key    = span[..colonIndex];
            span   = span[( colonIndex + 1 )..];
            format = default;

            if ( span.Contains(_config.openOffsetDelimiter) )
            {
                int start = span.IndexOf(_config.openOffsetDelimiter);
                defaultValue = span[..start];
                span         = span.Slice(_config.openOffsetDelimiter, _config.closeOffsetDelimiter, false);
                ParseOffsetOptions(span, out offset, out options);
            }
            else
            {
                defaultValue = span;
                offset       = 1;
                options      = default;
            }
        }

        else if ( span.Contains(_config.openOffsetDelimiter) ) // has only offset info
        {
            int colonIndex = span.IndexOf(_config.openOffsetDelimiter);
            key          = span[..colonIndex];
            format       = default;
            defaultValue = default;
            span         = span.Slice(_config.openOffsetDelimiter, _config.closeOffsetDelimiter, false);
            ParseOffsetOptions(span, out offset, out options);
        }

        else // Has nothing; it's just the property name
        {
            key          = value;
            format       = default;
            defaultValue = default;
            offset       = 1;
            options      = default;
        }
    }
    private void ParseOffsetOptions( in ReadOnlySpan<char> value, out double offset, out MapperOptions? options )
    {
        if ( value.Contains(_config.openOffsetDelimiter) || value.Contains(_config.closeOffsetDelimiter) ) { throw new FormatException($"'{value}' cannot contain either '{_config.openOffsetDelimiter}' or '{_config.closeOffsetDelimiter}'"); }


        bool minus = value.Contains(_config.negativeOffsetChar);

        switch ( value.Contains(_config.positiveOffsetChar) )
        {
            case false when !minus: { throw new FormatException($"'{value}' must contain either {_config.negativeOffsetChar} or {_config.positiveOffsetChar}"); }

            case true:
            {
                options = MapperOptions.Greater;
                break;
            }

            default:
            {
                if ( minus ) { options = MapperOptions.Less; }
                else { throw new InvalidOperationException("this should not be happening...?"); }

                break;
            }
        }


        if ( value.Length == 1 )
        {
            offset = 1;
            return;
        }

        Span<char> buffer = stackalloc char[value.Length * 2];
        Spans.RemoveAll(value, ref buffer, out int charWritten, _config.RemovedOffsets());
        offset = Spans.As(buffer[..charWritten], 1d);

        // bool end = value.Contains(_config.closeOffsetDelimiter);
        //
        // switch ( value.Contains(_config.openOffsetDelimiter) )
        // {
        //     case true when !end:
        //     case false when end:
        //     {
        //         throw new FormatException($"'{value}' cannot contain either '{_config.openOffsetDelimiter}' or '{_config.closeOffsetDelimiter}'");
        //     }
        //
        //     case true when end:
        //     {
        //         return;
        //     }
        //
        //     default:
        //     {
        //         offset  = default;
        //         options = default;
        //         return;
        //     }
        // }
    }


    private static ReadOnlySpan<char> Convert( in string? value, in ReadOnlySpan<char> defaultValue ) => string.IsNullOrWhiteSpace(value)
                                                                                                             ? defaultValue
                                                                                                             : value;
    private static ReadOnlySpan<char> Convert( in object? value, in ReadOnlySpan<char> defaultValue ) => value?.ToString() ?? defaultValue;


    private static int HandleOptions( in int x, in int offset, in MapperOptions? options )
    {
        int result = options switch
                     {
                         null                  => x,
                         MapperOptions.Greater => x + Math.Abs(offset),
                         MapperOptions.Less    => x - Math.Abs(offset),
                         _                     => throw new OutOfRangeException(nameof(options), options)
                     };

        return result;
    }
    private static long HandleOptions( in long x, in long offset, in MapperOptions? options )
    {
        long result = options switch
                      {
                          null                  => x,
                          MapperOptions.Greater => x + Math.Abs(offset),
                          MapperOptions.Less    => x - Math.Abs(offset),
                          _                     => throw new OutOfRangeException(nameof(options), options)
                      };

        return result;
    }
    private static double HandleOptions( in double x, in double offset, in MapperOptions? options )
    {
        double result = options switch
                        {
                            null                  => x,
                            MapperOptions.Greater => x + Math.Abs(offset),
                            MapperOptions.Less    => x - Math.Abs(offset),
                            _                     => throw new OutOfRangeException(nameof(options), options)
                        };

        return result;
    }
    private static DateTime HandleOptions( in DateTime x, in double offset, in MapperOptions? options )
    {
        TimeSpan temp = TimeSpan.FromSeconds(offset);

        DateTime result = options switch
                          {
                              null                  => x,
                              MapperOptions.Greater => x.Add(temp),
                              MapperOptions.Less    => x.Subtract(temp),
                              _                     => throw new OutOfRangeException(nameof(options), options)
                          };

        return result;
    }
    private static DateTimeOffset HandleOptions( in DateTimeOffset x, in double offset, in MapperOptions? options )
    {
        TimeSpan temp = TimeSpan.FromSeconds(offset);

        DateTimeOffset result = options switch
                                {
                                    null                  => x,
                                    MapperOptions.Greater => x.Add(temp),
                                    MapperOptions.Less    => x.Subtract(temp),
                                    _                     => throw new OutOfRangeException(nameof(options), options)
                                };

        return result;
    }
    private static TimeSpan HandleOptions( in TimeSpan x, in double offset, in MapperOptions? options )
    {
        TimeSpan temp = TimeSpan.FromSeconds(offset);

        TimeSpan result = options switch
                          {
                              null                  => x,
                              MapperOptions.Greater => x.Add(temp),
                              MapperOptions.Less    => x.Subtract(temp),
                              _                     => throw new OutOfRangeException(nameof(options), options)
                          };

        return result;
    }


    private ReadOnlySpan<char> Convert( in JToken? item, in ReadOnlySpan<char> defaultValue, in ReadOnlySpan<char> format, in double offset, in MapperOptions? options )
    {
        ReadOnlySpan<char> result = item?.Type switch
                                    {
                                        null                   => defaultValue,
                                        JTokenType.Null        => defaultValue,
                                        JTokenType.Bytes       => throw new NotImplementedException(),
                                        JTokenType.None        => throw new NotImplementedException(),
                                        JTokenType.Undefined   => throw new NotImplementedException(),
                                        JTokenType.Constructor => throw new NotImplementedException(),
                                        JTokenType.Property    => throw new NotImplementedException(),
                                        JTokenType.Comment     => throw new NotImplementedException(),
                                        JTokenType.Raw         => throw new NotImplementedException(),
                                        JTokenType.Object      => throw new NotImplementedException(),
                                        JTokenType.Array       => throw new NotImplementedException(),
                                        JTokenType.Integer     => Convert(GetNumber(item, (long)offset, options), defaultValue, format),
                                        JTokenType.Float       => Convert(GetNumber(item, offset,       options), defaultValue, format),
                                        JTokenType.Uri         => Convert(GetUri(item),                           defaultValue, format),
                                        JTokenType.String      => Convert(GetString(item),                        defaultValue),
                                        JTokenType.Boolean     => Convert(GetBool(item),                          defaultValue, format),
                                        JTokenType.Date        => Convert(GetDateTime(item, offset, options),     defaultValue, format),
                                        JTokenType.Guid        => Convert(GetGuid(item),                          defaultValue, format),
                                        JTokenType.TimeSpan    => Convert(GetTimeSpan(item, offset, options),     defaultValue, format),
                                        _                      => throw new OutOfRangeException(nameof(item.Type), item.Type),
                                    };

        return result;
    }
    private static string? GetString( in JToken token ) => token.ToObject<string>();
    private static Uri? GetUri( in       JToken token ) => token.ToObject<Uri>();
    private static bool GetBool( in      JToken token ) => token.ToObject<bool>();
    private static Guid GetGuid( in      JToken token ) => token.ToObject<Guid>();
    private static long GetNumber( in JToken token, in long offset, in MapperOptions? options )
    {
        var  n      = token.ToObject<long>();
        long result = HandleOptions(n, offset, options);

        return result;
    }
    private static double GetNumber( in JToken token, in double offset, in MapperOptions? options )
    {
        var    n      = token.ToObject<double>();
        double result = HandleOptions(n, offset, options);

        return result;
    }
    private static DateTime GetDateTime( in JToken token, in double offset, in MapperOptions? options )
    {
        var      n      = token.ToObject<DateTime>();
        DateTime result = HandleOptions(n, offset, options);

        return result;
    }
    private static DateTimeOffset GetDateTimeOffset( in JToken token, in double offset, in MapperOptions? options )
    {
        var            n      = token.ToObject<DateTimeOffset>();
        DateTimeOffset result = HandleOptions(n, offset, options);

        return result;
    }
    private static TimeSpan GetTimeSpan( in JToken token, in double offset, in MapperOptions? options )
    {
        var      n      = token.ToObject<TimeSpan>();
        TimeSpan result = HandleOptions(n, offset, options);

        return result;
    }
}



#endif