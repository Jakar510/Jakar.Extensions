// Jakar.Extensions :: Jakar.Extensions
// 04/23/2024  11:04

using Microsoft.Extensions.Primitives;



namespace Jakar.Extensions;


[Serializable, DefaultValue( nameof(Empty) )]
public readonly struct Alert( string? title, string? message, TimeSpan? ttl = null )
{
    public static readonly Alert     Empty      = new(null, null);
    public readonly        bool      IsValid    = CheckIsValid( title, message );
    public readonly        bool      IsNotValid = CheckIsValid( title, message ) is false;
    public readonly        string?   Title      = title;
    public readonly        string?   Message    = message;
    public readonly        TimeSpan? TTL        = ttl;


    public Alert( string?                    title, string? message, double ttlSeconds ) : this( title, message, TimeSpan.FromSeconds( ttlSeconds ) ) { }
    public static bool CheckIsValid( string? title, string? message ) => string.IsNullOrWhiteSpace( title ) is false || string.IsNullOrWhiteSpace( message ) is false;
}



[Serializable, DefaultValue( nameof(Empty) )]
[method: JsonConstructor]
public readonly struct Errors( Alert? alert, params Error[]? details )
{
    public static readonly Errors   Empty       = new(null, null);
    public readonly        Alert?   Alert       = alert;
    public readonly        string?  Description = details?.GetMessage();
    public readonly        Error[]? Details     = details;


    public static Errors Create( Alert?      alert, params Error[]? details ) => new(alert, details);
    public static Errors Create( List<Error> details )                    => Create( null,  details );
    public static Errors Create( Alert?      alert, List<Error> details ) => Create( alert, details.ToArray() );
    public static Errors Create( Error       details ) => Create( new Alert( details.Title, details.Detail ), details );
    public static Errors Create( Alert       details ) => Create( details,                                    new Error( null, null, details.Title, details.Message, null, StringValues.Empty ) );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public        Status GetStatus()                 => Details?.GetStatus() ?? Status.Ok;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Status GetStatus( Errors? errors ) => errors?.GetStatus()  ?? Status.Ok;


    public static implicit operator ReadOnlySpan<Error>( Errors    result )  => result.Details;
    public static implicit operator ReadOnlyMemory<Error>( Errors  result )  => result.Details;
    public static implicit operator ReadOnlySpan<Error>( Errors?   result )  => result?.Details;
    public static implicit operator ReadOnlyMemory<Error>( Errors? result )  => result?.Details;
    public static implicit operator Errors( string                 value )   => Create( value );
    public static implicit operator Errors( Alert                  details ) => Create( details );
    public static implicit operator Errors( Error                  details ) => Create( details );
    public static implicit operator Errors( Error[]                details ) => Create( null, details );
    public static implicit operator Errors( List<Error>            details ) => Create( details );
}
