// Jakar.Extensions :: Jakar.Extensions
// 04/23/2024  11:04

using Microsoft.Extensions.Primitives;



namespace Jakar.Extensions;


[Serializable, DefaultValue( nameof(Empty) )]
public readonly record struct Alert()
{
    public static readonly Alert Empty = new(null);


    public bool      IsValid    => CheckIsValid( Title, Message );
    public bool      IsNotValid => CheckIsValid( Title, Message ) is false;
    public string?   Title      { get; init; }
    public string?   Message    { get; init; }
    public TimeSpan? TTL        { get; init; }


    public Alert( string? title, string? message, double ttlSeconds ) : this( title, message, TimeSpan.FromSeconds( ttlSeconds ) ) { }
    public Alert( string? title, string? message = null, TimeSpan? ttl = null ) : this()
    {
        Title   = title;
        Message = message;
        TTL     = ttl;
    }
    public static bool CheckIsValid( string? title, string? message ) => string.IsNullOrWhiteSpace( title ) is false || string.IsNullOrWhiteSpace( message ) is false;

    public static implicit operator Alert( string? value ) => new(value);


    [Pure] public Error ToError() => new(null, null, Title, Message, null, StringValues.Empty);
}



[Serializable, DefaultValue( nameof(Empty) )]
[method: JsonConstructor]
public sealed record Errors()
{
    private static readonly Error[] _details = [];
    public static readonly Errors Empty = new()
                                          {
                                              Alert   = null,
                                              Details = _details
                                          };


    [JsonRequired] public required Alert?  Alert       { get; init; }
    [JsonRequired] public required Error[] Details     { get; init; }
    public                         string  Description => Details.GetMessage();
    public                         bool    IsValid     => Alert?.IsValid is true || ReferenceEquals( Details, _details ) is false && Details.Length > 0;


    public static Errors Create( params Error[]? details ) => Create( null,                                       details );
    public static Errors Create( Error           details ) => Create( new Alert( details.Title, details.Detail ), details );
    public static Errors Create( Alert           details ) => Create( details,                                    details.ToError() );
    public static Errors Create( Alert? alert, params Error[]? details ) => new()
                                                                            {
                                                                                Alert   = alert,
                                                                                Details = details ?? _details
                                                                            };


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public        Status GetStatus()                 => Details.GetStatus( Status.Ok );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Status GetStatus( Errors? errors ) => errors?.GetStatus() ?? Status.Ok;


    public static implicit operator ReadOnlySpan<Error>( Errors   result )  => result.Details;
    public static implicit operator ReadOnlyMemory<Error>( Errors result )  => result.Details;
    public static implicit operator Errors( string                title )   => Create( new Alert( title ) );
    public static implicit operator Errors( Alert                 details ) => Create( details );
    public static implicit operator Errors( Error                 details ) => Create( details );
    public static implicit operator Errors( Error[]               details ) => Create( null, details );
    public static implicit operator Errors( List<Error>           details ) => Create( details.ToArray() );
    public static implicit operator Errors( ReadOnlySpan<Error>   details ) => Create( details.ToArray() );
}
