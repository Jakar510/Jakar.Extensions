// Jakar.Extensions :: Jakar.Extensions
// 06/06/2022  3:43 PM

namespace Jakar.Extensions;


public static class Statuses
{
    private static readonly ConcurrentDictionary<Type, Status> _statusMap = new()
                                                                            {
                                                                                [typeof(NotImplementedException)]     = Status.NotImplemented,
                                                                                [typeof(UnauthorizedAccessException)] = Status.Unauthorized,
                                                                                [typeof(ValidationException)]         = Status.BadRequest,
                                                                                [typeof(ApiDisabledException)]        = Status.Disabled,
                                                                                [typeof(ExpectedValueTypeException)]  = Status.InternalServerError,
                                                                                [typeof(OutOfRangeException)]         = Status.InternalServerError,
                                                                                [typeof(ArgumentOutOfRangeException)] = Status.InternalServerError,
                                                                                [typeof(ArgumentNullException)]       = Status.InternalServerError,
                                                                                [typeof(ArgumentException)]           = Status.InternalServerError,
                                                                                [typeof(InvalidOperationException)]   = Status.InternalServerError,
                                                                                [typeof(FileLoadException)]           = Status.ServiceUnavailable,
                                                                                [typeof(IOException)]                 = Status.ServiceUnavailable,
                                                                                [typeof(ApiDisabledException)]        = Status.Disabled,
                                                                                [typeof(AppOutDatedException)]        = Status.ExpectationFailed,
                                                                                [typeof(AmbiguousMatchException)]     = Status.Conflict,
                                                                                [typeof(DuplicateNameException)]      = Status.Conflict,
                                                                                [typeof(AccessViolationException)]    = Status.Unauthorized,
                                                                                [typeof(HttpRequestException)]        = Status.ServiceUnavailable,
                                                                                [typeof(ValidationException)]         = Status.BadRequest,
                                                                                [typeof(OperationCanceledException)]  = Status.RequestTimeout,
                                                                            };
    public static Func<Exception, Status> GetStatusFromException { get; set; } = Statuses.AsStatus;


    public static void RegisterStatus<TException>( Status status )
        where TException : Exception => _statusMap[typeof(TException)] = status;
    public static Status AsStatus( this Exception e )
    {
        Type type = e.GetType();
        if ( _statusMap.TryGetValue(type, out var status) ) { return status; }

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach ( KeyValuePair<Type, Status> kv in _statusMap )
        {
            if ( kv.Key.IsAssignableFrom(type) ) { return kv.Value; }
        }

        return Status.InternalServerError;
    }



    extension( HttpStatusCode code )
    {
        public Status ToStatus() => (Status)code;
        public string ToStringFast() => code.ToStatus()
                                            .ToStringFast();
    }



    extension( Status code )
    {
        public HttpStatusCode ToStatus() => (HttpStatusCode)code;
        public string ToStringFast() => code switch
                                        {
                                            Status.Continue                      => nameof(Status.Continue),
                                            Status.SwitchingProtocols            => nameof(Status.SwitchingProtocols),
                                            Status.Processing                    => nameof(Status.Processing),
                                            Status.EarlyHints                    => nameof(Status.EarlyHints),
                                            Status.Ok                            => nameof(Status.Ok),
                                            Status.Created                       => nameof(Status.Created),
                                            Status.Accepted                      => nameof(Status.Accepted),
                                            Status.NonAuthoritativeInformation   => nameof(Status.NonAuthoritativeInformation),
                                            Status.NoContent                     => nameof(Status.NoContent),
                                            Status.ResetContent                  => nameof(Status.ResetContent),
                                            Status.PartialContent                => nameof(Status.PartialContent),
                                            Status.MultiStatus                   => nameof(Status.MultiStatus),
                                            Status.AlreadyReported               => nameof(Status.AlreadyReported),
                                            Status.ImUsed                        => nameof(Status.ImUsed),
                                            Status.Ambiguous                     => nameof(Status.Ambiguous),
                                            Status.Moved                         => nameof(Status.Moved),
                                            Status.Found                         => nameof(Status.Found),
                                            Status.RedirectMethod                => nameof(Status.RedirectMethod),
                                            Status.NotModified                   => nameof(Status.NotModified),
                                            Status.UseProxy                      => nameof(Status.UseProxy),
                                            Status.Unused                        => nameof(Status.Unused),
                                            Status.RedirectKeepVerb              => nameof(Status.RedirectKeepVerb),
                                            Status.PermanentRedirect             => nameof(Status.PermanentRedirect),
                                            Status.BadRequest                    => nameof(Status.BadRequest),
                                            Status.Unauthorized                  => nameof(Status.Unauthorized),
                                            Status.PaymentRequired               => nameof(Status.PaymentRequired),
                                            Status.Forbidden                     => nameof(Status.Forbidden),
                                            Status.NotFound                      => nameof(Status.NotFound),
                                            Status.MethodNotAllowed              => nameof(Status.MethodNotAllowed),
                                            Status.NotAcceptable                 => nameof(Status.NotAcceptable),
                                            Status.ProxyAuthenticationRequired   => nameof(Status.ProxyAuthenticationRequired),
                                            Status.RequestTimeout                => nameof(Status.RequestTimeout),
                                            Status.Conflict                      => nameof(Status.Conflict),
                                            Status.Gone                          => nameof(Status.Gone),
                                            Status.LengthRequired                => nameof(Status.LengthRequired),
                                            Status.PreconditionFailed            => nameof(Status.PreconditionRequired),
                                            Status.RequestEntityTooLarge         => nameof(Status.RequestEntityTooLarge),
                                            Status.RequestUriTooLong             => nameof(Status.RequestUriTooLong),
                                            Status.UnsupportedMediaType          => nameof(Status.UnsupportedMediaType),
                                            Status.RequestedRangeNotSatisfiable  => nameof(Status.RequestedRangeNotSatisfiable),
                                            Status.ExpectationFailed             => nameof(Status.ExpectationFailed),
                                            Status.Teapot                        => nameof(Status.Teapot),
                                            Status.MisdirectedRequest            => nameof(Status.MisdirectedRequest),
                                            Status.UnprocessableEntity           => nameof(Status.UnprocessableEntity),
                                            Status.Locked                        => nameof(Status.Locked),
                                            Status.FailedDependency              => nameof(Status.FailedDependency),
                                            Status.TooEarly                      => nameof(Status.TooEarly),
                                            Status.UpgradeRequired               => nameof(Status.UpgradeRequired),
                                            Status.PreconditionRequired          => nameof(Status.PreconditionRequired),
                                            Status.TooManyRequests               => nameof(Status.TooManyRequests),
                                            Status.Disabled                      => nameof(Status.Disabled),
                                            Status.RequestHeaderFieldsTooLarge   => nameof(Status.RequestHeaderFieldsTooLarge),
                                            Status.AlreadyExists                 => nameof(Status.AlreadyExists),
                                            Status.NoResponse                    => nameof(Status.NoResponse),
                                            Status.UnavailableForLegalReasons    => nameof(Status.UnavailableForLegalReasons),
                                            Status.ClientClosedRequest           => nameof(Status.ClientClosedRequest),
                                            Status.InternalServerError           => nameof(Status.InternalServerError),
                                            Status.NotImplemented                => nameof(Status.NotImplemented),
                                            Status.BadGateway                    => nameof(Status.BadGateway),
                                            Status.ServiceUnavailable            => nameof(Status.ServiceUnavailable),
                                            Status.GatewayTimeout                => nameof(Status.GatewayTimeout),
                                            Status.HttpVersionNotSupported       => nameof(Status.HttpVersionNotSupported),
                                            Status.VariantAlsoNegotiates         => nameof(Status.VariantAlsoNegotiates),
                                            Status.InsufficientStorage           => nameof(Status.InsufficientStorage),
                                            Status.LoopDetected                  => nameof(Status.LoopDetected),
                                            Status.NotExtended                   => nameof(Status.NotExtended),
                                            Status.NetworkAuthenticationRequired => nameof(Status.NetworkAuthenticationRequired),
                                            _                                    => code.ToString()
                                        };
    }
}
