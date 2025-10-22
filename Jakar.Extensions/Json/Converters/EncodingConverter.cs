// Jakar.Extensions :: Jakar.Extensions
// 09/23/2025  18:44

namespace Jakar.Extensions;


public class EncodingConverter : JsonConverter<Encoding>
{
    public static readonly EncodingConverter Instance = new();
    public override Encoding? Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
    {
        string? value = reader.GetString();
        if ( string.IsNullOrWhiteSpace(value) ) { return null; }

        if ( string.Equals(value, nameof(Encoding.Default), StringComparison.InvariantCultureIgnoreCase) ) { return Encoding.Default; }

        if ( string.Equals(value, nameof(Encoding.UTF32), StringComparison.InvariantCultureIgnoreCase) ) { return Encoding.UTF32; }

        if ( string.Equals(value, nameof(Encoding.UTF8), StringComparison.InvariantCultureIgnoreCase) ) { return Encoding.UTF8; }

        if ( string.Equals(value, nameof(Encoding.Unicode), StringComparison.InvariantCultureIgnoreCase) ) { return Encoding.Unicode; }

        if ( string.Equals(value, nameof(Encoding.BigEndianUnicode), StringComparison.InvariantCultureIgnoreCase) ) { return Encoding.BigEndianUnicode; }

        if ( string.Equals(value, nameof(Encoding.ASCII), StringComparison.InvariantCultureIgnoreCase) ) { return Encoding.ASCII; }

        return Encoding.GetEncoding(value);
    }
    public override void Write( Utf8JsonWriter writer, Encoding value, JsonSerializerOptions options ) { writer.WriteStringValue(value.EncodingName); }
}
