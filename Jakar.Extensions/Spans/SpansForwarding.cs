// Jakar.Extensions
// 10/07/2026

// ValueStringBuilder and Sizes moved to Jakar.Spans (Jakar.Json SPEC.md, decision D1). The forwards keep binaries compiled against
// Jakar.Extensions 10.x working without recompiling: the runtime resolves these types in Jakar.Spans.

[assembly: TypeForwardedTo(typeof(Jakar.Extensions.ValueStringBuilder))]
[assembly: TypeForwardedTo(typeof(Jakar.Extensions.Sizes))]



namespace Jakar.Extensions;


internal static class SpansRegistration
{
#pragma warning disable CA2255 // ModuleInitializer in a library: registers a buffer size, no other side effects
    /// <summary> <see cref="Sizes"/> can't name <see cref="AppVersion"/> any more (it lives below this library), so the core registers its own types. </summary>
    [ModuleInitializer] internal static void Register() => Sizes.Register<AppVersion>(200);
#pragma warning restore CA2255
}
