﻿namespace Jakar.Extensions;


public sealed class NullableLongConverter : Newtonsoft.Json.JsonConverter<long?>
{
    public override void WriteJson( JsonWriter writer, long? value, JsonNetSerializer serializer ) => writer.WriteValue( value );
    public override long? ReadJson( JsonReader reader, Type objectType, long? existingValue, bool hasExistingValue, JsonNetSerializer serializer ) =>
        reader.Value switch
        {
            null    => existingValue,
            short n => n,
            int n   => n,
            long n  => n,
            string s => long.TryParse( s, out long n )
                            ? n
                            : null,
            _ => throw new OutOfRangeException( nameof(reader.Value), reader.Value )
        };
}
