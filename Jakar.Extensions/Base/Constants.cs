// Jakar.Extensions :: Jakar.Database
// 10/15/2023  1:13 PM

namespace Jakar.Extensions;


public static class Constants
{
    public const           string         ERROR_MESSAGE  = "Error Message: ";
    public const           string         SESSION        = "Session";
    public const           string         UNKNOWN_ERROR  = "Unknown Error";
    public const           string         VERIFY_DEVICE  = "VerifyDevice";
    public const           string         VERIFY_EMAIL   = "VerifyEmail";
    public static readonly DateTimeOffset SQLMinDate     = new(1753, 1, 1, 0, 0, 0, 0, TimeSpan.Zero);
    public static readonly DateOnly       SQLMinDateOnly = new(1753, 1, 1);


    public static string GetAndOr( this bool matchAll ) => matchAll
                                                               ? AND
                                                               : OR;
    public static string ToStringFast( this bool value ) => value
                                                                ? TRUE
                                                                : FALSE;



    public static class Characters
    {
        public const string ALPHANUMERIC   = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        public const char   MINUS          = '-';
        public const char   EQUALS         = '=';
        public const char   FLAG_SEPARATOR = '-';
        public const string GUID_FORMAT    = "D";
        public const string HEX_CHARS      = "0123456789ABCDEF";
        public const char   HYPHEN         = '-';
        public const string LOWER_CASE     = @"abcdefghijklmnopqrstuvwxyz";
        public const string NUMERIC        = @"0123456789";
        public const char   PLUS           = '+';
        public const byte   PLUS_BYTE      = (byte)'+';
        public const char   QUOTE          = '"';
        public const char   SLASH          = '/';
        public const byte   SLASH_BYTE     = (byte)'/';
        public const string SPECIAL_CHARS  = @"_-.!#@+/*^=>|/\";
        public const char   UNDERSCORE     = '_';
        public const string UPPER_CASE     = @"ABCDEFGHJKLMNOPQRSTUVWXYZ";
    }



    public static class ErrorTypes
    {
        public const string ACCEPTED_TYPE                        = "Server.Accepted";
        public const string ALREADY_EXISTS_TYPE                  = "Server.AlreadyExists";
        public const string ALREADY_REPORTED_TYPE                = "Server.AlreadyReported";
        public const string AMBIGUOUS_TYPE                       = "Client.Ambiguous";
        public const string BAD_GATEWAY_TYPE                     = "Server.BadGateway";
        public const string BAD_REQUEST_TYPE                     = "Client.BadRequest";
        public const string BLOCKED_PASSED                       = "Password.Blocked";
        public const string CLIENT_CLOSED_REQUEST_TYPE           = "Client.ClosedRequest";
        public const string CLIENT_IS_OUTDATED_TYPE              = "Client.Outdated";
        public const string CLIENT_IS_UNAVAILABLE_TYPE           = "Client.Unavailable";
        public const string CLIENT_OPERATION_CANCELED            = "Client.OperationCanceled";
        public const string CONFLICT_TYPE                        = "General.Conflict";
        public const string CONTINUE_TYPE                        = "General.Continue";
        public const string CREATED_TYPE                         = "Server.Created";
        public const string DISABLED_TYPE                        = "User.Disabled";
        public const string EARLY_HINTS_TYPE                     = "EarlyHints";
        public const string EXPECTATION_FAILED_TYPE              = "Client.ExpectationFailed";
        public const string EXPIRED_SUBSCRIPTION_TYPE            = "User.Subscription.Expired";
        public const string FAILED_DEPENDENCY_TYPE               = "Client.FailedDependency";
        public const string FORBIDDEN_TYPE                       = "General.Forbidden";
        public const string FOUND_TYPE                           = "Server.Found";
        public const string GATEWAY_TIMEOUT_TYPE                 = "Server.GatewayTimeout";
        public const string GENERAL_FAILURE_TYPE                 = "General.Failure";
        public const string GONE_TYPE                            = "Gone";
        public const string HTTP_VERSION_NOT_SUPPORTED_TYPE      = "Server.HttpVersionNotSupported";
        public const string IM_USED_TYPE                         = "ImUsed";
        public const string INSUFFICIENT_STORAGE_TYPE            = "Server.InsufficientStorage";
        public const string INTERNAL_SERVER_ERROR_TYPE           = "Server.InternalServerError";
        public const string INVALID_SUBSCRIPTION_TYPE            = "User.Subscription.Invalid";
        public const string LENGTH_REQUIRED_TYPE                 = "Client.LengthRequired";
        public const string LOCKED_TYPE                          = "User.Locked";
        public const string LOOP_DETECTED_TYPE                   = "LoopDetected";
        public const string METHOD_NOT_ALLOWED_TYPE              = "Client.MethodNotAllowed";
        public const string MISDIRECTED_REQUEST_TYPE             = "Client.MisdirectedRequest";
        public const string MOVED_TYPE                           = "Server.Moved";
        public const string MULTI_STATUS_TYPE                    = "Server.MultiStatus";
        public const string NETWORK_AUTHENTICATION_REQUIRED_TYPE = "Client.NetworkAuthenticationRequired";
        public const string NO_CONTENT_TYPE                      = "Server.NoContent";
        public const string NO_INTERNET_TYPE                     = "Internet.Disabled";
        public const string NO_INTERNET_WIFI_TYPE                = "Internet.Disabled.WiFi";
        public const string NO_RESPONSE_TYPE                     = "NoResponse";
        public const string NO_SUBSCRIPTION_TYPE                 = "User.Subscription.None";
        public const string NON_AUTHORITATIVE_INFORMATION_TYPE   = "Server.NonAuthoritativeInformation";
        public const string NOT_ACCEPTABLE_TYPE                  = "Client.NotAcceptable";
        public const string NOT_EXTENDED_TYPE                    = "Client.NotExtended";
        public const string NOT_FOUND_TYPE                       = "General.NotFound";
        public const string NOT_IMPLEMENTED_TYPE                 = "NotImplemented";
        public const string NOT_MODIFIED_TYPE                    = "Server.NotModified";
        public const string NOT_SET_TYPE                         = "General.NotSet";
        public const string OK_TYPE                              = "Ok";
        public const string PARTIAL_CONTENT_TYPE                 = "Server.PartialContent";
        public const string PASSWORD_VALIDATION_TYPE             = "Password.Unauthorized";
        public const string PAYMENT_REQUIRED_TYPE                = "Client.PaymentRequired";
        public const string PERMANENT_REDIRECT_TYPE              = "Server.PermanentRedirect";
        public const string PRECONDITION_FAILED_TYPE             = "Client.PreconditionFailed";
        public const string PRECONDITION_REQUIRED_TYPE           = "Client.PreconditionRequired";
        public const string PROCESSING_TYPE                      = "Server.Processing";
        public const string PROXY_AUTHENTICATION_REQUIRED_TYPE   = "Client.ProxyAuthenticationRequired";
        public const string REDIRECT_KEEP_VERB_TYPE              = "Server.RedirectKeepVerb";
        public const string REDIRECT_METHOD_TYPE                 = "Server.Redirect";
        public const string REQUEST_ENTITY_TOO_LARGE_TYPE        = "Client.RequestEntityTooLarge";
        public const string REQUEST_HEADER_FIELDS_TOO_LARGE_TYPE = "Client.RequestHeaderFieldsTooLarge";
        public const string REQUEST_TIMEOUT_TYPE                 = "Client.RequestTimeout";
        public const string REQUEST_URI_TOO_LONG_TYPE            = "Client.RequestUriTooLong";
        public const string REQUESTED_RANGE_NOT_SATISFIABLE_TYPE = "Client.RequestedRangeNotSatisfiable";
        public const string RESET_CONTENT_TYPE                   = "Server.ResetContent";
        public const string SERIALIZATION_REQUIRES_DYNAMIC_CODE  = "SerializationRequiresDynamicCode";
        public const string SERIALIZATION_UNREFERENCED_CODE      = "SerializationUnreferencedCode";
        public const string SERVER_IS_OUTDATED_TYPE              = "Server.Outdated";
        public const string SERVER_IS_UNAVAILABLE_TYPE           = "Server.Unavailable";
        public const string SERVICE_UNAVAILABLE_TYPE             = "Server.ServiceUnavailable";
        public const string SESSION_EXPIRED_TYPE                 = "Client.SessionExpired";
        public const string SWITCHING_PROTOCOLS_TYPE             = "Server.SwitchingProtocols";
        public const string TEAPOT_TYPE                          = "Teapot";
        public const string TOO_EARLY_TYPE                       = "Client.TooEarly";
        public const string TOO_MANY_REQUESTS_TYPE               = "Client.TooManyRequests";
        public const string UNAUTHORIZED_TYPE                    = "General.Unauthorized";
        public const string UNAVAILABLE_FOR_LEGAL_REASONS_TYPE   = "Server.UnavailableForLegalReasons";
        public const string UNEXPECTED_TYPE                      = "General.Unexpected";
        public const string UNPROCESSABLE_ENTITY_TYPE            = "Client.UnprocessableEntity";
        public const string UNSUPPORTED_MEDIA_TYPE_TYPE          = "Client.UnsupportedMediaType";
        public const string UNUSED_TYPE                          = "Unused";
        public const string UPGRADE_REQUIRED_TYPE                = "Client.UpgradeRequired";
        public const string USE_PROXY_TYPE                       = "UseProxy";
        public const string VALIDATION_TYPE                      = "General.Unexpected";
        public const string VARIANT_ALSO_NEGOTIATES_TYPE         = "VariantAlsoNegotiates";
    }



    public static class Files
    {
        public const string ACCOUNTS_FILE      = "Accounts.json";
        public const string APP_CACHE_ZIP_FILE = "App.Cache.zip";
        public const string APP_DATA_DIRECTORY = "AppData";
        public const string APP_DATA_ZIP_FILE  = "App.Data.zip";
        public const string APP_STATE_FILE     = "AppState.json";
        public const string CACHE_DIRECTORY    = "Cache";
        public const string CRASH_DATA         = "Crash.dat";
        public const string FEEDBACK_FILE      = "Feedback.txt";
        public const string INCOMING_FILE      = "Incoming.json";
        public const string LOGS_DIRECTORY     = "Logs";
        public const string LOGS_FILE          = "App.Logs";
        public const string LOGS_ZIP_FILE_NAME = "App.logs.zip";
        public const string OUTGOING_FILE      = "Outgoing.json";
        public const string SCREEN_SHOT_FILE   = "ScreenShot.png";
    }



    public static class Jwt
    {
        public const string JWT            = "JWT";
        public const string JWT_KEY        = "JWT.key";
        public const string VALID_AUDIENCE = "TokenValidationParameters:ValidAudience";
        public const string VALID_ISSUER   = "TokenValidationParameters:ValidIssuer";
    }



    public static class Logging
    {
        public const string APP                                 = nameof(APP);
        public const string CONSOLE_DEFAULT_OUTPUT_TEMPLATE     = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";
        public const string CREATE_WINDOW                       = nameof(CREATE_WINDOW);
        public const string DEBUG_DEFAULT_DEBUG_OUTPUT_TEMPLATE = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";
        public const string ELAPSED_TIME                        = nameof(ELAPSED_TIME);
        public const string ERROR                               = nameof(ERROR);
        public const string INFORMATION                         = nameof(INFORMATION);
        public const string ON_RESUME                           = nameof(ON_RESUME);
        public const string ON_SLEEP                            = nameof(ON_SLEEP);
        public const string ON_START                            = nameof(ON_START);
        public const string SEQ_API_KEY_NAME                    = "X-Seq-ApiKey";
        public const string SEQ_BUFFER_DIRECTORY                = "SeqBuffer";
        public const string START_STOP_ID                       = nameof(START_STOP_ID);
        public const string WARNING                             = nameof(WARNING);
    }



    public static class Numbers
    {
        public const int ADDRESS                             = 4096;
        public const int AUTHENTICATOR_KEY                   = 4096;
        public const int CITY                                = 256;
        public const int COMPANY                             = 512;
        public const int CONCURRENCY_STAMP                   = 4096;
        public const int CONNECTION_ID                       = 4096;
        public const int COUNTRY                             = 256;
        public const int DECIMAL_MAX_PRECISION               = 38;
        public const int DECIMAL_MAX_SCALE                   = 29;
        public const int DEFAULT_BAD_LOGIN_DISABLE_THRESHOLD = 5;
        public const int DEFAULT_CAPACITY                    = 64;
        public const int DEPARTMENT                          = 256;
        public const int DESCRIPTION                         = 2048;
        public const int EMAIL                               = 256;
        public const int ENCRYPTED_MAX_PASSWORD_SIZE         = 550;
        public const int FILE_NAME                           = 512;
        public const int FIRST_NAME                          = 256;
        public const int FULL_NAME                           = 512;
        public const int GENDER                              = 64;
        public const int HASH                                = 512;
        public const int LAST_NAME                           = 256;
        public const int LINE1                               = 512;
        public const int LINE2                               = 256;
        public const int MAX_FIXED                           = 4096;
        public const int MAX_PASSWORD_SIZE                   = 250;
        public const int MAX_SIZE                            = 0x3FFFFFDF; // string.MaxLength = 2,147,483,647 (2^31-1) - overhead
        public const int MAX_VARIABLE                        = 8192;
        public const int NAME                                = 512;
        public const int NORMALIZED_NAME                     = 1024;
        public const int NOT_FOUND                           = -1;
        public const int PASSWORD                            = 256;
        public const int PASSWORDS_MAX_LENGTH                = 255;
        public const int PASSWORDS_MIN_LENGTH                = 10;
        public const int PHONE                               = 256;
        public const int PHONE_EXT                           = 256;
        public const int POSTAL_CODE                         = 10;
        public const int REFRESH_TOKEN                       = 4096;
        public const int RIGHTS                              = 8192;
        public const int SECURITY_STAMP                      = 4096;
        public const int STATE_OR_PROVINCE                   = 256;
        public const int TITLE                               = 256;
        public const int TYPE                                = 512;
        public const int USER_NAME                           = 256;
        public const int WEBSITE                             = 8192;
    }



    public static class Telemetry
    {
        public const string PARENT_ID     = "ParentId";
        public const string PARENT_ID_KEY = "Serilog.ParentId";
        public const string SPAN_ID       = "SpanId";
        public const string SPAN_ID_KEY   = "Serilog.SpanId";
        public const string TRACE_ID      = "TraceId";
        public const string TRACE_ID_KEY  = "Serilog.TraceId";
    }



    public static class Types
    {
        public const string NULLABLE         = "System.Runtime.CompilerServices.NullableAttribute";
        public const string NULLABLE_CONTEXT = "System.Runtime.CompilerServices.NullableContextAttribute";
    }



    public static class Values
    {
        public const string ALPHA              = "alpha";
        public const string AND                = " AND ";
        public const string BETA               = "beta";
        public const string CLOSE              = " ]";
        public const string COUNT              = "count";
        public const string EMPTY              = "";
        public const string EMPTY_PHONE_NUMBER = "(000) 000-0000";
        public const string EQUALS_SPACE       = " = ";
        public const string FALSE              = "false";
        public const string LIST_SEPARATOR     = ", ";
        public const string NULL               = "null";
        public const string OPEN               = "[ ";
        public const string OR                 = " OR ";
        public const string RC                 = "rc";
        public const string TRUE               = "true";
    }
}
