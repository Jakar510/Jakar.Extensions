// Jakar.Extensions :: Jakar.Xml
// 04/26/2022  9:56 AM

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;


#nullable enable
namespace Jakar.Xml.Serialization;


[SuppressMessage("ReSharper", "PossiblyImpureMethodCallOnReadonlyVariable")]
public ref struct XArray
{
    private readonly ReadOnlySpan<char> _name;
    private          XWriter            _writer;


    public XArray( in ReadOnlySpan<char> name, ref XWriter context )
    {
        _name   = name;
        _writer = context;
    }


    public XArray Init()
    {
        _writer.StartBlock(_name);
        return this;
    }
    public XArray Init( in XAttributeBuilder builder )
    {
        _writer.StartBlock(_name, builder);
        return this;
    }


    public XArray Null( in ReadOnlySpan<char> key )
    {
        _writer.Indent(key).Append(XWriter.NULL).Next(key);
        return this;
    }
    public XArray Add( in ReadOnlySpan<char> key, in char value )
    {
        _writer.Indent(key).Append(value).Next(key);
        return this;
    }
    public XArray Add( in ReadOnlySpan<char> key, in short value )
    {
        _writer.Indent(key).Append(value).Next(key);
        return this;
    }
    public XArray Add( in ReadOnlySpan<char> key, in ushort value )
    {
        _writer.Indent(key).Append(value).Next(key);
        return this;
    }
    public XArray Add( in ReadOnlySpan<char> key, in int value )
    {
        _writer.Indent(key).Append(value).Next(key);
        return this;
    }
    public XArray Add( in ReadOnlySpan<char> key, in uint value )
    {
        _writer.Indent(key).Append(value).Next(key);
        return this;
    }
    public XArray Add( in ReadOnlySpan<char> key, in long value )
    {
        _writer.Indent(key).Append(value).Next(key);
        return this;
    }
    public XArray Add( in ReadOnlySpan<char> key, in ulong value )
    {
        _writer.Indent(key).Append(value).Next(key);
        return this;
    }
    public XArray Add( in ReadOnlySpan<char> key, in float value )
    {
        _writer.Indent(key).Append(value).Next(key);
        return this;
    }
    public XArray Add( in ReadOnlySpan<char> key, in double value )
    {
        _writer.Indent(key).Append(value).Next(key);
        return this;
    }
    public XArray Add( in ReadOnlySpan<char> key, in decimal value )
    {
        _writer.Indent(key).Append(value).Next(key);
        return this;
    }


    public XArray Add( in ReadOnlySpan<char> key, in char? value ) => value.HasValue
                                                                          ? Add(key, value.Value)
                                                                          : Null(key);
    public XArray Add( in ReadOnlySpan<char> key, in short? value ) => value.HasValue
                                                                           ? Add(key, value.Value)
                                                                           : Null(key);
    public XArray Add( in ReadOnlySpan<char> key, in ushort? value ) => value.HasValue
                                                                            ? Add(key, value.Value)
                                                                            : Null(key);
    public XArray Add( in ReadOnlySpan<char> key, in int? value ) => value.HasValue
                                                                         ? Add(key, value.Value)
                                                                         : Null(key);
    public XArray Add( in ReadOnlySpan<char> key, in uint? value ) => value.HasValue
                                                                          ? Add(key, value.Value)
                                                                          : Null(key);
    public XArray Add( in ReadOnlySpan<char> key, in long? value ) => value.HasValue
                                                                          ? Add(key, value.Value)
                                                                          : Null(key);
    public XArray Add( in ReadOnlySpan<char> key, in ulong? value ) => value.HasValue
                                                                           ? Add(key, value.Value)
                                                                           : Null(key);
    public XArray Add( in ReadOnlySpan<char> key, in float? value ) => value.HasValue
                                                                           ? Add(key, value.Value)
                                                                           : Null(key);
    public XArray Add( in ReadOnlySpan<char> key, in double? value ) => value.HasValue
                                                                            ? Add(key, value.Value)
                                                                            : Null(key);
    public XArray Add( in ReadOnlySpan<char> key, in decimal? value ) => value.HasValue
                                                                             ? Add(key, value.Value)
                                                                             : Null(key);


    public XArray Add( in ReadOnlySpan<char> key, in string value ) => Add(key, value.AsSpan());
    public XArray Add( in ReadOnlySpan<char> key, in ReadOnlySpan<char> value )
    {
        _writer.Indent(key).Append(value).Next(key);
        return this;
    }


    public XArray Add( in ReadOnlySpan<char> key, in ISpanFormattable value, in int bufferSize ) => Add(key,                         value, bufferSize, CultureInfo.CurrentCulture);
    public XArray Add( in ReadOnlySpan<char> key, in ISpanFormattable value, in int bufferSize, in CultureInfo culture ) => Add(key, value, bufferSize, default, culture);
    public XArray Add( in ReadOnlySpan<char> key, in ISpanFormattable value, in int bufferSize, in ReadOnlySpan<char> format, in CultureInfo culture )
    {
        _writer.Indent(key).Append(value, format, culture, bufferSize).Next(key);
        return this;
    }


    public XArray Add<T>( in ReadOnlySpan<char> key, in T? value, in int bufferSize ) where T : struct, ISpanFormattable => Add(key,                         value, bufferSize, CultureInfo.CurrentCulture);
    public XArray Add<T>( in ReadOnlySpan<char> key, in T? value, in int bufferSize, in CultureInfo culture ) where T : struct, ISpanFormattable => Add(key, value, bufferSize, default, culture);
    public XArray Add<T>( in ReadOnlySpan<char> key, in T? value, in int bufferSize, in ReadOnlySpan<char> format, in CultureInfo culture ) where T : struct, ISpanFormattable
    {
        _writer.Indent(key).Append(value, format, culture, bufferSize).Next(key);
        return this;
    }


    public XArray Add( in KeyValuePair<string, string>  pair ) => Add(pair.Key, pair.Value);
    public XArray Add( in KeyValuePair<string, char>    pair ) => Add(pair.Key, pair.Value);
    public XArray Add( in KeyValuePair<string, short>   pair ) => Add(pair.Key, pair.Value);
    public XArray Add( in KeyValuePair<string, ushort>  pair ) => Add(pair.Key, pair.Value);
    public XArray Add( in KeyValuePair<string, int>     pair ) => Add(pair.Key, pair.Value);
    public XArray Add( in KeyValuePair<string, uint>    pair ) => Add(pair.Key, pair.Value);
    public XArray Add( in KeyValuePair<string, long>    pair ) => Add(pair.Key, pair.Value);
    public XArray Add( in KeyValuePair<string, ulong>   pair ) => Add(pair.Key, pair.Value);
    public XArray Add( in KeyValuePair<string, float>   pair ) => Add(pair.Key, pair.Value);
    public XArray Add( in KeyValuePair<string, double>  pair ) => Add(pair.Key, pair.Value);
    public XArray Add( in KeyValuePair<string, decimal> pair ) => Add(pair.Key, pair.Value);


    public XArray Add( in KeyValuePair<string, DateOnly>         pair ) => Add(pair.Key,                           pair.Value, 100);
    public XArray Add( in KeyValuePair<string, TimeOnly>         pair ) => Add(pair.Key,                           pair.Value, 100);
    public XArray Add( in KeyValuePair<string, TimeSpan>         pair ) => Add(pair.Key,                           pair.Value, 100);
    public XArray Add( in KeyValuePair<string, DateTimeOffset>   pair ) => Add(pair.Key,                           pair.Value, 100);
    public XArray Add( in KeyValuePair<string, DateTime>         pair ) => Add(pair.Key,                           pair.Value, 100);
    public XArray Add( in KeyValuePair<string, ISpanFormattable> pair, in int bufferSize = 1000 ) => Add(pair.Key, pair.Value, bufferSize);


    public XArray Add( in DictionaryEntry pair, in int bufferSize = 1000 )
    {
        ReadOnlySpan<char> k = pair.Key.ToString();

        return pair.Value switch
               {
                   null               => Null(k),
                   string v           => Add(k, v),
                   char v             => Add(k, v),
                   short v            => Add(k, v, 10),
                   ushort v           => Add(k, v, 10),
                   int v              => Add(k, v, 20),
                   uint v             => Add(k, v, 20),
                   long v             => Add(k, v, 30),
                   ulong v            => Add(k, v, 30),
                   float v            => Add(k, v, 350),
                   double v           => Add(k, v, 650),
                   decimal v          => Add(k, v, 200),
                   DateOnly v         => Add(k, v, 100),
                   TimeOnly v         => Add(k, v, 100),
                   TimeSpan v         => Add(k, v, 100),
                   DateTimeOffset v   => Add(k, v, 100),
                   DateTime v         => Add(k, v, 100),
                   ISpanFormattable v => Add(k, v, bufferSize),
                   _                  => Add(k, pair.Value.ToString() ?? XWriter.NULL)
               };
    }


    public XArray AddArray( in   ReadOnlySpan<char> name ) => new(name, ref _writer);
    public XObject AddObject( in ReadOnlySpan<char> name ) => new(name, ref _writer);


    public void Dispose() => _writer.FinishBlock(_name);
}
