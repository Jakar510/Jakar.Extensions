namespace Jakar.Extensions;


[Serializable]
public record BaseRecord
{
    public const int         ANSI_CAPACITY         = 8000;
    public const int         ANSI_TEXT_CAPACITY    = 2_147_483_647;
    public const int         BINARY_CAPACITY       = ANSI_TEXT_CAPACITY;
    public const int         DECIMAL_MAX_PRECISION = 38;
    public const int         DECIMAL_MAX_SCALE     = 29;
    public const string      EMPTY                 = "";
    public const int         MAX_STRING_SIZE       = int.MaxValue;
    public const string      NULL                  = "null";
    public const int         UNICODE_CAPACITY      = 4000;
    public const int         UNICODE_TEXT_CAPACITY = 1_073_741_823;
    protected    JsonObject? _additionalData;


    [StringLength(UNICODE_TEXT_CAPACITY), JsonExtensionData] public virtual JsonObject? AdditionalData { get => _additionalData; set => _additionalData = value; }
}



public abstract record BaseRecord<TClass> : BaseRecord, IEquatable<TClass>, IComparable<TClass>, IComparable
    where TClass : BaseRecord<TClass>, IJsonModel<TClass>
{
    public abstract bool Equals( TClass?    other );
    public abstract int  CompareTo( TClass? other );
    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        return other is TClass t
                   ? CompareTo(t)
                   : throw new ExpectedValueTypeException(nameof(other), other, typeof(TClass));
    }


    public static bool TryFromJson( string? json, [NotNullWhen(true)] out TClass? result )
    {
        try
        {
            if ( string.IsNullOrWhiteSpace(json) )
            {
                result = null;
                return false;
            }

            result = FromJson(json);
            return true;
        }
        catch ( Exception e ) { SelfLogger.WriteLine("{Exception}", e.ToString()); }

        result = null;
        return false;
    }
    public static TClass FromJson( string json ) => Validate.ThrowIfNull(JsonSerializer.Deserialize(json, TClass.JsonTypeInfo));


    public TClass WithAdditionalData( IJsonModel value ) => WithAdditionalData(value.AdditionalData);
    public virtual TClass WithAdditionalData( JsonObject? additionalData )
    {
        if ( additionalData is null ) { return (TClass)this; }

        JsonObject json = _additionalData ??= new JsonObject();
        foreach ( ( string key, JsonNode? jToken ) in additionalData ) { json[key] = jToken; }

        return (TClass)this;
    }
}



public abstract record BaseRecord<TClass, TID> : BaseRecord<TClass>, IUniqueID<TID>
    where TClass : BaseRecord<TClass, TID>, IJsonModel<TClass>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    private TID __id;


    public virtual TID ID { get => __id; init => __id = value; }


    protected BaseRecord() : base() { }
    protected BaseRecord( TID id ) => ID = id;


    protected bool SetID( TClass record ) => SetID(record.ID);
    protected bool SetID( TID id )
    {
        __id = id;
        return true;
    }
}
