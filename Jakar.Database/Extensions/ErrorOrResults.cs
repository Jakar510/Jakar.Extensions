// Jakar.Extensions :: Jakar.Database
// 03/06/2023  12:59 AM


using Newtonsoft.Json.Linq;



namespace Jakar.Database;


public static class ErrorOrResults
{
    public static BadRequest             BadRequest          { get; } = TypedResults.BadRequest();
    public static Conflict               Conflict            { get; } = TypedResults.Conflict();
    public static NoContent              NoContent           { get; } = TypedResults.NoContent();
    public static NotFound               NotFound            { get; } = TypedResults.NotFound();
    public static Ok                     Ok                  { get; } = TypedResults.Ok();
    public static UnauthorizedHttpResult Unauthorized        { get; } = TypedResults.Unauthorized();
    public static UnprocessableEntity    UnprocessableEntity { get; } = TypedResults.UnprocessableEntity();


    public static JsonResult<TValue> ToJsonResult<TValue>( this TValue value, Status status = Status.Ok ) => JsonResult<TValue>.Create(value, status);
    public static JsonResult<Errors> ToJsonResult( this         Error  value ) => JsonResult<Errors>.Create(Errors.Create([value]), Errors.JsonTypeInfo, value.GetStatus());
    public static JsonResult<Errors> ToJsonResult( this         Errors value ) => JsonResult<Errors>.Create(value,                  Errors.JsonTypeInfo, value.GetStatus());


     public static bool IsAuthorized( this ClaimsPrincipal principal, RecordID<UserRecord> id ) => principal.IsAuthorized(id.Value);
     public static bool IsAuthorized( this ClaimsIdentity  principal, RecordID<UserRecord> id ) => principal.IsAuthorized(id.Value);


    public static Status GetStatus( this Error error ) => error.StatusCode ?? Status.Ok;


    public static SerializableError? ToSerializableError<TValue>( this ErrorOrResult<TValue> result ) =>
        result.TryGetValue(out Errors? errors)
            ? new SerializableError(errors.ToModelStateDictionary())
            : null;
    public static ModelStateDictionary? ToModelStateDictionary<TValue>( this ErrorOrResult<TValue> result ) =>
        result.TryGetValue(out Errors? errors)
            ? errors.ToModelStateDictionary()
            : null;
    public static SerializableError ToSerializableError( this Errors errors ) => new(errors.ToModelStateDictionary());


    public static ModelStateDictionary ToModelStateDictionary( this Errors errors )
    {
        ModelStateDictionary state = new();
        foreach ( Error error in errors.Details.AsSpan() ) { state.Add(error); }

        return state;
    }
    public static ModelStateDictionary ToModelStateDictionary( this Error error )
    {
        ModelStateDictionary state = new() { error };
        return state;
    }


    public static void Add( this ModelStateDictionary state, Errors errors )
    {
        foreach ( Error error in errors.Details.AsSpan() ) { state.Add(error); }
    }
    public static void Add( this ModelStateDictionary state, Error error )
    {
        if ( error.Errors is not null )
        {
            StringTags tags = error.Errors.Value;
            foreach ( string e in tags.Values ) { state.TryAddModelError(nameof(Error.Errors), e); }

            foreach ( Pair e in tags.Tags ) { state.TryAddModelError(e.Key, e.Value ?? NULL); }
        }

        state.TryAddModelError(nameof(Error.Detail),     error.Detail                 ?? NULL);
        state.TryAddModelError(nameof(Error.Type),       error.Type                   ?? NULL);
        state.TryAddModelError(nameof(Error.Title),      error.Title                  ?? NULL);
        state.TryAddModelError(nameof(Error.StatusCode), error.StatusCode?.ToString() ?? NULL);
        state.TryAddModelError(nameof(Error.Instance),   error.Instance               ?? NULL);
    }


    public static async Task<Results<JsonResult<TValue>, JsonResult<Errors>>> ToResult<TValue>( this Task<ErrorOrResult<TValue>> result )
    {
        ErrorOrResult<TValue> errorOrResult = await result.ConfigureAwait(false);
        return errorOrResult.ToResult();
    }
    public static async ValueTask<Results<JsonResult<TValue>, JsonResult<Errors>>> ToResult<TValue>( this ValueTask<ErrorOrResult<TValue>> result )
    {
        ErrorOrResult<TValue> errorOrResult = await result.ConfigureAwait(false);
        return errorOrResult.ToResult();
    }
    public static JsonResult<Errors> ToResult( this Error  value ) => JsonResult<Errors>.Create(Errors.Create([value]), Errors.JsonTypeInfo, value.GetStatus());
    public static JsonResult<Errors> ToResult( this Errors value ) => JsonResult<Errors>.Create(value,                  Errors.JsonTypeInfo, value.GetStatus());
    public static Results<JsonResult<TValue>, JsonResult<Errors>> ToResult<TValue>( this ErrorOrResult<TValue> result ) =>
        result.TryGetValue(out TValue? value, out Errors? errors)
            ? JsonResult<TValue>.Create(value, Errors.GetStatus(errors))
            : errors.ToResult();


    public static ActionResult<TValue> ToActionResult<TValue>( this ErrorOrResult<TValue> result ) => result.TryGetValue(out TValue? value, out Errors? errors)
                                                                                                          ? new ObjectResult(value) { StatusCode  = (int)Errors.GetStatus(errors) }
                                                                                                          : new ObjectResult(errors) { StatusCode = (int)errors.GetStatus() };
    public static ObjectResult ToActionResult( this Error  error ) => new(error) { StatusCode = (int)( error.StatusCode ?? Status.Ok ) };
    public static ObjectResult ToActionResult( this Errors error ) => new(error) { StatusCode = (int)error.GetStatus() };
    public static async Task<ActionResult<TValue>> ToActionResult<TValue>( this Task<ErrorOrResult<TValue>> result )
    {
        ErrorOrResult<TValue> errorOrResult = await result.ConfigureAwait(false);
        return errorOrResult.ToActionResult();
    }
    public static async ValueTask<ActionResult<TValue>> ToActionResult<TValue>( this ValueTask<ErrorOrResult<TValue>> result )
    {
        ErrorOrResult<TValue> errorOrResult = await result.ConfigureAwait(false);
        return errorOrResult.ToActionResult();
    }
}
