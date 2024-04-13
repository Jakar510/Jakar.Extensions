// Jakar.Extensions :: Jakar.Database
// 03/06/2023  12:59 AM


namespace Jakar.Database;


public readonly record struct Error( Status Status, ProblemDetails? Details = default, ModelStateDictionary? State = default, object? Value = default, string[]? Errors = default )
{
    public Error( Status status, object?              value ) : this( status, Value: value ) { }
    public Error( Status status, ModelStateDictionary value ) : this( status, State: value ) { }
    public Error( Status status, params string[]      errors ) : this( status, Errors: errors ) { }

    public static implicit operator Error( Status result ) => new(result);


    public ModelStateDictionary GetState()
    {
        const string         NULL  = "null";
        ModelStateDictionary state = State ?? new ModelStateDictionary();

        if ( Errors is not null )
        {
            foreach ( string error in Errors ) { state.TryAddModelError( nameof(Errors), error ); }
        }

        if ( Details is not null )
        {
            state.TryAddModelError( nameof(ProblemDetails.Detail),     Details.Detail             ?? NULL );
            state.TryAddModelError( nameof(ProblemDetails.Type),       Details.Type               ?? NULL );
            state.TryAddModelError( nameof(ProblemDetails.Title),      Details.Title              ?? NULL );
            state.TryAddModelError( nameof(ProblemDetails.Status),     Details.Status?.ToString() ?? NULL );
            state.TryAddModelError( nameof(ProblemDetails.Instance),   Details.Instance           ?? NULL );
            state.TryAddModelError( nameof(ProblemDetails.Extensions), Details.Extensions.ToJson() );
        }

        if ( Value is not IEnumerable<string> enumerable ) { return state; }

        string name = Value.GetType().Name;

        foreach ( string error in enumerable ) { state.TryAddModelError( name, error ); }

        return state;
    }


    private object? GetResult() => State is not null
                                       ? new SerializableError( State )
                                       : Errors is not null
                                           ? new SerializableError { [nameof(Errors)] = Errors }
                                           : Details ?? Value;
    public ActionResult ToActionResult() => new ObjectResult( GetResult() ) { StatusCode = (int)Status };
    public IResult ToResult()
    {
        object? result = GetResult();

        return result switch
               {
                   null                                 => Results.StatusCode( (int)Status ),
                   IDictionary<string, string[]> errors => Results.ValidationProblem( errors ),
                   ProblemDetails details               => Results.Problem( details ),
                   SerializableError serializableError  => new JsonResult<SerializableError>( serializableError, (int)Status ),
                   _                                    => new JsonResult<object>( result, (int)Status )
               };
    }
}



public readonly record struct Error<T>( Status Status, ProblemDetails? Details = default, ModelStateDictionary? State = default, T? Value = default, string[]? Errors = default )
{
    public Error( Status status, T?                   value ) : this( status, Value: value ) { }
    public Error( Status status, ModelStateDictionary value ) : this( status, State: value ) { }
    public Error( Status status, params string[]      errors ) : this( status, Errors: errors ) { }

    public static implicit operator Error<T>( Status result ) => new(result);


    public ModelStateDictionary GetState()
    {
        const string         NULL  = "null";
        ModelStateDictionary state = State ?? new ModelStateDictionary();

        if ( Errors is not null )
        {
            foreach ( string error in Errors ) { state.TryAddModelError( nameof(Errors), error ); }
        }

        if ( Details is not null )
        {
            state.TryAddModelError( nameof(ProblemDetails.Detail),     Details.Detail             ?? NULL );
            state.TryAddModelError( nameof(ProblemDetails.Type),       Details.Type               ?? NULL );
            state.TryAddModelError( nameof(ProblemDetails.Title),      Details.Title              ?? NULL );
            state.TryAddModelError( nameof(ProblemDetails.Status),     Details.Status?.ToString() ?? NULL );
            state.TryAddModelError( nameof(ProblemDetails.Instance),   Details.Instance           ?? NULL );
            state.TryAddModelError( nameof(ProblemDetails.Extensions), Details.Extensions.ToJson() );
        }

        if ( Value is not IEnumerable<string> enumerable ) { return state; }

        string name = Value.GetType().Name;

        foreach ( string error in enumerable ) { state.TryAddModelError( name, error ); }

        return state;
    }


    private object? GetResult() => State is not null
                                       ? new SerializableError( State )
                                       : Errors is not null
                                           ? new SerializableError { [nameof(Errors)] = Errors }
                                           : Details is not null
                                               ? Details
                                               : Value;
    public ActionResult ToActionResult() => new ObjectResult( GetResult() ) { StatusCode = (int)Status };
    public IResult ToResult()
    {
        object? result = GetResult();

        return result switch
               {
                   null                                 => Results.StatusCode( (int)Status ),
                   IDictionary<string, string[]> errors => Results.ValidationProblem( errors ),
                   ProblemDetails details               => Results.Problem( details ),
                   SerializableError serializableError  => new JsonResult<SerializableError>( serializableError, (int)Status ),
                   T t                                  => new JsonResult<T>( t, (int)Status ),
                   _                                    => throw new ExpectedValueTypeException( nameof(result), result, typeof(T), typeof(ProblemDetails), typeof(SerializableError), typeof(IDictionary<string, string[]>) )
               };
    }
}



public static class ErrorExtensions
{
    public static async Task<IResult>      MatchResult<T>( this Task<OneOf<T, Error>>               value ) => (await value).MatchResult();
    public static async ValueTask<IResult> MatchResult<T>( this ValueTask<OneOf<T, Error>>          value ) => (await value).MatchResult();
    public static       IResult            MatchResult<T>( this OneOf<T, Error>                     value ) => value.Match( static x => new JsonResult<T>( x ), static x => x.ToResult() );
    public static async Task<IResult>      MatchResult<T>( this Task<OneOf<T, List<T>, Error>>      value ) => (await value).MatchResult();
    public static async ValueTask<IResult> MatchResult<T>( this ValueTask<OneOf<T, List<T>, Error>> value ) => (await value).MatchResult();
    public static       IResult            MatchResult<T>( this OneOf<T, List<T>, Error>            value ) => value.Match( static x => new JsonResult<T>( x ), static x => new JsonResult<List<T>>( x ), static x => x.ToResult() );
    public static async Task<IResult>      MatchResult<T>( this Task<OneOf<T, T[], Error>>          value ) => (await value).MatchResult();
    public static async ValueTask<IResult> MatchResult<T>( this ValueTask<OneOf<T, T[], Error>>     value ) => (await value).MatchResult();
    public static       IResult            MatchResult<T>( this OneOf<T, T[], Error>                value ) => value.Match( static x => new JsonResult<T>( x ), static x => new JsonResult<T[]>( x ), static x => x.ToResult() );


    public static async Task<ActionResult<T>>      MatchActionResult<T>( this Task<OneOf<T, Error>>               value ) => (await value).MatchActionResult();
    public static async ValueTask<ActionResult<T>> MatchActionResult<T>( this ValueTask<OneOf<T, Error>>          value ) => (await value).MatchActionResult();
    public static       ActionResult<T>            MatchActionResult<T>( this OneOf<T, Error>                     value ) => value.Match<ActionResult<T>>( static x => x, static x => x.ToActionResult() );
    public static       ActionResult               MatchActionResult( this    OneOf<ActionResult, Error>          value ) => value.Match( static x => x, static x => x.ToActionResult() );
    public static async Task<ActionResult<T>>      MatchActionResult<T>( this Task<OneOf<T, List<T>, Error>>      value ) => (await value).MatchActionResult();
    public static async ValueTask<ActionResult<T>> MatchActionResult<T>( this ValueTask<OneOf<T, List<T>, Error>> value ) => (await value).MatchActionResult();
    public static       ActionResult               MatchActionResult<T>( this OneOf<T, List<T>, Error>            value ) => value.Match( static x => new ObjectResult( x ), static x => new ObjectResult( x ), static x => x.ToActionResult() );
    public static async Task<ActionResult<T>>      MatchActionResult<T>( this Task<OneOf<T, T[], Error>>          value ) => (await value).MatchActionResult();
    public static async ValueTask<ActionResult<T>> MatchActionResult<T>( this ValueTask<OneOf<T, T[], Error>>     value ) => (await value).MatchActionResult();
    public static       ActionResult               MatchActionResult<T>( this OneOf<T, T[], Error>                value ) => value.Match( static x => new ObjectResult( x ), static x => new ObjectResult( x ), static x => x.ToActionResult() );
}
