// Jakar.Extensions :: Jakar.Database
// 08/20/2023  9:16 PM


namespace Jakar.Database;


[DefaultMember( nameof(Empty) )]
public readonly struct RecordID<TRecord>( Guid value ) : IEquatable<RecordID<TRecord>>, IComparable<RecordID<TRecord>>, ISpanFormattable, ISpanParsable<RecordID<TRecord>>, IRegisterDapperTypeHandlers
    where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    public static readonly RecordID<TRecord> Empty = new(Guid.Empty);
    public readonly        string            key   = $"{TRecord.TableName}:{value}";
    public readonly        Guid              value = value;
    public                 Guid              Value { get => value; }


    public static ValueEqualizer<RecordID<TRecord>> Equalizer { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => ValueEqualizer<RecordID<TRecord>>.Default; }
    public static ValueSorter<RecordID<TRecord>>    Sorter    { [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] get => ValueSorter<RecordID<TRecord>>.Default; }


    [Pure] public static RecordID<TRecord>  New()                                                                     => New( DateTimeOffset.UtcNow );
    [Pure] public static RecordID<TRecord>  New( DateTimeOffset                           timeStamp )                 => Create( Guid.CreateVersion7( timeStamp ) );
    [Pure] public static RecordID<TRecord>  Parse( string                                 value )                     => Create( Guid.Parse( value ) );
    [Pure] public static RecordID<TRecord>  Parse( scoped ref readonly ReadOnlySpan<char> value )                     => Create( Guid.Parse( value ) );
    [Pure] public static RecordID<TRecord>  ID( DbDataReader                              reader )                    => Create( reader, SQL.ID );
    [Pure] public static RecordID<TRecord>? CreatedBy( DbDataReader                       reader )                    => TryCreate( reader, SQL.CREATED_BY );
    [Pure] public static RecordID<TRecord>? TryCreate( DbDataReader                       reader, string columnName ) => TryCreate( reader.GetFieldValue<Guid?>( columnName ) );
    [Pure]
    public static RecordID<TRecord>? TryCreate( [NotNullIfNotNull( nameof(id) )] Guid? id ) => id.HasValue
                                                                                                   ? new RecordID<TRecord>( id.Value )
                                                                                                   : null;


    [Pure] public static RecordID<TRecord> Create( DbDataReader reader, string columnName ) => Create( reader.GetFieldValue<Guid>( columnName ) );
    [Pure] public static RecordID<TRecord> Create( Guid         id ) => new(id);
    [Pure]
    public static RecordID<TRecord> Create<T>( T id )
        where T : IUniqueID<Guid> => Create( id.ID );
    [Pure]
    public static IEnumerable<RecordID<TRecord>> Create<T>( IEnumerable<T> ids )
        where T : IUniqueID<Guid> => ids.Select( Create );
    [Pure]
    public static IAsyncEnumerable<RecordID<TRecord>> Create<T>( IAsyncEnumerable<T> ids )
        where T : IUniqueID<Guid> => ids.Select( Create );


    public static RecordID<TRecord> Parse( string                           value, IFormatProvider?      provider ) => new(Guid.Parse( value, provider ));
    public static bool              TryParse( [NotNullWhen( true )] string? value, out RecordID<TRecord> result )   => TryParse( value, null, out result );
    public static bool TryParse( [NotNullWhen(              true )] string? value, IFormatProvider? provider, out RecordID<TRecord> result )
    {
        if ( Guid.TryParse( value, provider, out Guid guid ) )
        {
            result = Create( guid );
            return true;
        }

        result = Empty;
        return false;
    }


    public static RecordID<TRecord> Parse( ReadOnlySpan<char>    value, IFormatProvider?      provider ) => new(Guid.Parse( value, provider ));
    public static bool              TryParse( ReadOnlySpan<char> value, out RecordID<TRecord> result )   => TryParse( value, null, out result );
    public static bool TryParse( ReadOnlySpan<char> value, IFormatProvider? provider, out RecordID<TRecord> result )
    {
        if ( Guid.TryParse( value, provider, out Guid guid ) )
        {
            result = Create( guid );
            return true;
        }

        result = Empty;
        return false;
    }


    public static implicit operator RecordID<TRecord>( TRecord record ) => new(record.ID.value);


    [Pure]
    public DynamicParameters ToDynamicParameters()
    {
        DynamicParameters parameters = new();
        parameters.Add( nameof(IRecordPair.ID), value );
        return parameters;
    }


    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public          bool   IsValid()                                                                                                                       => Guid.Empty.Equals( value ) is false;
    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public          bool   IsNotValid()                                                                                                                    => Guid.Empty.Equals( value );
    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public override string ToString()                                                                                                                      => value.ToString();
    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public          string ToString( string?            format,      IFormatProvider? formatProvider )                                                     => value.ToString( format, formatProvider );
    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public          bool   TryFormat( Span<char>        destination, out int          charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider ) => value.TryFormat( destination, out charsWritten, format );
    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public          bool   Equals( RecordID<TRecord>    other ) => value.Equals( other.value );
    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public          int    CompareTo( RecordID<TRecord> other ) => value.CompareTo( other.value );
    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public override int    GetHashCode()                        => value.GetHashCode();


    public static bool operator true( RecordID<TRecord>  recordID )               => recordID.IsValid();
    public static bool operator false( RecordID<TRecord> recordID )               => recordID.IsNotValid();
    public static bool operator ==( RecordID<TRecord>    a, RecordID<TRecord> b ) => a.Equals( b );
    public static bool operator !=( RecordID<TRecord>    a, RecordID<TRecord> b ) => a.Equals( b ) is false;


    public static void RegisterDapperTypeHandlers()
    {
        NullableDapperTypeHandler.Register();
        DapperTypeHandler.Register();
    }



    public class DapperTypeHandler : SqlConverter<DapperTypeHandler, RecordID<TRecord>>
    {
        public override void SetValue( IDbDataParameter parameter, RecordID<TRecord> value ) => parameter.Value = value.value;

        public override RecordID<TRecord> Parse( object value ) =>
            value switch
            {
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

        public override void WriteJson( JsonWriter writer, RecordID<TRecord> value, JsonSerializer serializer ) => serializer.Serialize( writer, value.value );
    }



    public class NullableDapperTypeHandler : SqlConverter<NullableDapperTypeHandler, RecordID<TRecord>?>
    {
        public override void SetValue( IDbDataParameter parameter, RecordID<TRecord>? value ) => parameter.Value = value?.value;

        public override RecordID<TRecord>? Parse( object value ) =>
            value switch
            {
                null                                                                                                          => default,
                Guid guidValue                                                                                                => new RecordID<TRecord>( guidValue ),
                string stringValue when !string.IsNullOrEmpty( stringValue ) && Guid.TryParse( stringValue, out Guid result ) => new RecordID<TRecord>( result ),
                _                                                                                                             => throw new InvalidCastException( $"Unable to cast object of type {value.GetType()} to RecordID<TRecord>" )
            };
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
                if ( destinationType == typeof(Guid) ) { return idValue.value; }

                if ( destinationType == typeof(string) ) { return idValue.value.ToString(); }
            }

            return base.ConvertTo( context, culture, value, destinationType );
        }
    }
}
