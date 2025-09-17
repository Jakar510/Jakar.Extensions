// Jakar.Extensions :: Jakar.Database
// 08/20/2023  9:16 PM


namespace Jakar.Database;


[DefaultMember(nameof(Empty))]
public readonly struct RecordID<TClass>( Guid value ) : IEquatable<RecordID<TClass>>, IComparable<RecordID<TClass>>, ISpanFormattable, ISpanParsable<RecordID<TClass>>, IRegisterDapperTypeHandlers
    where TClass : class, ITableRecord<TClass>, IDbReaderMapping<TClass>
{
    public static readonly RecordID<TClass> Empty = new(Guid.Empty);
    public readonly        string           key   = $"{TClass.TableName}:{value}";
    public readonly        Guid             value = value;
    public                 Guid             Value => value;


    public static ValueSorter<RecordID<TClass>> Sorter { [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)] get => ValueSorter<RecordID<TClass>>.Default; }


    [Pure] public static RecordID<TClass>  New()                                                                     => New(DateTimeOffset.UtcNow);
    [Pure] public static RecordID<TClass>  New( DateTimeOffset                           timeStamp )                 => Create(Guid.CreateVersion7(timeStamp));
    [Pure] public static RecordID<TClass>  Parse( string                                 value )                     => Create(Guid.Parse(value));
    [Pure] public static RecordID<TClass>  Parse( scoped ref readonly ReadOnlySpan<char> value )                     => Create(Guid.Parse(value));
    [Pure] public static RecordID<TClass>  ID( DbDataReader                              reader )                    => Create(reader, nameof(IRecordPair.ID));
    [Pure] public static RecordID<TClass>? CreatedBy( DbDataReader                       reader )                    => TryCreate(reader, nameof(IOwnedTableRecord.CreatedBy));
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


    public static implicit operator RecordID<TClass>( TClass record ) => new(record.ID.value);


    [Pure]
    public DynamicParameters ToDynamicParameters()
    {
        DynamicParameters parameters = new();
        parameters.Add(nameof(IRecordPair.ID), value);
        return parameters;
    }


    public          bool   IsValid()                                                                                                                      => !Guid.Empty.Equals(value);
    public          bool   IsNotValid()                                                                                                                   => Guid.Empty.Equals(value);
    public override string ToString()                                                                                                                     => value.ToString();
    public          string ToString( string?           format,      IFormatProvider? formatProvider )                                                     => value.ToString(format, formatProvider);
    public          bool   TryFormat( Span<char>       destination, out int          charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider ) => value.TryFormat(destination, out charsWritten, format);
    public          bool   Equals( RecordID<TClass>    other )         => value.Equals(other.value);
    public          int    CompareTo( RecordID<TClass> other )         => value.CompareTo(other.value);
    public override int    GetHashCode()                               => value.GetHashCode();
    public override bool   Equals( [NotNullWhen(true)] object? other ) => other is RecordID<TClass> id && Equals(id);


    public static bool operator true( RecordID<TClass>  recordID )              => recordID.IsValid();
    public static bool operator false( RecordID<TClass> recordID )              => recordID.IsNotValid();
    public static bool operator ==( RecordID<TClass>    a, RecordID<TClass> b ) => a.Equals(b);
    public static bool operator !=( RecordID<TClass>    a, RecordID<TClass> b ) => !a.Equals(b);


    public static void RegisterDapperTypeHandlers()
    {
        NullableDapperTypeHandler.Register();
        DapperTypeHandler.Register();
    }



    public class DapperTypeHandler : SqlConverter<DapperTypeHandler, RecordID<TClass>>
    {
        public override void SetValue( IDbDataParameter parameter, RecordID<TClass> value ) => parameter.Value = value.value;

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
        public override RecordID<TClass> ReadJson( JsonReader reader, Type objectType, RecordID<TClass> existingValue, bool hasExistingValue, JsonSerializer serializer )
        {
            Guid? guid = serializer.Deserialize<Guid?>(reader);

            return guid.HasValue
                       ? new RecordID<TClass>(guid.Value)
                       : default;
        }

        public override void WriteJson( JsonWriter writer, RecordID<TClass> value, JsonSerializer serializer ) => serializer.Serialize(writer, value.value);
    }



    public class NullableDapperTypeHandler : SqlConverter<NullableDapperTypeHandler, RecordID<TClass>?>
    {
        public override void SetValue( IDbDataParameter parameter, RecordID<TClass>? value ) => parameter.Value = value?.value;

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
                if ( destinationType == typeof(Guid) ) { return idValue.value; }

                if ( destinationType == typeof(string) ) { return idValue.value.ToString(); }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
