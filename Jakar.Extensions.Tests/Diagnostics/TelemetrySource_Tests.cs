// Jakar.Extensions :: Jakar.Extensions.Tests
// 09/28/2026

using System.Collections.Generic;
using System.Diagnostics;



namespace Jakar.Extensions.Tests;


[TestFixture]
[NonParallelizable]
[TestOf(typeof(TelemetrySource))]

// ReSharper disable once InconsistentNaming
public class TelemetrySource_Tests : Assert
{
    private readonly List<IDisposable> __disposables = [];
    private          Activity?         __current;
    private          ActivityIdFormat  __defaultIdFormat;
    private          bool              __forceDefaultIdFormat;
    private          TelemetrySource?  __telemetrySource;


    [SetUp] public void SetUp()
    {
        __current              = Activity.Current;
        __defaultIdFormat      = Activity.DefaultIdFormat;
        __forceDefaultIdFormat = Activity.ForceDefaultIdFormat;
        __telemetrySource      = TelemetrySource.Current;
        Activity.Current       = null;
    }
    [TearDown] public void TearDown()
    {
        for ( int i = __disposables.Count - 1; i >= 0; i-- ) { __disposables[i].Dispose(); }

        __disposables.Clear();
        Activity.Current              = __current;
        Activity.DefaultIdFormat      = __defaultIdFormat;
        Activity.ForceDefaultIdFormat = __forceDefaultIdFormat;
        TelemetrySource.Current       = __telemetrySource;
    }


    private TelemetrySource CreateSource()
    {
        TelemetrySource source = new(new AppVersion(1, 2, 3), Guid.NewGuid(), $"{nameof(TelemetrySource_Tests)}.{Guid.NewGuid():N}", null);
        __disposables.Add(source);
        return source;
    }
    private void Listen( TelemetrySource source, ActivitySamplingResult result )
    {
        string name = source.Source.Name;

        ActivityListener listener = new()
                                    {
                                        ShouldListenTo      = activitySource => string.Equals(activitySource.Name, name, StringComparison.Ordinal),
                                        Sample              = ( ref ActivityCreationOptions<ActivityContext> _ ) => result,
                                        SampleUsingParentId = ( ref ActivityCreationOptions<string>          _ ) => result
                                    };

        ActivitySource.AddActivityListener(listener);
        __disposables.Add(listener);
    }
    private T Track<T>( T value )
        where T : IDisposable
    {
        __disposables.Add(value);
        return value;
    }


    [Test] public void Constructor_DoesNotChange_DefaultIdFormat()
    {
        Activity.DefaultIdFormat = ActivityIdFormat.W3C;
        _                        = CreateSource();
        this.AreEqual(ActivityIdFormat.W3C, Activity.DefaultIdFormat);
    }


    [Test] public void StartActivity_Uses_W3C()
    {
        TelemetrySource source = CreateSource();
        Listen(source, ActivitySamplingResult.AllDataAndRecorded);

        Activity? activity = source.StartActivity("x");
        this.NotNull(activity);
        Track(activity!);

        this.AreEqual(ActivityIdFormat.W3C, activity!.IdFormat);
        this.NotEqual(default,                            activity.TraceId);
        this.NotEqual(default,                            activity.SpanId);
        this.NotEqual("00000000000000000000000000000000", activity.TraceId.ToHexString());
        this.NotEqual("0000000000000000",                 activity.SpanId.ToHexString());
    }


    [Test] public void StartActivity_SampledOut_ReturnsNull()
    {
        TelemetrySource source = CreateSource();
        Listen(source, ActivitySamplingResult.None);

        Activity? activity = null;
        DoesNotThrow(() => activity = source.StartActivity("x"));
        this.IsNull(activity);
    }


    [Test] public void StartActivity_PropagationData_IsNotRecorded()
    {
        TelemetrySource source = CreateSource();
        Listen(source, ActivitySamplingResult.PropagationData);

        Activity? activity = source.StartActivity("x");
        this.NotNull(activity);
        Track(activity!);

        this.IsFalse(activity!.IsAllDataRequested);
        this.IsFalse(activity.ActivityTraceFlags.HasFlag(ActivityTraceFlags.Recorded));
    }


    [Test] public void StartActivity_Status_IsUnset()
    {
        TelemetrySource source = CreateSource();
        Listen(source, ActivitySamplingResult.AllDataAndRecorded);

        Activity? activity = source.StartActivity("x");
        this.NotNull(activity);
        Track(activity!);

        this.AreEqual(ActivityStatusCode.Unset, activity!.Status);
    }


    [Test] public void Constructor_Keeps_ActivityCurrent()
    {
        TelemetrySource parentSource = CreateSource();
        Listen(parentSource, ActivitySamplingResult.AllDataAndRecorded);

        Activity? parent = parentSource.StartActivity("parent");
        this.NotNull(parent);
        Track(parent!);
        this.AreEqual(parent, Activity.Current);

        _ = CreateSource();
        this.AreEqual(parent, Activity.Current);
    }


    [Test] public void StartActivity_Keeps_TraceState()
    {
        TelemetrySource source = CreateSource();
        Listen(source, ActivitySamplingResult.AllDataAndRecorded);

        const string    TRACE_STATE   = "vendor=value";
        ActivityContext parentContext = new(ActivityTraceId.CreateRandom(), ActivitySpanId.CreateRandom(), ActivityTraceFlags.Recorded, TRACE_STATE, true);

        Activity? activity = source.StartActivity("x", in parentContext);
        this.NotNull(activity);
        Track(activity!);

        this.AreEqual(TRACE_STATE,           activity!.TraceStateString);
        this.AreEqual(parentContext.TraceId, activity.TraceId);
    }


    [Test] public void TelemetrySpan_Targets_Source()
    {
        TelemetrySource sourceA = CreateSource();
        TelemetrySource sourceB = CreateSource();
        Listen(sourceA, ActivitySamplingResult.AllDataAndRecorded);
        Listen(sourceB, ActivitySamplingResult.AllDataAndRecorded);
        TelemetrySource.Current = sourceB;

        using TelemetrySpan span = TelemetrySpan.Create(sourceA, "x");
        this.IsTrue(span.IsValid);
        this.NotNull(Activity.Current);
        this.AreEqual(sourceA.Source.Name, Activity.Current!.Source.Name);

        using TelemetrySpan child = span.SubSpan("y");
        this.IsTrue(child.IsValid);
        this.AreEqual(sourceA.Source.Name, Activity.Current!.Source.Name);
    }


    [Test] public void TelemetrySpan_WithoutCurrent_IsNoOp()
    {
        TelemetrySource.Current = null;

        using TelemetrySpan span = TelemetrySpan.Create("x");
        this.IsFalse(span.IsValid);
    }
}
