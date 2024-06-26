﻿// Jakar.Extensions :: Jakar.Database
// 03/06/2023  12:59 AM


namespace Jakar.Database;


public static class Errors
{
    public static Status GetStatus( this IEnumerable<Error>? errors )                            => errors?.Max( GetStatus ) ?? Status.Ok;
    public static Status GetStatus( this Error[]?            errors, Status status = Status.Ok ) => new ReadOnlySpan<Error>( errors ).GetStatus( status );
    public static Status GetStatus( this ReadOnlySpan<Error> errors, Status status = Status.Ok )
    {
        if ( errors.IsEmpty ) { return status; }

        foreach ( var error in errors )
        {
            Status? code = error.StatusCode;
            if ( code > status ) { status = code.Value; }
        }

        return status;
    }
    private static Status GetStatus( this Error error ) => error.StatusCode ?? Status.Ok;


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


    public static async Task<IResult>       ToResult<T>( this Task<ErrorOrResult<T>>           result ) => (await result.ConfigureAwait( false )).ToResult();
    public static async ValueTask<IResult>  ToResult<T>( this ValueTask<ErrorOrResult<T>>      result ) => (await result.ConfigureAwait( false )).ToResult();
    public static async Task<IResult>       ToResult<T>( this Task<OneOf<T, Error>>      result ) => (await result.ConfigureAwait( false )).ToResult();
    public static async ValueTask<IResult>  ToResult<T>( this ValueTask<OneOf<T, Error>> result ) => (await result.ConfigureAwait( false )).ToResult();
    public static       IResult             ToResult<T>( this OneOf<T, Error>            result ) => result.Match<IResult>( static x => new JsonResult<T>( x ), static x => x.ToResult() );
    public static       JsonResult<Error>   ToResult( this    Error                      error )  => new(error, (int)(error.StatusCode ?? Status.Ok));
    public static       JsonResult<Error[]> ToResult( this    Error[]                    error )  => new(error, (int)error.GetStatus());
    public static IResult ToResult<T>( this ErrorOrResult<T> result ) => result.TryGetValue( out T? t, out Error[]? errors )
                                                                       ? new JsonResult<T>( t, (int)errors.GetStatus() )
                                                                       : new JsonResult<Error[]>( errors, (int)errors.GetStatus() );


    public static ActionResult<T> ToActionResult<T>( this ErrorOrResult<T> result ) => result.TryGetValue( out T? t, out Error[]? errors )
                                                                                     ? new ObjectResult( t ) { StatusCode      = (int)errors.GetStatus() }
                                                                                     : new ObjectResult( errors ) { StatusCode = (int)errors.GetStatus() };
    public static       ObjectResult               ToActionResult( this    Error                      error )  => new(error) { StatusCode = (int)(error.StatusCode ?? Status.Ok) };
    public static       ObjectResult               ToActionResult( this    Error[]                    error )  => new(error) { StatusCode = (int)error.GetStatus() };
    public static async Task<ActionResult<T>>      ToActionResult<T>( this Task<ErrorOrResult<T>>           result ) => (await result.ConfigureAwait( false )).ToActionResult();
    public static async ValueTask<ActionResult<T>> ToActionResult<T>( this ValueTask<ErrorOrResult<T>>      result ) => (await result.ConfigureAwait( false )).ToActionResult();
    public static async Task<ActionResult<T>>      ToActionResult<T>( this Task<OneOf<T, Error>>      result ) => (await result.ConfigureAwait( false )).ToActionResult();
    public static async ValueTask<ActionResult<T>> ToActionResult<T>( this ValueTask<OneOf<T, Error>> result ) => (await result.ConfigureAwait( false )).ToActionResult();
    public static       ActionResult<T>            ToActionResult<T>( this OneOf<T, Error>            result ) => result.Match<ActionResult<T>>( static x => x, static x => x.ToActionResult() );
}
