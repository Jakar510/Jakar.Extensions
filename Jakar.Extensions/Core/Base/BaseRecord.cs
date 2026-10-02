namespace Jakar.Extensions;


[Serializable]
public record BaseRecord : IJsonModel
{
    protected Dictionary<string, JsonElement>? _additionalData;

    /// <summary> JSON members this type doesn't declare, kept so they survive a round trip. </summary>
    [JsonExtensionData] public virtual Dictionary<string, JsonElement>? AdditionalData { get => _additionalData; set => _additionalData = value; }
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


    public static TSelf FromJson( string             json )     => JsonModel.FromJson(json,     TSelf.JsonTypeInfo);
    public static TSelf FromJson( ReadOnlySpan<byte> utf8Json ) => JsonModel.FromJson(utf8Json, TSelf.JsonTypeInfo);
    public static bool TryFromJson( [NotNullWhen(true)] string? json,     [NotNullWhen(true)] out TSelf? result ) => JsonModel.TryFromJson(json,     TSelf.JsonTypeInfo, out result);
    public static bool TryFromJson( ReadOnlySpan<byte>          utf8Json, [NotNullWhen(true)] out TSelf? result ) => JsonModel.TryFromJson(utf8Json, TSelf.JsonTypeInfo, out result);
    public static ValueTask<TSelf> FromJsonAsync( Stream stream, CancellationToken token = default ) => JsonModel.FromJsonAsync(stream, TSelf.JsonTypeInfo, token);


    public TSelf WithAdditionalData( IJsonModel value ) => WithAdditionalData(value.AdditionalData);
    public virtual TSelf WithAdditionalData( IReadOnlyDictionary<string, JsonElement>? additionalData )
    {
        _additionalData = Json.Merge(_additionalData, additionalData);
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
