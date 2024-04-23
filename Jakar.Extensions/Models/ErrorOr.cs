// Jakar.Extensions :: Jakar.Extensions
// 04/10/2024  21:04

using Microsoft.Extensions.Primitives;
using static Jakar.Extensions.Errors;



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
    [RequiresUnreferencedCode( nameof(ExceptionExtensions.MethodName) )]
#endif
    public static Error Create( Exception e, in StringValues errors = default, Status status = Status.InternalServerError ) => new(status, e.GetType().Name, e.Message, e.Source, e.MethodName(), errors);


    public static Error Password( scoped in PasswordValidator.Results results,
                                  string                              lengthPassed  = "Password not long enough",
                                  string                              mustBeTrimmed = "Password must be trimmed",
                                  string                              specialPassed = "Password must contain a special character",
                                  string                              numericPassed = "Password must contain a numeric character",
                                  string                              lowerPassed   = "Password must contain a lower case character",
                                  string                              upperPassed   = "Password must contain a upper case character",
                                  string                              blockedPassed = "Password cannot be a blocked password",
                                  string                              type          = PASSWORD_VALIDATION_TYPE,
                                  string                              title         = PASSWORD_VALIDATION_TITLE,
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

        return Password( [.. errors.Span], type, title, detail, instance );
    }
    public static Error Password( in StringValues errors, string type = PASSWORD_VALIDATION_TYPE, string title = PASSWORD_VALIDATION_TITLE, string? detail = null, string? instance = null ) => new(Status.Unauthorized, type, title, detail, instance, errors);


    public static Error Disabled( in            StringValues errors = default, string type = DISABLED_TYPE,             string title = DISABLED_TITLE,             string? detail = null, string? instance = null ) => new(Status.Disabled, type, title, detail, instance, errors);
    public static Error Locked( in              StringValues errors = default, string type = LOCKED_TYPE,               string title = LOCKED_TITLE,               string? detail = null, string? instance = null ) => new(Status.Locked, type, title, detail, instance, errors);
    public static Error ExpiredSubscription( in StringValues errors = default, string type = EXPIRED_SUBSCRIPTION_TYPE, string title = EXPIRED_SUBSCRIPTION_TITLE, string? detail = null, string? instance = null ) => new(Status.PaymentRequired, type, title, detail, instance, errors);
    public static Error InvalidSubscription( in StringValues errors = default, string type = INVALID_SUBSCRIPTION_TYPE, string title = INVALID_SUBSCRIPTION_TITLE, string? detail = null, string? instance = null ) => new(Status.PaymentRequired, type, title, detail, instance, errors);
    public static Error NoSubscription( in      StringValues errors = default, string type = NO_SUBSCRIPTION_TYPE,      string title = NO_SUBSCRIPTION_TITLE,      string? detail = null, string? instance = null ) => new(Status.PaymentRequired, type, title, detail, instance, errors);


    /// <summary> Creates an <see cref="Error"/> of type <see cref="Status.PreconditionFailed"/> from a type and title. </summary>
    public static Error Failure( in StringValues errors = default, string type = GENERAL_TYPE, string title = GENERAL_TITLE, string? detail = null, string? instance = null ) => new(Status.PreconditionFailed, type, title, detail, instance, errors);

    /// <summary> Creates an <see cref="Error"/> of type <see cref="Status.UnprocessableEntity"/> from a type and title. </summary>
    public static Error Unexpected( in StringValues errors = default, string type = UNEXPECTED_TYPE, string title = UNEXPECTED_TITLE, string? detail = null, string? instance = null ) => new(Status.UnprocessableEntity, type, title, detail, instance, errors);

    /// <summary> Creates an <see cref="Error"/> of type <see cref="Status.BadRequest"/> from a type and title. </summary>
    public static Error Validation( in StringValues errors = default, string type = VALIDATION_TYPE, string title = VALIDATION_TITLE, string? detail = null, string? instance = null ) => new(Status.BadRequest, type, title, detail, instance, errors);

    /// <summary> Creates an <see cref="Error"/> of type <see cref="Status.Conflict"/> from a type and title. </summary>
    public static Error Conflict( in StringValues errors = default, string type = CONFLICT_TYPE, string title = CONFLICT_TITLE, string? detail = null, string? instance = null ) => new(Status.Conflict, type, title, detail, instance, errors);

    /// <summary> Creates an <see cref="Error"/> of type <see cref="Status.NotFound"/> from a type and title. </summary>
    public static Error NotFound( in StringValues errors = default, string type = NOT_FOUND_TYPE, string title = NOT_FOUND_TITLE, string? detail = null, string? instance = null ) => new(Status.NotFound, type, title, detail, instance, errors);

    /// <summary> Creates an <see cref="Error"/> of type <see cref="Status.Unauthorized"/> from a type and title. </summary>
    public static Error Unauthorized( in StringValues errors = default, string type = UNAUTHORIZED_TYPE, string title = UNAUTHORIZED_TITLE, string? detail = null, string? instance = null ) => new(Status.Unauthorized, type, title, detail, instance, errors);

    /// <summary> Creates an <see cref="Error"/> of type <see cref="Status.Forbidden"/> from a type and title. </summary>
    public static Error Forbidden( in StringValues errors = default, string type = FORBIDDEN_TYPE, string title = FORBIDDEN_TITLE, string? detail = null, string? instance = null ) => new(Status.Forbidden, type, title, detail, instance, errors);
}



/// <summary> Inspired by https://github.com/amantinband/error-or/tree/main </summary>
/// <typeparam name="T"> </typeparam>
/// <param name="Value"> </param>
/// <param name="Errors"> </param>
[Serializable, DefaultValue( nameof(Empty) )]
public readonly record struct ErrorOr<T>( in T? Value, in Error[]? Errors )
{
    public static ErrorOr<T> Empty { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(default, null); }

#if NET6_0_OR_GREATER
    [MemberNotNullWhen( true, nameof(Errors) )]
#endif
    public bool HasErrors { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Errors?.Length is > 0; }


#if NET6_0_OR_GREATER
    [MemberNotNullWhen( true, nameof(Value) )]
#endif
    public bool HasValue { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Value is not null; }


    public static ErrorOr<T> Create( in     T       value )  => new(value, null);
    public static ErrorOr<T> Create( params Error[] errors ) => new(default, errors);


    public Status GetStatus() => Errors?.Max( static x => x.StatusCode ) ?? Status.Ok;


    public bool TryGetValue( [NotNullWhen( true )] out T? value, [NotNullWhen( false )] out Error[]? error )
    {
        if ( Value is not null )
        {
            value = Value;
            error = null;
            return true;
        }

        value = default;
        error = Errors ?? [];
        return false;
    }
    public bool TryGetValue( [NotNullWhen( true )] out T? value, out ReadOnlyMemory<Error> error )
    {
        if ( Value is not null )
        {
            value = Value;
            error = null;
            return true;
        }

        value = default;
        error = Errors;
        return false;
    }
    public bool TryGetValue( [NotNullWhen( true )] out T? value )
    {
        value = Value;
        return value is not null;
    }
    public bool TryGetValue( [NotNullWhen( true )] out Error[]? error )
    {
        error = Errors;
        return error is not null;
    }
    public bool TryGetValue( out ReadOnlyMemory<Error> error )
    {
        error = Errors;
        return error.IsEmpty is false;
    }
    public bool TryGetValue( out ReadOnlySpan<Error> error )
    {
        error = Errors;
        return error.IsEmpty is false;
    }


    public static implicit operator OneOf<T?, Error[]>( ErrorOr<T> result ) => result.HasValue
                                                                                   ? result.Value
                                                                                   : result.Errors ?? [];
    public static implicit operator T?( ErrorOr<T>                    result ) => result.Value;
    public static implicit operator Error[]( ErrorOr<T>               result ) => result.Errors ?? [];
    public static implicit operator ReadOnlySpan<Error>( ErrorOr<T>   result ) => result.Errors;
    public static implicit operator ReadOnlyMemory<Error>( ErrorOr<T> result ) => result.Errors;
    public static implicit operator ErrorOr<T>( T                     value )  => Create( value );
    public static implicit operator ErrorOr<T>( Error                 error )  => Create( error );
    public static implicit operator ErrorOr<T>( List<Error>           errors ) => Create( [..errors] );
    public static implicit operator ErrorOr<T>( Error[]               errors ) => Create( errors );
}
