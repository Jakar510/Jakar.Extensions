namespace Jakar.Extensions;


[Serializable]
public class BaseClass
{
    public const int    ANSI_CAPACITY    = BaseRecord.ANSI_CAPACITY;
    public const int    BINARY_CAPACITY  = BaseRecord.BINARY_CAPACITY;
    public const string EMPTY            = BaseRecord.EMPTY;
    public const int    MAX_STRING_SIZE  = BaseRecord.MAX_STRING_SIZE; // 1GB
    public const string NULL             = BaseRecord.NULL;
    public const int    UNICODE_CAPACITY = BaseRecord.UNICODE_CAPACITY;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected static void ClearAndDispose<TValue>( ref TValue? field )
        where TValue : IDisposable => Disposables.CastAndDispose( ref field );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    protected static ValueTask ClearAndDisposeAsync<TValue>( ref TValue? resource )
        where TValue : class, IDisposable => Disposables.CastAndDisposeAsync( ref resource );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] protected static ValueTask CastAndDispose( IDisposable? resource ) => Disposables.CastAndDisposeAsync( resource );
}



[Serializable]
public abstract class BaseClass<TClass> : BaseClass, IEquatable<TClass>, IComparable<TClass>, IComparable, IParsable<TClass>
    where TClass : BaseClass<TClass>, IEqualComparable<TClass>
{
    public static Sorter<TClass> Sorter { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Sorter<TClass>.Default; }


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
    public override bool Equals( object? other ) => ReferenceEquals( this, other ) || other is TClass x && Equals( x );
    public override int  GetHashCode()           => RuntimeHelpers.GetHashCode( this );


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
