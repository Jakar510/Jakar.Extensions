﻿namespace Jakar.Extensions;


public sealed class NullableDoubleConverter : Newtonsoft.Json.JsonConverter<double?>
{
    public override void WriteJson( JsonWriter writer, double? value, JsonNetSerializer serializer ) => writer.WriteValue( value );
    public override double? ReadJson( JsonReader reader, Type objectType, double? existingValue, bool hasExistingValue, JsonNetSerializer serializer ) =>
        reader.Value switch
        {
            null     => existingValue,
            short n  => n,
            int n    => n,
            long n   => n,
            float n  => n,
            double n => n,
            string s => double.TryParse( s, out double n )
                            ? n
                            : null,
            _ => throw new OutOfRangeException( nameof(reader.Value), reader.Value )
        };
}
