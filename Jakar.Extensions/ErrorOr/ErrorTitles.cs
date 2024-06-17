// Jakar.Extensions :: Jakar.Extensions
// 06/17/2024  16:06

namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public class ErrorTitles
{
    public virtual string  BlockedPassed                                   => "Password cannot be a blocked password";
    public virtual string  Conflict                                        => "A conflict has occurred.";
    public virtual string  Disabled                                        => "User is disabled.";
    public virtual string  ExpiredSubscription                             => "User's subscription is expired.";
    public virtual string  Forbidden                                       => "A 'Forbidden' has occurred.";
    public virtual string  General                                         => "A failure has occurred.";
    public virtual string  GivenPortIsNotAValidPortNumberInRangeOf1To65535 => "Given port is not a valid port number in range of 1 to 65535";
    public virtual string  InvalidCredentials                              => "Invalid credentials";
    public virtual string  InvalidSubscription                             => "User's subscription is no longer valid.";
    public virtual string  InvalidValue                                    => "Invalid value";
    public virtual string  LengthPassed                                    => "Password not long enough";
    public virtual string  ServerIsUnavailable                             => "Server Is unavailable";
    public virtual string  Locked                                          => "User is locked.";
    public virtual string  LowerPassed                                     => "Password must contain a lower case character";
    public virtual string  MustBeTrimmed                                   => "Password must be trimmed";
    public virtual string  MustHaveValidDeviceName                         => "Must have valid device name";
    public virtual string  MustHaveValidIPHostName                         => "MustHave valid IP host name";
    public virtual string  NoInternet                                      => "No internet";
    public virtual string  NoSubscription                                  => "User is not subscribed.";
    public virtual string  NotFound                                        => "A 'Not Found' has occurred.";
    public virtual string  NumericPassed                                   => "Password must contain a numeric character";
    public virtual string  PasswordCannotBeEmpty                           => "Password cannot be empty";
    public virtual string  PasswordValidation                              => "Password validation failed";
    public virtual string  SpecialPassed                                   => "Password must contain a special character";
    public virtual string  Unauthorized                                    => "A 'Unauthorized' has occurred.";
    public virtual string  Unexpected                                      => "A unexpected has occurred.";
    public virtual string  UpperPassed                                     => "Password must contain a upper case character";
    public virtual string  UserNameCannotBeEmpty                           => "User Name cannot be empty";
    public virtual string  Validation                                      => "A validation has occurred.";
    public virtual string  WiFiIsDisabled                                  => "WiFi is disabled";
    public virtual string? ClientIsUnavailable                             => "Client is unavailable";
    public virtual string? ClientIsOutdated                                => "Client is outdated";
    public virtual string? ServerIsOutdated                                => "Server Is Outdated";
}
