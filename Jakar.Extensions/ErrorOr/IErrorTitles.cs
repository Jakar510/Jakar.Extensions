// Jakar.Extensions :: Jakar.Extensions
// 11/26/2025  14:52

namespace Jakar.Extensions;


public interface IErrorTitles
{
    public static IErrorTitles Current { get => field ??= ErrorTitles.Instance; set; }


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
