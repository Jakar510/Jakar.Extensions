// Jakar.Extensions :: Jakar.Extensions.Tests
// 10/02/2026

using System.Runtime.CompilerServices;



namespace Jakar.Extensions.Tests.Serialization;


internal static class TestJsonSetup
{
    /// <summary> Json.Options freezes on first use, so register the test context before any test runs (what an app does at startup). </summary>
    [ModuleInitializer] internal static void Register() => Json.AddResolver(TestJsonContext.Default);
}
