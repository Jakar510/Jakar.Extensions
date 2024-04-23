// Jakar.Extensions :: Jakar.Extensions
// 04/10/2024  21:04

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
    public const string CONFLICT_DESCRIPTION     = "A conflict has occurred.";
    public const string CONFLICT_TYPE            = "General.Conflict";
    public const string FORBIDDEN_DESCRIPTION    = "A 'Forbidden' has occurred.";
    public const string FORBIDDEN_TYPE           = "General.Forbidden";
    public const string GENERAL_DESCRIPTION      = "A failure has occurred.";
    public const string GENERAL_TYPE             = "General.Failure";
    public const string NOT_FOUND_DESCRIPTION    = "A 'Not Found' has occurred.";
    public const string NOT_FOUND_TYPE           = "General.NotFound";
    public const string UNAUTHORIZED_DESCRIPTION = "A 'Unauthorized' has occurred.";
    public const string UNAUTHORIZED_TYPE        = "General.Unauthorized";
    public const string UNEXPECTED_DESCRIPTION   = "A unexpected has occurred.";
    public const string UNEXPECTED_TYPE          = "General.Unexpected";
    public const string VALIDATION_DESCRIPTION   = "A validation has occurred.";
    public const string VALIDATION_TYPE          = "General.Unexpected";


    public static Error Empty { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(null, null, null, null, null, StringValues.Empty); }


    public static implicit operator Error( Status       result ) => Create( result,            StringValues.Empty );
    public static implicit operator Error( string       error )  => Create( Status.BadRequest, error );
    public static implicit operator Error( string[]     error )  => Create( Status.BadRequest, error );
    public static implicit operator Error( StringValues error )  => Create( Status.BadRequest, error );


    public static Error Create( IErrorDetails details )                                                                                                                               => new(details.StatusCode, details.Type, details.Title, details.Detail, details.Instance, details.Errors);
    public static Error Create( Status        status, in     StringValues errors )                                                                                                    => new(status, null, null, null, null, errors);
    public static Error Create( Status        status, params string[]     errors )                                                                                                    => Create( status, new StringValues( errors ) );
    public static Error Create( Status        status, string              type, in     StringValues errors )                                                                          => new(status, type, null, null, null, in errors);
    public static Error Create( Status        status, string              type, params string[]     errors )                                                                          => Create( status, type, null, null, null, new StringValues( errors ) );
    public static Error Create( Status        status, string              type, string?             title, in     StringValues errors )                                               => new(status, type, title, null, null, in errors);
    public static Error Create( Status        status, string              type, string?             title, params string[]     errors )                                               => Create( status, type, title, null, null, new StringValues( errors ) );
    public static Error Create( Status        status, string              type, string?             title, string?             detail, string? instance, in     StringValues errors ) => new(status, type, title, detail, instance, in errors);
    public static Error Create( Status        status, string              type, string?             title, string?             detail, string? instance, params string[]     errors ) => Create( status, type, title, detail, instance, new StringValues( errors ) );


    /// <summary> Creates an <see cref="Error"/> of type <see cref="Status.PreconditionFailed"/> from a type and description. </summary>
    public static Error Failure( string type = GENERAL_TYPE, string description = GENERAL_DESCRIPTION, in StringValues errors = default, string? detail = null, string? instance = null ) =>
        new(Status.PreconditionFailed, type, description, detail, instance, errors);

    /// <summary> Creates an <see cref="Error"/> of type <see cref="Status.UnprocessableEntity"/> from a type and description. </summary>
    public static Error Unexpected( string type = UNEXPECTED_TYPE, string description = UNEXPECTED_DESCRIPTION, in StringValues errors = default, string? detail = null, string? instance = null ) =>
        new(Status.UnprocessableEntity, type, description, detail, instance, errors);

    /// <summary> Creates an <see cref="Error"/> of type <see cref="Status.BadRequest"/> from a type and description. </summary>
    public static Error Validation( string type = VALIDATION_TYPE, string description = VALIDATION_DESCRIPTION, in StringValues errors = default, string? detail = null, string? instance = null ) =>
        new(Status.BadRequest, type, description, detail, instance, errors);

    /// <summary> Creates an <see cref="Error"/> of type <see cref="Status.Conflict"/> from a type and description. </summary>
    public static Error Conflict( string type = CONFLICT_TYPE, string description = CONFLICT_DESCRIPTION, in StringValues errors = default, string? detail = null, string? instance = null ) =>
        new(Status.Conflict, type, description, detail, instance, errors);

    /// <summary> Creates an <see cref="Error"/> of type <see cref="Status.NotFound"/> from a type and description. </summary>
    public static Error NotFound( string type = NOT_FOUND_TYPE, string description = NOT_FOUND_DESCRIPTION, in StringValues errors = default, string? detail = null, string? instance = null ) =>
        new(Status.NotFound, type, description, detail, instance, errors);

    /// <summary> Creates an <see cref="Error"/> of type <see cref="Status.Unauthorized"/> from a type and description. </summary>
    public static Error Unauthorized( string type = UNAUTHORIZED_TYPE, string description = UNAUTHORIZED_DESCRIPTION, in StringValues errors = default, string? detail = null, string? instance = null ) =>
        new(Status.Unauthorized, type, description, detail, instance, errors);

    /// <summary> Creates an <see cref="Error"/> of type <see cref="Status.Forbidden"/> from a type and description. </summary>
    public static Error Forbidden( string type = FORBIDDEN_TYPE, string description = FORBIDDEN_DESCRIPTION, in StringValues errors = default, string? detail = null, string? instance = null ) =>
        new(Status.Forbidden, type, description, detail, instance, errors);
}



/// <summary> Inspired by https://github.com/amantinband/error-or/tree/main </summary>
/// <typeparam name="T"> </typeparam>
/// <param name="Value"> </param>
/// <param name="Errors"> </param>
[Serializable, DefaultValue( nameof(Empty) )]
public readonly record struct ErrorOr<T>( in T? Value, Error[]? Errors )
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


    public static implicit operator T?( ErrorOr<T>                    result ) => result.Value;
    public static implicit operator Error[]( ErrorOr<T>               result ) => result.Errors ?? [];
    public static implicit operator ReadOnlySpan<Error>( ErrorOr<T>   result ) => result.Errors;
    public static implicit operator ReadOnlyMemory<Error>( ErrorOr<T> result ) => result.Errors;
    public static implicit operator ErrorOr<T>( T                     value )  => Create( value );
    public static implicit operator ErrorOr<T>( Error                 error )  => Create( error );
    public static implicit operator ErrorOr<T>( List<Error>           errors ) => Create( [..errors] );
    public static implicit operator ErrorOr<T>( Error[]               errors ) => Create( errors );
}



/// <summary> Inspired by https://github.com/amantinband/error-or/tree/main </summary>
/// <typeparam name="T"> </typeparam>
/// <typeparam name="TError"> </typeparam>
/// <param name="Value"> </param>
/// <param name="Errors"> </param>
[Serializable, DefaultValue( nameof(Empty) )]
public readonly record struct ErrorOr<T, TError>( in T? Value, TError[]? Errors )
    where TError : IErrorDetails
{
    public static ErrorOr<T, TError> Empty { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => new(default, null); }

#if NET6_0_OR_GREATER
    [MemberNotNullWhen( true, nameof(Errors) )]
#endif
    public bool HasErrors { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Errors?.Length is > 0; }


#if NET6_0_OR_GREATER
    [MemberNotNullWhen( true, nameof(Value) )]
#endif
    public bool HasValue { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Value is not null; }


    public static ErrorOr<T, TError> Create( in     T        value )  => new(value, null);
    public static ErrorOr<T, TError> Create( params TError[] errors ) => new(default, errors);


    public bool TryGetValue( [NotNullWhen( true )] out T? value, [NotNullWhen( false )] out TError[]? error )
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
    public bool TryGetValue( [NotNullWhen( true )] out T? value, out ReadOnlyMemory<TError> error )
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
    public bool TryGetValue( [NotNullWhen( true )] out TError[]? error )
    {
        error = Errors;
        return error is not null;
    }
    public bool TryGetValue( out ReadOnlyMemory<TError> error )
    {
        error = Errors;
        return error.IsEmpty is false;
    }
    public bool TryGetValue( out ReadOnlySpan<TError> error )
    {
        error = Errors;
        return error.IsEmpty is false;
    }


    public static implicit operator T?( ErrorOr<T, TError>                     result ) => result.Value;
    public static implicit operator TError[]( ErrorOr<T, TError>               result ) => result.Errors ?? [];
    public static implicit operator ReadOnlySpan<TError>( ErrorOr<T, TError>   result ) => result.Errors;
    public static implicit operator ReadOnlyMemory<TError>( ErrorOr<T, TError> result ) => result.Errors;
    public static implicit operator ErrorOr<T, TError>( T                      value )  => Create( value );
    public static implicit operator ErrorOr<T, TError>( TError                 error )  => Create( error );
    public static implicit operator ErrorOr<T, TError>( List<TError>           errors ) => Create( [..errors] );
    public static implicit operator ErrorOr<T, TError>( TError[]               errors ) => Create( errors );
}
