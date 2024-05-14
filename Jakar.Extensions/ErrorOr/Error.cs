// Jakar.Extensions :: Jakar.Extensions
// 04/26/2024  13:04

using Microsoft.Extensions.Primitives;



namespace Jakar.Extensions;


/// <summary> Inspired by https://github.com/amantinband/error-or/tree/main </summary>
/// <param name="StatusCode"> </param>
/// <param name="Type"> </param>
/// <param name="Title"> </param>
/// <param name="Detail"> </param>
/// <param name="Instance"> </param>
/// <param name="Errors"> </param>
[Serializable, DefaultValue( nameof(Empty) )]
public readonly record struct Error( Status? StatusCode, string? Type, string? Title, string? Detail, string? Instance, in StringValues Errors ) : IErrorDetails
{
    public static Error Empty { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(null, null, null, null, null, StringValues.Empty); }


    public static implicit operator Error( Status               result )                                                                                                                                => Create( result,            StringValues.Empty );
    public static implicit operator Error( string               error )                                                                                                                                 => Create( Status.BadRequest, error );
    public static implicit operator Error( string[]             error )                                                                                                                                 => Create( Status.BadRequest, error );
    public static implicit operator Error( StringValues         error )                                                                                                                                 => Create( Status.BadRequest, error );
    public static                   Error Create( IErrorDetails details )                                                                                                                               => new(details.StatusCode, details.Type, details.Title, details.Detail, details.Instance, details.Errors);
    public static                   Error Create( Status        status, in     StringValues errors )                                                                                                    => new(status, null, null, null, null, errors);
    public static                   Error Create( Status        status, params string[]     errors )                                                                                                    => Create( status, new StringValues( errors ) );
    public static                   Error Create( Status        status, string              type, in     StringValues errors )                                                                          => new(status, type, null, null, null, in errors);
    public static                   Error Create( Status        status, string              type, params string[]     errors )                                                                          => Create( status, type, null, null, null, new StringValues( errors ) );
    public static                   Error Create( Status        status, string              type, string?             title, in     StringValues errors )                                               => new(status, type, title, null, null, in errors);
    public static                   Error Create( Status        status, string              type, string?             title, params string[]     errors )                                               => Create( status, type, title, null, null, new StringValues( errors ) );
    public static                   Error Create( Status        status, string              type, string?             title, string?             detail, string? instance, in     StringValues errors ) => new(status, type, title, detail, instance, in errors);
    public static                   Error Create( Status        status, string              type, string?             title, string?             detail, string? instance, params string[]     errors ) => Create( status, type, title, detail, instance, new StringValues( errors ) );


#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode( "Metadata for the method might be incomplete or removed" )]
#endif
    public static Error Create( Exception e, in StringValues errors = default, Status status = Status.InternalServerError ) => Create( e, e.MethodSignature(), errors, status );
    public static Error Create( Exception e, string? instance, in StringValues errors = default, Status status = Status.InternalServerError ) => new(status, e.GetType().Name, e.Message, e.Source, instance, errors);


    public static Error Unauthorized( scoped in PasswordValidator.Results results,
                                      string                              lengthPassed  = Extensions.Errors.LENGTH_PASSED,
                                      string                              mustBeTrimmed = Extensions.Errors.MUST_BE_TRIMMED,
                                      string                              specialPassed = Extensions.Errors.SPECIAL_PASSED,
                                      string                              numericPassed = Extensions.Errors.NUMERIC_PASSED,
                                      string                              lowerPassed   = Extensions.Errors.LOWER_PASSED,
                                      string                              upperPassed   = Extensions.Errors.UPPER_PASSED,
                                      string                              blockedPassed = Extensions.Errors.BLOCKED_PASSED,
                                      string                              type          = Extensions.Errors.PASSWORD_VALIDATION_TYPE,
                                      string                              title         = Extensions.Errors.PASSWORD_VALIDATION_TITLE,
                                      string?                             detail        = null,
                                      string?                             instance      = null
    )
    {
        using Buffer<string> errors = new(10);

        if ( results.LengthPassed ) { errors.Add( lengthPassed ); }

        if ( results.MustBeTrimmed ) { errors.Add( mustBeTrimmed ); }

        if ( results.SpecialPassed ) { errors.Add( specialPassed ); }

        if ( results.NumericPassed ) { errors.Add( numericPassed ); }

        if ( results.LowerPassed ) { errors.Add( lowerPassed ); }

        if ( results.UpperPassed ) { errors.Add( upperPassed ); }

        if ( results.BlockedPassed ) { errors.Add( blockedPassed ); }

        return Unauthorized( [.. errors.Span], type, title, detail, instance );
    }


    /// <summary> Creates an <see cref="Error"/> of type <see cref="Status.Unauthorized"/> from a type and title. </summary>
    public static Error Unauthorized( in StringValues errors = default, string type = Extensions.Errors.UNAUTHORIZED_TYPE, string title = Extensions.Errors.UNAUTHORIZED_TITLE, string? detail = null, string? instance = null ) => new(Status.Unauthorized, type, title, detail, instance, errors);


    public static Error Disabled( in            StringValues errors = default, string title = Extensions.Errors.DISABLED_TITLE,             string? detail = null, string? instance = null, string type = Extensions.Errors.DISABLED_TYPE )             => new(Status.Disabled, type, title, detail, instance, errors);
    public static Error Locked( in              StringValues errors = default, string title = Extensions.Errors.LOCKED_TITLE,               string? detail = null, string? instance = null, string type = Extensions.Errors.LOCKED_TYPE )               => new(Status.Locked, type, title, detail, instance, errors);
    public static Error ExpiredSubscription( in StringValues errors = default, string title = Extensions.Errors.EXPIRED_SUBSCRIPTION_TITLE, string? detail = null, string? instance = null, string type = Extensions.Errors.EXPIRED_SUBSCRIPTION_TYPE ) => new(Status.PaymentRequired, type, title, detail, instance, errors);
    public static Error InvalidSubscription( in StringValues errors = default, string title = Extensions.Errors.INVALID_SUBSCRIPTION_TITLE, string? detail = null, string? instance = null, string type = Extensions.Errors.INVALID_SUBSCRIPTION_TYPE ) => new(Status.PaymentRequired, type, title, detail, instance, errors);
    public static Error NoSubscription( in      StringValues errors = default, string title = Extensions.Errors.NO_SUBSCRIPTION_TITLE,      string? detail = null, string? instance = null, string type = Extensions.Errors.NO_SUBSCRIPTION_TYPE )      => new(Status.PaymentRequired, type, title, detail, instance, errors);


    /// <summary> Creates an <see cref="Error"/> of type <see cref="Status.PreconditionFailed"/> from a type and title. </summary>
    public static Error Failure( in StringValues errors = default, string title = Extensions.Errors.GENERAL_TITLE, string? detail = null, string? instance = null, string type = Extensions.Errors.GENERAL_TYPE ) => new(Status.PreconditionFailed, type, title, detail, instance, errors);

    /// <summary> Creates an <see cref="Error"/> of type <see cref="Status.UnprocessableEntity"/> from a type and title. </summary>
    public static Error Unexpected( in StringValues errors = default, string title = Extensions.Errors.UNEXPECTED_TITLE, string? detail = null, string? instance = null, string type = Extensions.Errors.UNEXPECTED_TYPE ) => new(Status.UnprocessableEntity, type, title, detail, instance, errors);

    /// <summary> Creates an <see cref="Error"/> of type <see cref="Status.BadRequest"/> from a type and title. </summary>
    public static Error Validation( in StringValues errors = default, string title = Extensions.Errors.VALIDATION_TITLE, string? detail = null, string? instance = null, string type = Extensions.Errors.VALIDATION_TYPE ) => new(Status.BadRequest, type, title, detail, instance, errors);

    /// <summary> Creates an <see cref="Error"/> of type <see cref="Status.Conflict"/> from a type and title. </summary>
    public static Error Conflict( in StringValues errors = default, string title = Extensions.Errors.CONFLICT_TITLE, string? detail = null, string? instance = null, string type = Extensions.Errors.CONFLICT_TYPE ) => new(Status.Conflict, type, title, detail, instance, errors);

    /// <summary> Creates an <see cref="Error"/> of type <see cref="Status.NotFound"/> from a type and title. </summary>
    public static Error NotFound( in StringValues errors = default, string title = Extensions.Errors.NOT_FOUND_TITLE, string? detail = null, string? instance = null, string type = Extensions.Errors.NOT_FOUND_TYPE ) => new(Status.NotFound, type, title, detail, instance, errors);

    /// <summary> Creates an <see cref="Error"/> of type <see cref="Status.Forbidden"/> from a type and title. </summary>
    public static Error Forbidden( in StringValues errors = default, string title = Extensions.Errors.FORBIDDEN_TITLE, string? detail = null, string? instance = null, string type = Extensions.Errors.FORBIDDEN_TYPE ) => new(Status.Forbidden, type, title, detail, instance, errors);
}
