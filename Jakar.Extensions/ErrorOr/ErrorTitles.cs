// Jakar.Extensions :: Jakar.Extensions
// 06/17/2024  16:06

namespace Jakar.Extensions;


public interface IErrorTitles
{
    string BlockedPassed                                   { get; }
    string Conflict                                        { get; }
    string Disabled                                        { get; }
    string ExpiredSubscription                             { get; }
    string Forbidden                                       { get; }
    string General                                         { get; }
    string GivenPortIsNotAValidPortNumberInRangeOf1To65535 { get; }
    string InvalidCredentials                              { get; }
    string InvalidSubscription                             { get; }
    string InvalidValue                                    { get; }
    string LengthPassed                                    { get; }
    string ServerIsUnavailable                             { get; }
    string Locked                                          { get; }
    string LowerPassed                                     { get; }
    string MustBeTrimmed                                   { get; }
    string MustHaveValidDeviceName                         { get; }
    string MustHaveValidIPHostName                         { get; }
    string NoInternet                                      { get; }
    string NoSubscription                                  { get; }
    string NotFound                                        { get; }
    string NumericPassed                                   { get; }
    string PasswordCannotBeEmpty                           { get; }
    string PasswordValidation                              { get; }
    string SpecialPassed                                   { get; }
    string Unauthorized                                    { get; }
    string Unexpected                                      { get; }
    string UpperPassed                                     { get; }
    string UserNameCannotBeEmpty                           { get; }
    string Validation                                      { get; }
    string WiFiIsDisabled                                  { get; }
    string ClientIsUnavailable                             { get; }
    string ClientIsOutdated                                { get; }
    string ServerIsOutdated                                { get; }
    string NotSet                                          { get; }
    string Continue                                        { get; }
    string SwitchingProtocols                              { get; }
    string Processing                                      { get; }
    string EarlyHints                                      { get; }
    string Ok                                              { get; }
    string Created                                         { get; }
    string Accepted                                        { get; }
    string NonAuthoritativeInformation                     { get; }
    string NoContent                                       { get; }
    string ResetContent                                    { get; }
    string PartialContent                                  { get; }
    string MultiStatus                                     { get; }
    string AlreadyReported                                 { get; }
    string ImUsed                                          { get; }
    string Ambiguous                                       { get; }
    string Moved                                           { get; }
    string Found                                           { get; }
    string RedirectMethod                                  { get; }
    string NotModified                                     { get; }
    string UseProxy                                        { get; }
    string Unused                                          { get; }
    string RedirectKeepVerb                                { get; }
    string PermanentRedirect                               { get; }
    string BadRequest                                      { get; }
    string PaymentRequired                                 { get; }
    string MethodNotAllowed                                { get; }
    string NotAcceptable                                   { get; }
    string ProxyAuthenticationRequired                     { get; }
    string RequestTimeout                                  { get; }
    string Gone                                            { get; }
    string LengthRequired                                  { get; }
    string PreconditionFailed                              { get; }
    string RequestEntityTooLarge                           { get; }
    string RequestUriTooLong                               { get; }
    string UnsupportedMediaType                            { get; }
    string RequestedRangeNotSatisfiable                    { get; }
    string ExpectationFailed                               { get; }
    string Teapot                                          { get; }
    string MisdirectedRequest                              { get; }
    string UnprocessableEntity                             { get; }
    string FailedDependency                                { get; }
    string TooEarly                                        { get; }
    string UpgradeRequired                                 { get; }
    string PreconditionRequired                            { get; }
    string TooManyRequests                                 { get; }
    string RequestHeaderFieldsTooLarge                     { get; }
    string AlreadyExists                                   { get; }
    string NoResponse                                      { get; }
    string UnavailableForLegalReasons                      { get; }
    string SessionExpired                                  { get; }
    string ClientClosedRequest                             { get; }
    string InternalServerError                             { get; }
    string NotImplemented                                  { get; }
    string BadGateway                                      { get; }
    string ServiceUnavailable                              { get; }
    string GatewayTimeout                                  { get; }
    string HttpVersionNotSupported                         { get; }
    string VariantAlsoNegotiates                           { get; }
    string InsufficientStorage                             { get; }
    string LoopDetected                                    { get; }
    string NotExtended                                     { get; }
    string NetworkAuthenticationRequired                   { get; }



    [SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
    public sealed class Defaults : IErrorTitles
    {
        public static readonly IErrorTitles Instance = new Defaults();
        public                 string       BlockedPassed                                   => "Password cannot be a blocked password";
        public                 string       Conflict                                        => "A conflict has occurred";
        public                 string       Disabled                                        => "User is disabled";
        public                 string       ExpiredSubscription                             => "User's subscription is expired";
        public                 string       Forbidden                                       => "A 'Forbidden' has occurred";
        public                 string       General                                         => "A failure has occurred";
        public                 string       GivenPortIsNotAValidPortNumberInRangeOf1To65535 => "Given port is not a valid port number in range of 1 to 65535";
        public                 string       InvalidCredentials                              => "Invalid credentials";
        public                 string       InvalidSubscription                             => "User's subscription is no longer valid";
        public                 string       InvalidValue                                    => "Invalid value";
        public                 string       LengthPassed                                    => "Password not long enough";
        public                 string       ServerIsUnavailable                             => "Server Is unavailable";
        public                 string       Locked                                          => "User is locked";
        public                 string       LowerPassed                                     => "Password must contain a lower case character";
        public                 string       MustBeTrimmed                                   => "Password must be trimmed";
        public                 string       MustHaveValidDeviceName                         => "Must have valid device name";
        public                 string       MustHaveValidIPHostName                         => "Must have valid IP host name";
        public                 string       NoInternet                                      => "No internet";
        public                 string       NoSubscription                                  => "User is not subscribed";
        public                 string       NotFound                                        => "A 'Not Found' has occurred";
        public                 string       NumericPassed                                   => "Password must contain a number";
        public                 string       PasswordCannotBeEmpty                           => "Password cannot be empty";
        public                 string       PasswordValidation                              => "Password validation failed";
        public                 string       SpecialPassed                                   => "Password must contain a special character";
        public                 string       Unauthorized                                    => "A 'Unauthorized' has occurred";
        public                 string       Unexpected                                      => "A unexpected has occurred";
        public                 string       UpperPassed                                     => "Password must contain a upper case character";
        public                 string       UserNameCannotBeEmpty                           => "User Name cannot be empty";
        public                 string       Validation                                      => "A validation has occurred";
        public                 string       WiFiIsDisabled                                  => "WiFi is disabled";
        public                 string       ClientIsUnavailable                             => "Client is unavailable";
        public                 string       ClientIsOutdated                                => "Client is outdated";
        public                 string       ServerIsOutdated                                => "Server Is outdated";
        public                 string       NotSet                                          => "NotSet";
        public                 string       Continue                                        => "Continue";
        public                 string       SwitchingProtocols                              => "SwitchingProtocols";
        public                 string       Processing                                      => "Processing";
        public                 string       EarlyHints                                      => "EarlyHints";
        public                 string       Ok                                              => "Ok";
        public                 string       Created                                         => "Created";
        public                 string       Accepted                                        => "Accepted";
        public                 string       NonAuthoritativeInformation                     => "NonAuthoritativeInformation";
        public                 string       NoContent                                       => "NoContent";
        public                 string       ResetContent                                    => "ResetContent";
        public                 string       PartialContent                                  => "PartialContent";
        public                 string       MultiStatus                                     => "MultiStatus";
        public                 string       AlreadyReported                                 => "AlreadyReported";
        public                 string       ImUsed                                          => "ImUsed";
        public                 string       Ambiguous                                       => "Ambiguous";
        public                 string       Moved                                           => "Moved";
        public                 string       Found                                           => "Found";
        public                 string       RedirectMethod                                  => "RedirectMethod";
        public                 string       NotModified                                     => "NotModified";
        public                 string       UseProxy                                        => "UseProxy";
        public                 string       Unused                                          => "Unused";
        public                 string       RedirectKeepVerb                                => "RedirectKeepVerb";
        public                 string       PermanentRedirect                               => "PermanentRedirect";
        public                 string       BadRequest                                      => "BadRequest";
        public                 string       PaymentRequired                                 => "PaymentRequired";
        public                 string       MethodNotAllowed                                => "MethodNotAllowed";
        public                 string       NotAcceptable                                   => "NotAcceptable";
        public                 string       ProxyAuthenticationRequired                     => "ProxyAuthenticationRequired";
        public                 string       RequestTimeout                                  => "RequestTimeout";
        public                 string       Gone                                            => "Gone";
        public                 string       LengthRequired                                  => "LengthRequired";
        public                 string       PreconditionFailed                              => "PreconditionFailed";
        public                 string       RequestEntityTooLarge                           => "RequestEntityTooLarge";
        public                 string       RequestUriTooLong                               => "RequestUriTooLong";
        public                 string       UnsupportedMediaType                            => "UnsupportedMediaType";
        public                 string       RequestedRangeNotSatisfiable                    => "RequestedRangeNotSatisfiable";
        public                 string       ExpectationFailed                               => "ExpectationFailed";
        public                 string       Teapot                                          => "Teapot";
        public                 string       MisdirectedRequest                              => "MisdirectedRequest";
        public                 string       UnprocessableEntity                             => "UnprocessableEntity";
        public                 string       FailedDependency                                => "FailedDependency";
        public                 string       TooEarly                                        => "TooEarly";
        public                 string       UpgradeRequired                                 => "UpgradeRequired";
        public                 string       PreconditionRequired                            => "PreconditionRequired";
        public                 string       TooManyRequests                                 => "TooManyRequests";
        public                 string       RequestHeaderFieldsTooLarge                     => "RequestHeaderFieldsTooLarge";
        public                 string       AlreadyExists                                   => "AlreadyExists";
        public                 string       NoResponse                                      => "NoResponse";
        public                 string       UnavailableForLegalReasons                      => "UnavailableForLegalReasons";
        public                 string       SessionExpired                                  => "SessionExpired";
        public                 string       ClientClosedRequest                             => "ClientClosedRequest";
        public                 string       InternalServerError                             => "InternalServerError";
        public                 string       NotImplemented                                  => "NotImplemented";
        public                 string       BadGateway                                      => "BadGateway";
        public                 string       ServiceUnavailable                              => "ServiceUnavailable";
        public                 string       GatewayTimeout                                  => "GatewayTimeout";
        public                 string       HttpVersionNotSupported                         => "HttpVersionNotSupported";
        public                 string       VariantAlsoNegotiates                           => "VariantAlsoNegotiates";
        public                 string       InsufficientStorage                             => "InsufficientStorage";
        public                 string       LoopDetected                                    => "LoopDetected";
        public                 string       NotExtended                                     => "NotExtended";
        public                 string       NetworkAuthenticationRequired                   => "NetworkAuthenticationRequired";

        private Defaults() { }
    }
}
