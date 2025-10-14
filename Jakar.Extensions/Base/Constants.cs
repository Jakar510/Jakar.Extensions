// Jakar.Extensions :: Jakar.Database
// 10/15/2023  1:13 PM

namespace Jakar.Extensions;


public static class Constants
{
    public const           int            ADDRESS               = 4096;
    public const           string         AND                   = " AND ";
    public const           int            AUTHENTICATOR_KEY     = 4096;
    public const           int            CITY                  = 256;
    public const           int            COMPANY               = 512;
    public const           int            CONCURRENCY_STAMP     = 4096;
    public const           int            CONNECTION_ID         = 4096;
    public const           string         COUNT                 = "count";
    public const           int            COUNTRY               = 256;
    public const           int            DECIMAL_MAX_PRECISION = 38;
    public const           int            DECIMAL_MAX_SCALE     = 29;
    public const           int            DEFAULT_CAPACITY      = 64;
    public const           int            DEPARTMENT            = 256;
    public const           int            DESCRIPTION           = 2048;
    public const           int            EMAIL                 = 256;
    public const           string         EMPTY                 = "";
    public const           int            FIRST_NAME            = 256;
    public const           int            FULL_NAME             = 512;
    public const           int            GENDER                = 64;
    public const           string         GUID_FORMAT           = "D";
    public const           int            HASH                  = 512;
    public const           int            LAST_NAME             = 256;
    public const           int            LINE1                 = 512;
    public const           int            LINE2                 = 256;
    public const           string         LIST_SEPARATOR        = ", ";
    public const           int            MAX_FIXED             = 4096;
    public const           int            MAX_SIZE              = 0x3FFFFFDF; // string.MaxLength = 2,147,483,647 (2^31-1) - overhead
    public const           int            MAX_VARIABLE          = 8192;
    public const           int            NAME                  = 512;
    public const           int            FILE_NAME             = 512;
    public const           int            NORMALIZED_NAME       = 1024;
    public const           int            NOT_FOUND             = -1;
    public const           string         NULL                  = "null";
    public const           string         OR                    = " OR ";
    public const           int            PASSWORD              = 256;
    public const           int            PHONE                 = 256;
    public const           int            PHONE_EXT             = 256;
    public const           int            POSTAL_CODE           = 10;
    public const           char           QUOTE                 = '"';
    public const           int            REFRESH_TOKEN         = 4096;
    public const           int            RIGHTS                = 8192;
    public const           int            SECURITY_STAMP        = 4096;
    public const           int            STATE_OR_PROVINCE     = 256;
    public const           int            TITLE                 = 256;
    public const           int            TYPE                  = 512;
    public const           int            USER_NAME             = 256;
    public const           int            WEBSITE               = 8192;
    public static readonly DateTimeOffset SQLMinDate            = new(1753, 1, 1, 0, 0, 0, 0, TimeSpan.Zero);
    public static readonly DateOnly       SQLMinDateOnly        = new(1753, 1, 1);


    public static string GetAndOr( this bool matchAll ) => matchAll
                                                               ? AND
                                                               : OR;
}
