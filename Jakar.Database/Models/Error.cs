// Jakar.Extensions :: Jakar.Database
// 03/06/2023  12:59 AM


namespace Jakar.Database;


public readonly record struct Error( Status Status, ProblemDetails? Details = default, ModelStateDictionary? State = default, object? Value = default, string[]? Errors = default )
{
    public Error( Status status, object?              value ) : this( status, Value: value ) { }
    public Error( Status status, ModelStateDictionary value ) : this( status, State: value ) { }
    public Error( Status status, params string[]      errors ) : this( status, Errors: errors ) { }

    public static implicit operator Error( Status result ) => new(result);


    private object? GetResult() => State is not null
                                       ? new SerializableError( State )
                                       : Errors is not null
                                           ? new SerializableError
                                             {
                                                 [nameof(Errors)] = Errors,
                                             }
                                           : Details ?? Value;
    public ActionResult ToActionResult() => new ObjectResult( GetResult() )
                                            {
                                                StatusCode = (int)Status,
                                            };
}



public static class ErrorExtensions
{
    public static async Task<ActionResult<T>> Match<T>( this      Task<OneOf<T, Error>>      value ) => (await value).Match();
    public static async ValueTask<ActionResult<T>> Match<T>( this ValueTask<OneOf<T, Error>> value ) => (await value).Match();
    public static ActionResult<T> Match<T>( this                  OneOf<T, Error>            value ) => value.Match<ActionResult<T>>( x => x, x => x.ToActionResult() );
    public static ActionResult Match( this                        OneOf<ActionResult, Error> value ) => value.Match<ActionResult>( x => x, x => x.ToActionResult() );


    public static async Task<ActionResult<T>> Match<T>( this      Task<OneOf<T, List<T>, Error>>      value ) => (await value).Match();
    public static async ValueTask<ActionResult<T>> Match<T>( this ValueTask<OneOf<T, List<T>, Error>> value ) => (await value).Match();
    public static ActionResult Match<T>( this OneOf<T, List<T>, Error> value ) =>
        value.Match( x => new ObjectResult( x )
                          {
                              StatusCode = (int)Status.Created
                          },
                     x => new ObjectResult( x )
                          {
                              StatusCode = (int)Status.Created
                          },
                     x => x.ToActionResult() );


    public static async Task<ActionResult<T>> Match<T>( this      Task<OneOf<T, T[], Error>>      value ) => (await value).Match();
    public static async ValueTask<ActionResult<T>> Match<T>( this ValueTask<OneOf<T, T[], Error>> value ) => (await value).Match();
    public static ActionResult Match<T>( this OneOf<T, T[], Error> value ) =>
        value.Match( x => new ObjectResult( x )
                          {
                              StatusCode = (int)Status.Created
                          },
                     x => new ObjectResult( x )
                          {
                              StatusCode = (int)Status.Created
                          },
                     x => x.ToActionResult() );
}
