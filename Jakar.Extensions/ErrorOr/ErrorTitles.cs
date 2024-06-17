// Jakar.Extensions :: Jakar.Extensions
// 06/17/2024  16:06

namespace Jakar.Extensions;


public class ErrorTitles
{
    /*
    public const   string CONFLICT_TITLE             = "A conflict has occurred.";
    public const   string VALIDATION_TITLE           = "A validation has occurred.";
    public const   string FORBIDDEN_TITLE            = "A 'Forbidden' has occurred.";
    public const   string INVALID_SUBSCRIPTION_TITLE = "User's subscription is no longer valid.";
    public const   string LOCKED_TITLE               = "User is locked.";
    public const   string GENERAL_TITLE              = "A failure has occurred.";
    public const   string EXPIRED_SUBSCRIPTION_TITLE = "User's subscription is expired.";
    public const   string UNEXPECTED_TITLE           = "A unexpected has occurred.";
    public const   string NOT_FOUND_TITLE            = "A 'Not Found' has occurred.";
    public const   string NO_SUBSCRIPTION_TITLE      = "User is not subscribed.";
    public const   string UNAUTHORIZED_TITLE         = "A 'Unauthorized' has occurred.";
    public const   string PASSWORD_VALIDATION_TITLE  = "Password validation failed";
    public const   string DISABLED_TITLE             = "User is disabled.";
    public const   string NUMERIC_PASSED             = "Password must contain a numeric character";
    public const   string SPECIAL_PASSED             = "Password must contain a special character";
    public const   string LOWER_PASSED               = "Password must contain a lower case character";
    public const   string MUST_BE_TRIMMED            = "Password must be trimmed";
    public const   string UPPER_PASSED               = "Password must contain a upper case character";
    public const   string LENGTH_PASSED              = "Password not long enough";
    */


    public virtual string BlockedPassed                                   => "Password cannot be a blocked password";
    public virtual string Conflict                                        => "A conflict has occurred.";
    public virtual string Disabled                                        => "User is disabled.";
    public virtual string ExpiredSubscription                             => "User's subscription is expired.";
    public virtual string Forbidden                                       => "A 'Forbidden' has occurred.";
    public virtual string General                                         => "A failure has occurred.";
    public virtual string GivenPortIsNotAValidPortNumberInRangeOf1To65535 => "Given Port Is Not A Valid Port Number In Range Of 1 To 65535";
    public virtual string InvalidCredentials                              => "Invalid Credentials";
    public virtual string InvalidSubscription                             => "User's subscription is no longer valid.";
    public virtual string InvalidValue                                    => "Invalid Value";
    public virtual string LengthPassed                                    => "Password not long enough";
    public virtual string LocalServerIsUnauthorized                       => "Local Server Is Unauthorized";
    public virtual string Locked                                          => "User is locked.";
    public virtual string LowerPassed                                     => "Password must contain a lower case character";
    public virtual string MustBeTrimmed                                   => "Password must be trimmed";
    public virtual string MustHaveValidDeviceName                         => "Must Have Valid DeviceName";
    public virtual string MustHaveValidIPHostName                         => "MustHave Valid IP Host Name";
    public virtual string NoInternet                                      => "No Internet";
    public virtual string NoSubscription                                  => "User is not subscribed.";
    public virtual string NotFound                                        => "A 'Not Found' has occurred.";
    public virtual string NumericPassed                                   => "Password must contain a numeric character";
    public virtual string PasswordCannotBeEmpty                           => "Password Cannot Be Empty";
    public virtual string PasswordValidation                              => "Password validation failed";
    public virtual string SpecialPassed                                   => "Password must contain a special character";
    public virtual string Unauthorized                                    => "A 'Unauthorized' has occurred.";
    public virtual string Unexpected                                      => "A unexpected has occurred.";
    public virtual string UpperPassed                                     => "Password must contain a upper case character";
    public virtual string UserNameCannotBeEmpty                           => "User Name Cannot Be Empty";
    public virtual string Validation                                      => "A validation has occurred.";
    public virtual string WiFiIsDisabled                                  => "WiFi Is Disabled";
}
