// Jakar.Extensions :: Jakar.Extensions
// 04/26/2024  13:04

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


    public static implicit operator Error( Status       result ) => Create( result,            StringValues.Empty );
    public static implicit operator Error( string       error )  => Create( Status.BadRequest, error );
    public static implicit operator Error( string[]     error )  => Create( Status.BadRequest, error );
    public static implicit operator Error( StringValues error )  => Create( Status.BadRequest, error );


    public Status GetStatus() => StatusCode ?? Status.Ok;


    public static Error Create<T>( T details )
        where T : IErrorDetails => new(details.StatusCode, details.Type, details.Title, details.Detail, details.Instance, details.Errors);
    public static Error Create( Status status, in StringValues errors, [CallerMemberName] string       type = BaseRecord.EMPTY )                                                         => new(status, type, null, null, null, errors);
    public static Error Create( Status status, string          type,   in                 StringValues errors )                                                                          => new(status, type, null, null, null, in errors);
    public static Error Create( Status status, string          type,   params             string[]     errors )                                                                          => Create( status, type, null, null, null, new StringValues( errors ) );
    public static Error Create( Status status, string          type,   string?                         title, in     StringValues errors )                                               => new(status, type, title, null, null, in errors);
    public static Error Create( Status status, string          type,   string?                         title, params string[]     errors )                                               => Create( status, type, title, null, null, new StringValues( errors ) );
    public static Error Create( Status status, string          type,   string?                         title, string?             detail, string? instance, in     StringValues errors ) => new(status, type, title, detail, instance, in errors);
    public static Error Create( Status status, string          type,   string?                         title, string?             detail, string? instance, params string[]     errors ) => Create( status, type, title, detail, instance, new StringValues( errors ) );


#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode( "Metadata for the method might be incomplete or removed" )]
#endif
    public static Error Create( Exception e, in StringValues errors = default, Status status = Status.InternalServerError ) => Create( e.Source, e, e.MethodSignature(), errors, status );
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode( "Metadata for the method might be incomplete or removed" )]
#endif
    public static Error Create( string? instance, Exception e, in StringValues errors = default, Status status = Status.InternalServerError ) => Create( instance, e, e.MethodSignature(), errors, status );
    public static Error Create( Exception e,        string?   detail, in StringValues errors = default, Status          status = Status.InternalServerError )                          => Create( e.Source, e, detail, errors, status );
    public static Error Create( string?   instance, Exception e,      string?         detail,           in StringValues errors = default, Status status = Status.InternalServerError ) => new(status, e.GetType().Name, e.Message, detail, instance, errors);


    public static Error Unauthorized( string?                             instance, in StringValues errors   = default )                                                                            => Unauthorized( errors,             instance, title: Titles.InvalidCredentials );
    public static Error Unauthorized( scoped in PasswordValidator.Results results,  string?         instance = null, string? detail = null, string  type  = PASSWORD_VALIDATION_TYPE )              => Unauthorized( results.ToValues(), instance, detail, Titles.PasswordValidation, type );
    public static Error Unauthorized( in        StringValues              errors,   string?         instance,        string? detail = null, string? title = null, string type = UNAUTHORIZED_TYPE ) => new(Status.Unauthorized, type, title ?? Titles.Unauthorized, detail, instance, errors);
    public static Error NoInternet( in          StringValues              errors = default )                                                                                                                => Disabled( errors, title: Titles.NoInternet,     type: NO_INTERNET_TYPE );
    public static Error WiFiDisabled( in        StringValues              errors = default )                                                                                                                => Disabled( errors, title: Titles.WiFiIsDisabled, type: NO_INTERNET_WIFI_TYPE );
    public static Error Disabled( in            StringValues              errors = default, string? instance = null, string? detail = null, string? title = null, string type = DISABLED_TYPE )             => new(Status.Disabled, type, title            ?? Titles.Disabled, detail, instance, errors);
    public static Error Locked( in              StringValues              errors = default, string? instance = null, string? detail = null, string? title = null, string type = LOCKED_TYPE )               => new(Status.Locked, type, title              ?? Titles.Locked, detail, instance, errors);
    public static Error ExpiredSubscription( in StringValues              errors = default, string? instance = null, string? detail = null, string? title = null, string type = EXPIRED_SUBSCRIPTION_TYPE ) => new(Status.PaymentRequired, type, title     ?? Titles.ExpiredSubscription, detail, instance, errors);
    public static Error Subscription( in        StringValues              errors = default, string? instance = null, string? detail = null, string? title = null, string type = INVALID_SUBSCRIPTION_TYPE ) => new(Status.PaymentRequired, type, title     ?? Titles.InvalidSubscription, detail, instance, errors);
    public static Error NoSubscription( in      StringValues              errors = default, string? instance = null, string? detail = null, string? title = null, string type = NO_SUBSCRIPTION_TYPE )      => new(Status.PaymentRequired, type, title     ?? Titles.NoSubscription, detail, instance, errors);
    public static Error Failure( in             StringValues              errors = default, string? instance = null, string? detail = null, string? title = null, string type = GENERAL_TYPE )              => new(Status.PreconditionFailed, type, title  ?? Titles.General, detail, instance, errors);
    public static Error Unexpected( in          StringValues              errors = default, string? instance = null, string? detail = null, string? title = null, string type = UNEXPECTED_TYPE )           => new(Status.UnprocessableEntity, type, title ?? Titles.Unexpected, detail, instance, errors);
    public static Error Conflict( in            StringValues              errors = default, string? instance = null, string? detail = null, string? title = null, string type = CONFLICT_TYPE )             => new(Status.Conflict, type, title            ?? Titles.Conflict, detail, instance, errors);
    public static Error NotFound( in            StringValues              errors = default, string? instance = null, string? detail = null, string? title = null, string type = NOT_FOUND_TYPE )            => new(Status.NotFound, type, title            ?? Titles.NotFound, detail, instance, errors);
    public static Error Forbidden( in           StringValues              errors = default, string? instance = null, string? detail = null, string? title = null, string type = FORBIDDEN_TYPE )            => new(Status.Forbidden, type, title           ?? Titles.Forbidden, detail, instance, errors);
    public static Error Validation( in          StringValues              errors = default, string? instance = null, string? detail = null, string? title = null, string type = VALIDATION_TYPE )           => new(Status.BadRequest, type, title          ?? Titles.Validation, detail, instance, errors);
    public static Error DeviceName( in          StringValues              errors = default )                                                                                                                 => Validation( errors, title: Titles.MustHaveValidDeviceName );
    public static Error Host( in                StringValues              errors = default )                                                                                                                 => Validation( errors, title: Titles.MustHaveValidIPHostName );
    public static Error Password( in            StringValues              errors = default )                                                                                                                 => Validation( errors, title: Titles.PasswordCannotBeEmpty );
    public static Error Port( in                StringValues              errors = default )                                                                                                                 => Validation( errors, title: Titles.GivenPortIsNotAValidPortNumberInRangeOf1To65535 );
    public static Error UserName( in            StringValues              errors = default )                                                                                                                 => Validation( errors, title: Titles.UserNameCannotBeEmpty );
    public static Error ServerIsUnavailable( in StringValues              errors = default, string? instance = null, string? detail = null, string? title = null, string type = SERVER_IS_UNAVAILABLE_TYPE ) => new(Status.PreconditionFailed, type, title  ?? Titles.ServerIsUnavailable, detail, instance, errors);
    public static Error ServerIsOutdated( in    StringValues              errors = default, string? instance = null, string? detail = null, string? title = null, string type = SERVER_IS_OUTDATED_TYPE )    => new(Status.PreconditionFailed, type, title  ?? Titles.ServerIsOutdated, detail, instance, errors);
    public static Error ClientIsOutdated( in    StringValues              errors = default, string? instance = null, string? detail = null, string? title = null, string type = CLIENT_IS_OUTDATED_TYPE )    => new(Status.PreconditionFailed, type, title  ?? Titles.ClientIsOutdated, detail, instance, errors);
    public static Error ClientIsUnavailable( in StringValues              errors = default, string? instance = null, string? detail = null, string? title = null, string type = CLIENT_IS_UNAVAILABLE_TYPE ) => new(Status.PreconditionFailed, type, title  ?? Titles.ClientIsUnavailable, detail, instance, errors);
    public static Error OperationCanceled( in   StringValues              errors = default, string? instance = null, string? detail = null, string? title = null, string type = CLIENT_OPERATION_CANCELED )  => new(Status.ClientClosedRequest, type, title ?? Titles.ClientIsUnavailable, detail, instance, errors);
}
