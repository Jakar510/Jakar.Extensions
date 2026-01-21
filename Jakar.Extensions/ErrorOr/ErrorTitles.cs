// Jakar.Extensions :: Jakar.Extensions
// 06/17/2024  16:06

namespace Jakar.Extensions;


public sealed class ErrorTitles : IErrorTitles
{
    public static readonly ErrorTitles Instance = new ErrorTitles();


    public        string       Accepted                                        => "Accepted";
    public        string       AlreadyExists                                   => "Already Exists";
    public        string       AlreadyReported                                 => "Already Reported";
    public        string       Ambiguous                                       => "Ambiguous";
    public        string       BadGateway                                      => "Bad Gateway";
    public        string       BadRequest                                      => "Bad Request";
    public        string       BlockedPassed                                   => "Password cannot be a blocked password";
    public        string       ClientClosedRequest                             => "Client Closed Request";
    public        string       ClientIsOutdated                                => "Client is outdated";
    public        string       ClientIsUnavailable                             => "Client is unavailable";
    public        string       Conflict                                        => "A conflict has occurred";
    public        string       Continue                                        => "Continue";
    public        string       Created                                         => "Created";
    public        string       Disabled                                        => "User is disabled";
    public        string       EarlyHints                                      => "Early Hints";
    public        string       ExpectationFailed                               => "Expectation Failed";
    public        string       ExpiredSubscription                             => "User's subscription is expired";
    public        string       FailedDependency                                => "FailedDependency";
    public        string       Forbidden                                       => "A 'Forbidden' has occurred";
    public        string       Found                                           => "Found";
    public        string       GatewayTimeout                                  => "Gateway Timeout";
    public        string       General                                         => "A failure has occurred";
    public        string       GivenPortIsNotAValidPortNumberInRangeOf1To65535 => "Given port is not a valid port number in range of 1 to 65535";
    public        string       Gone                                            => "Gone";
    public        string       HttpVersionNotSupported                         => "Http Version Not Supported";
    public        string       ImUsed                                          => "ImUsed";
    public        string       InsufficientStorage                             => "Insufficient Storage";
    public        string       InternalServerError                             => "Internal Server Error";
    public        string       InvalidCredentials                              => "Invalid credentials";
    public        string       InvalidSubscription                             => "User's subscription is no longer valid";
    public        string       InvalidValue                                    => "Invalid value";
    public        string       LengthPassed                                    => "Password not long enough";
    public        string       LengthRequired                                  => "Length Required";
    public        string       Locked                                          => "User is locked";
    public        string       LoopDetected                                    => "LoopDetected";
    public        string       LowerPassed                                     => "Password must contain a lower case character";
    public        string       MethodNotAllowed                                => "Method Not Allowed";
    public        string       MisdirectedRequest                              => "Misdirected Request";
    public        string       Moved                                           => "Moved";
    public        string       MultiStatus                                     => "Multi Status";
    public        string       MustBeTrimmed                                   => "Password must be trimmed";
    public        string       MustHaveValidDeviceName                         => "Must have valid device name";
    public        string       MustHaveValidIPHostName                         => "Must have valid IP host name";
    public        string       NetworkAuthenticationRequired                   => "NetworkAuthenticationRequired";
    public        string       NoContent                                       => "No Content";
    public        string       NoInternet                                      => "No internet";
    public        string       NonAuthoritativeInformation                     => "Non Authoritative Information";
    public        string       NoResponse                                      => "NoResponse";
    public        string       NoSubscription                                  => "User is not subscribed";
    public        string       NotAcceptable                                   => "Not Acceptable";
    public        string       NotExtended                                     => "Not Extended";
    public        string       NotFound                                        => "A 'Not Found' has occurred";
    public        string       NotImplemented                                  => "Not Implemented";
    public        string       NotModified                                     => "Not Modified";
    public        string       NotSet                                          => "NotSet";
    public        string       NumericPassed                                   => "Password must contain a number";
    public        string       Ok                                              => "Ok";
    public        string       PartialContent                                  => "Partial Content";
    public        string       PasswordCannotBeEmpty                           => "Password cannot be empty";
    public        string       PasswordValidation                              => "Password validation failed";
    public        string       PaymentRequired                                 => "Payment Required";
    public        string       PermanentRedirect                               => "Permanent Redirect";
    public        string       PreconditionFailed                              => "Precondition Failed";
    public        string       PreconditionRequired                            => "Precondition Required";
    public        string       Processing                                      => "Processing";
    public        string       ProxyAuthenticationRequired                     => "ProxyAuthenticationRequired";
    public        string       RedirectKeepVerb                                => "RedirectKeepVerb";
    public        string       RedirectMethod                                  => "RedirectMethod";
    public        string       RequestedRangeNotSatisfiable                    => "RequestedRangeNotSatisfiable";
    public        string       RequestEntityTooLarge                           => "RequestEntityTooLarge";
    public        string       RequestHeaderFieldsTooLarge                     => "RequestHeaderFieldsTooLarge";
    public        string       RequestTimeout                                  => "RequestTimeout";
    public        string       RequestUriTooLong                               => "RequestUriTooLong";
    public        string       ResetContent                                    => "ResetContent";
    public        string       ServerIsOutdated                                => "Server Is outdated";
    public        string       ServerIsUnavailable                             => "Server Is unavailable";
    public        string       ServiceUnavailable                              => "ServiceUnavailable";
    public        string       SessionExpired                                  => "SessionExpired";
    public        string       SpecialPassed                                   => "Password must contain a special character";
    public        string       SwitchingProtocols                              => "SwitchingProtocols";
    public        string       Teapot                                          => "Teapot";
    public        string       TooEarly                                        => "TooEarly";
    public        string       TooManyRequests                                 => "TooManyRequests";
    public        string       Unauthorized                                    => "A 'Unauthorized' has occurred";
    public        string       UnavailableForLegalReasons                      => "UnavailableForLegalReasons";
    public        string       Unexpected                                      => "A unexpected has occurred";
    public        string       UnprocessableEntity                             => "UnprocessableEntity";
    public        string       UnsupportedMediaType                            => "UnsupportedMediaType";
    public        string       Unused                                          => "Unused";
    public        string       UpgradeRequired                                 => "UpgradeRequired";
    public        string       UpperPassed                                     => "Password must contain a upper case character";
    public        string       UseProxy                                        => "UseProxy";
    public        string       UserNameCannotBeEmpty                           => "User Name cannot be empty";
    public        string       Validation                                      => "A validation has occurred";
    public        string       VariantAlsoNegotiates                           => "VariantAlsoNegotiates";
    public        string       WiFiIsDisabled                                  => "WiFi is disabled";

    private ErrorTitles() { }
}
