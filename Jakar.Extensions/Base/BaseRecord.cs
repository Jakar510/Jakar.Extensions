namespace Jakar.Extensions;


[Serializable]
public record BaseRecord
{
    public const int    ANSI_CAPACITY    = 8000;
    public const int    BINARY_CAPACITY  = int.MaxValue;
    public const int    MAX_STRING_SIZE  = int.MaxValue;
    public const int    UNICODE_CAPACITY = 4000;
    public const string EMPTY            = "";
    public const string NULL             = "null";
}



public abstract record BaseRecord<TRecord> : BaseRecord, IEquatable<TRecord>, IComparable<TRecord>, IComparable
    where TRecord : BaseRecord<TRecord>
{
    public static Equalizer<TRecord> Equalizer { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Equalizer<TRecord>.Default; }
    public static Sorter<TRecord>    Sorter    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Sorter<TRecord>.Default; }


    public static TRecord? FromJson( [NotNullIfNotNull( nameof(json) )] string? json ) => json?.FromJson<TRecord>();
    public        string   ToJson()                                                    => this.ToJson( Formatting.None );
    public        string   ToPrettyJson()                                              => this.ToJson( Formatting.Indented );


    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        return other is TRecord t
                   ? CompareTo( t )
                   : throw new ExpectedValueTypeException( nameof(other), other, typeof(TRecord) );
    }
    public abstract int  CompareTo( TRecord? other );
    public abstract bool Equals( TRecord?    other );
}



public abstract record BaseRecord<TRecord, TID> : BaseRecord<TRecord>, IUniqueID<TID>
    where TRecord : BaseRecord<TRecord, TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    private TID _id;


    public virtual TID ID { get => _id; init => _id = value; }


    protected BaseRecord() : base() { }
    protected BaseRecord( TID id ) => ID = id;


    protected bool SetID( TRecord record ) => SetID( record.ID );
    protected bool SetID( TID id )
    {
        _id = id;
        return true;
    }
}
