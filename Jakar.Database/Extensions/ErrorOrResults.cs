// Jakar.Extensions :: Jakar.Database
// 03/06/2023  12:59 AM


using Status = Jakar.Extensions.Status;



namespace Jakar.Database;


public static class ErrorOrResults
{
    public static NotFound               NotFound            { get; } = TypedResults.NotFound();
    public static UnauthorizedHttpResult Unauthorized        { get; } = TypedResults.Unauthorized();
    public static BadRequest             BadRequest          { get; } = TypedResults.BadRequest();
    public static Conflict               Conflict            { get; } = TypedResults.Conflict();
    public static NoContent              NoContent           { get; } = TypedResults.NoContent();
    public static Ok                     Ok                  { get; } = TypedResults.Ok();
    public static UnprocessableEntity    UnprocessableEntity { get; } = TypedResults.UnprocessableEntity();


    public static JsonResult<T>       ToJsonResult<T>( this T       value, Status status = Status.Ok ) => new(value, status.AsInt());
    public static JsonResult<Error[]> ToJsonResult( this    Error   value ) => new([value], value.GetStatus());
    public static JsonResult<Error[]> ToJsonResult( this    Error[] value ) => new(value, value.GetStatus());


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsAuthorized( this ClaimsPrincipal principal, RecordID<UserRecord> id ) => principal.IsAuthorized( id.Value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsAuthorized( this ClaimsIdentity  principal, RecordID<UserRecord> id ) => principal.IsAuthorized( id.Value );


    public static Status GetStatus( this Error error ) => error.StatusCode ?? Status.Ok;


    public static SerializableError? ToSerializableError<T>( this ErrorOrResult<T> result ) => result.HasErrors
                                                                                                   ? new SerializableError( result.Errors.ToModelStateDictionary() )
                                                                                                   : null;
    public static ModelStateDictionary? ToModelStateDictionary<T>( this ErrorOrResult<T> result ) => result.HasErrors
                                                                                                         ? result.Errors.ToModelStateDictionary()
                                                                                                         : null;
    public static SerializableError ToSerializableError( this Error[] errors ) => new(errors.ToModelStateDictionary());
    public static ModelStateDictionary ToModelStateDictionary( this Error[] errors )
    {
        ModelStateDictionary state = new();
        foreach ( Error error in errors.AsSpan() ) { error.ToModelStateDictionary( state ); }

        return state;
    }


    public static ModelStateDictionary ToModelStateDictionary( this Error error )
    {
        ModelStateDictionary state = new();
        error.ToModelStateDictionary( state );
        return state;
    }
    public static void ToModelStateDictionary( this Error error, in ModelStateDictionary state )
    {
        foreach ( string? e in error.Errors ) { state.TryAddModelError( nameof(Error.Errors), e ?? NULL ); }

        state.TryAddModelError( nameof(Error.Detail),     error.Detail                 ?? NULL );
        state.TryAddModelError( nameof(Error.Type),       error.Type                   ?? NULL );
        state.TryAddModelError( nameof(Error.Title),      error.Title                  ?? NULL );
        state.TryAddModelError( nameof(Error.StatusCode), error.StatusCode?.ToString() ?? NULL );
        state.TryAddModelError( nameof(Error.Instance),   error.Instance               ?? NULL );
    }


    public static async Task<Results<JsonResult<T>, JsonResult<Error[]>>>      ToResult<T>( this Task<ErrorOrResult<T>>      result ) => (await result.ConfigureAwait( false )).ToResult();
    public static async ValueTask<Results<JsonResult<T>, JsonResult<Error[]>>> ToResult<T>( this ValueTask<ErrorOrResult<T>> result ) => (await result.ConfigureAwait( false )).ToResult();
    public static       JsonResult<Error>                                      ToResult( this    Error                       error )  => new(error, (int)(error.StatusCode ?? Status.Ok));
    public static       JsonResult<Error[]>                                    ToResult( this    Error[]                     error )  => new(error, (int)error.GetStatus());
    public static Results<JsonResult<T>, JsonResult<Error[]>> ToResult<T>( this ErrorOrResult<T> result ) => result.TryGetValue( out T? value, out Error[]? errors )
                                                                                                                 ? new JsonResult<T>( value, (int)errors.GetStatus() )
                                                                                                                 : errors.ToResult();


    public static ActionResult<T> ToActionResult<T>( this ErrorOrResult<T> result ) => result.TryGetValue( out T? value, out Error[]? errors )
                                                                                           ? new ObjectResult( value ) { StatusCode  = (int)errors.GetStatus() }
                                                                                           : new ObjectResult( errors ) { StatusCode = (int)errors.GetStatus() };
    public static       ObjectResult               ToActionResult( this    Error                       error )  => new(error) { StatusCode = (int)(error.StatusCode ?? Status.Ok) };
    public static       ObjectResult               ToActionResult( this    Error[]                     error )  => new(error) { StatusCode = (int)error.GetStatus() };
    public static async Task<ActionResult<T>>      ToActionResult<T>( this Task<ErrorOrResult<T>>      result ) => (await result.ConfigureAwait( false )).ToActionResult();
    public static async ValueTask<ActionResult<T>> ToActionResult<T>( this ValueTask<ErrorOrResult<T>> result ) => (await result.ConfigureAwait( false )).ToActionResult();
}
