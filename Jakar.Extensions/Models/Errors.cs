// Jakar.Extensions :: Jakar.Extensions
// 04/23/2024  11:04

namespace Jakar.Extensions;


public static class Errors
{
    public const string CONFLICT_TITLE             = "A conflict has occurred.";
    public const string CONFLICT_TYPE              = "General.Conflict";
    public const string DISABLED_TITLE             = "User is disabled.";
    public const string DISABLED_TYPE              = "User.Disabled";
    public const string EXPIRED_SUBSCRIPTION_TITLE = "User's subscription is expired.";
    public const string EXPIRED_SUBSCRIPTION_TYPE  = "User.Subscription.Expired";
    public const string FORBIDDEN_TITLE            = "A 'Forbidden' has occurred.";
    public const string FORBIDDEN_TYPE             = "General.Forbidden";
    public const string GENERAL_TITLE              = "A failure has occurred.";
    public const string GENERAL_TYPE               = "General.Failure";
    public const string INVALID_SUBSCRIPTION_TITLE = "User's subscription is no longer valid.";
    public const string INVALID_SUBSCRIPTION_TYPE  = "User.Subscription.Invalid";
    public const string LOCKED_TITLE               = "User is locked.";
    public const string LOCKED_TYPE                = "User.Disabled";
    public const string NO_SUBSCRIPTION_TITLE      = "User is not subscribed.";
    public const string NO_SUBSCRIPTION_TYPE       = "User.Subscription.None";
    public const string NOT_FOUND_TITLE            = "A 'Not Found' has occurred.";
    public const string NOT_FOUND_TYPE             = "General.NotFound";
    public const string PASSWORD_VALIDATION_TITLE  = "Password validation failed";
    public const string PASSWORD_VALIDATION_TYPE   = "Password.Unauthorized";
    public const string UNAUTHORIZED_TITLE         = "A 'Unauthorized' has occurred.";
    public const string UNAUTHORIZED_TYPE          = "General.Unauthorized";
    public const string UNEXPECTED_TITLE           = "A unexpected has occurred.";
    public const string UNEXPECTED_TYPE            = "General.Unexpected";
    public const string VALIDATION_TITLE           = "A validation has occurred.";
    public const string VALIDATION_TYPE            = "General.Unexpected";


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
}
