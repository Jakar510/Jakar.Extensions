﻿namespace Jakar.Extensions;


public sealed class NullableIntConverter : Newtonsoft.Json.JsonConverter<int?>
{
    public override void WriteJson( JsonWriter writer, int? value, JsonNetSerializer serializer ) => writer.WriteValue( value );
    public override int? ReadJson( JsonReader reader, Type objectType, int? existingValue, bool hasExistingValue, JsonNetSerializer serializer ) =>
        reader.Value switch
        {
            null    => existingValue,
            short n => n,
            int n   => n,
            long n  => (int)n,
            string s => int.TryParse( s, out int n )
                            ? n
                            : null,
            _ => throw new OutOfRangeException( nameof(reader.Value), reader.Value )
        };
}
