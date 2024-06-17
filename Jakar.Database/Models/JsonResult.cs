// Jakar.Extensions :: Jakar.Database
// 04/10/2024  20:04

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.WebUtilities;



namespace Jakar.Database;


public sealed class JsonResult<TValue> : ActionResult, IResult, IEndpointMetadataProvider, IStatusCodeHttpResult, IValueHttpResult, IValueHttpResult<TValue>
{
    // ReSharper disable once StaticMemberInGenericType
    public static readonly Dictionary<int, (string Type, string Title)> Defaults = new()
                                                                                   {
                                                                                       [400] = ("https://tools.ietf.org/html/rfc9110#section-15.5.1", "Bad Request"),
                                                                                       [401] = ("https://tools.ietf.org/html/rfc9110#section-15.5.2", "Unauthorized"),
                                                                                       [403] = ("https://tools.ietf.org/html/rfc9110#section-15.5.4", "Forbidden"),
                                                                                       [404] = ("https://tools.ietf.org/html/rfc9110#section-15.5.5", "Not Found"),
                                                                                       [405] = ("https://tools.ietf.org/html/rfc9110#section-15.5.6", "Method Not Allowed"),
                                                                                       [406] = ("https://tools.ietf.org/html/rfc9110#section-15.5.7", "Not Acceptable"),
                                                                                       [408] = ("https://tools.ietf.org/html/rfc9110#section-15.5.9", "Request Timeout"),
                                                                                       [409] = ("https://tools.ietf.org/html/rfc9110#section-15.5.10", "Conflict"),
                                                                                       [412] = ("https://tools.ietf.org/html/rfc9110#section-15.5.13", "Precondition Failed"),
                                                                                       [415] = ("https://tools.ietf.org/html/rfc9110#section-15.5.16", "Unsupported Media Type"),
                                                                                       [422] = ("https://tools.ietf.org/html/rfc4918#section-11.2", "Unprocessable Entity"),
                                                                                       [426] = ("https://tools.ietf.org/html/rfc9110#section-15.5.22", "Upgrade Required"),
                                                                                       [500] = ("https://tools.ietf.org/html/rfc9110#section-15.6.1", "An error occurred while processing your request."),
                                                                                       [502] = ("https://tools.ietf.org/html/rfc9110#section-15.6.3", "Bad Gateway"),
                                                                                       [503] = ("https://tools.ietf.org/html/rfc9110#section-15.6.4", "Service Unavailable"),
                                                                                       [504] = ("https://tools.ietf.org/html/rfc9110#section-15.6.5", "Gateway Timeout")
                                                                                   };
    public int                 StatusCode { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    int? IStatusCodeHttpResult.StatusCode => StatusCode;
    public TValue?             Value      { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; }
    object? IValueHttpResult.  Value      => Value;


    public JsonResult( TValue? value, Status status ) : this( value, status.AsInt() ) { }
    public JsonResult( TValue? value, int status )
    {
        Value      = value;
        StatusCode = status;
        if ( value is ProblemDetails details ) { Apply( details, StatusCode ); }
    }
    public static JsonResult<TValue> Create( TValue? value, Status status = Status.Ok ) => new(value, status);


    public static implicit operator JsonResult<TValue>( Status                      status ) => new(default, status);
    public static implicit operator JsonResult<TValue>( TValue?                     value )  => new(value, Status.Ok);
    public static implicit operator JsonResult<TValue>( Ok<TValue>                  result ) => new(result.Value, result.StatusCode);
    public static implicit operator JsonResult<TValue>( Created<TValue>             result ) => new(result.Value, result.StatusCode);
    public static implicit operator JsonResult<TValue>( CreatedAtRoute<TValue>      result ) => new(result.Value, result.StatusCode);
    public static implicit operator JsonResult<TValue>( Accepted<TValue>            result ) => new(result.Value, result.StatusCode);
    public static implicit operator JsonResult<TValue>( AcceptedAtRoute<TValue>     result ) => new(result.Value, result.StatusCode);
    public static implicit operator JsonResult<TValue>( Conflict<TValue>            result ) => new(result.Value, result.StatusCode);
    public static implicit operator JsonResult<TValue>( JsonHttpResult<TValue>      result ) => new(result.Value, result.StatusCode ?? 200);
    public static implicit operator JsonResult<TValue>( UnprocessableEntity<TValue> result ) => new(result.Value, result.StatusCode);


    public static void Apply( ProblemDetails details, int? statusCode )
    {
        if ( details.Status.HasValue is false )
        {
            details.Status = statusCode ??
                             (details is HttpValidationProblemDetails
                                  ? 400
                                  : 500);
        }

        int valueOrDefault = details.Status.GetValueOrDefault();

        if ( Defaults.TryGetValue( valueOrDefault, out (string Type, string Title) tuple ) )
        {
            details.Title ??= tuple.Title;
            details.Type  ??= tuple.Type;
        }
        else
        {
            if ( details.Title is not null ) { return; }

            string reasonPhrase = ReasonPhrases.GetReasonPhrase( valueOrDefault );
            if ( string.IsNullOrEmpty( reasonPhrase ) ) { return; }

            details.Title = reasonPhrase;
        }
    }


    public override Task ExecuteResultAsync( ActionContext context )
    {
        if ( Value is ModelStateDictionary dictionary ) { context.ModelState.Merge( dictionary ); }

        return ExecuteAsync( context.HttpContext );
    }
    public async Task ExecuteAsync( HttpContext httpContext )
    {
        ArgumentNullException.ThrowIfNull( httpContext );
        httpContext.Response.StatusCode = StatusCode;
        string json = Value?.ToPrettyJson() ?? string.Empty;
        await httpContext.Response.WriteAsync( json, Encoding.Default, CancellationToken.None );
    }
    static void IEndpointMetadataProvider.PopulateMetadata( MethodInfo method, EndpointBuilder builder )
    {
        ArgumentNullException.ThrowIfNull( method );
        ArgumentNullException.ThrowIfNull( builder );
        foreach ( Status status in Enum.GetValues<Status>() ) { builder.Metadata.Add( new ProducesResponseTypeMetadata( status.AsInt(), typeof(TValue), ["application/json"] ) ); }
    }
}
