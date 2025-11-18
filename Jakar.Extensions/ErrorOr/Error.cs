// Jakar.Extensions :: Jakar.Extensions
// 04/26/2024  13:04

using OpenTelemetry.Trace;



namespace Jakar.Extensions;


/// <summary> Inspired by https://github.com/amantinband/error-or/tree/main </summary>
[Serializable]
[DefaultValue(nameof(Empty))]
public sealed class Error : BaseClass, IErrorDetails, IEqualComparable<Error>
{
    public static readonly Error       Empty = new(null, null, null, null, null, null);
    internal readonly      Status?     statusCode;
    internal readonly      StringTags? errors;


    public static                  IErrorTitles Titles     { get;               set; } = IErrorTitles.Defaults.Instance;
    [JsonRequired] public required string?      Detail     { get;               init; }
    [JsonRequired] public required StringTags?  Errors     { get => errors;     init => errors = value; }
    [JsonRequired] public required string?      Instance   { get;               init; }
    [JsonRequired] public required Status?      StatusCode { get => statusCode; init => statusCode = value; }
    [JsonRequired] public required string?      Title      { get;               init; }
    [JsonRequired] public required string?      Type       { get;               init; }


    public Error() : base() { }
    [SetsRequiredMembers] public Error( Status? statusCode, string? type, string? title, string? detail, string? instance, StringTags? errors ) : base()
    {
        this.statusCode = statusCode;
        this.errors     = errors;
        Detail          = detail;
        Instance        = instance;
        Title           = title;
        Type            = type;
    }


    public static implicit operator Error( Status     result ) => Create(result,            StringTags.Empty);
    public static implicit operator Error( string     error )  => Create(Status.BadRequest, new StringTags(error));
    public static implicit operator Error( Pair       error )  => Create(Status.BadRequest, error);
    public static implicit operator Error( Exception  error )  => Create(error);
    public static implicit operator Error( Pair[]     error )  => Create(Status.BadRequest, error);
    public static implicit operator Error( StringTags error )  => Create(Status.BadRequest, error);


    public Status GetStatus() => StatusCode ?? Status.Ok;


    public static Error Create<TValue>( TValue details )
        where TValue : IErrorDetails => new(details.StatusCode, details.Type, details.Title, details.Detail, details.Instance, details.Errors);
    public static Error Create( Status status, StringTags? errors = null, [CallerMemberName] string type = EMPTY )                                                          => new(status, type, null, null, null, errors);
    public static Error Create( Status status, string      type,          StringTags?               errors )                                                                => new(status, type, null, null, null, errors);
    public static Error Create( Status status, string      type,          params string[]           errors )                                                                => Create(status, type, null, null, null, errors);
    public static Error Create( Status status, string      type,          params Pair[]             errors )                                                                => Create(status, type, null, null, null, errors);
    public static Error Create( Status status, string      type,          string?                   title, StringTags?     errors )                                         => new(status, type, title, null, null, errors);
    public static Error Create( Status status, string      type,          string?                   title, params string[] errors )                                         => Create(status, type, title, null, null);
    public static Error Create( Status status, string      type,          string?                   title, params Pair[]   errors )                                         => Create(status, type, title, null, null);
    public static Error Create( Status status, string      type,          string?                   title, string?         detail, string? instance, StringTags?   errors ) => new(status, type, title, detail, instance, errors);
    public static Error Create( Status status, string      type,          string?                   title, string?         detail, string? instance, params Pair[] errors ) => new(status, type, title, detail, instance, errors);


    public static Error Create( Exception e, StringTags? errors = null, Status? status = null ) => Create(e, e.MethodSignature(), e.Source, errors, status);
    public static Error Create( Exception e, string? detail, string? instance, StringTags? errors = null, Status? status = null ) => new(status ?? Statuses.GetStatusFromException(e),
                                                                                                                                         e.GetType()
                                                                                                                                          .Name,
                                                                                                                                         e.Message,
                                                                                                                                         detail,
                                                                                                                                         instance,
                                                                                                                                         errors);


    public static Error Unauthorized( ref readonly PasswordValidator.Results results,  string?    instance                 = null, string? detail = null, string type = PASSWORD_VALIDATION_TYPE ) => Unauthorized(results.ToValues(), instance, detail, Titles.PasswordValidation, type);
    public static Error Unauthorized( string?                                instance, StringTags errors                   = default )                                                     => Unauthorized(errors, instance, title: Titles.InvalidCredentials);
    public static Error Unauthorized( StringTags?                            errors,   string?    instance, string? detail = null, string? title = null, string type = UNAUTHORIZED_TYPE ) => new(Status.Unauthorized, type, title ?? Titles.Unauthorized, detail, instance, errors);
    public static Error NoInternet( StringTags?                              errors = null )                                                                                                                => Disabled(errors, title: Titles.NoInternet,     type: NO_INTERNET_TYPE);
    public static Error WiFiDisabled( StringTags?                            errors = null )                                                                                                                => Disabled(errors, title: Titles.WiFiIsDisabled, type: NO_INTERNET_WIFI_TYPE);
    public static Error ExpiredSubscription( StringTags?                     errors = null, string? instance = null, string? detail = null, string? title = null, string type = EXPIRED_SUBSCRIPTION_TYPE ) => new(Status.PaymentRequired, type, title     ?? Titles.ExpiredSubscription, detail, instance, errors);
    public static Error Subscription( StringTags?                            errors = null, string? instance = null, string? detail = null, string? title = null, string type = INVALID_SUBSCRIPTION_TYPE ) => new(Status.PaymentRequired, type, title     ?? Titles.InvalidSubscription, detail, instance, errors);
    public static Error NoSubscription( StringTags?                          errors = null, string? instance = null, string? detail = null, string? title = null, string type = NO_SUBSCRIPTION_TYPE )      => new(Status.PaymentRequired, type, title     ?? Titles.NoSubscription, detail, instance, errors);
    public static Error Failure( StringTags?                                 errors = null, string? instance = null, string? detail = null, string? title = null, string type = GENERAL_FAILURE_TYPE )      => new(Status.PreconditionFailed, type, title  ?? Titles.General, detail, instance, errors);
    public static Error Unexpected( StringTags?                              errors = null, string? instance = null, string? detail = null, string? title = null, string type = UNEXPECTED_TYPE )           => new(Status.UnprocessableEntity, type, title ?? Titles.Unexpected, detail, instance, errors);
    public static Error Validation( StringTags?                              errors = null, string? instance = null, string? detail = null, string? title = null, string type = VALIDATION_TYPE )           => new(Status.BadRequest, type, title          ?? Titles.Validation, detail, instance, errors);
    public static Error DeviceName( StringTags?                              errors = null )                                                                                                                           => Validation(errors, title: Titles.MustHaveValidDeviceName);
    public static Error Host( StringTags?                                    errors = null )                                                                                                                           => Validation(errors, title: Titles.MustHaveValidIPHostName);
    public static Error Password( StringTags?                                errors = null )                                                                                                                           => Validation(errors, title: Titles.PasswordCannotBeEmpty);
    public static Error Port( StringTags?                                    errors = null )                                                                                                                           => Validation(errors, title: Titles.GivenPortIsNotAValidPortNumberInRangeOf1To65535);
    public static Error UserName( StringTags?                                errors = null )                                                                                                                           => Validation(errors, title: Titles.UserNameCannotBeEmpty);
    public static Error ServerIsUnavailable( StringTags?                     errors = null, string? instance = null, string? detail = null, string? title = null, string type = SERVER_IS_UNAVAILABLE_TYPE )           => new(Status.PreconditionFailed, type, title            ?? Titles.ServerIsUnavailable, detail, instance, errors);
    public static Error ServerIsOutdated( StringTags?                        errors = null, string? instance = null, string? detail = null, string? title = null, string type = SERVER_IS_OUTDATED_TYPE )              => new(Status.PreconditionFailed, type, title            ?? Titles.ServerIsOutdated, detail, instance, errors);
    public static Error ClientIsOutdated( StringTags?                        errors = null, string? instance = null, string? detail = null, string? title = null, string type = CLIENT_IS_OUTDATED_TYPE )              => new(Status.PreconditionFailed, type, title            ?? Titles.ClientIsOutdated, detail, instance, errors);
    public static Error ClientIsUnavailable( StringTags?                     errors = null, string? instance = null, string? detail = null, string? title = null, string type = CLIENT_IS_UNAVAILABLE_TYPE )           => new(Status.PreconditionFailed, type, title            ?? Titles.ClientIsUnavailable, detail, instance, errors);
    public static Error OperationCanceled( StringTags?                       errors = null, string? instance = null, string? detail = null, string? title = null, string type = CLIENT_OPERATION_CANCELED )            => new(Status.ClientClosedRequest, type, title           ?? Titles.ClientIsUnavailable, detail, instance, errors);
    public static Error NotSet( StringTags?                                  errors = null, string? instance = null, string? detail = null, string? title = null, string type = NOT_SET_TYPE )                         => new(Status.NotSet, type, title                        ?? Titles.NotSet, detail, instance, errors);
    public static Error Continue( StringTags?                                errors = null, string? instance = null, string? detail = null, string? title = null, string type = CONTINUE_TYPE )                        => new(Status.Continue, type, title                      ?? Titles.Continue, detail, instance, errors);
    public static Error SwitchingProtocols( StringTags?                      errors = null, string? instance = null, string? detail = null, string? title = null, string type = SWITCHING_PROTOCOLS_TYPE )             => new(Status.SwitchingProtocols, type, title            ?? Titles.SwitchingProtocols, detail, instance, errors);
    public static Error Processing( StringTags?                              errors = null, string? instance = null, string? detail = null, string? title = null, string type = PROCESSING_TYPE )                      => new(Status.Processing, type, title                    ?? Titles.Processing, detail, instance, errors);
    public static Error EarlyHints( StringTags?                              errors = null, string? instance = null, string? detail = null, string? title = null, string type = EARLY_HINTS_TYPE )                     => new(Status.EarlyHints, type, title                    ?? Titles.EarlyHints, detail, instance, errors);
    public static Error Ok( StringTags?                                      errors = null, string? instance = null, string? detail = null, string? title = null, string type = OK_TYPE )                              => new(Status.Ok, type, title                            ?? Titles.Ok, detail, instance, errors);
    public static Error Created( StringTags?                                 errors = null, string? instance = null, string? detail = null, string? title = null, string type = CREATED_TYPE )                         => new(Status.Created, type, title                       ?? Titles.Created, detail, instance, errors);
    public static Error Accepted( StringTags?                                errors = null, string? instance = null, string? detail = null, string? title = null, string type = ACCEPTED_TYPE )                        => new(Status.Accepted, type, title                      ?? Titles.Accepted, detail, instance, errors);
    public static Error NonAuthoritativeInformation( StringTags?             errors = null, string? instance = null, string? detail = null, string? title = null, string type = NON_AUTHORITATIVE_INFORMATION_TYPE )   => new(Status.NonAuthoritativeInformation, type, title   ?? Titles.NonAuthoritativeInformation, detail, instance, errors);
    public static Error NoContent( StringTags?                               errors = null, string? instance = null, string? detail = null, string? title = null, string type = NO_CONTENT_TYPE )                      => new(Status.NoContent, type, title                     ?? Titles.NoContent, detail, instance, errors);
    public static Error ResetContent( StringTags?                            errors = null, string? instance = null, string? detail = null, string? title = null, string type = RESET_CONTENT_TYPE )                   => new(Status.ResetContent, type, title                  ?? Titles.ResetContent, detail, instance, errors);
    public static Error PartialContent( StringTags?                          errors = null, string? instance = null, string? detail = null, string? title = null, string type = PARTIAL_CONTENT_TYPE )                 => new(Status.PartialContent, type, title                ?? Titles.PartialContent, detail, instance, errors);
    public static Error MultiStatus( StringTags?                             errors = null, string? instance = null, string? detail = null, string? title = null, string type = MULTI_STATUS_TYPE )                    => new(Status.MultiStatus, type, title                   ?? Titles.MultiStatus, detail, instance, errors);
    public static Error AlreadyReported( StringTags?                         errors = null, string? instance = null, string? detail = null, string? title = null, string type = ALREADY_REPORTED_TYPE )                => new(Status.AlreadyReported, type, title               ?? Titles.AlreadyReported, detail, instance, errors);
    public static Error ImUsed( StringTags?                                  errors = null, string? instance = null, string? detail = null, string? title = null, string type = IM_USED_TYPE )                         => new(Status.ImUsed, type, title                        ?? Titles.ImUsed, detail, instance, errors);
    public static Error Ambiguous( StringTags?                               errors = null, string? instance = null, string? detail = null, string? title = null, string type = AMBIGUOUS_TYPE )                       => new(Status.Ambiguous, type, title                     ?? Titles.Ambiguous, detail, instance, errors);
    public static Error Moved( StringTags?                                   errors = null, string? instance = null, string? detail = null, string? title = null, string type = MOVED_TYPE )                           => new(Status.Moved, type, title                         ?? Titles.Moved, detail, instance, errors);
    public static Error Found( StringTags?                                   errors = null, string? instance = null, string? detail = null, string? title = null, string type = FOUND_TYPE )                           => new(Status.Found, type, title                         ?? Titles.Found, detail, instance, errors);
    public static Error RedirectMethod( StringTags?                          errors = null, string? instance = null, string? detail = null, string? title = null, string type = REDIRECT_METHOD_TYPE )                 => new(Status.RedirectMethod, type, title                ?? Titles.RedirectMethod, detail, instance, errors);
    public static Error NotModified( StringTags?                             errors = null, string? instance = null, string? detail = null, string? title = null, string type = NOT_MODIFIED_TYPE )                    => new(Status.NotModified, type, title                   ?? Titles.NotModified, detail, instance, errors);
    public static Error UseProxy( StringTags?                                errors = null, string? instance = null, string? detail = null, string? title = null, string type = USE_PROXY_TYPE )                       => new(Status.UseProxy, type, title                      ?? Titles.UseProxy, detail, instance, errors);
    public static Error Unused( StringTags?                                  errors = null, string? instance = null, string? detail = null, string? title = null, string type = UNUSED_TYPE )                          => new(Status.Unused, type, title                        ?? Titles.Unused, detail, instance, errors);
    public static Error RedirectKeepVerb( StringTags?                        errors = null, string? instance = null, string? detail = null, string? title = null, string type = REDIRECT_KEEP_VERB_TYPE )              => new(Status.RedirectKeepVerb, type, title              ?? Titles.RedirectKeepVerb, detail, instance, errors);
    public static Error PermanentRedirect( StringTags?                       errors = null, string? instance = null, string? detail = null, string? title = null, string type = PERMANENT_REDIRECT_TYPE )              => new(Status.PermanentRedirect, type, title             ?? Titles.PermanentRedirect, detail, instance, errors);
    public static Error BadRequest( StringTags?                              errors = null, string? instance = null, string? detail = null, string? title = null, string type = BAD_REQUEST_TYPE )                     => new(Status.BadRequest, type, title                    ?? Titles.BadRequest, detail, instance, errors);
    public static Error PaymentRequired( StringTags?                         errors = null, string? instance = null, string? detail = null, string? title = null, string type = PAYMENT_REQUIRED_TYPE )                => new(Status.PaymentRequired, type, title               ?? Titles.PaymentRequired, detail, instance, errors);
    public static Error Forbidden( StringTags?                               errors = null, string? instance = null, string? detail = null, string? title = null, string type = FORBIDDEN_TYPE )                       => new(Status.Forbidden, type, title                     ?? Titles.Forbidden, detail, instance, errors);
    public static Error NotFound( StringTags?                                errors = null, string? instance = null, string? detail = null, string? title = null, string type = NOT_FOUND_TYPE )                       => new(Status.NotFound, type, title                      ?? Titles.NotFound, detail, instance, errors);
    public static Error MethodNotAllowed( StringTags?                        errors = null, string? instance = null, string? detail = null, string? title = null, string type = METHOD_NOT_ALLOWED_TYPE )              => new(Status.MethodNotAllowed, type, title              ?? Titles.MethodNotAllowed, detail, instance, errors);
    public static Error NotAcceptable( StringTags?                           errors = null, string? instance = null, string? detail = null, string? title = null, string type = NOT_ACCEPTABLE_TYPE )                  => new(Status.NotAcceptable, type, title                 ?? Titles.NotAcceptable, detail, instance, errors);
    public static Error ProxyAuthenticationRequired( StringTags?             errors = null, string? instance = null, string? detail = null, string? title = null, string type = PROXY_AUTHENTICATION_REQUIRED_TYPE )   => new(Status.ProxyAuthenticationRequired, type, title   ?? Titles.ProxyAuthenticationRequired, detail, instance, errors);
    public static Error RequestTimeout( StringTags?                          errors = null, string? instance = null, string? detail = null, string? title = null, string type = REQUEST_TIMEOUT_TYPE )                 => new(Status.RequestTimeout, type, title                ?? Titles.RequestTimeout, detail, instance, errors);
    public static Error Conflict( StringTags?                                errors = null, string? instance = null, string? detail = null, string? title = null, string type = CONFLICT_TYPE )                        => new(Status.Conflict, type, title                      ?? Titles.Conflict, detail, instance, errors);
    public static Error Gone( StringTags?                                    errors = null, string? instance = null, string? detail = null, string? title = null, string type = GONE_TYPE )                            => new(Status.Gone, type, title                          ?? Titles.Gone, detail, instance, errors);
    public static Error LengthRequired( StringTags?                          errors = null, string? instance = null, string? detail = null, string? title = null, string type = LENGTH_REQUIRED_TYPE )                 => new(Status.LengthRequired, type, title                ?? Titles.LengthRequired, detail, instance, errors);
    public static Error PreconditionFailed( StringTags?                      errors = null, string? instance = null, string? detail = null, string? title = null, string type = PRECONDITION_FAILED_TYPE )             => new(Status.PreconditionFailed, type, title            ?? Titles.PreconditionFailed, detail, instance, errors);
    public static Error RequestEntityTooLarge( StringTags?                   errors = null, string? instance = null, string? detail = null, string? title = null, string type = REQUEST_ENTITY_TOO_LARGE_TYPE )        => new(Status.RequestEntityTooLarge, type, title         ?? Titles.RequestEntityTooLarge, detail, instance, errors);
    public static Error RequestUriTooLong( StringTags?                       errors = null, string? instance = null, string? detail = null, string? title = null, string type = REQUEST_URI_TOO_LONG_TYPE )            => new(Status.RequestUriTooLong, type, title             ?? Titles.RequestUriTooLong, detail, instance, errors);
    public static Error UnsupportedMediaType( StringTags?                    errors = null, string? instance = null, string? detail = null, string? title = null, string type = UNSUPPORTED_MEDIA_TYPE_TYPE )          => new(Status.UnsupportedMediaType, type, title          ?? Titles.UnsupportedMediaType, detail, instance, errors);
    public static Error RequestedRangeNotSatisfiable( StringTags?            errors = null, string? instance = null, string? detail = null, string? title = null, string type = REQUESTED_RANGE_NOT_SATISFIABLE_TYPE ) => new(Status.RequestedRangeNotSatisfiable, type, title  ?? Titles.RequestedRangeNotSatisfiable, detail, instance, errors);
    public static Error ExpectationFailed( StringTags?                       errors = null, string? instance = null, string? detail = null, string? title = null, string type = EXPECTATION_FAILED_TYPE )              => new(Status.ExpectationFailed, type, title             ?? Titles.ExpectationFailed, detail, instance, errors);
    public static Error Teapot( StringTags?                                  errors = null, string? instance = null, string? detail = null, string? title = null, string type = TEAPOT_TYPE )                          => new(Status.Teapot, type, title                        ?? Titles.Teapot, detail, instance, errors);
    public static Error MisdirectedRequest( StringTags?                      errors = null, string? instance = null, string? detail = null, string? title = null, string type = MISDIRECTED_REQUEST_TYPE )             => new(Status.MisdirectedRequest, type, title            ?? Titles.MisdirectedRequest, detail, instance, errors);
    public static Error UnprocessableEntity( StringTags?                     errors = null, string? instance = null, string? detail = null, string? title = null, string type = UNPROCESSABLE_ENTITY_TYPE )            => new(Status.UnprocessableEntity, type, title           ?? Titles.UnprocessableEntity, detail, instance, errors);
    public static Error Locked( StringTags?                                  errors = null, string? instance = null, string? detail = null, string? title = null, string type = LOCKED_TYPE )                          => new(Status.Locked, type, title                        ?? Titles.Locked, detail, instance, errors);
    public static Error FailedDependency( StringTags?                        errors = null, string? instance = null, string? detail = null, string? title = null, string type = FAILED_DEPENDENCY_TYPE )               => new(Status.FailedDependency, type, title              ?? Titles.FailedDependency, detail, instance, errors);
    public static Error TooEarly( StringTags?                                errors = null, string? instance = null, string? detail = null, string? title = null, string type = TOO_EARLY_TYPE )                       => new(Status.TooEarly, type, title                      ?? Titles.TooEarly, detail, instance, errors);
    public static Error UpgradeRequired( StringTags?                         errors = null, string? instance = null, string? detail = null, string? title = null, string type = UPGRADE_REQUIRED_TYPE )                => new(Status.UpgradeRequired, type, title               ?? Titles.UpgradeRequired, detail, instance, errors);
    public static Error PreconditionRequired( StringTags?                    errors = null, string? instance = null, string? detail = null, string? title = null, string type = PRECONDITION_REQUIRED_TYPE )           => new(Status.PreconditionRequired, type, title          ?? Titles.PreconditionRequired, detail, instance, errors);
    public static Error TooManyRequests( StringTags?                         errors = null, string? instance = null, string? detail = null, string? title = null, string type = TOO_MANY_REQUESTS_TYPE )               => new(Status.TooManyRequests, type, title               ?? Titles.TooManyRequests, detail, instance, errors);
    public static Error Disabled( StringTags?                                errors = null, string? instance = null, string? detail = null, string? title = null, string type = DISABLED_TYPE )                        => new(Status.Disabled, type, title                      ?? Titles.Disabled, detail, instance, errors);
    public static Error RequestHeaderFieldsTooLarge( StringTags?             errors = null, string? instance = null, string? detail = null, string? title = null, string type = REQUEST_HEADER_FIELDS_TOO_LARGE_TYPE ) => new(Status.RequestHeaderFieldsTooLarge, type, title   ?? Titles.RequestHeaderFieldsTooLarge, detail, instance, errors);
    public static Error AlreadyExists( StringTags?                           errors = null, string? instance = null, string? detail = null, string? title = null, string type = ALREADY_EXISTS_TYPE )                  => new(Status.AlreadyExists, type, title                 ?? Titles.AlreadyExists, detail, instance, errors);
    public static Error NoResponse( StringTags?                              errors = null, string? instance = null, string? detail = null, string? title = null, string type = NO_RESPONSE_TYPE )                     => new(Status.NoResponse, type, title                    ?? Titles.NoResponse, detail, instance, errors);
    public static Error UnavailableForLegalReasons( StringTags?              errors = null, string? instance = null, string? detail = null, string? title = null, string type = UNAVAILABLE_FOR_LEGAL_REASONS_TYPE )   => new(Status.UnavailableForLegalReasons, type, title    ?? Titles.UnavailableForLegalReasons, detail, instance, errors);
    public static Error SessionExpired( StringTags?                          errors = null, string? instance = null, string? detail = null, string? title = null, string type = SESSION_EXPIRED_TYPE )                 => new(Status.SessionExpired, type, title                ?? Titles.SessionExpired, detail, instance, errors);
    public static Error ClientClosedRequest( StringTags?                     errors = null, string? instance = null, string? detail = null, string? title = null, string type = CLIENT_CLOSED_REQUEST_TYPE )           => new(Status.ClientClosedRequest, type, title           ?? Titles.ClientClosedRequest, detail, instance, errors);
    public static Error InternalServerError( StringTags?                     errors = null, string? instance = null, string? detail = null, string? title = null, string type = INTERNAL_SERVER_ERROR_TYPE )           => new(Status.InternalServerError, type, title           ?? Titles.InternalServerError, detail, instance, errors);
    public static Error NotImplemented( StringTags?                          errors = null, string? instance = null, string? detail = null, string? title = null, string type = NOT_IMPLEMENTED_TYPE )                 => new(Status.NotImplemented, type, title                ?? Titles.NotImplemented, detail, instance, errors);
    public static Error BadGateway( StringTags?                              errors = null, string? instance = null, string? detail = null, string? title = null, string type = BAD_GATEWAY_TYPE )                     => new(Status.BadGateway, type, title                    ?? Titles.BadGateway, detail, instance, errors);
    public static Error ServiceUnavailable( StringTags?                      errors = null, string? instance = null, string? detail = null, string? title = null, string type = SERVICE_UNAVAILABLE_TYPE )             => new(Status.ServiceUnavailable, type, title            ?? Titles.ServiceUnavailable, detail, instance, errors);
    public static Error GatewayTimeout( StringTags?                          errors = null, string? instance = null, string? detail = null, string? title = null, string type = GATEWAY_TIMEOUT_TYPE )                 => new(Status.GatewayTimeout, type, title                ?? Titles.GatewayTimeout, detail, instance, errors);
    public static Error HttpVersionNotSupported( StringTags?                 errors = null, string? instance = null, string? detail = null, string? title = null, string type = HTTP_VERSION_NOT_SUPPORTED_TYPE )      => new(Status.HttpVersionNotSupported, type, title       ?? Titles.HttpVersionNotSupported, detail, instance, errors);
    public static Error VariantAlsoNegotiates( StringTags?                   errors = null, string? instance = null, string? detail = null, string? title = null, string type = VARIANT_ALSO_NEGOTIATES_TYPE )         => new(Status.VariantAlsoNegotiates, type, title         ?? Titles.VariantAlsoNegotiates, detail, instance, errors);
    public static Error InsufficientStorage( StringTags?                     errors = null, string? instance = null, string? detail = null, string? title = null, string type = INSUFFICIENT_STORAGE_TYPE )            => new(Status.InsufficientStorage, type, title           ?? Titles.InsufficientStorage, detail, instance, errors);
    public static Error LoopDetected( StringTags?                            errors = null, string? instance = null, string? detail = null, string? title = null, string type = LOOP_DETECTED_TYPE )                   => new(Status.LoopDetected, type, title                  ?? Titles.LoopDetected, detail, instance, errors);
    public static Error NotExtended( StringTags?                             errors = null, string? instance = null, string? detail = null, string? title = null, string type = NOT_EXTENDED_TYPE )                    => new(Status.NotExtended, type, title                   ?? Titles.NotExtended, detail, instance, errors);
    public static Error NetworkAuthenticationRequired( StringTags?           errors = null, string? instance = null, string? detail = null, string? title = null, string type = NETWORK_AUTHENTICATION_REQUIRED_TYPE ) => new(Status.NetworkAuthenticationRequired, type, title ?? Titles.NetworkAuthenticationRequired, detail, instance, errors);


    public int CompareTo( Error? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        int statusCodeComparison = Nullable.Compare(statusCode, other.statusCode);
        if ( statusCodeComparison != 0 ) { return statusCodeComparison; }

        int typeComparison = string.Compare(Type, other.Type, StringComparison.Ordinal);
        if ( typeComparison != 0 ) { return typeComparison; }

        int titleComparison = string.Compare(Title, other.Title, StringComparison.Ordinal);
        if ( titleComparison != 0 ) { return titleComparison; }

        int detailComparison = string.Compare(Detail, other.Detail, StringComparison.Ordinal);
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

        return Nullable.Equals(statusCode, other.statusCode) && string.Equals(Type, other.Type, StringComparison.Ordinal) && string.Equals(Title, other.Title, StringComparison.Ordinal) && string.Equals(Detail, other.Detail, StringComparison.Ordinal) && string.Equals(Instance, other.Instance, StringComparison.Ordinal);
    }
    public override bool Equals( object? other )                  => ReferenceEquals(this, other) || ( other is Error error && Equals(error) );
    public override int  GetHashCode()                            => HashCode.Combine(statusCode, Type, Title, Detail, Instance, errors);
    public static   bool operator ==( Error? left, Error? right ) => EqualityComparer<Error>.Default.Equals(left, right);
    public static   bool operator !=( Error? left, Error? right ) => !EqualityComparer<Error>.Default.Equals(left, right);
    public static   bool operator >( Error   left, Error  right ) => Comparer<Error>.Default.Compare(left, right) > 0;
    public static   bool operator >=( Error  left, Error  right ) => Comparer<Error>.Default.Compare(left, right) >= 0;
    public static   bool operator <( Error   left, Error  right ) => Comparer<Error>.Default.Compare(left, right) < 0;
    public static   bool operator <=( Error  left, Error  right ) => Comparer<Error>.Default.Compare(left, right) <= 0;
}
