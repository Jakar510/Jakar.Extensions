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
}



[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public sealed class ErrorTitles : IErrorTitles
{
    public static ErrorTitles Instance                                        { get; } = new();
    public        string      BlockedPassed                                   => "Password cannot be a blocked password";
    public        string      Conflict                                        => "A conflict has occurred";
    public        string      Disabled                                        => "User is disabled";
    public        string      ExpiredSubscription                             => "User's subscription is expired";
    public        string      Forbidden                                       => "A 'Forbidden' has occurred";
    public        string      General                                         => "A failure has occurred";
    public        string      GivenPortIsNotAValidPortNumberInRangeOf1To65535 => "Given port is not a valid port number in range of 1 to 65535";
    public        string      InvalidCredentials                              => "Invalid credentials";
    public        string      InvalidSubscription                             => "User's subscription is no longer valid";
    public        string      InvalidValue                                    => "Invalid value";
    public        string      LengthPassed                                    => "Password not long enough";
    public        string      ServerIsUnavailable                             => "Server Is unavailable";
    public        string      Locked                                          => "User is locked";
    public        string      LowerPassed                                     => "Password must contain a lower case character";
    public        string      MustBeTrimmed                                   => "Password must be trimmed";
    public        string      MustHaveValidDeviceName                         => "Must have valid device name";
    public        string      MustHaveValidIPHostName                         => "Must have valid IP host name";
    public        string      NoInternet                                      => "No internet";
    public        string      NoSubscription                                  => "User is not subscribed";
    public        string      NotFound                                        => "A 'Not Found' has occurred";
    public        string      NumericPassed                                   => "Password must contain a number";
    public        string      PasswordCannotBeEmpty                           => "Password cannot be empty";
    public        string      PasswordValidation                              => "Password validation failed";
    public        string      SpecialPassed                                   => "Password must contain a special character";
    public        string      Unauthorized                                    => "A 'Unauthorized' has occurred";
    public        string      Unexpected                                      => "A unexpected has occurred";
    public        string      UpperPassed                                     => "Password must contain a upper case character";
    public        string      UserNameCannotBeEmpty                           => "User Name cannot be empty";
    public        string      Validation                                      => "A validation has occurred";
    public        string      WiFiIsDisabled                                  => "WiFi is disabled";
    public        string      ClientIsUnavailable                             => "Client is unavailable";
    public        string      ClientIsOutdated                                => "Client is outdated";
    public        string      ServerIsOutdated                                => "Server Is outdated";

    private ErrorTitles() { }
}
