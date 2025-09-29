/*
// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 03/14/2025  15:03

using System.Runtime;
using System.Runtime.ExceptionServices;
using System.Runtime.Versioning;
using System.Threading;
using Serilog;
using Serilog.Core;
using Serilog.Events;



namespace Jakar.Extensions.Telemetry.Meters;


public sealed class RuntimeMetricsEnricher : ILogEventEnricher, IDisposable
{
    [ThreadStatic] private static bool _handlingFirstChanceException;

    // These MUST align to the possible attribute values defined in the semantic conventions (TODO: link to the spec)
    public static readonly string[] GcGenNames = ["gen0", "gen1", "gen2", "loh", "poh"];

    public static readonly int MaxGenerations = Math.Min( GC.GetGCMemoryInfo().GenerationInfo.Length, GcGenNames.Length );


    public void Dispose()                                                             { }
    public void Enrich( LogEvent logEvent, ILogEventPropertyFactory propertyFactory ) { }


    public RuntimeMetricsEnricher()
    {
        // CreateCounter( "dotnet.exceptions", unit: "{exception}", description: "The number of exceptions that have been thrown in managed code." );
        // AppDomain.CurrentDomain.FirstChanceException += OnCurrentDomainOnFirstChanceException;

        // if ( OperatingSystem.IsBrowser() is false && OperatingSystem.IsTvOS() is false && (OperatingSystem.IsIOS() || OperatingSystem.IsMacCatalyst()) ) { CreateCounter( GetCpuTime, "dotnet.process.cpu.time", unit: "s", description: "CPU time used by the process." ); }
    }
    public static void OnCurrentDomainOnFirstChanceException( object? source, FirstChanceExceptionEventArgs args )
    {
        // Avoid recursion if the listener itself throws an exception while recording the measurement
        // in its `OnMeasurementRecorded` callback.
        if ( _handlingFirstChanceException ) { return; }

        _handlingFirstChanceException = true;
        Exception e = args.Exception;
        Log.Logger.Error( e, "{Caller} - {ExceptionType}", e.Source, e.GetType().Name );
        _handlingFirstChanceException = false;
    }


    public static LogEventProperty Get_Environment_WorkingSet( ILogEventPropertyFactory propertyFactory )
    {
        const string NAME        = "dotnet.process.memory.working_set";
        const string UNIT        = "By";
        const string DESCRIPTION = "The number of bytes of physical memory mapped to the process context.";
        var          value       = Environment.WorkingSet;
        return propertyFactory.CreateProperty( NAME, value );
    }
    public static LogEventProperty Get_GC_GetTotalAllocatedBytes( ILogEventPropertyFactory propertyFactory )
    {
        const string NAME        = "dotnet.gc.heap.total_allocated";
        const string UNIT        = "By";
        const string DESCRIPTION = "The approximate number of bytes allocated on the managed GC heap since the process has started. The returned value does not include any native allocations.";
        var          value       = GC.GetTotalAllocatedBytes();
        return propertyFactory.CreateProperty( NAME, value );
    }
    public static LogEventProperty Get_GC_GetGCMemoryInfo( ILogEventPropertyFactory propertyFactory )
    {
        const string NAME        = "dotnet.gc.last_collection.memory.committed_size";
        const string UNIT        = "By";
        const string DESCRIPTION = "The amount of committed virtual memory in use by the .NET GC, as observed during the latest garbage collection.";
        var          value       = GC.GetGCMemoryInfo().TotalCommittedBytes;
        return propertyFactory.CreateProperty( NAME, value );
    }
    public static LogEventProperty Get_GC_GetTotalPauseDuration( ILogEventPropertyFactory propertyFactory )
    {
        const string NAME        = "dotnet.gc.pause.time";
        const string UNIT        = "seconds";
        const string DESCRIPTION = "The total amount of time paused in GC since the process has started.";
        var          value       = GC.GetTotalPauseDuration().TotalSeconds;
        return propertyFactory.CreateProperty( NAME, value );
    }
    public static LogEventProperty Get_JitInfo_GetCompiledILBytes( ILogEventPropertyFactory propertyFactory )
    {
        const string NAME        = "dotnet.jit.compiled_il.size";
        const string UNIT        = "By";
        const string DESCRIPTION = "Count of bytes of intermediate language that have been compiled since the process has started.";
        var          value       = JitInfo.GetCompiledILBytes();
        return propertyFactory.CreateProperty( NAME, value );
    }
    public static LogEventProperty Get_JitInfo_GetCompiledMethodCount( ILogEventPropertyFactory propertyFactory )
    {
        const string NAME        = "dotnet.jit.compiled_methods";
        const string UNIT        = "{method}";
        const string DESCRIPTION = "The number of times the JIT compiler (re)compiled methods since the process has started.";
        var          value       = JitInfo.GetCompiledMethodCount();
        return propertyFactory.CreateProperty( NAME, value );
    }
    public static LogEventProperty Get_JitInfo_GetCompilationTime( ILogEventPropertyFactory propertyFactory )
    {
        const string NAME        = "dotnet.jit.compilation.time";
        const string UNIT        = "seconds";
        const string DESCRIPTION = "The number of times the JIT compiler (re)compiled methods since the process has started.";
        var          value       = JitInfo.GetCompilationTime().TotalSeconds;
        return propertyFactory.CreateProperty( NAME, value );
    }
    public static LogEventProperty Get_Monitor_LockContentionCount( ILogEventPropertyFactory propertyFactory )
    {
        const string NAME        = "dotnet.monitor.lock_contentions";
        const string UNIT        = "{contention}";
        const string DESCRIPTION = "The number of times there was contention when trying to acquire a monitor lock since the process has started.";
        var          value       = Monitor.LockContentionCount;
        return propertyFactory.CreateProperty( NAME, value );
    }
    public static LogEventProperty Get_Environment_ProcessorCount( ILogEventPropertyFactory propertyFactory )
    {
        const string NAME        = "dotnet.process.cpu.count";
        const string UNIT        = "{cpu}";
        const string DESCRIPTION = "The number of processors available to the process.";
        var          value       = Environment.ProcessorCount;
        return propertyFactory.CreateProperty( NAME, value );
    }
    public static LogEventProperty Get_ThreadPool_ThreadCount( ILogEventPropertyFactory propertyFactory )
    {
        const string NAME        = "dotnet.thread_pool.thread.count";
        const string UNIT        = "{thread}";
        const string DESCRIPTION = "The number of thread pool threads that currently exist.";
        var          value       = ThreadPool.ThreadCount;
        return propertyFactory.CreateProperty( NAME, value );
    }
    public static LogEventProperty Get_ThreadPool_CompletedWorkItemCount( ILogEventPropertyFactory propertyFactory )
    {
        const string NAME        = "dotnet.thread_pool.work_item.count";
        const string UNIT        = "{work_item}";
        const string DESCRIPTION = "The number of work items that the thread pool has completed since the process has started.";
        var          value       = ThreadPool.CompletedWorkItemCount;
        return propertyFactory.CreateProperty( NAME, value );
    }
    public static LogEventProperty Get_ThreadPool_PendingWorkItemCount( ILogEventPropertyFactory propertyFactory )
    {
        const string NAME        = "dotnet.thread_pool.queue.length";
        const string UNIT        = "{work_item}";
        const string DESCRIPTION = "The number of work items that are currently queued to be processed by the thread pool.";
        var          value       = ThreadPool.PendingWorkItemCount;
        return propertyFactory.CreateProperty( NAME, value );
    }
    public static LogEventProperty Get_Timer_ActiveCount( ILogEventPropertyFactory propertyFactory )
    {
        const string NAME        = "dotnet.timer.count";
        const string UNIT        = "{timer}";
        const string DESCRIPTION = "The number of timer instances that are currently active. An active timer is registered to tick at some point in the future and has not yet been canceled.";
        return propertyFactory.CreateProperty( NAME, Timer.ActiveCount );
    }
    public static LogEventProperty GetLoadedAssemblyCount( ILogEventPropertyFactory propertyFactory )
    {
        const string NAME        = "dotnet.assembly.count";
        const string UNIT        = "{assembly}";
        const string DESCRIPTION = "The number of .NET assemblies that are currently loaded.";
        var          value       = AppDomain.CurrentDomain.GetAssemblies().Length;
        return propertyFactory.CreateProperty( NAME, value );
    }


    public static LogEventProperty GetGarbageCollectionCounts()
    {
        const string    NAME                            = "dotnet.gc.collections";
        const string    UNIT                            = "{collection}";
        const string    DESCRIPTION                     = "The number of garbage collections that have occurred since the process has started.";
        long            collectionsFromHigherGeneration = 0;
        Reading<long>[] array                           = new Reading<long>[GC.MaxGeneration];

        for ( int gen = GC.MaxGeneration; gen >= 0; --gen )
        {
            long collectionsFromThisGeneration = GC.CollectionCount( gen );
            array[gen]                      = new Reading<long>( collectionsFromThisGeneration - collectionsFromHigherGeneration, new Pair( "gc.heap.generation", GcGenNames[gen] ) );
            collectionsFromHigherGeneration = collectionsFromThisGeneration;
        }

        return array;
    }

    [UnsupportedOSPlatform( "tvos" )]
    [UnsupportedOSPlatform( "browser" )]
    [SupportedOSPlatform( "ios" )]
    [SupportedOSPlatform( "maccatalyst" )]
    public static LogEventProperty GetCpuTime()
    {
        Debug.Assert( !OperatingSystem.IsBrowser() && !OperatingSystem.IsTvOS() && !(OperatingSystem.IsIOS() && !OperatingSystem.IsMacCatalyst()) );
        Environment.ProcessCpuUsage processCpuUsage = Environment.CpuUsage;
        return [new Reading<double>( processCpuUsage.UserTime.TotalSeconds, new Pair( "cpu.mode", "user" ) ), new Reading<double>( processCpuUsage.PrivilegedTime.TotalSeconds, new Pair( "cpu.mode", "system" ) )];
    }

    public static LogEventProperty GetHeapSizes()
    {
        const string    NAME        = "dotnet.gc.last_collection.heap.size";
        const string    UNIT        = "By";
        const string    DESCRIPTION = "The managed GC heap size (including fragmentation), as observed during the latest garbage collection.";
        GCMemoryInfo    gcInfo      = GC.GetGCMemoryInfo();
        Reading<long>[] array       = new Reading<long>[MaxGenerations];
        for ( int i = 0; i < MaxGenerations; ++i ) { array[i] = new Reading<long>( gcInfo.GenerationInfo[i].SizeAfterBytes, new Pair( "gc.heap.generation", GcGenNames[i] ) ); }

        return array;
    }

    public static LogEventProperty GetHeapFragmentation()
    {
        const string    NAME        = "dotnet.gc.last_collection.heap.fragmentation.size";
        const string    UNIT        = "By";
        const string    DESCRIPTION = "The heap fragmentation, as observed during the latest garbage collection.";
        GCMemoryInfo    gcInfo      = GC.GetGCMemoryInfo();
        Reading<long>[] array       = new Reading<long>[MaxGenerations];
        for ( int i = 0; i < MaxGenerations; ++i ) { array[i] = new Reading<long>( gcInfo.GenerationInfo[i].FragmentationAfterBytes, new Pair( "gc.heap.generation", GcGenNames[i] ) ); }

        return;
    }
}
*/
