/*
// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 03/13/2025  13:03

using System.Diagnostics.Metrics;
using System.Runtime;
using System.Runtime.Versioning;
using System.Threading;



namespace Jakar.Extensions.Telemetry.Meters;


internal static class TelemetryRuntimeMetrics
{
    [ThreadStatic] private static bool _t_HandlingFirstChanceException;

    private const string METER_NAME = "System.Runtime";

    public static readonly TelemetryMeter<double> MeterDouble = new()
                                                                {
                                                                    Name                = METER_NAME,
                                                                    DefaultCurrentValue = 0,
                                                                    DefaultPrecision    = 1
                                                                };
    public static readonly TelemetryMeter<long> MeterLong = new()
                                                            {
                                                                Name                = METER_NAME,
                                                                DefaultCurrentValue = 0,
                                                                DefaultPrecision    = 1
                                                            };


    private static readonly TelemetryCounter<long> _s_Exceptions;

    public static void EnsureInitialized()
    {
        // Dummy method to ensure that the static constructor run and created the meters
    }


    static TelemetryRuntimeMetrics()
    {
        MeterLong.CreateCounter( GetGarbageCollectionCounts, "dotnet.gc.collections", unit: "{collection}", description: "The number of garbage collections that have occurred since the process has started.", null );

        MeterDouble.CreateCounter( static () => Environment.WorkingSet, "dotnet.process.memory.working_set", unit: "By", description: "The number of bytes of physical memory mapped to the process context." );

        MeterLong.CreateCounter( static () => GC.GetTotalAllocatedBytes(), "dotnet.gc.heap.total_allocated", unit: "By", description: "The approximate number of bytes allocated on the managed GC heap since the process has started. The returned value does not include any native allocations." );

        MeterLong.CreateCounter( static () => GC.GetGCMemoryInfo().TotalCommittedBytes, "dotnet.gc.last_collection.memory.committed_size", unit: "By", description: "The amount of committed virtual memory in use by the .NET GC, as observed during the latest garbage collection." );

        MeterLong.CreateCounter( GetHeapSizes, "dotnet.gc.last_collection.heap.size", unit: "By", description: "The managed GC heap size (including fragmentation), as observed during the latest garbage collection." );

        MeterLong.CreateCounter( GetHeapFragmentation, "dotnet.gc.last_collection.heap.fragmentation.size", unit: "By", description: "The heap fragmentation, as observed during the latest garbage collection." );

        MeterDouble.CreateCounter( static () => GC.GetTotalPauseDuration().TotalSeconds, "dotnet.gc.pause.time", unit: "s", description: "The total amount of time paused in GC since the process has started." );

        MeterLong.CreateCounter( static () => JitInfo.GetCompiledILBytes(), "dotnet.jit.compiled_il.size", unit: "By", description: "Count of bytes of intermediate language that have been compiled since the process has started." );

        MeterLong.CreateCounter( static () => JitInfo.GetCompiledMethodCount(), "dotnet.jit.compiled_methods", unit: "{method}", description: "The number of times the JIT compiler (re)compiled methods since the process has started." );

        MeterDouble.CreateCounter( static () => JitInfo.GetCompilationTime().TotalSeconds, "dotnet.jit.compilation.time", unit: "s", description: "The number of times the JIT compiler (re)compiled methods since the process has started." );

        MeterLong.CreateCounter( static () => Monitor.LockContentionCount, "dotnet.monitor.lock_contentions", unit: "{contention}", description: "The number of times there was contention when trying to acquire a monitor lock since the process has started." );

        MeterLong.CreateCounter( static () => ThreadPool.ThreadCount, "dotnet.thread_pool.thread.count", unit: "{thread}", description: "The number of thread pool threads that currently exist." );

        MeterLong.CreateCounter( static () => ThreadPool.CompletedWorkItemCount, "dotnet.thread_pool.work_item.count", unit: "{work_item}", description: "The number of work items that the thread pool has completed since the process has started." );

        MeterLong.CreateCounter( static () => ThreadPool.PendingWorkItemCount, "dotnet.thread_pool.queue.length", unit: "{work_item}", description: "The number of work items that are currently queued to be processed by the thread pool." );

        MeterDouble.CreateCounter( static () => Timer.ActiveCount, "dotnet.timer.count", unit: "{timer}", description: "The number of timer instances that are currently active. An active timer is registered to tick at some point in the future and has not yet been canceled." );

        MeterDouble.CreateCounter( static () => AppDomain.CurrentDomain.GetAssemblies().Length, "dotnet.assembly.count", unit: "{assembly}", description: "The number of .NET assemblies that are currently loaded." );

        _s_Exceptions = MeterLong.CreateCounter( "dotnet.exceptions", unit: "{exception}", description: "The number of exceptions that have been thrown in managed code." );

        AppDomain.CurrentDomain.FirstChanceException += ( source, e ) =>
                                                        {
                                                            // Avoid recursion if the listener itself throws an exception while recording the measurement
                                                            // in its `OnMeasurementRecorded` callback.
                                                            if ( _t_HandlingFirstChanceException ) return;
                                                            _t_HandlingFirstChanceException = true;
                                                            _s_Exceptions.RecordMeasurement( 1, new Pair( "error.type", e.Exception.GetType().Name ) );
                                                            _t_HandlingFirstChanceException = false;
                                                        };

        MeterDouble.CreateCounter( static () => Environment.ProcessorCount, "dotnet.process.cpu.count", unit: "{cpu}", description: "The number of processors available to the process." );

        if ( OperatingSystem.IsBrowser() is false && OperatingSystem.IsTvOS() is false && (OperatingSystem.IsIOS() || OperatingSystem.IsMacCatalyst()) ) { MeterDouble.CreateCounter( GetCpuTime, "dotnet.process.cpu.time", unit: "s", description: "CPU time used by the process." ); }
    }

}
*/
