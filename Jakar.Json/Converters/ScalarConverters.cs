// Jakar.Json
// 10/07/2026

namespace Jakar.Json.Converters;


public readonly struct JsonBooleanConverter : IJsonConverter<bool>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in bool value )
        where TWriter : IJsonWriter, allows ref struct => writer.WriteBoolean(value);

    public static bool TryRead<TReader>( ref TReader reader, out bool value )
        where TReader : IJsonReader, allows ref struct => reader.TryReadBoolean(out value);
}



/// <summary> A non-nullable string: <see langword="null"/> is written as <c>null</c>, but reading <c>null</c> fails (wrap in <see cref="JsonNullableReferenceConverter{T,TConverter}"/> to allow it). </summary>
public readonly struct JsonStringConverter : IJsonConverter<string>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in string value )
        where TWriter : IJsonWriter, allows ref struct
    {
        if ( value is null ) { writer.WriteNull(); }
        else { writer.WriteString(value); }
    }

    public static bool TryRead<TReader>( ref TReader reader, [MaybeNullWhen(false)] out string value )
        where TReader : IJsonReader, allows ref struct
    {
        if ( reader.PeekKind() == JsonTokenKind.Null )
        {
            value = null;
            return reader.Fail(JsonErrorKind.InvalidValue);
        }

        return reader.TryReadString(out value);
    }
}



public readonly struct JsonCharConverter : IJsonConverter<char>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in char value )
        where TWriter : IJsonWriter, allows ref struct => writer.WriteString(new ReadOnlySpan<char>(in value));

    public static bool TryRead<TReader>( ref TReader reader, out char value )
        where TReader : IJsonReader, allows ref struct
    {
        value = '\0';
        JsonReaderCheckpoint mark = reader.Checkpoint();
        if ( !reader.TryReadStringSpan(out JsonSpan text) ) { return false; }

        Span<char> chars = stackalloc char[4];

        if ( text.Length <= 4 && text.CopyTo(chars) == 1 )
        {
            value = chars[0];
            return true;
        }

        reader.Rewind(mark);
        return reader.Fail(JsonErrorKind.InvalidValue);
    }
}



/// <summary> Any integer type. <typeparamref name="TMode"/> adds <see cref="JsonNumberMode.AllowReadingFromString"/> / <see cref="JsonNumberMode.WriteLargeAsString"/>. </summary>
public readonly struct JsonIntegerConverter<T, TMode> : IJsonConverter<T>
    where T : struct, IBinaryInteger<T>
    where TMode : IJsonNumberMode
{
    public static void Write<TWriter>( ref TWriter writer, scoped in T value )
        where TWriter : IJsonWriter, allows ref struct
    {
        if ( ( TMode.Mode & JsonNumberMode.WriteLargeAsString ) != 0 && !JsonNumbers.IsSafeInteger(value) ) { writer.WriteFormatted(value); }
        else { writer.WriteInteger(value); }
    }

    public static bool TryRead<TReader>( ref TReader reader, out T value )
        where TReader : IJsonReader, allows ref struct => reader.TryReadInteger(out value, ( TMode.Mode & ( JsonNumberMode.AllowReadingFromString | JsonNumberMode.WriteLargeAsString ) ) != 0);
}



/// <summary> <see cref="float"/>, <see cref="double"/>, <see cref="Half"/>, <see cref="decimal"/>. <typeparamref name="TMode"/> adds strings and NaN/Infinity handling. </summary>
public readonly struct JsonFloatConverter<T, TMode> : IJsonConverter<T>
    where T : struct, IFloatingPoint<T>
    where TMode : IJsonNumberMode
{
    public static void Write<TWriter>( ref TWriter writer, scoped in T value )
        where TWriter : IJsonWriter, allows ref struct
    {
        if ( ( TMode.Mode & JsonNumberMode.NonFiniteAsString ) != 0 && !T.IsFinite(value) ) { writer.WriteString(JsonNumbers.NonFiniteName(value)); }
        else { writer.WriteFloat(value); }
    }

    public static bool TryRead<TReader>( ref TReader reader, out T value )
        where TReader : IJsonReader, allows ref struct
    {
        JsonFloatRead mode = JsonFloatRead.Strict;
        if ( ( TMode.Mode & JsonNumberMode.AllowReadingFromString ) != 0 ) { mode |= JsonFloatRead.AllowString; }

        if ( ( TMode.Mode & JsonNumberMode.NonFiniteAsString ) != 0 ) { mode |= JsonFloatRead.AllowNonFinite; }

        return reader.TryReadFloat(out value, mode);
    }
}



public readonly struct JsonGuidConverter : IJsonConverter<Guid>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in Guid value )
        where TWriter : IJsonWriter, allows ref struct => writer.WriteFormatted(value, "D");

    public static bool TryRead<TReader>( ref TReader reader, out Guid value )
        where TReader : IJsonReader, allows ref struct => JsonText.TryRead<TReader, Guid, JsonText.Parsable<Guid>>(ref reader, out value);
}



/// <summary> ISO 8601 (<c>O</c>). <typeparamref name="TMode"/> decides what happens to <see cref="DateTimeKind.Local"/> values (§5.3). Reading keeps the kind (<see cref="DateTimeStyles.RoundtripKind"/>). </summary>
public readonly struct JsonDateTimeConverter<TMode> : IJsonConverter<DateTime>
    where TMode : IJsonDateTimeMode
{
    public static void Write<TWriter>( ref TWriter writer, scoped in DateTime value )
        where TWriter : IJsonWriter, allows ref struct
    {
        DateTime text = value;

        if ( value.Kind == DateTimeKind.Local )
        {
            switch ( TMode.Mode )
            {
                case JsonLocalDateTimes.Error:
                    throw new JsonWriteException(new JsonError(JsonErrorKind.InvalidValue, 0, 0, 0), "A local DateTime depends on the machine's time zone (LocalDateTimes = Error); use UTC or DateTimeOffset.");

                case JsonLocalDateTimes.WriteOffset:
                    break;

                default:
                    text = value.ToUniversalTime();
                    break;
            }
        }

        writer.WriteFormatted(text, "O");
    }

    public static bool TryRead<TReader>( ref TReader reader, out DateTime value )
        where TReader : IJsonReader, allows ref struct => JsonText.TryRead<TReader, DateTime, JsonText.DateTimeText>(ref reader, out value);
}



public readonly struct JsonDateTimeOffsetConverter : IJsonConverter<DateTimeOffset>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in DateTimeOffset value )
        where TWriter : IJsonWriter, allows ref struct => writer.WriteFormatted(value, "O");

    public static bool TryRead<TReader>( ref TReader reader, out DateTimeOffset value )
        where TReader : IJsonReader, allows ref struct => JsonText.TryRead<TReader, DateTimeOffset, JsonText.DateTimeOffsetText>(ref reader, out value);
}



public readonly struct JsonDateOnlyConverter : IJsonConverter<DateOnly>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in DateOnly value )
        where TWriter : IJsonWriter, allows ref struct => writer.WriteFormatted(value, "yyyy-MM-dd");

    public static bool TryRead<TReader>( ref TReader reader, out DateOnly value )
        where TReader : IJsonReader, allows ref struct => JsonText.TryRead<TReader, DateOnly, JsonText.Parsable<DateOnly>>(ref reader, out value);
}



public readonly struct JsonTimeOnlyConverter : IJsonConverter<TimeOnly>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in TimeOnly value )
        where TWriter : IJsonWriter, allows ref struct => writer.WriteFormatted(value, "HH:mm:ss.fffffff");

    public static bool TryRead<TReader>( ref TReader reader, out TimeOnly value )
        where TReader : IJsonReader, allows ref struct => JsonText.TryRead<TReader, TimeOnly, JsonText.Parsable<TimeOnly>>(ref reader, out value);
}



public readonly struct JsonTimeSpanConverter : IJsonConverter<TimeSpan>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in TimeSpan value )
        where TWriter : IJsonWriter, allows ref struct => writer.WriteFormatted(value, "c");

    public static bool TryRead<TReader>( ref TReader reader, out TimeSpan value )
        where TReader : IJsonReader, allows ref struct => JsonText.TryRead<TReader, TimeSpan, JsonText.Parsable<TimeSpan>>(ref reader, out value);
}



/// <summary> <see cref="Uri.OriginalString"/>; reads relative or absolute URIs. Non-nullable: reading <c>null</c> fails. </summary>
public readonly struct JsonUriConverter : IJsonConverter<Uri>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in Uri value )
        where TWriter : IJsonWriter, allows ref struct
    {
        if ( value is null ) { writer.WriteNull(); }
        else { writer.WriteString(value.OriginalString); }
    }

    public static bool TryRead<TReader>( ref TReader reader, [MaybeNullWhen(false)] out Uri value )
        where TReader : IJsonReader, allows ref struct
    {
        value = null;
        JsonReaderCheckpoint mark = reader.Checkpoint();
        if ( !reader.TryReadString(out string? text) ) { return false; }

        if ( Uri.TryCreate(text, UriKind.RelativeOrAbsolute, out value) ) { return true; }

        reader.Rewind(mark);
        return reader.Fail(JsonErrorKind.InvalidValue);
    }
}



/// <summary> <see cref="Version"/> as <c>"1.2.3.4"</c>. Non-nullable: reading <c>null</c> fails. </summary>
public readonly struct JsonVersionConverter : IJsonConverter<Version>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in Version value )
        where TWriter : IJsonWriter, allows ref struct
    {
        if ( value is null ) { writer.WriteNull(); }
        else { writer.WriteFormatted(value); }
    }

    public static bool TryRead<TReader>( ref TReader reader, [MaybeNullWhen(false)] out Version value )
        where TReader : IJsonReader, allows ref struct
    {
        if ( reader.PeekKind() == JsonTokenKind.Null )
        {
            value = null;
            return reader.Fail(JsonErrorKind.InvalidValue);
        }

        return JsonText.TryRead<TReader, Version, JsonText.VersionText>(ref reader, out value);
    }
}



/// <summary> Any <see cref="ISpanFormattable"/> + <see cref="ISpanParsable{TSelf}"/> type as a JSON string, invariant culture (<see cref="Version"/>, <c>Email</c>, <c>AppVersion</c>, ...). </summary>
public readonly struct JsonParsableConverter<T> : IJsonConverter<T>
    where T : ISpanFormattable, ISpanParsable<T>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in T value )
        where TWriter : IJsonWriter, allows ref struct
    {
        if ( value is null ) { writer.WriteNull(); }
        else { writer.WriteFormatted(value); }
    }

    public static bool TryRead<TReader>( ref TReader reader, [MaybeNullWhen(false)] out T value )
        where TReader : IJsonReader, allows ref struct
    {
        if ( reader.PeekKind() == JsonTokenKind.Null )
        {
            value = default;
            return reader.Fail(JsonErrorKind.InvalidValue);
        }

        return JsonText.TryRead<TReader, T, JsonText.Parsable<T>>(ref reader, out value);
    }
}



/// <summary> A <c>[GenerateJson]</c> type (or any <see cref="IJsonSerializable{TSelf}"/>). Non-nullable: reading <c>null</c> fails. </summary>
public readonly struct JsonModelConverter<T> : IJsonConverter<T>
    where T : IJsonSerializable<T>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in T value )
        where TWriter : IJsonWriter, allows ref struct
    {
        if ( value is null ) { writer.WriteNull(); }
        else { T.WriteJson(ref writer, in value); }
    }

    public static bool TryRead<TReader>( ref TReader reader, [MaybeNullWhen(false)] out T value )
        where TReader : IJsonReader, allows ref struct
    {
        if ( default(T) is null && reader.PeekKind() == JsonTokenKind.Null ) // reference types only; folds to a constant
        {
            value = default;
            return reader.Fail(JsonErrorKind.InvalidValue);
        }

        return T.TryReadJson(ref reader, out value);
    }
}



/// <summary> <c>T?</c> for a value type: <c>null</c> ↔ <see langword="null"/>, anything else through <typeparamref name="TConverter"/>. </summary>
public readonly struct JsonNullableConverter<T, TConverter> : IJsonConverter<T?>
    where T : struct
    where TConverter : IJsonConverter<T>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in T? value )
        where TWriter : IJsonWriter, allows ref struct
    {
        if ( !value.HasValue )
        {
            writer.WriteNull();
            return;
        }

        T inner = value.GetValueOrDefault();
        TConverter.Write(ref writer, in inner);
    }

    public static bool TryRead<TReader>( ref TReader reader, out T? value )
        where TReader : IJsonReader, allows ref struct
    {
        value = null;
        if ( reader.TryReadNull() ) { return true; }

        if ( !TConverter.TryRead(ref reader, out T inner) ) { return false; }

        value = inner;
        return true;
    }
}



/// <summary> A nullable reference type: <c>null</c> ↔ <see langword="null"/>, anything else through <typeparamref name="TConverter"/>. </summary>
public readonly struct JsonNullableReferenceConverter<T, TConverter> : IJsonConverter<T?>
    where T : class
    where TConverter : IJsonConverter<T>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in T? value )
        where TWriter : IJsonWriter, allows ref struct
    {
        if ( value is null ) { writer.WriteNull(); }
        else { TConverter.Write(ref writer, in value); }
    }

    public static bool TryRead<TReader>( ref TReader reader, out T? value )
        where TReader : IJsonReader, allows ref struct
    {
        value = null;
        if ( reader.TryReadNull() ) { return true; }

        return TConverter.TryRead(ref reader, out value);
    }
}



/// <summary> An enum as its underlying number (<c>Enums = Number</c>). Any numeric value round-trips, named or not. </summary>
public readonly struct JsonEnumNumberConverter<TEnum, TUnderlying> : IJsonConverter<TEnum>
    where TEnum : struct, Enum
    where TUnderlying : struct, IBinaryInteger<TUnderlying>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in TEnum value )
        where TWriter : IJsonWriter, allows ref struct => writer.WriteInteger(Unsafe.BitCast<TEnum, TUnderlying>(value));

    public static bool TryRead<TReader>( ref TReader reader, out TEnum value )
        where TReader : IJsonReader, allows ref struct
    {
        value = default;
        if ( !reader.TryReadInteger(out TUnderlying number) ) { return false; }

        value = Unsafe.BitCast<TUnderlying, TEnum>(number);
        return true;
    }
}
