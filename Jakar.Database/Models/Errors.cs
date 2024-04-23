// Jakar.Extensions :: Jakar.Database
// 03/06/2023  12:59 AM


using Google.Protobuf.WellKnownTypes;
using static Jakar.Database.LoginResult;



namespace Jakar.Database;


public static class Errors
{
    public const string NULL = "null";

    public static Error[] ToErrors<T>( this PasswordValidator.Results results )
    {
        List<Error> errors = new(10) { Error.Validation( "Password Validation Failed" ) };

        if ( results.LengthPassed ) { errors.Add( Error.Validation( "Password not long enough" ) ); }

        if ( results.SpecialPassed ) { errors.Add( Error.Validation( "Password must contain a special character" ) ); }

        if ( results.NumericPassed ) { errors.Add( Error.Validation( "Password must contain a numeric character" ) ); }

        if ( results.LowerPassed ) { errors.Add( Error.Validation( "Password must contain a lower case character" ) ); }

        if ( results.UpperPassed ) { errors.Add( Error.Validation( "Password must contain a upper case character" ) ); }

        if ( results.BlockedPassed ) { errors.Add( Error.Validation( "Password cannot be a blocked password" ) ); }

        return errors.ToArray();
    }


    public static  Status GetStatus( this IEnumerable<Error>? errors ) => errors?.Max( GetStatus ) ?? Status.Ok;
    private static Status GetStatus( this Error               error )  => error.StatusCode         ?? Status.Ok;


    public static SerializableError? ToSerializableError<T>( this ErrorOr<T> result ) => result.HasErrors
                                                                                             ? new SerializableError( result.Errors.ToModelStateDictionary() )
                                                                                             : null;
    public static ModelStateDictionary? ToModelStateDictionary<T>( this ErrorOr<T> result ) => result.HasErrors
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


    public static async Task<IResult>       ToResult<T>( this Task<ErrorOr<T>>           result ) => (await result.ConfigureAwait( false )).ToResult();
    public static async ValueTask<IResult>  ToResult<T>( this ValueTask<ErrorOr<T>>      result ) => (await result.ConfigureAwait( false )).ToResult();
    public static async Task<IResult>       ToResult<T>( this Task<OneOf<T, Error>>      result ) => (await result.ConfigureAwait( false )).ToResult();
    public static async ValueTask<IResult>  ToResult<T>( this ValueTask<OneOf<T, Error>> result ) => (await result.ConfigureAwait( false )).ToResult();
    public static       IResult             ToResult<T>( this OneOf<T, Error>            result ) => result.Match<IResult>( static x => new JsonResult<T>( x ), static x => x.ToResult() );
    public static       JsonResult<Error>   ToResult( this    Error                      error )  => new(error, (int)(error.StatusCode ?? Status.Ok));
    public static       JsonResult<Error[]> ToResult( this    Error[]                    error )  => new(error, (int)error.GetStatus());
    public static IResult ToResult<T>( this ErrorOr<T> result ) => result.TryGetValue( out T? t, out Error[]? errors )
                                                                       ? new JsonResult<T>( t, (int)errors.GetStatus() )
                                                                       : new JsonResult<Error[]>( errors, (int)errors.GetStatus() );


    public static ActionResult<T> ToActionResult<T>( this ErrorOr<T> result ) => result.TryGetValue( out T? t, out Error[]? errors )
                                                                                     ? new ObjectResult( t ) { StatusCode      = (int)errors.GetStatus() }
                                                                                     : new ObjectResult( errors ) { StatusCode = (int)errors.GetStatus() };
    public static       ObjectResult               ToActionResult( this    Error                      error )  => new(error) { StatusCode = (int)(error.StatusCode ?? Status.Ok) };
    public static       ObjectResult               ToActionResult( this    Error[]                    error )  => new(error) { StatusCode = (int)error.GetStatus() };
    public static async Task<ActionResult<T>>      ToActionResult<T>( this Task<ErrorOr<T>>           result ) => (await result.ConfigureAwait( false )).ToActionResult();
    public static async ValueTask<ActionResult<T>> ToActionResult<T>( this ValueTask<ErrorOr<T>>      result ) => (await result.ConfigureAwait( false )).ToActionResult();
    public static async Task<ActionResult<T>>      ToActionResult<T>( this Task<OneOf<T, Error>>      result ) => (await result.ConfigureAwait( false )).ToActionResult();
    public static async ValueTask<ActionResult<T>> ToActionResult<T>( this ValueTask<OneOf<T, Error>> result ) => (await result.ConfigureAwait( false )).ToActionResult();
    public static       ActionResult<T>            ToActionResult<T>( this OneOf<T, Error>            result ) => result.Match<ActionResult<T>>( static x => x, static x => x.ToActionResult() );
}
