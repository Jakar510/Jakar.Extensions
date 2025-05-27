using System.Formats.Asn1;



namespace Jakar.Extensions;


[Serializable]
public record BaseRecord
{
    public const int    ANSI_CAPACITY    = 8000;
    public const int    BINARY_CAPACITY  = int.MaxValue;
    public const string EMPTY            = "";
    public const int    MAX_STRING_SIZE  = int.MaxValue;
    public const string NULL             = "null";
    public const int    UNICODE_CAPACITY = 4000;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected static void ClearAndDispose<TValue>( ref TValue? field )
        where TValue : IDisposable => Disposables.CastAndDispose( ref field );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected static ValueTask ClearAndDisposeAsync<TValue>( ref TValue? resource )
        where TValue : class, IDisposable => Disposables.CastAndDisposeAsync( ref resource );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] protected static ValueTask CastAndDispose( IDisposable? resource ) => Disposables.CastAndDisposeAsync( resource );
}



public abstract record BaseRecord<TClass> : BaseRecord, IEquatable<TClass>, IComparable<TClass>, IComparable, IParsable<TClass>
    where TClass : BaseRecord<TClass>, IComparisonOperators<TClass>
{
    public static Equalizer<TClass> Equalizer { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Equalizer<TClass>.Default; }
    public static Sorter<TClass>    Sorter    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Sorter<TClass>.Default; }


    public string ToJson()       => this.ToJson( Formatting.None );
    public string ToPrettyJson() => this.ToJson( Formatting.Indented );

    public abstract bool Equals( TClass?    other );
    public abstract int  CompareTo( TClass? other );

    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        return other is TClass t
                   ? CompareTo( t )
                   : throw new ExpectedValueTypeException( nameof(other), other, typeof(TClass) );
    }


    public static TClass Parse( [NotNullIfNotNull( nameof(json) )] string? json, IFormatProvider? provider )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace( json );
        return json.FromJson<TClass>();
    }
    public static bool TryParse( [NotNullWhen( true )] string? json, IFormatProvider? provider, [NotNullWhen( true )] out TClass? result )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        try
        {
            result = json?.FromJson<TClass>();
            return result is not null;
        }
        catch ( Exception e )
        {
            telemetrySpan.AddException( e );
            result = null;
            return false;
        }
    }
}



public abstract record BaseRecord<TClass, TID> : BaseRecord<TClass>, IUniqueID<TID>
    where TClass : BaseRecord<TClass, TID>, IComparisonOperators<TClass>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    private TID _id;


    public virtual TID ID { get => _id; init => _id = value; }


    protected BaseRecord() : base() { }
    protected BaseRecord( TID id ) => ID = id;


    protected bool SetID( TClass record ) => SetID( record.ID );
    protected bool SetID( TID id )
    {
        _id = id;
        return true;
    }
}
