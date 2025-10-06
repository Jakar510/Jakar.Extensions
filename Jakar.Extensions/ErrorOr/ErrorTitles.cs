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



    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public sealed class Defaults : IErrorTitles
    {
        public static readonly IErrorTitles Instance = new Defaults();
        public                 string       Accepted                                        => "Accepted";
        public                 string       AlreadyExists                                   => "AlreadyExists";
        public                 string       AlreadyReported                                 => "AlreadyReported";
        public                 string       Ambiguous                                       => "Ambiguous";
        public                 string       BadGateway                                      => "BadGateway";
        public                 string       BadRequest                                      => "BadRequest";
        public                 string       BlockedPassed                                   => "Password cannot be a blocked password";
        public                 string       ClientClosedRequest                             => "ClientClosedRequest";
        public                 string       ClientIsOutdated                                => "Client is outdated";
        public                 string       ClientIsUnavailable                             => "Client is unavailable";
        public                 string       Conflict                                        => "A conflict has occurred";
        public                 string       Continue                                        => "Continue";
        public                 string       Created                                         => "Created";
        public                 string       Disabled                                        => "User is disabled";
        public                 string       EarlyHints                                      => "EarlyHints";
        public                 string       ExpectationFailed                               => "ExpectationFailed";
        public                 string       ExpiredSubscription                             => "User's subscription is expired";
        public                 string       FailedDependency                                => "FailedDependency";
        public                 string       Forbidden                                       => "A 'Forbidden' has occurred";
        public                 string       Found                                           => "Found";
        public                 string       GatewayTimeout                                  => "GatewayTimeout";
        public                 string       General                                         => "A failure has occurred";
        public                 string       GivenPortIsNotAValidPortNumberInRangeOf1To65535 => "Given port is not a valid port number in range of 1 to 65535";
        public                 string       Gone                                            => "Gone";
        public                 string       HttpVersionNotSupported                         => "HttpVersionNotSupported";
        public                 string       ImUsed                                          => "ImUsed";
        public                 string       InsufficientStorage                             => "InsufficientStorage";
        public                 string       InternalServerError                             => "InternalServerError";
        public                 string       InvalidCredentials                              => "Invalid credentials";
        public                 string       InvalidSubscription                             => "User's subscription is no longer valid";
        public                 string       InvalidValue                                    => "Invalid value";
        public                 string       LengthPassed                                    => "Password not long enough";
        public                 string       LengthRequired                                  => "LengthRequired";
        public                 string       Locked                                          => "User is locked";
        public                 string       LoopDetected                                    => "LoopDetected";
        public                 string       LowerPassed                                     => "Password must contain a lower case character";
        public                 string       MethodNotAllowed                                => "MethodNotAllowed";
        public                 string       MisdirectedRequest                              => "MisdirectedRequest";
        public                 string       Moved                                           => "Moved";
        public                 string       MultiStatus                                     => "MultiStatus";
        public                 string       MustBeTrimmed                                   => "Password must be trimmed";
        public                 string       MustHaveValidDeviceName                         => "Must have valid device name";
        public                 string       MustHaveValidIPHostName                         => "Must have valid IP host name";
        public                 string       NetworkAuthenticationRequired                   => "NetworkAuthenticationRequired";
        public                 string       NoContent                                       => "NoContent";
        public                 string       NoInternet                                      => "No internet";
        public                 string       NonAuthoritativeInformation                     => "NonAuthoritativeInformation";
        public                 string       NoResponse                                      => "NoResponse";
        public                 string       NoSubscription                                  => "User is not subscribed";
        public                 string       NotAcceptable                                   => "NotAcceptable";
        public                 string       NotExtended                                     => "NotExtended";
        public                 string       NotFound                                        => "A 'Not Found' has occurred";
        public                 string       NotImplemented                                  => "NotImplemented";
        public                 string       NotModified                                     => "NotModified";
        public                 string       NotSet                                          => "NotSet";
        public                 string       NumericPassed                                   => "Password must contain a number";
        public                 string       Ok                                              => "Ok";
        public                 string       PartialContent                                  => "PartialContent";
        public                 string       PasswordCannotBeEmpty                           => "Password cannot be empty";
        public                 string       PasswordValidation                              => "Password validation failed";
        public                 string       PaymentRequired                                 => "PaymentRequired";
        public                 string       PermanentRedirect                               => "PermanentRedirect";
        public                 string       PreconditionFailed                              => "PreconditionFailed";
        public                 string       PreconditionRequired                            => "PreconditionRequired";
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
