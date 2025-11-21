// Jakar.Extensions :: Jakar.Extensions
// 04/26/2024  13:04

namespace Jakar.Extensions;


/// <summary> Inspired by https://github.com/amantinband/error-or/tree/main </summary>
[Serializable]
[DefaultValue(nameof(Empty))]
public sealed class Error : BaseClass, IErrorDetails, IEqualComparable<Error>
{
    public static readonly       Error      Empty = new(Status.NotSet);
    [JsonIgnore] public readonly StringTags details;


    public static         IErrorTitles Titles      { get;            set; } = ErrorTitles.Defaults.Instance;
    [JsonRequired] public string?      Description { get;            init; }
    [JsonRequired] public StringTags   Details     { get => details; init => details = value; }
    [JsonRequired] public string?      Instance    { get;            init; }
    [JsonRequired] public Status       StatusCode  { get;            init; }
    [JsonRequired] public string?      Title       { get;            init; }
    [JsonRequired] public string?      Type        { get;            init; }


    public Error() : base() { }
    public Error( Status statusCode, string? title = null, string? description = null, string? instance = null, StringTags details = default, string? type = null ) : base()
    {
        Details     = details;
        StatusCode  = statusCode;
        Description = description;
        Instance    = instance;
        Title       = title ?? Titles.GetErrorTitle(statusCode);
        Type        = type  ?? statusCode.GetErrorType();
    }


    public static implicit operator Error( Status     result ) => Create(result);
    public static implicit operator Error( string     error )  => Create(Status.BadRequest, details: new StringTags(error));
    public static implicit operator Error( Pair       error )  => Create(Status.BadRequest, details: error);
    public static implicit operator Error( Exception  error )  => Create(error);
    public static implicit operator Error( Pair[]     error )  => Create(Status.BadRequest, details: error);
    public static implicit operator Error( StringTags error )  => Create(Status.BadRequest, details: error);


    public static Status GetStatus( Error error ) => error.StatusCode;


    public static Error Create<TValue>( TValue details )
        where TValue : IErrorDetails => new(details.StatusCode, details.Title, details.Description, details.Instance, details.Details, details.Type);
    public static Error Create( Status status, string? title = null, string? description = null, string? instance = null, StringTags details = default, string? type = null ) => new(status, title, description, instance, details, type);


    public static Error Create( Exception e, StringTags details = default, Status? status = null, string? type = null ) => Create(e, e.Source, e.MethodSignature(), details, status, type);
    public static Error Create( Exception e, string? description, string? instance, StringTags details = default, Status? status = null, string? type = null )
    {
        string classType = e.GetType()
                            .Name;

        return new Error(status ?? Statuses.GetStatusFromException(e),
                         e.Message,
                         description,
                         instance,
                         details,
                         type is null
                             ? classType
                             : $"{type}.{classType}");
    }


    public static Error Unauthorized( ref readonly PasswordValidator.Results results,  string?    instance                      = null, string? description = null, string? type = null ) => Unauthorized(results.ToStringTags(), instance, description, Titles.PasswordValidation);
    public static Error Unauthorized( string?                                instance, StringTags details                       = default )                                         => Unauthorized(details, instance, title: Titles.InvalidCredentials);
    public static Error Unauthorized( StringTags                             details,  string?    instance, string? description = null, string? title = null, string? type = null ) => new(Status.Unauthorized, title, description, instance, details);
    public static Error NoInternet( StringTags                               details = default )                                                                                                 => Disabled(details, title: Titles.NoInternet,     type: NO_INTERNET_TYPE);
    public static Error WiFiDisabled( StringTags                             details = default )                                                                                                 => Disabled(details, title: Titles.WiFiIsDisabled, type: NO_INTERNET_WIFI_TYPE);
    public static Error ExpiredSubscription( StringTags                      details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.PaymentRequired, title, description, instance, details, type);
    public static Error Subscription( StringTags                             details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.PaymentRequired, title, description, instance, details, type);
    public static Error NoSubscription( StringTags                           details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.PaymentRequired, title, description, instance, details, type);
    public static Error Failure( StringTags                                  details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.PreconditionFailed, title, description, instance, details, type);
    public static Error Unexpected( StringTags                               details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.UnprocessableEntity, title, description, instance, details, type);
    public static Error Validation( StringTags                               details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.BadRequest, title, description, instance, details, type);
    public static Error DeviceName( StringTags                               details = default )                                                                                                 => Validation(details, title: Titles.MustHaveValidDeviceName);
    public static Error Host( StringTags                                     details = default )                                                                                                 => Validation(details, title: Titles.MustHaveValidIPHostName);
    public static Error Password( StringTags                                 details = default )                                                                                                 => Validation(details, title: Titles.PasswordCannotBeEmpty);
    public static Error Port( StringTags                                     details = default )                                                                                                 => Validation(details, title: Titles.GivenPortIsNotAValidPortNumberInRangeOf1To65535);
    public static Error UserName( StringTags                                 details = default )                                                                                                 => Validation(details, title: Titles.UserNameCannotBeEmpty);
    public static Error ServerIsUnavailable( StringTags                      details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.PreconditionFailed, title, description, instance, details, type);
    public static Error ServerIsOutdated( StringTags                         details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.PreconditionFailed, title, description, instance, details, type);
    public static Error ClientIsOutdated( StringTags                         details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.PreconditionFailed, title, description, instance, details, type);
    public static Error ClientIsUnavailable( StringTags                      details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.PreconditionFailed, title, description, instance, details, type);
    public static Error OperationCanceled( StringTags                        details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.ClientClosedRequest, title, description, instance, details, type);
    public static Error NotSet( StringTags                                   details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.NotSet, title, description, instance, details, type);
    public static Error Continue( StringTags                                 details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.Continue, title, description, instance, details, type);
    public static Error SwitchingProtocols( StringTags                       details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.SwitchingProtocols, title, description, instance, details, type);
    public static Error Processing( StringTags                               details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.Processing, title, description, instance, details, type);
    public static Error EarlyHints( StringTags                               details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.EarlyHints, title, description, instance, details, type);
    public static Error Ok( StringTags                                       details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.Ok, title, description, instance, details, type);
    public static Error Created( StringTags                                  details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.Created, title, description, instance, details, type);
    public static Error Accepted( StringTags                                 details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.Accepted, title, description, instance, details, type);
    public static Error NonAuthoritativeInformation( StringTags              details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.NonAuthoritativeInformation, title, description, instance, details, type);
    public static Error NoContent( StringTags                                details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.NoContent, title, description, instance, details, type);
    public static Error ResetContent( StringTags                             details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.ResetContent, title, description, instance, details, type);
    public static Error PartialContent( StringTags                           details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.PartialContent, title, description, instance, details, type);
    public static Error MultiStatus( StringTags                              details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.MultiStatus, title, description, instance, details, type);
    public static Error AlreadyReported( StringTags                          details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.AlreadyReported, title, description, instance, details, type);
    public static Error ImUsed( StringTags                                   details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.ImUsed, title, description, instance, details, type);
    public static Error Ambiguous( StringTags                                details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.Ambiguous, title, description, instance, details, type);
    public static Error Moved( StringTags                                    details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.Moved, title, description, instance, details, type);
    public static Error Found( StringTags                                    details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.Found, title, description, instance, details, type);
    public static Error RedirectMethod( StringTags                           details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.RedirectMethod, title, description, instance, details, type);
    public static Error NotModified( StringTags                              details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.NotModified, title, description, instance, details, type);
    public static Error UseProxy( StringTags                                 details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.UseProxy, title, description, instance, details, type);
    public static Error Unused( StringTags                                   details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.Unused, title, description, instance, details, type);
    public static Error RedirectKeepVerb( StringTags                         details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.RedirectKeepVerb, title, description, instance, details, type);
    public static Error PermanentRedirect( StringTags                        details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.PermanentRedirect, title, description, instance, details, type);
    public static Error BadRequest( StringTags                               details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.BadRequest, title, description, instance, details, type);
    public static Error PaymentRequired( StringTags                          details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.PaymentRequired, title, description, instance, details, type);
    public static Error Forbidden( StringTags                                details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.Forbidden, title, description, instance, details, type);
    public static Error NotFound( StringTags                                 details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.NotFound, title, description, instance, details, type);
    public static Error MethodNotAllowed( StringTags                         details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.MethodNotAllowed, title, description, instance, details, type);
    public static Error NotAcceptable( StringTags                            details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.NotAcceptable, title, description, instance, details, type);
    public static Error ProxyAuthenticationRequired( StringTags              details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.ProxyAuthenticationRequired, title, description, instance, details, type);
    public static Error RequestTimeout( StringTags                           details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.RequestTimeout, title, description, instance, details, type);
    public static Error Conflict( StringTags                                 details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.Conflict, title, description, instance, details, type);
    public static Error Gone( StringTags                                     details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.Gone, title, description, instance, details, type);
    public static Error LengthRequired( StringTags                           details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.LengthRequired, title, description, instance, details, type);
    public static Error PreconditionFailed( StringTags                       details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.PreconditionFailed, title, description, instance, details, type);
    public static Error RequestEntityTooLarge( StringTags                    details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.RequestEntityTooLarge, title, description, instance, details, type);
    public static Error RequestUriTooLong( StringTags                        details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.RequestUriTooLong, title, description, instance, details, type);
    public static Error UnsupportedMediaType( StringTags                     details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.UnsupportedMediaType, title, description, instance, details, type);
    public static Error RequestedRangeNotSatisfiable( StringTags             details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.RequestedRangeNotSatisfiable, title, description, instance, details, type);
    public static Error ExpectationFailed( StringTags                        details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.ExpectationFailed, title, description, instance, details, type);
    public static Error Teapot( StringTags                                   details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.Teapot, title, description, instance, details, type);
    public static Error MisdirectedRequest( StringTags                       details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.MisdirectedRequest, title, description, instance, details, type);
    public static Error UnprocessableEntity( StringTags                      details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.UnprocessableEntity, title, description, instance, details, type);
    public static Error Locked( StringTags                                   details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.Locked, title, description, instance, details, type);
    public static Error FailedDependency( StringTags                         details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.FailedDependency, title, description, instance, details, type);
    public static Error TooEarly( StringTags                                 details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.TooEarly, title, description, instance, details, type);
    public static Error UpgradeRequired( StringTags                          details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.UpgradeRequired, title, description, instance, details, type);
    public static Error PreconditionRequired( StringTags                     details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.PreconditionRequired, title, description, instance, details, type);
    public static Error TooManyRequests( StringTags                          details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.TooManyRequests, title, description, instance, details, type);
    public static Error Disabled( StringTags                                 details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.Disabled, title, description, instance, details, type);
    public static Error RequestHeaderFieldsTooLarge( StringTags              details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.RequestHeaderFieldsTooLarge, title, description, instance, details, type);
    public static Error AlreadyExists( StringTags                            details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.AlreadyExists, title, description, instance, details, type);
    public static Error NoResponse( StringTags                               details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.NoResponse, title, description, instance, details, type);
    public static Error UnavailableForLegalReasons( StringTags               details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.UnavailableForLegalReasons, title, description, instance, details, type);
    public static Error SessionExpired( StringTags                           details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.SessionExpired, title, description, instance, details, type);
    public static Error ClientClosedRequest( StringTags                      details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.ClientClosedRequest, title, description, instance, details, type);
    public static Error InternalServerError( StringTags                      details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.InternalServerError, title, description, instance, details, type);
    public static Error NotImplemented( StringTags                           details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.NotImplemented, title, description, instance, details, type);
    public static Error BadGateway( StringTags                               details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.BadGateway, title, description, instance, details, type);
    public static Error ServiceUnavailable( StringTags                       details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.ServiceUnavailable, title, description, instance, details, type);
    public static Error GatewayTimeout( StringTags                           details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.GatewayTimeout, title, description, instance, details, type);
    public static Error HttpVersionNotSupported( StringTags                  details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.HttpVersionNotSupported, title, description, instance, details, type);
    public static Error VariantAlsoNegotiates( StringTags                    details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.VariantAlsoNegotiates, title, description, instance, details, type);
    public static Error InsufficientStorage( StringTags                      details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.InsufficientStorage, title, description, instance, details, type);
    public static Error LoopDetected( StringTags                             details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.LoopDetected, title, description, instance, details, type);
    public static Error NotExtended( StringTags                              details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.NotExtended, title, description, instance, details, type);
    public static Error NetworkAuthenticationRequired( StringTags            details = default, string? instance = null, string? description = null, string? title = null, string? type = null ) => new(Status.NetworkAuthenticationRequired, title, description, instance, details, type);


    public int CompareTo( Error? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        int statusCodeComparison = StatusCode.CompareTo(other.StatusCode);
        if ( statusCodeComparison != 0 ) { return statusCodeComparison; }

        int typeComparison = string.Compare(Type, other.Type, StringComparison.Ordinal);
        if ( typeComparison != 0 ) { return typeComparison; }

        int titleComparison = string.Compare(Title, other.Title, StringComparison.Ordinal);
        if ( titleComparison != 0 ) { return titleComparison; }

        int detailComparison = string.Compare(Description, other.Description, StringComparison.Ordinal);
        if ( detailComparison != 0 ) { return detailComparison; }

        return string.Compare(Instance, other.Instance, StringComparison.Ordinal);
    }
    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        return other is Error error
                   ? CompareTo(error)
                   : throw new ArgumentException($"Object must be of type {nameof(Error)}");
    }
    public static bool Equals( ReadOnlySpan<Error> left, ReadOnlySpan<Error> right ) => left.SequenceEqual(right);
    public bool Equals( Error? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return StatusCode.Equals(other.StatusCode) && string.Equals(Type, other.Type, StringComparison.Ordinal) && string.Equals(Title, other.Title, StringComparison.Ordinal) && string.Equals(Description, other.Description, StringComparison.Ordinal) && string.Equals(Instance, other.Instance, StringComparison.Ordinal);
    }
    public override bool Equals( object? other )                  => ReferenceEquals(this, other) || ( other is Error error && Equals(error) );
    public override int  GetHashCode()                            => HashCode.Combine(StatusCode, Type, Title, Description, Instance, details);
    public static   bool operator ==( Error? left, Error? right ) => EqualityComparer<Error>.Default.Equals(left, right);
    public static   bool operator !=( Error? left, Error? right ) => !EqualityComparer<Error>.Default.Equals(left, right);
    public static   bool operator >( Error   left, Error  right ) => Comparer<Error>.Default.Compare(left, right) > 0;
    public static   bool operator >=( Error  left, Error  right ) => Comparer<Error>.Default.Compare(left, right) >= 0;
    public static   bool operator <( Error   left, Error  right ) => Comparer<Error>.Default.Compare(left, right) < 0;
    public static   bool operator <=( Error  left, Error  right ) => Comparer<Error>.Default.Compare(left, right) <= 0;
}
