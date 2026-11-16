// Jakar.Extensions :: Jakar.Extensions
// 11/15/2026

// Since 10.12.1 these live in Jakar.ErrorOr (github.com/Jakar510/Jakar.ErrorOr), which Jakar.Extensions references:
// code compiled against an older Jakar.Extensions still finds them here.
[assembly: TypeForwardedTo(typeof(Jakar.Extensions.Pair))]
[assembly: TypeForwardedTo(typeof(Jakar.Extensions.Pair<>))]
[assembly: TypeForwardedTo(typeof(Jakar.Extensions.StringTags))]
[assembly: TypeForwardedTo(typeof(Jakar.Extensions.Status))]
[assembly: TypeForwardedTo(typeof(Jakar.Extensions.Statuses))]
[assembly: TypeForwardedTo(typeof(Jakar.Extensions.IErrorDetails))]
[assembly: TypeForwardedTo(typeof(Jakar.Extensions.IErrorTitles))]
[assembly: TypeForwardedTo(typeof(Jakar.Extensions.ErrorTitles))]
[assembly: TypeForwardedTo(typeof(Jakar.Extensions.NullAsEmptyArrayConverter<,>))]



namespace Jakar.Extensions;


internal static class ErrorOrForwarding
{
#pragma warning disable CA2255 // ModuleInitializer in a library: registers exception → status mappings, no other side effects
    /// <summary> Statuses (Jakar.ErrorOr) maps the BCL exceptions; these are Jakar.Extensions' own. A module initializer runs before any type here is used, so before one of these exceptions can exist. </summary>
    [ModuleInitializer] internal static void Register()
#pragma warning restore CA2255
    {
        Statuses.RegisterStatus<ApiDisabledException>(Status.Disabled);
        Statuses.RegisterStatus<ExpectedValueTypeException>(Status.InternalServerError);
        Statuses.RegisterStatus<OutOfRangeException>(Status.InternalServerError);
        Statuses.RegisterStatus<AppOutDatedException>(Status.ExpectationFailed);
    }
}