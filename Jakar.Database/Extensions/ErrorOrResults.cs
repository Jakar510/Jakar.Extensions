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


    public static JsonResult<T>      ToJsonResult<T>( this ref readonly T      value, Status status = Status.Ok ) => new(value, status.AsInt());
    public static JsonResult<Errors> ToJsonResult( this ref readonly    Error  value ) => new(Errors.Create( [value] ), value.GetStatus());
    public static JsonResult<Errors> ToJsonResult( this ref readonly    Errors value ) => new(value, value.GetStatus());


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsAuthorized( this ClaimsPrincipal principal, RecordID<UserRecord> id ) => principal.IsAuthorized( id.value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsAuthorized( this ClaimsIdentity  principal, RecordID<UserRecord> id ) => principal.IsAuthorized( id.value );


    public static Status GetStatus( this Error error ) => error.StatusCode ?? Status.Ok;


    public static SerializableError? ToSerializableError<T>( this ref readonly ErrorOrResult<T> result ) => result.TryGetValue( out Errors errors )
                                                                                                                ? new SerializableError( errors.ToModelStateDictionary() )
                                                                                                                : null;
    public static ModelStateDictionary? ToModelStateDictionary<T>( this ref readonly ErrorOrResult<T> result ) => result.TryGetValue( out Errors errors )
                                                                                                                      ? errors.ToModelStateDictionary()
                                                                                                                      : null;
    public static SerializableError ToSerializableError( this ref readonly Errors errors ) => new(errors.ToModelStateDictionary());
    public static ModelStateDictionary ToModelStateDictionary( this ref readonly Errors errors )
    {
        ModelStateDictionary state = new();
        foreach ( Error error in errors.Details.AsSpan() ) { error.ToModelStateDictionary( state ); }

        return state;
    }
    public static ModelStateDictionary? ToModelStateDictionary( [NotNullIfNotNull( nameof(errors) )] this ref readonly Errors? errors )
    {
        return errors.HasValue
                   ? errors.ToModelStateDictionary()
                   : null;
    }


    public static ModelStateDictionary ToModelStateDictionary( this ref readonly Error error )
    {
        ModelStateDictionary state = new();
        error.ToModelStateDictionary( state );
        return state;
    }
    public static void ToModelStateDictionary( this ref readonly Error error, in ModelStateDictionary state )
    {
        foreach ( string? e in error.Errors ) { state.TryAddModelError( nameof(Error.Errors), e ?? NULL ); }

        state.TryAddModelError( nameof(Error.Detail),     error.Detail                 ?? NULL );
        state.TryAddModelError( nameof(Error.Type),       error.Type                   ?? NULL );
        state.TryAddModelError( nameof(Error.Title),      error.Title                  ?? NULL );
        state.TryAddModelError( nameof(Error.StatusCode), error.StatusCode?.ToString() ?? NULL );
        state.TryAddModelError( nameof(Error.Instance),   error.Instance               ?? NULL );
    }


    public static async Task<Results<JsonResult<T>, JsonResult<Errors>>>      ToResult<T>( this           Task<ErrorOrResult<T>>      result ) => (await result.ConfigureAwait( false )).ToResult();
    public static async ValueTask<Results<JsonResult<T>, JsonResult<Errors>>> ToResult<T>( this           ValueTask<ErrorOrResult<T>> result ) => (await result.ConfigureAwait( false )).ToResult();
    public static       JsonResult<Error>                                     ToResult( this ref readonly Error                       error )  => new(error, (int)(error.StatusCode ?? Status.Ok));
    public static       JsonResult<Errors>                                    ToResult( this ref readonly Errors                      error )  => new(error, (int)error.GetStatus());
    public static Results<JsonResult<T>, JsonResult<Errors>> ToResult<T>( this ref readonly ErrorOrResult<T> result ) => result.TryGetValue( out T? value, out Errors errors )
                                                                                                                             ? new JsonResult<T>( value, Errors.GetStatus( errors ) )
                                                                                                                             : errors.ToResult();


    public static ActionResult<T> ToActionResult<T>( this ref readonly ErrorOrResult<T> result ) => result.TryGetValue( out T? value, out Errors errors )
                                                                                                        ? new ObjectResult( value ) { StatusCode  = (int)errors.GetStatus() }
                                                                                                        : new ObjectResult( errors ) { StatusCode = (int)errors.GetStatus() };
    public static       ObjectResult               ToActionResult( this ref readonly Error                       error )  => new(error) { StatusCode = (int)(error.StatusCode ?? Status.Ok) };
    public static       ObjectResult               ToActionResult( this ref readonly Errors                      error )  => new(error) { StatusCode = (int)error.GetStatus() };
    public static async Task<ActionResult<T>>      ToActionResult<T>( this           Task<ErrorOrResult<T>>      result ) => (await result.ConfigureAwait( false )).ToActionResult();
    public static async ValueTask<ActionResult<T>> ToActionResult<T>( this           ValueTask<ErrorOrResult<T>> result ) => (await result.ConfigureAwait( false )).ToActionResult();
}
