// Jakar.Extensions :: Jakar.Database
// 08/20/2023  9:16 PM


using System.Text.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;



namespace Jakar.Database;


// [System.Text.Json.Serialization.JsonConverter( typeof(RecordIDSystemTextJsonConverter) )]
// [TypeConverter( typeof(RecordIDTypeConverter) )]
// [JsonConverter( typeof(RecordIDJsonNetConverter) )]
public readonly record struct RecordID<TRecord>( Guid Value ) : IComparable<RecordID<TRecord>>, ISpanFormattable, IRegisterDapperTypeHandlers
    where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    public static readonly RecordID<TRecord> Empty = new(Guid.Empty);
    public static          RecordID<TRecord> New()                     => new(Guid.NewGuid());
    public static          RecordID<TRecord> New( Guid          id )   => new(id);
    public static          RecordID<TRecord> FromString( string guid ) => new(Guid.Parse( guid ));


    public static RecordID<TRecord>  ID( DbDataReader        reader )                    => new(reader.GetFieldValue<Guid>( nameof(IUniqueID<Guid>.ID) ));
    public static RecordID<TRecord>? CreatedBy( DbDataReader reader )                    => TryCreate( reader.GetFieldValue<Guid?>( nameof(IOwnedTableRecord.CreatedBy) ) );
    public static RecordID<TRecord>? TryCreate( DbDataReader reader, string columnName ) => TryCreate( reader.GetFieldValue<Guid?>( columnName ) );
    public static RecordID<TRecord>? TryCreate( [ NotNullIfNotNull( nameof(id) ) ] Guid? id ) => id.HasValue
                                                                                                     ? new RecordID<TRecord>( id.Value )
                                                                                                     : default;
    public static RecordID<TRecord>? TryCreate( ref Utf8JsonReader reader ) => Guid.TryParse( reader.GetString(), out Guid id )
                                                                                   ? new RecordID<TRecord>( id )
                                                                                   : default;


    public static implicit operator RecordID<TRecord>( TRecord record ) => new(record.ID.Value);

    public DynamicParameters ToDynamicParameters()
    {
        DynamicParameters parameters = new();
        parameters.Add( SQL.ID, Value );
        return parameters;
    }


    public bool IsValid()    => Guid.Empty.Equals( Value ) is false;
    public bool IsNotValid() => Guid.Empty.Equals( Value );


    public override string ToString() => Value.ToString();

    public string ToString( string? format, IFormatProvider? formatProvider ) => Value.ToString( format, formatProvider );

    public bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider ) => Value.TryFormat( destination, out charsWritten, format );


    public static bool operator true( RecordID<TRecord>  recordID ) => recordID.IsValid();
    public static bool operator false( RecordID<TRecord> recordID ) => recordID.IsNotValid();


    public          bool Equals( RecordID<TRecord>    other ) => Value.Equals( other.Value );
    public          int  CompareTo( RecordID<TRecord> other ) => Value.CompareTo( other.Value );
    public override int  GetHashCode()                        => Value.GetHashCode();


    public static void RegisterDapperTypeHandlers()
    {
        NullableDapperTypeHandler.Register();
        DapperTypeHandler.Register();
    }



    public class DapperTypeHandler : SqlConverter<DapperTypeHandler, RecordID<TRecord>>
    {
        public override void SetValue( IDbDataParameter parameter, RecordID<TRecord> value ) => parameter.Value = value.Value;

        public override RecordID<TRecord> Parse( object value ) =>
            value switch
            {
                Guid guidValue                                                                                                => new RecordID<TRecord>( guidValue ),
                string stringValue when !string.IsNullOrEmpty( stringValue ) && Guid.TryParse( stringValue, out Guid result ) => new RecordID<TRecord>( result ),
                _                                                                                                             => throw new InvalidCastException( $"Unable to cast object of type {value.GetType()} to RecordID<TRecord>" )
            };
    }



    public class NullableDapperTypeHandler : SqlConverter<NullableDapperTypeHandler, RecordID<TRecord>?>
    {
        public override void SetValue( IDbDataParameter parameter, RecordID<TRecord>? value ) => parameter.Value = value?.Value;

        public override RecordID<TRecord>? Parse( object value ) =>
            value switch
            {
                null                                                                                                          => default,
                Guid guidValue                                                                                                => new RecordID<TRecord>( guidValue ),
                string stringValue when !string.IsNullOrEmpty( stringValue ) && Guid.TryParse( stringValue, out Guid result ) => new RecordID<TRecord>( result ),
                _                                                                                                             => throw new InvalidCastException( $"Unable to cast object of type {value.GetType()} to RecordID<TRecord>" )
            };
    }



    public class JsonNetConverter : JsonConverter<RecordID<TRecord>>
    {
        public override RecordID<TRecord> ReadJson( JsonReader reader, Type objectType, RecordID<TRecord> existingValue, bool hasExistingValue, JsonSerializer serializer )
        {
            var guid = serializer.Deserialize<Guid?>( reader );

            return guid.HasValue
                       ? new RecordID<TRecord>( guid.Value )
                       : default;
        }

        public override void WriteJson( JsonWriter writer, RecordID<TRecord> value, JsonSerializer serializer ) => serializer.Serialize( writer, value.Value );
    }



    public class JsonConverter : System.Text.Json.Serialization.JsonConverter<RecordID<TRecord>>
    {
        public override RecordID<TRecord> Read( ref Utf8JsonReader reader, Type              typeToConvert, JsonSerializerOptions options ) => TryCreate( ref reader ) ?? default;
        public override void              Write( Utf8JsonWriter    writer, RecordID<TRecord> value,         JsonSerializerOptions options ) => writer.WriteStringValue( value.Value );
    }



    public class NullableJsonConverter : System.Text.Json.Serialization.JsonConverter<RecordID<TRecord>?>
    {
        public override RecordID<TRecord>? Read( ref Utf8JsonReader reader, Type               typeToConvert, JsonSerializerOptions options ) => TryCreate( ref reader ) ?? default;
        public override void               Write( Utf8JsonWriter    writer, RecordID<TRecord>? value,         JsonSerializerOptions options ) => writer.WriteStringValue( value?.Value ?? Guid.Empty );
    }



    public class TypeConverter : System.ComponentModel.TypeConverter
    {
        public override bool CanConvertFrom( ITypeDescriptorContext? context, Type sourceType ) => sourceType == typeof(Guid) || sourceType == typeof(string) || base.CanConvertFrom( context, sourceType );

        public override object? ConvertFrom( ITypeDescriptorContext? context, CultureInfo? culture, object value ) =>
            value switch
            {
                Guid guidValue                                                                                                => new RecordID<TRecord>( guidValue ),
                string stringValue when !string.IsNullOrEmpty( stringValue ) && Guid.TryParse( stringValue, out Guid result ) => new RecordID<TRecord>( result ),
                _                                                                                                             => base.ConvertFrom( context, culture, value )
            };

        public override bool CanConvertTo( ITypeDescriptorContext? context, Type? sourceType ) => sourceType == typeof(Guid) || sourceType == typeof(string) || base.CanConvertTo( context, sourceType );

        public override object? ConvertTo( ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType )
        {
            if ( value is RecordID<TRecord> idValue )
            {
                if ( destinationType == typeof(Guid) ) { return idValue.Value; }

                if ( destinationType == typeof(string) ) { return idValue.Value.ToString(); }
            }

            return base.ConvertTo( context, culture, value, destinationType );
        }
    }
}
