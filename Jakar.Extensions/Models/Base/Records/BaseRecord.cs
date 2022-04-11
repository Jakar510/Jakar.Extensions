using Jakar.Extensions.Attributes;
using System.Text.Json;
using System.Xml;
using StronglyTypedIds;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;



namespace Jakar.Extensions.Models.Base.Records;


[Serializable]
public record BaseRecord : IDataBaseID
{
    [Key] public virtual long ID { get; init; }
}



[Serializable]
public abstract partial record BaseRecord<T> : IUniqueID<BaseRecord<T>.UniqueID> where T : BaseRecord<T>
{
    [DataBaseType(DbType.Int64)] [Key] public virtual UniqueID ID { get; init; }


    static BaseRecord() => SqlMapper.AddTypeHandler(new UniqueID.DapperHandler());
    protected BaseRecord() { }
    protected BaseRecord( long id ) => ID = new UniqueID(id);
    
    // [StronglyTypedId(StronglyTypedIdBackingType.Long,
    //                  StronglyTypedIdConverter.NewtonsoftJson | StronglyTypedIdConverter.TypeConverter | StronglyTypedIdConverter.DapperTypeHandler | StronglyTypedIdConverter.SystemTextJson,
    //                  StronglyTypedIdImplementations.IComparable | StronglyTypedIdImplementations.IEquatable)]
    // public partial struct UniqueID { }
    


    [Serializable]
    [DataBaseType(DbType.Int64)]
    [TypeConverter(typeof(Converter))]
    [System.Text.Json.Serialization.JsonConverter(typeof(JsonConverter))]
    [JsonConverter(typeof(JsonNetConverter))]
    public readonly struct UniqueID : IEquatable<UniqueID>, IComparable<UniqueID>, IComparable // where T : BaseRecord<T>, IComparable<T>, IEquatable<T>
    {
        public readonly long value;
        public UniqueID( ReadOnlySpan<char> value ) => this.value = long.Parse(value);
        public UniqueID( T               value ) => this.value = value.ID;
        public UniqueID( long               value ) => this.value = value;
    
    
        public static implicit operator UniqueID( T value ) => new(value);
        public static implicit operator UniqueID( long value ) => new(value);
        public static implicit operator long( UniqueID id ) => id.value;
    
    
        public int CompareTo( UniqueID other ) => value.CompareTo(other.value);
        public int CompareTo( object? other )
        {
            if ( other is null ) { return 1; }
    
            return other is UniqueID id
                       ? CompareTo(id)
                       : throw new ExpectedValueTypeException(nameof(other), other, typeof(UniqueID));
        }
    
    
        public bool Equals( UniqueID         other ) => EqualityComparer<long>.Default.Equals(value, other.value);
        public override bool Equals( object? obj ) => obj is UniqueID other && Equals(other);
        public override int GetHashCode() => EqualityComparer<long>.Default.GetHashCode(value);
    
    
        public static bool operator ==( UniqueID left, UniqueID right ) => left.Equals(right);
        public static bool operator !=( UniqueID left, UniqueID right ) => !left.Equals(right);
        public static bool operator <( UniqueID  left, UniqueID right ) => left.CompareTo(right) < 0;
        public static bool operator >( UniqueID  left, UniqueID right ) => left.CompareTo(right) > 0;
        public static bool operator <=( UniqueID left, UniqueID right ) => left.CompareTo(right) <= 0;
        public static bool operator >=( UniqueID left, UniqueID right ) => left.CompareTo(right) >= 0;
    
    
    
        public class Sorter : ValueSorter<UniqueID> { }
    
    
    
        public class Equalizer : ValueEqualizer<UniqueID> { }
    
    
    
        public class Converter : TypeConverter
        {
            public override bool CanConvertFrom( ITypeDescriptorContext context, Type sourceType ) => sourceType.IsOneOf(typeof(long));
            public override bool CanConvertTo( ITypeDescriptorContext   context, Type destinationType ) => destinationType.IsOneOf(typeof(long));
    
            public override object ConvertTo( ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType )
            {
                if ( value is not UniqueID id ) { throw new ExpectedValueTypeException(nameof(value), value, typeof(UniqueID)); }
    
                if ( destinationType != typeof(long) ) { throw new ExpectedValueTypeException(nameof(destinationType), destinationType, typeof(long)); }
    
                return id.value;
            }
            public override object ConvertFrom( ITypeDescriptorContext context, CultureInfo culture, object value ) => value switch
                                                                                                                       {
                                                                                                                           long id => new UniqueID(id),
                                                                                                                           _       => throw new ExpectedValueTypeException(nameof(value), value, typeof(long))
                                                                                                                       };
        }
    
    
    
        public class JsonNetConverter : JsonConverter<UniqueID>
        {
            public override void WriteJson( JsonWriter writer, UniqueID id, JsonSerializer serializer ) => writer.WriteValue(id.value);
            public override UniqueID ReadJson( JsonReader reader, Type objectType, UniqueID existingValue, bool hasExistingValue, JsonSerializer serializer )
            {
                if ( reader.Value is long id ) { return new UniqueID(id); }
    
                throw new ExpectedValueTypeException(nameof(reader.Value), reader.Value, typeof(long));
            }
        }
    
    
    
        public class JsonConverter : System.Text.Json.Serialization.JsonConverter<UniqueID>
        {
            public override UniqueID Read( ref Utf8JsonReader reader, Type     typeToConvert, JsonSerializerOptions options ) => new(reader.GetInt64());
            public override void Write( Utf8JsonWriter        writer, UniqueID id,            JsonSerializerOptions options ) => writer.WriteNumberValue(id.value);
        }
    
    
    
        public class DapperHandler : SqlMapper.TypeHandler<UniqueID>
        {
            public override void SetValue( IDbDataParameter parameter, UniqueID value ) => parameter.Value = value.value;
            public override UniqueID Parse( object value )
            {
                if ( value is not long item ) { throw new ExpectedValueTypeException(nameof(value), value, typeof(long)); }
    
                return new UniqueID(item);
            }
        }
    }
}
