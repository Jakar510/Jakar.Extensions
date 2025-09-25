// Jakar.Extensions :: Jakar.Database
// 08/20/2023  9:16 PM


namespace Jakar.Database;


[DefaultMember(nameof(Empty))]
public readonly struct RecordID<TClass>( Guid id ) : IEquatable<RecordID<TClass>>, IComparable<RecordID<TClass>>, ISpanFormattable, ISpanParsable<RecordID<TClass>>, IRegisterDapperTypeHandlers
    where TClass : ITableRecord<TClass>
{
    public static readonly RecordID<TClass> Empty = new(Guid.Empty);
    public readonly        string           key   = $"{TClass.TableName}:{id}";
    public readonly        Guid             Value = id;


    [Pure] public static RecordID<TClass>  New()                                                                     => New(DateTimeOffset.UtcNow);
    [Pure] public static RecordID<TClass>  New( DateTimeOffset                           timeStamp )                 => Create(Guid.CreateVersion7(timeStamp));
    [Pure] public static RecordID<TClass>  Parse( string                                 value )                     => Create(Guid.Parse(value));
    [Pure] public static RecordID<TClass>  Parse( scoped ref readonly ReadOnlySpan<char> value )                     => Create(Guid.Parse(value));
    [Pure] public static RecordID<TClass>  ID( DbDataReader                              reader )                    => Create(reader, nameof(IDateCreated.ID));
    [Pure] public static RecordID<TClass>? CreatedBy( DbDataReader                       reader )                    => TryCreate(reader, nameof(ICreatedBy.CreatedBy));
    [Pure] public static RecordID<TClass>? TryCreate( DbDataReader                       reader, string columnName ) => TryCreate(reader.GetFieldValue<Guid?>(columnName));


    [Pure] public static RecordID<TClass> Create( DbDataReader reader, string columnName ) => Create(reader.GetFieldValue<Guid>(columnName));
    [Pure] public static RecordID<TClass> Create( Guid         id ) => new(id);
    [Pure]
    public static RecordID<TClass> Create( [NotNullIfNotNull(nameof(id))] Guid? id ) => id.HasValue
                                                                                            ? new RecordID<TClass>(id.Value)
                                                                                            : New();
    [Pure]
    public static RecordID<TClass> Create<TValue>( TValue id )
        where TValue : IUniqueID<Guid> => Create(id.ID);
    [Pure]
    public static IEnumerable<RecordID<TClass>> Create<TValue>( IEnumerable<TValue> ids )
        where TValue : IUniqueID<Guid> => ids.Select(Create);
    [Pure]
    public static IAsyncEnumerable<RecordID<TClass>> Create<TValue>( IAsyncEnumerable<TValue> ids )
        where TValue : IUniqueID<Guid> => ids.Select(Create);
    [Pure]
    public static RecordID<TClass>? TryCreate( [NotNullIfNotNull(nameof(id))] Guid? id ) => id.HasValue
                                                                                                ? new RecordID<TClass>(id.Value)
                                                                                                : default;


    public static RecordID<TClass> Parse( string                         value, IFormatProvider?     provider ) => new(Guid.Parse(value, provider));
    public static bool             TryParse( [NotNullWhen(true)] string? value, out RecordID<TClass> result )   => TryParse(value, null, out result);
    public static bool TryParse( [NotNullWhen(            true)] string? value, IFormatProvider? provider, out RecordID<TClass> result )
    {
        if ( Guid.TryParse(value, provider, out Guid guid) )
        {
            result = Create(guid);
            return true;
        }

        result = Empty;
        return false;
    }


    public static RecordID<TClass> Parse( ReadOnlySpan<char>    value, IFormatProvider?     provider ) => new(Guid.Parse(value, provider));
    public static bool             TryParse( ReadOnlySpan<char> value, out RecordID<TClass> result )   => TryParse(value, null, out result);
    public static bool TryParse( ReadOnlySpan<char> value, IFormatProvider? provider, out RecordID<TClass> result )
    {
        if ( Guid.TryParse(value, provider, out Guid guid) )
        {
            result = Create(guid);
            return true;
        }

        result = Empty;
        return false;
    }


    public static implicit operator RecordID<TClass>( TClass record ) => new(record.ID.Value);


    [Pure]
    public DynamicParameters ToDynamicParameters()
    {
        DynamicParameters parameters = new();
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


    public          bool Equals( RecordID<TClass>    other )         => Value.Equals(other.Value);
    public          int  CompareTo( RecordID<TClass> other )         => Value.CompareTo(other.Value);
    public override int  GetHashCode()                               => Value.GetHashCode();
    public override bool Equals( [NotNullWhen(true)] object? other ) => other is RecordID<TClass> id && Equals(id);


    public static bool operator true( RecordID<TClass>  recordID )                      => recordID.IsValid();
    public static bool operator false( RecordID<TClass> recordID )                      => recordID.IsNotValid();
    public static bool operator ==( RecordID<TClass>?   left, RecordID<TClass>? right ) => Nullable.Equals(left, right);
    public static bool operator !=( RecordID<TClass>?   left, RecordID<TClass>? right ) => !Nullable.Equals(left, right);
    public static bool operator ==( RecordID<TClass>    left, RecordID<TClass>  right ) => EqualityComparer<RecordID<TClass>>.Default.Equals(left, right);
    public static bool operator !=( RecordID<TClass>    left, RecordID<TClass>  right ) => !EqualityComparer<RecordID<TClass>>.Default.Equals(left, right);
    public static bool operator >( RecordID<TClass>     left, RecordID<TClass>  right ) => Comparer<RecordID<TClass>>.Default.Compare(left, right) > 0;
    public static bool operator >=( RecordID<TClass>    left, RecordID<TClass>  right ) => Comparer<RecordID<TClass>>.Default.Compare(left, right) >= 0;
    public static bool operator <( RecordID<TClass>     left, RecordID<TClass>  right ) => Comparer<RecordID<TClass>>.Default.Compare(left, right) < 0;
    public static bool operator <=( RecordID<TClass>    left, RecordID<TClass>  right ) => Comparer<RecordID<TClass>>.Default.Compare(left, right) <= 0;


    public static void RegisterDapperTypeHandlers()
    {
        NullableDapperTypeHandler.Register();
        DapperTypeHandler.Register();
    }



    public class DapperTypeHandler : SqlConverter<DapperTypeHandler, RecordID<TClass>>
    {
        public override void SetValue( IDbDataParameter parameter, RecordID<TClass> value ) => parameter.Value = value.Value;

        public override RecordID<TClass> Parse( object value ) =>
            value switch
            {
                Guid guidValue                                                                                            => new RecordID<TClass>(guidValue),
                string stringValue when !string.IsNullOrEmpty(stringValue) && Guid.TryParse(stringValue, out Guid result) => new RecordID<TClass>(result),
                _                                                                                                         => throw new InvalidCastException($"Unable to cast object of type {value.GetType()} to RecordID<TClass>")
            };
    }



    public class JsonNetConverter : JsonConverter<RecordID<TClass>>
    {
        public override RecordID<TClass> Read( ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options )
        {
            Guid? guid = reader.TryGetGuid(out Guid id)
                             ? id
                             : null;

            return guid.HasValue
                       ? new RecordID<TClass>(guid.Value)
                       : default;
        }
        public override void Write( Utf8JsonWriter writer, RecordID<TClass> id, JsonSerializerOptions options ) => writer.WriteStringValue(id.Value);
    }



    public class NullableDapperTypeHandler : SqlConverter<NullableDapperTypeHandler, RecordID<TClass>?>
    {
        public override void SetValue( IDbDataParameter parameter, RecordID<TClass>? id ) => parameter.Value = id?.Value;

        public override RecordID<TClass>? Parse( object value ) =>
            value switch
            {
                null                                                                                                      => default,
                Guid guidValue                                                                                            => new RecordID<TClass>(guidValue),
                string stringValue when !string.IsNullOrEmpty(stringValue) && Guid.TryParse(stringValue, out Guid result) => new RecordID<TClass>(result),
                _                                                                                                         => throw new InvalidCastException($"Unable to cast object of type {value.GetType()} to RecordID<TClass>")
            };
    }



    public class TypeConverter : System.ComponentModel.TypeConverter
    {
        public override bool CanConvertFrom( ITypeDescriptorContext? context, Type sourceType ) => sourceType == typeof(Guid) || sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

        public override object? ConvertFrom( ITypeDescriptorContext? context, CultureInfo? culture, object value ) =>
            value switch
            {
                Guid guidValue                                                                                            => new RecordID<TClass>(guidValue),
                string stringValue when !string.IsNullOrEmpty(stringValue) && Guid.TryParse(stringValue, out Guid result) => new RecordID<TClass>(result),
                _                                                                                                         => base.ConvertFrom(context, culture, value)
            };

        public override bool CanConvertTo( ITypeDescriptorContext? context, Type? sourceType ) => sourceType == typeof(Guid) || sourceType == typeof(string) || base.CanConvertTo(context, sourceType);

        public override object? ConvertTo( ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType )
        {
            if ( value is RecordID<TClass> idValue )
            {
                if ( destinationType == typeof(Guid) ) { return idValue.Value; }

                if ( destinationType == typeof(string) ) { return idValue.ToString(); }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
