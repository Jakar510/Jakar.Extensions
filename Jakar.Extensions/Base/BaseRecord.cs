namespace Jakar.Extensions;


[Serializable]
public record BaseRecord
{
    protected JObject? _additionalData;

    [JsonExtensionData] public virtual JObject? AdditionalData { get => _additionalData; set => _additionalData = value; }
}



public abstract record BaseRecord<TSelf> : BaseRecord, IEquatable<TSelf>, IComparable<TSelf>, IComparable
    where TSelf : BaseRecord<TSelf>, IJsonModel<TSelf>
{
    public abstract bool Equals( TSelf?    other );
    public abstract int  CompareTo( TSelf? other );
    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        return other is TSelf t
                   ? CompareTo(t)
                   : throw new ExpectedValueTypeException(nameof(other), other, typeof(TSelf));
    }


    public static bool TryFromJson( string? json, [NotNullWhen(true)] out TSelf? result )
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
    public static TSelf FromJson( string json ) => json.FromJson<TSelf>();


    public TSelf WithAdditionalData( IJsonModel value ) => WithAdditionalData(value.AdditionalData);
    public virtual TSelf WithAdditionalData( JObject? additionalData )
    {
        if ( additionalData is null ) { return (TSelf)this; }

        JObject json = _additionalData ??= new JObject();
        foreach ( ( string key, JToken? jToken ) in additionalData ) { json[key] = jToken; }

        return (TSelf)this;
    }
}



public abstract record BaseRecord<TSelf, TID> : BaseRecord<TSelf>, IUniqueID<TID>
    where TSelf : BaseRecord<TSelf, TID>, IJsonModel<TSelf>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    private TID __id;


    public virtual TID ID { get => __id; init => __id = value; }


    protected BaseRecord() : base() { }
    protected BaseRecord( TID id ) => ID = id;


    protected bool SetID( TSelf record ) => SetID(record.ID);
    protected bool SetID( TID id )
    {
        __id = id;
        return true;
    }
}
