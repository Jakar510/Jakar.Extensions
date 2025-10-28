// Jakar.Extensions :: Jakar.Extensions
// 07/29/2025  14:44

using Serilog.Events;
using ZLinq;



namespace Jakar.Extensions;


[Serializable]
[method: JsonConstructor]
public readonly struct GcInfo( long totalMemory, long totalAllocatedBytes, long allocatedBytesForCurrentThread, TimeSpan totalPauseDuration, in GcMemoryInformation info ) : IJsonModel<GcInfo>
{
    public readonly long                TotalMemory                    = totalMemory;
    public readonly TimeSpan            TotalPauseDuration             = totalPauseDuration;
    public readonly long                TotalAllocatedBytes            = totalAllocatedBytes;
    public readonly long                AllocatedBytesForCurrentThread = allocatedBytesForCurrentThread;
    public readonly GcMemoryInformation MemoryInfo                     = info;


    public static JsonSerializerContext  JsonContext   => JakarExtensionsContext.Default;
    public static JsonTypeInfo<GcInfo>   JsonTypeInfo  => JakarExtensionsContext.Default.GcInfo;
    public static JsonTypeInfo<GcInfo[]> JsonArrayInfo => JakarExtensionsContext.Default.GcInfoArray;


    public GcInfo() : this(GC.GetTotalMemory(false), GC.GetTotalAllocatedBytes(), GC.GetAllocatedBytesForCurrentThread(), GC.GetTotalPauseDuration(), GC.GetGCMemoryInfo()) { }
    public static GcInfo Create() => new();
    public LogEventProperty GetProperty() => new(nameof(GcInfo),
                                                 new StructureValue([
                                                                        Enricher.GetProperty(TotalMemory,                    nameof(TotalMemory)),
                                                                        Enricher.GetProperty(TotalPauseDuration,             nameof(TotalPauseDuration)),
                                                                        Enricher.GetProperty(TotalAllocatedBytes,            nameof(TotalAllocatedBytes)),
                                                                        Enricher.GetProperty(AllocatedBytesForCurrentThread, nameof(AllocatedBytesForCurrentThread)),
                                                                        MemoryInfo.GetProperty(nameof(MemoryInfo))
                                                                    ]));

    public static bool TryFromJson( string? json, out GcInfo result )
    {
        try
        {
            if ( string.IsNullOrWhiteSpace(json) )
            {
                result = default;
                return false;
            }

            result = FromJson(json);
            return true;
        }
        catch ( Exception e ) { SelfLogger.WriteLine("{Exception}", e.ToString()); }

        result = default;
        return false;
    }
    public static GcInfo FromJson( string json ) => Validate.ThrowIfNull(JsonSerializer.Deserialize(json, JsonTypeInfo));


    public int CompareTo( object? other ) => other is GcInfo app
                                                 ? CompareTo(app)
                                                 : throw new ExpectedValueTypeException(other, typeof(GcInfo));
    public int CompareTo( GcInfo other )
    {
        int totalMemoryComparison = TotalMemory.CompareTo(other.TotalMemory);
        if ( totalMemoryComparison != 0 ) { return totalMemoryComparison; }

        int totalPauseDurationComparison = TotalPauseDuration.CompareTo(other.TotalPauseDuration);
        if ( totalPauseDurationComparison != 0 ) { return totalPauseDurationComparison; }

        int totalAllocatedBytesComparison = TotalAllocatedBytes.CompareTo(other.TotalAllocatedBytes);
        if ( totalAllocatedBytesComparison != 0 ) { return totalAllocatedBytesComparison; }

        return AllocatedBytesForCurrentThread.CompareTo(other.AllocatedBytesForCurrentThread);
    }
    public override bool Equals( object? obj )   => obj is GcInfo other              && Equals(other);
    public          bool Equals( GcInfo  other ) => TotalMemory == other.TotalMemory && TotalPauseDuration.Equals(other.TotalPauseDuration) && TotalAllocatedBytes == other.TotalAllocatedBytes && AllocatedBytesForCurrentThread == other.AllocatedBytesForCurrentThread && MemoryInfo.Equals(other.MemoryInfo);
    public override int  GetHashCode()           => HashCode.Combine(TotalMemory, TotalPauseDuration, TotalAllocatedBytes, AllocatedBytesForCurrentThread, MemoryInfo);


    public static bool operator ==( GcInfo? left, GcInfo? right ) => Nullable.Equals(left, right);
    public static bool operator !=( GcInfo? left, GcInfo? right ) => !Nullable.Equals(left, right);
    public static bool operator ==( GcInfo  left, GcInfo  right ) => EqualityComparer<GcInfo>.Default.Equals(left, right);
    public static bool operator !=( GcInfo  left, GcInfo  right ) => !EqualityComparer<GcInfo>.Default.Equals(left, right);
    public static bool operator >( GcInfo   left, GcInfo  right ) => Comparer<GcInfo>.Default.Compare(left, right) > 0;
    public static bool operator >=( GcInfo  left, GcInfo  right ) => Comparer<GcInfo>.Default.Compare(left, right) >= 0;
    public static bool operator <( GcInfo   left, GcInfo  right ) => Comparer<GcInfo>.Default.Compare(left, right) < 0;
    public static bool operator <=( GcInfo  left, GcInfo  right ) => Comparer<GcInfo>.Default.Compare(left, right) <= 0;
}



/// <summary> Provides a set of APIs that can be used to retrieve garbage collection information. </summary>
/// <remarks> A GC is identified by its Index. which starts from 1 and increases with each GC (see more explanation of it in the Index prooperty). If you are asking for a GC that does not exist, eg, you called the GC.GetGCMemoryInfo API before a GC happened, or you are asking for a GC of GCKind.FullBlocking and no full blocking GCs have happened, you will get all 0's in the info, including the Index. So you can use Index 0 to detect that no GCs, or no GCs of the kind you specified have happened. </remarks>
[Serializable]
[method: JsonConstructor]
public readonly struct GcMemoryInformation( bool                      compacted,
                                            bool                      concurrent,
                                            long                      finalizationPendingCount,
                                            long                      fragmentedBytes,
                                            int                       generation,
                                            long                      heapSizeBytes,
                                            long                      highMemoryLoadThresholdBytes,
                                            long                      index,
                                            long                      memoryLoadBytes,
                                            double                    pauseTimePercentage,
                                            long                      pinnedObjectsCount,
                                            long                      promotedBytes,
                                            long                      totalAvailableMemoryBytes,
                                            long                      totalCommittedBytes,
                                            TimeSpan[]                pauseDurations,
                                            GcGenerationInformation[] generationInfo ) : IEquatable<GcMemoryInformation>
{
    /// <summary> High memory load threshold when this GC occurred </summary>
    public readonly long HighMemoryLoadThresholdBytes = highMemoryLoadThresholdBytes;

    /// <summary> Memory load when this GC occurred </summary>
    public readonly long MemoryLoadBytes = memoryLoadBytes;

    /// <summary> Total available memory for the GC to use when this GC occurred. If the environment variable DOTNET_GCHeapHardLimit is set, or "Server.GC.HeapHardLimit" is in runtimeconfig.json, this will come from that. If the program is run in a container, this will be an implementation-defined fraction of the container's size. Else, this is the physical memory on the machine that was available for the GC to use when this GC occurred. </summary>
    public readonly long TotalAvailableMemoryBytes = totalAvailableMemoryBytes;

    /// <summary> The total heap size when this GC occurred </summary>
    public readonly long HeapSizeBytes = heapSizeBytes;

    /// <summary> The total fragmentation when this GC occurred Let's take the example below: | OBJ_A |     OBJ_B     | OBJ_C |   OBJ_D   | OBJ_E | Let's say OBJ_B, OBJ_C and and OBJ_E are garbage and get collected, but the heap does not get compacted, the resulting heap will look like the following: | OBJ_A |           F           |   OBJ_D   | The memory between OBJ_A and OBJ_D marked `F` is considered part of the FragmentedBytes, and will be used to allocate new objects. The memory after OBJ_D will not be considered part of the FragmentedBytes, and will also be used to allocate new objects </summary>
    public readonly long FragmentedBytes = fragmentedBytes;

    /// <summary> The index of this GC. GC indices start with 1 and get increased at the beginning of a GC. Since the info is updated at the end of a GC, this means you can get the info for a BGC with a smaller index than a foreground GC finished earlier. </summary>
    public readonly long Index = index;

    /// <summary> The generation this GC collected. Collecting a generation means all its younger generation(s) are also collected. </summary>
    public readonly int Generation = generation;

    /// <summary> Is this a compacting GC or not. </summary>
    public readonly bool Compacted = compacted;

    /// <summary> Is this a concurrent GC (BGC) or not. </summary>
    public readonly bool Concurrent = concurrent;

    /// <summary> Total committed bytes of the managed heap. </summary>
    public readonly long TotalCommittedBytes = totalCommittedBytes;

    /// <summary> Promoted bytes for this GC. </summary>
    public readonly long PromotedBytes = promotedBytes;

    /// <summary> Number of pinned objects this GC observed. </summary>
    public readonly long PinnedObjectsCount = pinnedObjectsCount;

    /// <summary> Number of objects ready for finalization this GC observed. </summary>
    public readonly long FinalizationPendingCount = finalizationPendingCount;

    /// <summary> Pause durations. For blocking GCs there's only 1 pause; for BGC there are 2. </summary>
    public readonly TimeSpan[] PauseDurations = pauseDurations;

    /// <summary> This is the % pause time in GC so far. If it's 1.2%, this number is 1.2. </summary>
    public readonly double PauseTimePercentage = pauseTimePercentage;

    /// <summary> Generation info for all generations. </summary>
    public readonly GcGenerationInformation[] GenerationInfo = generationInfo;


    public GcMemoryInformation( in GCMemoryInfo data ) : this(data.Compacted,
                                                              data.Concurrent,
                                                              data.FinalizationPendingCount,
                                                              data.FragmentedBytes,
                                                              data.Generation,
                                                              data.HeapSizeBytes,
                                                              data.HighMemoryLoadThresholdBytes,
                                                              data.Index,
                                                              data.MemoryLoadBytes,
                                                              data.PauseTimePercentage,
                                                              data.PinnedObjectsCount,
                                                              data.PromotedBytes,
                                                              data.TotalAvailableMemoryBytes,
                                                              data.TotalCommittedBytes,
                                                              data.PauseDurations.ToArray(),
                                                              [
                                                                  .. data.GenerationInfo.AsValueEnumerable()
                                                                         .Select(static x => new GcGenerationInformation(x))
                                                              ]) { }


    public static implicit operator GcMemoryInformation( GCMemoryInfo info ) => new(in info);


    public StructureValue GetProperty() =>
        new([
                Enricher.GetProperty(Compacted,                    nameof(Compacted)),
                Enricher.GetProperty(HighMemoryLoadThresholdBytes, nameof(HighMemoryLoadThresholdBytes)),
                Enricher.GetProperty(MemoryLoadBytes,              nameof(MemoryLoadBytes)),
                Enricher.GetProperty(TotalAvailableMemoryBytes,    nameof(TotalAvailableMemoryBytes)),
                Enricher.GetProperty(HeapSizeBytes,                nameof(HeapSizeBytes)),
                Enricher.GetProperty(FragmentedBytes,              nameof(FragmentedBytes)),
                Enricher.GetProperty(Generation,                   nameof(Generation)),
                Enricher.GetProperty(Concurrent,                   nameof(Concurrent)),
                Enricher.GetProperty(TotalCommittedBytes,          nameof(TotalCommittedBytes)),
                Enricher.GetProperty(PromotedBytes,                nameof(PromotedBytes)),
                Enricher.GetProperty(PinnedObjectsCount,           nameof(PinnedObjectsCount)),
                Enricher.GetProperty(FinalizationPendingCount,     nameof(FinalizationPendingCount)),
                Enricher.GetProperty(PauseTimePercentage,          nameof(PauseTimePercentage)),
                Enricher.GetProperty(GenerationInfo,               nameof(GenerationInfo)),
                Enricher.GetProperty(PauseDurations,               nameof(PauseDurations))
            ]);
    public LogEventProperty GetProperty( string name ) => new(name, GetProperty());


    public bool Equals( GcMemoryInformation other ) => HighMemoryLoadThresholdBytes == other.HighMemoryLoadThresholdBytes &&
                                                       MemoryLoadBytes              == other.MemoryLoadBytes              &&
                                                       TotalAvailableMemoryBytes    == other.TotalAvailableMemoryBytes    &&
                                                       HeapSizeBytes                == other.HeapSizeBytes                &&
                                                       FragmentedBytes              == other.FragmentedBytes              &&
                                                       Index                        == other.Index                        &&
                                                       Generation                   == other.Generation                   &&
                                                       Compacted                    == other.Compacted                    &&
                                                       Concurrent                   == other.Concurrent                   &&
                                                       TotalCommittedBytes          == other.TotalCommittedBytes          &&
                                                       PromotedBytes                == other.PromotedBytes                &&
                                                       PinnedObjectsCount           == other.PinnedObjectsCount           &&
                                                       FinalizationPendingCount     == other.FinalizationPendingCount     &&
                                                       PauseDurations.Equals(other.PauseDurations)                        &&
                                                       PauseTimePercentage.Equals(other.PauseTimePercentage)              &&
                                                       GenerationInfo.Equals(other.GenerationInfo);
    public override bool Equals( object? obj ) => obj is GcMemoryInformation other && Equals(other);
    public override int GetHashCode()
    {
        HashCode hashCode = new();
        hashCode.Add(HighMemoryLoadThresholdBytes);
        hashCode.Add(MemoryLoadBytes);
        hashCode.Add(TotalAvailableMemoryBytes);
        hashCode.Add(HeapSizeBytes);
        hashCode.Add(FragmentedBytes);
        hashCode.Add(Index);
        hashCode.Add(Generation);
        hashCode.Add(Compacted);
        hashCode.Add(Concurrent);
        hashCode.Add(TotalCommittedBytes);
        hashCode.Add(PromotedBytes);
        hashCode.Add(PinnedObjectsCount);
        hashCode.Add(FinalizationPendingCount);
        hashCode.Add(PauseDurations);
        hashCode.Add(PauseTimePercentage);
        hashCode.Add(GenerationInfo);
        return hashCode.ToHashCode();
    }
    public static bool operator ==( GcMemoryInformation left, GcMemoryInformation right ) => left.Equals(right);
    public static bool operator !=( GcMemoryInformation left, GcMemoryInformation right ) => !left.Equals(right);
}



[Serializable]
[method: JsonConstructor]
public readonly struct GcGenerationInformation( long fragmentationAfterBytes, long fragmentationBeforeBytes, long sizeAfterBytes, long sizeBeforeBytes ) : IEquatable<GcGenerationInformation>
{
    /// <summary> Size in bytes on entry to the reported collection. </summary>
    public readonly long SizeBeforeBytes = sizeBeforeBytes;

    /// <summary> Fragmentation in bytes on entry to the reported collection. </summary>
    public readonly long FragmentationBeforeBytes = fragmentationBeforeBytes;

    /// <summary> Size in bytes on exit from the reported collection. </summary>
    public readonly long SizeAfterBytes = sizeAfterBytes;

    /// <summary> Fragmentation in bytes on exit from the reported collection. </summary>
    public readonly long FragmentationAfterBytes = fragmentationAfterBytes;


    public GcGenerationInformation( in GCGenerationInfo                       info ) : this(info.FragmentationAfterBytes, info.FragmentationBeforeBytes, info.SizeAfterBytes, info.SizeBeforeBytes) { }
    public static implicit operator GcGenerationInformation( GCGenerationInfo info ) => new(in info);


    public StructureValue   GetProperty()              => new([Enricher.GetProperty(SizeBeforeBytes, nameof(SizeBeforeBytes)), Enricher.GetProperty(FragmentationBeforeBytes, nameof(FragmentationBeforeBytes)), Enricher.GetProperty(SizeAfterBytes, nameof(SizeAfterBytes)), Enricher.GetProperty(FragmentationAfterBytes, nameof(FragmentationAfterBytes))]);
    public LogEventProperty GetProperty( string name ) => new(name, GetProperty());

    public          bool Equals( GcGenerationInformation other )                                    => SizeBeforeBytes == other.SizeBeforeBytes && FragmentationBeforeBytes == other.FragmentationBeforeBytes && SizeAfterBytes == other.SizeAfterBytes && FragmentationAfterBytes == other.FragmentationAfterBytes;
    public override bool Equals( object?                 obj )                                      => obj is GcGenerationInformation other     && Equals(other);
    public override int  GetHashCode()                                                              => HashCode.Combine(SizeBeforeBytes, FragmentationBeforeBytes, SizeAfterBytes, FragmentationAfterBytes);
    public static   bool operator ==( GcGenerationInformation left, GcGenerationInformation right ) => left.Equals(right);
    public static   bool operator !=( GcGenerationInformation left, GcGenerationInformation right ) => !left.Equals(right);
}
