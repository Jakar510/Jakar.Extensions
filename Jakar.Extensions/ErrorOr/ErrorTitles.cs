// Jakar.Extensions :: Jakar.Extensions
// 06/17/2024  16:06

namespace Jakar.Extensions;


public interface IErrorTitles
{
    string Accepted                                        { get; }
    string AlreadyExists                                   { get; }
    string AlreadyReported                                 { get; }
    string Ambiguous                                       { get; }
    string BadGateway                                      { get; }
    string BadRequest                                      { get; }
    string BlockedPassed                                   { get; }
    string ClientClosedRequest                             { get; }
    string ClientIsOutdated                                { get; }
    string ClientIsUnavailable                             { get; }
    string Conflict                                        { get; }
    string Continue                                        { get; }
    string Created                                         { get; }
    string Disabled                                        { get; }
    string EarlyHints                                      { get; }
    string ExpectationFailed                               { get; }
    string ExpiredSubscription                             { get; }
    string FailedDependency                                { get; }
    string Forbidden                                       { get; }
    string Found                                           { get; }
    string GatewayTimeout                                  { get; }
    string General                                         { get; }
    string GivenPortIsNotAValidPortNumberInRangeOf1To65535 { get; }
    string Gone                                            { get; }
    string HttpVersionNotSupported                         { get; }
    string ImUsed                                          { get; }
    string InsufficientStorage                             { get; }
    string InternalServerError                             { get; }
    string InvalidCredentials                              { get; }
    string InvalidSubscription                             { get; }
    string InvalidValue                                    { get; }
    string LengthPassed                                    { get; }
    string LengthRequired                                  { get; }
    string Locked                                          { get; }
    string LoopDetected                                    { get; }
    string LowerPassed                                     { get; }
    string MethodNotAllowed                                { get; }
    string MisdirectedRequest                              { get; }
    string Moved                                           { get; }
    string MultiStatus                                     { get; }
    string MustBeTrimmed                                   { get; }
    string MustHaveValidDeviceName                         { get; }
    string MustHaveValidIPHostName                         { get; }
    string NetworkAuthenticationRequired                   { get; }
    string NoContent                                       { get; }
    string NoInternet                                      { get; }
    string NonAuthoritativeInformation                     { get; }
    string NoResponse                                      { get; }
    string NoSubscription                                  { get; }
    string NotAcceptable                                   { get; }
    string NotExtended                                     { get; }
    string NotFound                                        { get; }
    string NotImplemented                                  { get; }
    string NotModified                                     { get; }
    string NotSet                                          { get; }
    string NumericPassed                                   { get; }
    string Ok                                              { get; }
    string PartialContent                                  { get; }
    string PasswordCannotBeEmpty                           { get; }
    string PasswordValidation                              { get; }
    string PaymentRequired                                 { get; }
    string PermanentRedirect                               { get; }
    string PreconditionFailed                              { get; }
    string PreconditionRequired                            { get; }
    string Processing                                      { get; }
    string ProxyAuthenticationRequired                     { get; }
    string RedirectKeepVerb                                { get; }
    string RedirectMethod                                  { get; }
    string RequestedRangeNotSatisfiable                    { get; }
    string RequestEntityTooLarge                           { get; }
    string RequestHeaderFieldsTooLarge                     { get; }
    string RequestTimeout                                  { get; }
    string RequestUriTooLong                               { get; }
    string ResetContent                                    { get; }
    string ServerIsOutdated                                { get; }
    string ServerIsUnavailable                             { get; }
    string ServiceUnavailable                              { get; }
    string SessionExpired                                  { get; }
    string SpecialPassed                                   { get; }
    string SwitchingProtocols                              { get; }
    string Teapot                                          { get; }
    string TooEarly                                        { get; }
    string TooManyRequests                                 { get; }
    string Unauthorized                                    { get; }
    string UnavailableForLegalReasons                      { get; }
    string Unexpected                                      { get; }
    string UnprocessableEntity                             { get; }
    string UnsupportedMediaType                            { get; }
    string Unused                                          { get; }
    string UpgradeRequired                                 { get; }
    string UpperPassed                                     { get; }
    string UseProxy                                        { get; }
    string UserNameCannotBeEmpty                           { get; }
    string Validation                                      { get; }
    string VariantAlsoNegotiates                           { get; }
    string WiFiIsDisabled                                  { get; }
}



public static class ErrorTitles
{
    public static string GetErrorTitle( this IErrorTitles titles, Status status )
    {
        return status switch
               {
                   Status.NotSet                        => titles.NotSet,
                   Status.Continue                      => titles.Continue,
                   Status.SwitchingProtocols            => titles.SwitchingProtocols,
                   Status.Processing                    => titles.Processing,
                   Status.EarlyHints                    => titles.EarlyHints,
                   Status.Ok                            => titles.Ok,
                   Status.Created                       => titles.Created,
                   Status.Accepted                      => titles.Accepted,
                   Status.NonAuthoritativeInformation   => titles.NonAuthoritativeInformation,
                   Status.NoContent                     => titles.NoContent,
                   Status.ResetContent                  => titles.ResetContent,
                   Status.PartialContent                => titles.PartialContent,
                   Status.MultiStatus                   => titles.MultiStatus,
                   Status.AlreadyReported               => titles.AlreadyReported,
                   Status.ImUsed                        => titles.ImUsed,
                   Status.Ambiguous                     => titles.Ambiguous,
                   Status.Moved                         => titles.Moved,
                   Status.Found                         => titles.Found,
                   Status.RedirectMethod                => titles.RedirectMethod,
                   Status.NotModified                   => titles.NotModified,
                   Status.UseProxy                      => titles.UseProxy,
                   Status.Unused                        => titles.Unused,
                   Status.RedirectKeepVerb              => titles.RedirectKeepVerb,
                   Status.PermanentRedirect             => titles.PermanentRedirect,
                   Status.BadRequest                    => titles.BadRequest,
                   Status.Unauthorized                  => titles.Unauthorized,
                   Status.PaymentRequired               => titles.PaymentRequired,
                   Status.Forbidden                     => titles.Forbidden,
                   Status.NotFound                      => titles.NotFound,
                   Status.MethodNotAllowed              => titles.MethodNotAllowed,
                   Status.NotAcceptable                 => titles.NotAcceptable,
                   Status.ProxyAuthenticationRequired   => titles.ProxyAuthenticationRequired,
                   Status.RequestTimeout                => titles.RequestTimeout,
                   Status.Conflict                      => titles.Conflict,
                   Status.Gone                          => titles.Gone,
                   Status.LengthRequired                => titles.LengthRequired,
                   Status.PreconditionFailed            => titles.PreconditionFailed,
                   Status.RequestEntityTooLarge         => titles.RequestEntityTooLarge,
                   Status.RequestUriTooLong             => titles.RequestUriTooLong,
                   Status.UnsupportedMediaType          => titles.UnsupportedMediaType,
                   Status.RequestedRangeNotSatisfiable  => titles.RequestedRangeNotSatisfiable,
                   Status.ExpectationFailed             => titles.ExpectationFailed,
                   Status.Teapot                        => titles.Teapot,
                   Status.MisdirectedRequest            => titles.MisdirectedRequest,
                   Status.UnprocessableEntity           => titles.UnprocessableEntity,
                   Status.Locked                        => titles.Locked,
                   Status.FailedDependency              => titles.FailedDependency,
                   Status.TooEarly                      => titles.TooEarly,
                   Status.UpgradeRequired               => titles.UpgradeRequired,
                   Status.PreconditionRequired          => titles.PreconditionFailed,
                   Status.TooManyRequests               => titles.TooManyRequests,
                   Status.Disabled                      => titles.Disabled,
                   Status.RequestHeaderFieldsTooLarge   => titles.RequestHeaderFieldsTooLarge,
                   Status.AlreadyExists                 => titles.AlreadyExists,
                   Status.NoResponse                    => titles.NoResponse,
                   Status.UnavailableForLegalReasons    => titles.UnavailableForLegalReasons,
                   Status.SessionExpired                => titles.SessionExpired,
                   Status.ClientClosedRequest           => titles.ClientClosedRequest,
                   Status.InternalServerError           => titles.InternalServerError,
                   Status.NotImplemented                => titles.NotImplemented,
                   Status.BadGateway                    => titles.BadGateway,
                   Status.ServiceUnavailable            => titles.ServerIsUnavailable,
                   Status.GatewayTimeout                => titles.GatewayTimeout,
                   Status.HttpVersionNotSupported       => titles.HttpVersionNotSupported,
                   Status.VariantAlsoNegotiates         => titles.VariantAlsoNegotiates,
                   Status.InsufficientStorage           => titles.InsufficientStorage,
                   Status.LoopDetected                  => titles.LoopDetected,
                   Status.NotExtended                   => titles.NotExtended,
                   Status.NetworkAuthenticationRequired => titles.NetworkAuthenticationRequired,
                   _                                    => throw new ArgumentOutOfRangeException(nameof(status), status, null)
               };
    }



    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public sealed class Defaults : IErrorTitles
    {
        public static readonly IErrorTitles Instance = new Defaults();
        public                 string       Accepted                                        => "Accepted";
        public                 string       AlreadyExists                                   => "Already Exists";
        public                 string       AlreadyReported                                 => "Already Reported";
        public                 string       Ambiguous                                       => "Ambiguous";
        public                 string       BadGateway                                      => "Bad Gateway";
        public                 string       BadRequest                                      => "Bad Request";
        public                 string       BlockedPassed                                   => "Password cannot be a blocked password";
        public                 string       ClientClosedRequest                             => "Client Closed Request";
        public                 string       ClientIsOutdated                                => "Client is outdated";
        public                 string       ClientIsUnavailable                             => "Client is unavailable";
        public                 string       Conflict                                        => "A conflict has occurred";
        public                 string       Continue                                        => "Continue";
        public                 string       Created                                         => "Created";
        public                 string       Disabled                                        => "User is disabled";
        public                 string       EarlyHints                                      => "Early Hints";
        public                 string       ExpectationFailed                               => "Expectation Failed";
        public                 string       ExpiredSubscription                             => "User's subscription is expired";
        public                 string       FailedDependency                                => "FailedDependency";
        public                 string       Forbidden                                       => "A 'Forbidden' has occurred";
        public                 string       Found                                           => "Found";
        public                 string       GatewayTimeout                                  => "Gateway Timeout";
        public                 string       General                                         => "A failure has occurred";
        public                 string       GivenPortIsNotAValidPortNumberInRangeOf1To65535 => "Given port is not a valid port number in range of 1 to 65535";
        public                 string       Gone                                            => "Gone";
        public                 string       HttpVersionNotSupported                         => "Http Version Not Supported";
        public                 string       ImUsed                                          => "ImUsed";
        public                 string       InsufficientStorage                             => "Insufficient Storage";
        public                 string       InternalServerError                             => "Internal Server Error";
        public                 string       InvalidCredentials                              => "Invalid credentials";
        public                 string       InvalidSubscription                             => "User's subscription is no longer valid";
        public                 string       InvalidValue                                    => "Invalid value";
        public                 string       LengthPassed                                    => "Password not long enough";
        public                 string       LengthRequired                                  => "Length Required";
        public                 string       Locked                                          => "User is locked";
        public                 string       LoopDetected                                    => "LoopDetected";
        public                 string       LowerPassed                                     => "Password must contain a lower case character";
        public                 string       MethodNotAllowed                                => "Method Not Allowed";
        public                 string       MisdirectedRequest                              => "Misdirected Request";
        public                 string       Moved                                           => "Moved";
        public                 string       MultiStatus                                     => "Multi Status";
        public                 string       MustBeTrimmed                                   => "Password must be trimmed";
        public                 string       MustHaveValidDeviceName                         => "Must have valid device name";
        public                 string       MustHaveValidIPHostName                         => "Must have valid IP host name";
        public                 string       NetworkAuthenticationRequired                   => "NetworkAuthenticationRequired";
        public                 string       NoContent                                       => "No Content";
        public                 string       NoInternet                                      => "No internet";
        public                 string       NonAuthoritativeInformation                     => "Non Authoritative Information";
        public                 string       NoResponse                                      => "NoResponse";
        public                 string       NoSubscription                                  => "User is not subscribed";
        public                 string       NotAcceptable                                   => "Not Acceptable";
        public                 string       NotExtended                                     => "Not Extended";
        public                 string       NotFound                                        => "A 'Not Found' has occurred";
        public                 string       NotImplemented                                  => "Not Implemented";
        public                 string       NotModified                                     => "Not Modified";
        public                 string       NotSet                                          => "NotSet";
        public                 string       NumericPassed                                   => "Password must contain a number";
        public                 string       Ok                                              => "Ok";
        public                 string       PartialContent                                  => "Partial Content";
        public                 string       PasswordCannotBeEmpty                           => "Password cannot be empty";
        public                 string       PasswordValidation                              => "Password validation failed";
        public                 string       PaymentRequired                                 => "Payment Required";
        public                 string       PermanentRedirect                               => "Permanent Redirect";
        public                 string       PreconditionFailed                              => "Precondition Failed";
        public                 string       PreconditionRequired                            => "Precondition Required";
        public                 string       Processing                                      => "Processing";
        public                 string       ProxyAuthenticationRequired                     => "ProxyAuthenticationRequired";
        public                 string       RedirectKeepVerb                                => "RedirectKeepVerb";
        public                 string       RedirectMethod                                  => "RedirectMethod";
        public                 string       RequestedRangeNotSatisfiable                    => "RequestedRangeNotSatisfiable";
        public                 string       RequestEntityTooLarge                           => "RequestEntityTooLarge";
        public                 string       RequestHeaderFieldsTooLarge                     => "RequestHeaderFieldsTooLarge";
        public                 string       RequestTimeout                                  => "RequestTimeout";
        public                 string       RequestUriTooLong                               => "RequestUriTooLong";
        public                 string       ResetContent                                    => "ResetContent";
        public                 string       ServerIsOutdated                                => "Server Is outdated";
        public                 string       ServerIsUnavailable                             => "Server Is unavailable";
        public                 string       ServiceUnavailable                              => "ServiceUnavailable";
        public                 string       SessionExpired                                  => "SessionExpired";
        public                 string       SpecialPassed                                   => "Password must contain a special character";
        public                 string       SwitchingProtocols                              => "SwitchingProtocols";
        public                 string       Teapot                                          => "Teapot";
        public                 string       TooEarly                                        => "TooEarly";
        public                 string       TooManyRequests                                 => "TooManyRequests";
        public                 string       Unauthorized                                    => "A 'Unauthorized' has occurred";
        public                 string       UnavailableForLegalReasons                      => "UnavailableForLegalReasons";
        public                 string       Unexpected                                      => "A unexpected has occurred";
        public                 string       UnprocessableEntity                             => "UnprocessableEntity";
        public                 string       UnsupportedMediaType                            => "UnsupportedMediaType";
        public                 string       Unused                                          => "Unused";
        public                 string       UpgradeRequired                                 => "UpgradeRequired";
        public                 string       UpperPassed                                     => "Password must contain a upper case character";
        public                 string       UseProxy                                        => "UseProxy";
        public                 string       UserNameCannotBeEmpty                           => "User Name cannot be empty";
        public                 string       Validation                                      => "A validation has occurred";
        public                 string       VariantAlsoNegotiates                           => "VariantAlsoNegotiates";
        public                 string       WiFiIsDisabled                                  => "WiFi is disabled";

        private Defaults() { }
    }
}
