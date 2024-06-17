// Jakar.Extensions :: Jakar.Extensions
// 04/23/2024  11:04

using Microsoft.Extensions.Primitives;



namespace Jakar.Extensions;


public static class Errors
{
    public const string BLOCKED_PASSED             = "Password cannot be a blocked password";
    public const string CLIENT_IS_OUTDATED_TYPE    = "Client.Outdated";
    public const string CLIENT_IS_UNAVAILABLE_TYPE = "Client.Unavailable";
    public const string CONFLICT_TYPE              = "General.Conflict";
    public const string DISABLED_TYPE              = "User.Disabled";
    public const string EXPIRED_SUBSCRIPTION_TYPE  = "User.Subscription.Expired";
    public const string FORBIDDEN_TYPE             = "General.Forbidden";
    public const string GENERAL_TYPE               = "General.Failure";
    public const string INVALID_SUBSCRIPTION_TYPE  = "User.Subscription.Invalid";
    public const string LOCKED_TYPE                = "User.Disabled";
    public const string NO_INTERNET_TYPE           = "Internet.Disabled";
    public const string NO_INTERNET_WIFI_TYPE      = "Internet.Disabled.WiFi";
    public const string NO_SUBSCRIPTION_TYPE       = "User.Subscription.None";
    public const string NOT_FOUND_TYPE             = "General.NotFound";
    public const string PASSWORD_VALIDATION_TYPE   = "Password.Unauthorized";
    public const string SERVER_IS_OUTDATED_TYPE    = "Server.Outdated";
    public const string SERVER_IS_UNAVAILABLE_TYPE = "Server.Unavailable";
    public const string UNAUTHORIZED_TYPE          = "General.Unauthorized";
    public const string UNEXPECTED_TYPE            = "General.Unexpected";
    public const string VALIDATION_TYPE            = "General.Unexpected";


    public static ErrorTitles Titles { get; set; } = new();


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static string GetMessage<T>( this T errors )
        where T : IEnumerable<Error> => string.Join( '\n', errors.Select( GetMessage ) );
    public static string GetMessage( this Error error ) => GetMessage( error.Title, error.Errors );
    public static string GetMessage( in string? title, in StringValues values )
    {
        switch ( values.Count )
        {
            case 0: return string.Empty;
            case 1: return values.ToString();
        }

        const string             BULLET  = "• ";
        const string             SPACER  = "\n    -";
        using ValueStringBuilder builder = new(4096);
        builder.Append( BULLET ).Append( title );

        foreach ( string? value in values ) { builder.Append( SPACER ).Append( value ); }

        return builder.ToString();
    }


    public static Status GetStatus<T>( this T? errors )
        where T : IEnumerable<Error> => errors?.Max( GetStatus ) ?? Status.Ok;
    public static Status GetStatus( this Error[]? errors, Status status = Status.Ok ) => new ReadOnlySpan<Error>( errors ).GetStatus( status );
    public static Status GetStatus( this ReadOnlySpan<Error> errors, Status status = Status.Ok )
    {
        if ( errors.IsEmpty ) { return status; }

        foreach ( Error error in errors )
        {
            Status? code = error.StatusCode;
            if ( code > status ) { status = code.Value; }
        }

        return status;
    }
    private static Status GetStatus( this Error error ) => error.StatusCode ?? Status.Ok;


    public static StringValues ToValues( this PasswordValidator.Results results )
    {
        using Buffer<string> errors = new(10);

        if ( results.LengthPassed ) { errors.Add( Titles.BlockedPassed ); }

        if ( results.MustBeTrimmed ) { errors.Add( Titles.MustBeTrimmed ); }

        if ( results.SpecialPassed ) { errors.Add( Titles.SpecialPassed ); }

        if ( results.NumericPassed ) { errors.Add( Titles.NumericPassed ); }

        if ( results.LowerPassed ) { errors.Add( Titles.LowerPassed ); }

        if ( results.UpperPassed ) { errors.Add( Titles.UpperPassed ); }

        if ( results.BlockedPassed ) { errors.Add( Titles.BlockedPassed ); }

        StringValues values = [.. errors.Span];
        return values;
    }
}
