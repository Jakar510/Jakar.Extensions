// Jakar.Extensions :: Jakar.Database
// 08/20/2023  9:16 PM


namespace Jakar.Database;


[DefaultMember(nameof(Empty))]
public readonly struct RecordID<TSelf> : IEquatable<RecordID<TSelf>>, IComparable<RecordID<TSelf>>, ISpanFormattable, ISpanParsable<RecordID<TSelf>>, IRegisterDapperTypeHandlers
    where TSelf : ITableRecord<TSelf>
{
    public static readonly RecordID<TSelf> Empty = new(Guid.Empty);
    public readonly        string          key;
    public readonly        Guid            Value;


    public RecordID( Guid id )
    {
        key   = $"{TSelf.TableName}:{id}";
        Value = id;
    }


    [Pure] public static RecordID<TSelf>  New()                                                        => New(DateTimeOffset.UtcNow);
    [Pure] public static RecordID<TSelf>  New( DateTimeOffset              timeStamp )                 => Create(Guid.CreateVersion7(timeStamp));
    [Pure] public static RecordID<TSelf>  Parse( string                    value )                     => Create(Guid.Parse(value));
    [Pure] public static RecordID<TSelf>  Parse( params ReadOnlySpan<char> value )                     => Create(Guid.Parse(value));
    [Pure] public static RecordID<TSelf>  ID( DbDataReader                 reader )                    => Create(reader, nameof(IDateCreated.ID));
    [Pure] public static RecordID<TSelf>? CreatedBy( DbDataReader          reader )                    => TryCreate(reader, nameof(ICreatedBy.CreatedBy));
    [Pure] public static RecordID<TSelf>? TryCreate( DbDataReader          reader, string columnName ) => TryCreate(reader.GetFieldValue<Guid?>(columnName));
    [Pure] public static RecordID<TSelf>  Create( DbDataReader             reader, string columnName ) => Create(reader.GetFieldValue<Guid>(columnName));
    [Pure] public static RecordID<TSelf>  Create( Guid                     id ) => new(id);
    [Pure] public static RecordID<TSelf> Create( [NotNullIfNotNull(nameof(id))] Guid? id ) => id.HasValue
                                                                                                  ? new RecordID<TSelf>(id.Value)
                                                                                                  : New();
    [Pure] public static RecordID<TSelf> Create<TValue>( TValue id )
        where TValue : IUniqueID<Guid> => Create(id.ID);
    [Pure] public static IEnumerable<RecordID<TSelf>> Create<TValue>( IEnumerable<TValue> ids )
        where TValue : IUniqueID<Guid> => ids.Select(Create);
    [Pure] public static IAsyncEnumerable<RecordID<TSelf>> Create<TValue>( IAsyncEnumerable<TValue> ids )
        where TValue : IUniqueID<Guid> => ids.Select(Create);
    [Pure] public static RecordID<TSelf>? TryCreate( [NotNullIfNotNull(nameof(id))] Guid? id ) => id.HasValue
                                                                                                      ? new RecordID<TSelf>(id.Value)
                                                                                                      : default;


    public static RecordID<TSelf> Parse( string                         value, IFormatProvider?    provider ) => new(Guid.Parse(value, provider));
    public static bool            TryParse( [NotNullWhen(true)] string? value, out RecordID<TSelf> result )   => TryParse(value, null, out result);
    public static bool TryParse( [NotNullWhen(           true)] string? value, IFormatProvider? provider, out RecordID<TSelf> result )
    {
        if ( Guid.TryParse(value, provider, out Guid guid) )
        {
            result = Create(guid);
            return true;
        }

        result = Empty;
        return false;
    }


    public static RecordID<TSelf> Parse( ReadOnlySpan<char>    value, IFormatProvider?    provider ) => new(Guid.Parse(value, provider));
    public static bool            TryParse( ReadOnlySpan<char> value, out RecordID<TSelf> result )   => TryParse(value, null, out result);
    public static bool TryParse( ReadOnlySpan<char> value, IFormatProvider? provider, out RecordID<TSelf> result )
    {
        if ( Guid.TryParse(value, provider, out Guid guid) )
        {
            result = Create(guid);
            return true;
        }

        result = Empty;
        return false;
    }


    public static implicit operator RecordID<TSelf>( TSelf record ) => new(record.ID.Value);


    public UInt128 GetHash() => key.Hash128();
    [Pure] public PostgresParameters ToDynamicParameters()
    {
        PostgresParameters parameters = PostgresParameters.Create<TSelf>();
        parameters.Add(nameof(IDateCreated.ID), Value);
        return parameters;
    }


    public bool IsValid()    => !Guid.Empty.Equals(Value);
    public bool IsNotValid() => Guid.Empty.Equals(Value);


    public override string ToString() => Value.ToString();
    public string ToString( string? format, IFormatProvider? formatProvider ) => string.Equals(format, "b64", StringComparison.InvariantCultureIgnoreCase)
                                                                                     ? Value.ToBase64()
                                                                                     : Value.ToString(format, formatProvider);
    public bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider )
    {
        if ( format is not "b64" ) { return Value.TryFormat(destination, out charsWritten, format); }

        ReadOnlySpan<char> span = Value.ToBase64();
        span.CopyTo(destination);
        charsWritten = span.Length;
        return span.Length > 0;
    }


    public          bool Equals( RecordID<TSelf>    other )          => Value.Equals(other.Value);
    public          int  CompareTo( RecordID<TSelf> other )          => Value.CompareTo(other.Value);
    public override int  GetHashCode()                               => Value.GetHashCode();
    public override bool Equals( [NotNullWhen(true)] object? other ) => other is RecordID<TSelf> id && Equals(id);


    public static bool operator true( RecordID<TSelf>  recordID )                     => recordID.IsValid();
    public static bool operator false( RecordID<TSelf> recordID )                     => recordID.IsNotValid();
    public static bool operator ==( RecordID<TSelf>?   left, RecordID<TSelf>? right ) => Nullable.Equals(left, right);
    public static bool operator !=( RecordID<TSelf>?   left, RecordID<TSelf>? right ) => !Nullable.Equals(left, right);
    public static bool operator ==( RecordID<TSelf>    left, RecordID<TSelf>  right ) => EqualityComparer<RecordID<TSelf>>.Default.Equals(left, right);
    public static bool operator !=( RecordID<TSelf>    left, RecordID<TSelf>  right ) => !EqualityComparer<RecordID<TSelf>>.Default.Equals(left, right);
    public static bool operator >( RecordID<TSelf>     left, RecordID<TSelf>  right ) => Comparer<RecordID<TSelf>>.Default.Compare(left, right) > 0;
    public static bool operator >=( RecordID<TSelf>    left, RecordID<TSelf>  right ) => Comparer<RecordID<TSelf>>.Default.Compare(left, right) >= 0;
    public static bool operator <( RecordID<TSelf>     left, RecordID<TSelf>  right ) => Comparer<RecordID<TSelf>>.Default.Compare(left, right) < 0;
    public static bool operator <=( RecordID<TSelf>    left, RecordID<TSelf>  right ) => Comparer<RecordID<TSelf>>.Default.Compare(left, right) <= 0;


    public static void RegisterDapperTypeHandlers()
    {
        NullableDapperTypeHandler.Register();
        DapperTypeHandler.Register();
    }



    public class DapperTypeHandler : SqlConverter<DapperTypeHandler, RecordID<TSelf>>
    {
        public override void SetValue( IDbDataParameter parameter, RecordID<TSelf> value ) => parameter.Value = value.Value;

        public override RecordID<TSelf> Parse( object value ) =>
            value switch
            {
                Guid guidValue                                                                                            => new RecordID<TSelf>(guidValue),
                string stringValue when !string.IsNullOrEmpty(stringValue) && Guid.TryParse(stringValue, out Guid result) => new RecordID<TSelf>(result),
                _                                                                                                         => throw new InvalidCastException($"Unable to cast object of type {value.GetType()} to RecordID<TSelf>")
            };
    }



    public class JsonNetConverter : JsonConverter<RecordID<TSelf>>
    {
        public override RecordID<TSelf> Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
        {
            Guid? guid = reader.TryGetGuid(out Guid id)
                             ? id
                             : null;

            return guid.HasValue
                       ? new RecordID<TSelf>(guid.Value)
                       : default;
        }
        public override void Write( Utf8JsonWriter writer, RecordID<TSelf> id, JsonSerializerOptions options ) => writer.WriteStringValue(id.Value);
    }



    public class NullableDapperTypeHandler : SqlConverter<NullableDapperTypeHandler, RecordID<TSelf>?>
    {
        public override void SetValue( IDbDataParameter parameter, RecordID<TSelf>? id ) => parameter.Value = id?.Value;

        public override RecordID<TSelf>? Parse( object value ) =>
            value switch
            {
                null                                                                                                      => default,
                Guid guidValue                                                                                            => new RecordID<TSelf>(guidValue),
                string stringValue when !string.IsNullOrEmpty(stringValue) && Guid.TryParse(stringValue, out Guid result) => new RecordID<TSelf>(result),
                _                                                                                                         => throw new InvalidCastException($"Unable to cast object of type {value.GetType()} to RecordID<TSelf>")
            };
    }



    public class TypeConverter : System.ComponentModel.TypeConverter
    {
        public override bool CanConvertFrom( ITypeDescriptorContext? context, Type sourceType ) => sourceType == typeof(Guid) || sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

        public override object? ConvertFrom( ITypeDescriptorContext? context, CultureInfo? culture, object value ) =>
            value switch
            {
                Guid guidValue                                                                                            => new RecordID<TSelf>(guidValue),
                string stringValue when !string.IsNullOrEmpty(stringValue) && Guid.TryParse(stringValue, out Guid result) => new RecordID<TSelf>(result),
                _                                                                                                         => base.ConvertFrom(context, culture, value)
            };

        public override bool CanConvertTo( ITypeDescriptorContext? context, Type? sourceType ) => sourceType == typeof(Guid) || sourceType == typeof(string) || base.CanConvertTo(context, sourceType);

        public override object? ConvertTo( ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType )
        {
            if ( value is RecordID<TSelf> idValue )
            {
                if ( destinationType == typeof(Guid) ) { return idValue.Value; }

                if ( destinationType == typeof(string) ) { return idValue.ToString(); }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
