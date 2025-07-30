// Jakar.Extensions :: Jakar.Extensions
// 07/29/2025  14:44

namespace Jakar.Extensions;


public readonly record struct GcInfo( long TotalMemory, long TotalAllocatedBytes, long AllocatedBytesForCurrentThread, TimeSpan TotalPauseDuration, GCMemoryInfo Info )
{
    public GcInfo() : this(GC.GetTotalMemory(false), GC.GetTotalAllocatedBytes(), GC.GetAllocatedBytesForCurrentThread(), GC.GetTotalPauseDuration(), GC.GetGCMemoryInfo()) { }


    public readonly long         TotalMemory                    = TotalMemory;
    public readonly TimeSpan     TotalPauseDuration             = TotalPauseDuration;
    public readonly long         TotalAllocatedBytes            = TotalAllocatedBytes;
    public readonly long         AllocatedBytesForCurrentThread = AllocatedBytesForCurrentThread;
    public readonly GCMemoryInfo Info                           = Info;
    public static   GcInfo       Create() => new();
}
