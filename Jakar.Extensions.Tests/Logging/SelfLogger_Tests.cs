// Jakar.Extensions :: Jakar.Extensions.Tests

using System.Collections.Generic;



namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf(typeof(SelfLogger))]
[NonParallelizable] // Enable/Disable swap process-wide state
public class SelfLogger_Tests : Assert
{
    // Strips the "<timestamp> " prefix.
    private static string Body( string rendered ) => rendered[( rendered.IndexOf(' ') + 1 )..];


    [Test] public void NamedHoles_AreFilledInOrder()   => this.AreEqual("a \n b",      Body(SelfLogger.Render("{Error} \n {StackTrace}", "a", "b")));
    [Test] public void DestructureMarkers_AreIgnored() => this.AreEqual("a b",         Body(SelfLogger.Render("{@First} {$Second}",      "a", "b")));
    [Test] public void NumericHoles_UseIndex()         => this.AreEqual("b a",         Body(SelfLogger.Render("{1} {0}",                 "a", "b")));
    [Test] public void Format_IsApplied()              => this.AreEqual("0042",        Body(SelfLogger.Render("{Value:D4}",              42)));
    [Test] public void Alignment_IsApplied()           => this.AreEqual("   ab|",      Body(SelfLogger.Render("{Value,5}|",              "ab")));
    [Test] public void LeftAlignment_IsApplied()       => this.AreEqual("ab   |",      Body(SelfLogger.Render("{Value,-5}|",             "ab")));
    [Test] public void AlignmentAndFormat_AreApplied() => this.AreEqual(" 0042|",      Body(SelfLogger.Render("{Value,5:D4}|",           42)));
    [Test] public void Escapes_AreHonoured()           => this.AreEqual("{x} a }",     Body(SelfLogger.Render("{{x}} {Name} }",          "a")));
    [Test] public void MissingArgument_IsLeftAsIs()    => this.AreEqual("a {Missing}", Body(SelfLogger.Render("{Name} {Missing}",        "a")));
    [Test] public void UnclosedHole_IsLiteral()        => this.AreEqual("a {oops",     Body(SelfLogger.Render("{Name} {oops",            "a")));
    [Test] public void NullArgument_RendersEmpty()     => this.AreEqual("[]",          Body(SelfLogger.Render("[{Name}]",                (string?)null)));
    [Test] public void NoArguments_KeepsHoles()        => this.AreEqual("x {Name}",    Body(SelfLogger.Render("x {Name}")));

    [Test] public void EveryArgumentPosition_IsReachable()
    {
        this.AreEqual("1 2 3 4 5",  Body(SelfLogger.Render("{A} {B} {C} {D} {E}", 1,    2,   3,   4, 5)));
        this.AreEqual("5 4 3 2 1",  Body(SelfLogger.Render("{4} {3} {2} {1} {0}", 1,    2,   3,   4, 5)));
        this.AreEqual("a b c d",    Body(SelfLogger.Render("{A} {B} {C} {D}",     "a",  "b", "c", "d")));
        this.AreEqual("c b a",      Body(SelfLogger.Render("{2} {1} {0}",         "a",  "b", "c")));
        this.AreEqual("True x 2.5", Body(SelfLogger.Render("{A} {B} {C}",         true, 'x', 2.5m)));
    }

    [Test] public void ManyDistinctFormats_BeyondTheCacheLimit_StillRender()
    {
        for ( int width = 1; width <= 300; width++ )
        {
            string format = "D" + width;
            this.AreEqual(42.ToString(format), Body(SelfLogger.Render("{Value:" + format + "}", 42)));
            this.AreEqual(42.ToString(format), Body(SelfLogger.Render("{Value:" + format + "}", 42))); // second pass hits the cache (or not, past the limit)
        }
    }

    [Test] public void Prefix_IsRoundTripUtcTimestamp()
    {
        string rendered = SelfLogger.Render("x");
        string stamp    = rendered[..rendered.IndexOf(' ')];
        this.IsTrue(DateTime.TryParse(stamp, null, System.Globalization.DateTimeStyles.RoundtripKind, out DateTime parsed));
        this.AreEqual(DateTimeKind.Utc, parsed.Kind);
    }

    [Test] public void WriteLine_WithNamedHoles_DoesNotThrow()
    {
        string? output = Capture(static () => SelfLogger.WriteLine("{Error} \n {StackTrace}", "message", "trace"));

        Assert.IsNotNull(output);
        this.IsTrue(output!.EndsWith("message \n trace", StringComparison.Ordinal));
    }

    [Test] public void FailureListener_MessageWithBraces_DoesNotThrow()
    {
        InvalidOperationException error = new("bad {json}");

        string? withException    = Capture(() => SelfLogger.FailureListener.OnLoggingFailed(this, "Kind", "failed {x}", error));
        string? withoutException = Capture(() => SelfLogger.FailureListener.OnLoggingFailed(this, "Kind", "failed {x}", null));

        this.IsTrue(withException!.Contains($"failed {{x}} (Kind){Environment.NewLine}System.InvalidOperationException: bad {{json}}", StringComparison.Ordinal));
        this.IsTrue(withoutException!.EndsWith("failed {x} (Kind)", StringComparison.Ordinal));
    }

    [Test] public void FailureListener_WithEvents_ReportsCount()
    {
        string? counted = Capture(() => SelfLogger.FailureListener.OnLoggingFailed(this,
                                                                                   3,
                                                                                   "failed",
                                                                                   new List<int>
                                                                                   {
                                                                                       1,
                                                                                       2
                                                                                   },
                                                                                   null));

        string? unknown = Capture(() => SelfLogger.FailureListener.OnLoggingFailed<int, int>(this, 3, "failed", null, new InvalidOperationException("boom")));

        this.IsTrue(counted!.EndsWith("failed (3, 2 events)", StringComparison.Ordinal));
        this.IsTrue(unknown!.Contains($"failed (3, unspecified events){Environment.NewLine}System.InvalidOperationException: boom", StringComparison.Ordinal));
    }


    private static string? Capture( Action action )
    {
        string? output = null;
        SelfLogger.Enable(m => output = m);

        try { action(); }
        finally { SelfLogger.Disable(); }

        return output;
    }
}
