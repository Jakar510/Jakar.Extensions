// Jakar.Extensions :: Jakar.Database
// 09/25/2025  23:14

using ZiggyCreatures.Caching.Fusion.Internals;
using ZiggyCreatures.Caching.Fusion.Serialization;



namespace Jakar.Database;


public interface IFusionCacheEntryOptions
{
    /// <summary>
    /// The amount of time after which a cache entry is <strong>considered expired</strong>.
    /// <br/><br/>
    /// Please note the wording "considered expired" here: what it means is that, although from the OUTSIDE what is observed is always the same (a piece of data logically expires after the specified <see cref="Duration"/>), on the INSIDE things change depending on the fact that fail-safe is enabled or not.
    /// <br/>
    /// More specifically:
    /// <br/>
    /// - if <see cref="IsFailSafeEnabled"/> is set to <see langword="false"/> the <see cref="Duration"/> corresponds to the actual underlying duration in the cache, nothing more, nothing less
    /// <br/>
    /// - if <see cref="IsFailSafeEnabled" /> is set to <see langword="true"/>, the underlying duration in the cache corresponds to <see cref="FailSafeMaxDuration"/> and the <see cref="Duration"/> option is used internally as a way to indicate when the data should be considered stale (expired), without making it actually expire inside the cache levels (memory and/or distributed)
    /// <br/><br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/FailSafe.md"/>
    /// <br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Options.md"/>
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// The threshold to apply when deciding whether to refresh the cache entry eagerly (that is, before the actual expiration).
    /// <br/>
    /// This value is intended as a percentage of the <see cref="Duration"/> option, expressed as a value between 0.0 and 1.0 (eg: 0.5 = 50%, 0.75 = 75%, etc).
    /// <br/><br/>
    /// For example by setting it to 0.8 (80%) with a <see cref="Duration"/> of 10 minutes, if there's a cache access for the entry after 8 minutes (80% of 10 minutes) an eager refresh will automatically start in the background, while immediately returning the (still valid) cached value to the caller.
    /// <br/><br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/EagerRefresh.md"/>
    /// <br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Options.md"/>
    /// </summary>
    public float? EagerRefreshThreshold { get; set; }

    /// <summary>
    /// The timeout to apply when trying to acquire a memory lock during a factory execution.
    /// <br/><br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/CacheStampede.md"/>
    /// <br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Options.md"/>
    /// </summary>
    public TimeSpan LockTimeout { get; set; }

    /// <summary>
    /// The maximum amount of extra duration to add to the normal <see cref="Duration"/> in a randomized way, to allow for more variable expirations.
    /// <br/>
    /// This may be useful in a horizontal scalable scenario(eg: multi-node scenario).
    /// <br/><br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Options.md"/>
    /// </summary>
    public TimeSpan JitterMaxDuration { get; set; }

    /// <summary>
    /// The size of the cache entry, used as a value for the <see cref="MemoryCacheEntryOptions.Size"/> option in the underlying memory cache.
    /// <br/><br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Options.md"/>
    /// <br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/StepByStep.md"/>
    /// </summary>
    public long? Size { get; set; }

    /// <summary>
    /// The <see cref="CacheItemPriority"/> of the entry in the underlying memory cache.
    /// <br/><br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Options.md"/>
    /// </summary>
    public CacheItemPriority Priority { get; set; }

    /// <summary>
    /// Returns a stale (expired) value even in read-only operations (eg: TryGet/GetOrDefault).
    /// <br/><br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/FailSafe.md"/>
    /// <br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Options.md"/>
    /// </summary>
    public bool AllowStaleOnReadOnly { get; set; }

    /// <summary>
    /// Enable the fail-safe mechanism, which will be activated if and when something goes wrong while calling a factory or getting data from a distributed cache.
    /// <br/><br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/FailSafe.md"/>
    /// <br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Options.md"/>
    /// </summary>
    public bool IsFailSafeEnabled { get; set; }

    /// <summary>
    /// When fail-safe is enabled this is the maximum amount of time a cache entry can be used in case of problems, even if expired.
    /// <br/><br/>
    /// Specifically:
    /// <br/>
    /// - if <see cref="IsFailSafeEnabled"/> is set to <see langword="true"/>, an entry will apparently expire normally after the specified <see cref="Duration"/>: behind the scenes though it will also be kept around for this (usually long) amount of time, so it may be used as a fallback value in case of problems
    /// <br/>
    /// - if <see cref="IsFailSafeEnabled"/> is set to <see langword="false"/>, this is ignored
    /// <br/><br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/FailSafe.md"/>
    /// <br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Options.md"/>
    /// </summary>
    public TimeSpan FailSafeMaxDuration { get; set; }

    /// <summary>
    /// If fail-safe is enabled, something goes wrong while getting data (from the distributed cache or while calling the factory) and there is an expired entry to be used as a fallback value, the fail-safe mechanism will actually be activated.
    /// In that case the fallback value will not only be returned to the caller but also put in the cache for this duration (usually small) to avoid excessive load on the distributed cache and/or the factory getting called continuously.
    /// <br/><br/>
    /// <strong>TL/DR:</strong> the amount of time an expired cache entry is temporarily considered non-expired before checking the source (calling the factory) again.
    /// <br/><br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/FailSafe.md"/>
    /// <br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Options.md"/>
    /// </summary>
    public TimeSpan FailSafeThrottleDuration { get; set; }

    /// <summary>
    /// The maximum execution time allowed for the factory, applied only if fail-safe is enabled and there is a fallback value to return.
    /// <br/><br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Timeouts.md"/>
    /// <br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Options.md"/>
    /// </summary>
    public TimeSpan FactorySoftTimeout { get; set; }

    /// <summary>
    /// The maximum execution time allowed for the factory in any case, even if there is not a stale value to fall back to.
    /// <br/><br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Timeouts.md"/>
    /// <br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Options.md"/>
    /// </summary>
    public TimeSpan FactoryHardTimeout { get; set; }

    /// <summary>
    /// Enable a factory that has hit a synthetic timeout (both soft/hard) to complete in the background and update the cache with the new value.
    /// <br/><br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Timeouts.md"/>
    /// <br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Options.md"/>
    /// </summary>
    public bool AllowTimedOutFactoryBackgroundCompletion { get; set; }

    /// <summary>
    /// The duration specific for the distributed cache, if any. If not set, <see cref="Duration"/> will be used.
    /// <br/><br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/CacheLevels.md"/>
    /// <br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Options.md"/>
    /// </summary>
    public TimeSpan? DistributedCacheDuration { get; set; }

    /// <summary>
    /// When fail-safe is enabled this is the maximum amount of time a cache entry can be used in case of problems, even if expired, in the distributed cache.
    /// <br/><br/>
    /// Specifically:
    /// <br/>
    /// - if <see cref="IsFailSafeEnabled"/> is set to <see langword="true"/>, an entry will apparently expire normally after the specified Duration: behind the scenes though it will also be kept around for this (usually long) amount of time, so it may be used as a fallback value in case of problems;
    /// <br/>
    /// - if <see cref="IsFailSafeEnabled"/> is set to <see langword="false"/>, this is ignored;
    /// <br/><br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/FailSafe.md"/>
    /// <br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Options.md"/>
    /// </summary>
    public TimeSpan? DistributedCacheFailSafeMaxDuration { get; set; }

    /// <summary>
    /// The maximum execution time allowed for each operation on the distributed cache, applied only if fail-safe is enabled and there is a fallback value to return.
    /// <br/><br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Timeouts.md"/>
    /// <br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Options.md"/>
    /// </summary>
    public TimeSpan DistributedCacheSoftTimeout { get; set; }

    /// <summary>
    /// The maximum execution time allowed for each operation on the distributed cache in any case, even if there is not a stale value to fall back to.
    /// <br/><br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Timeouts.md"/>
    /// <br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Options.md"/>
    /// </summary>
    public TimeSpan DistributedCacheHardTimeout { get; set; }

    /// <summary>
    /// Even if the distributed cache is a secondary level, by default every operation on it (get/set/remove/etc) is blocking: that is to say the FusionCache method call would not return until the inner distributed cache operation is completed.
    /// <br/>
    /// This is to avoid rare edge cases like saving a value in the cache and immediately checking the underlying distributed cache directly, not finding the value (because it is still being saved): very very rare, but still.
    /// <br/>
    /// Setting this flag to <see langword="true"/> will execute most of these operations in the background, resulting in a performance boost.
    /// <br/><br/>
    /// <strong>TL/DR:</strong> set this flag to <see langword="true"/> for a perf boost, but watch out for rare side effects.
    /// <br/><br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/CacheLevels.md"/>
    /// <br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/BackgroundDistributedOperations.md"/>
    /// <br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Options.md"/>
    /// </summary>
    public bool AllowBackgroundDistributedCacheOperations { get; set; }

    /// <summary>
    ///	Set this to <see langword="true"/> to allow the bubble up of distributed cache exceptions (default is <see langword="false"/>).
    ///	Please note that, even if set to <see langword="true"/>, in some cases you would also need <see cref="AllowBackgroundDistributedCacheOperations"/> set to <see langword="false"/> and no timeout (neither soft nor hard) specified.
    /// <br/><br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/CacheLevels.md"/>
    /// <br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Options.md"/>
    /// </summary>
    public bool ReThrowDistributedCacheExceptions { get; set; }

    /// <summary>
    ///	Set this to <see langword="true"/> to allow the bubble up of serialization exceptions (default is <see langword="true"/>).
    ///	Please note that, even if set to <see langword="true"/>, in some cases you would also need <see cref="AllowBackgroundDistributedCacheOperations"/> set to <see langword="false"/> and no timeout (neither soft nor hard) specified.
    /// <br/><br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/CacheLevels.md"/>
    /// <br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Options.md"/>
    /// </summary>
    public bool ReThrowSerializationExceptions { get; set; }

    /// <summary>
    /// Skip the usage of the backplane, if any.
    /// <br/>
    /// Normally, if you have a backplane setup, any change operation (like a SET via a Set/GetOrSet call or a REMOVE via a Remove call) will send backplane notifications: this option can skip it.
    /// <br/><br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Backplane.md"/>
    /// <br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Options.md"/>
    /// </summary>
    public bool SkipBackplaneNotifications { get; set; }

    /// <summary>
    /// By default, every operation on the backplane is non-blocking: that is to say the FusionCache method call would not wait for each backplane operation to be completed.
    /// <br/>
    /// Setting this flag to <see langword="false"/> will execute these operations in a blocking fashion, typically resulting in worse performance.
    /// <br/><br/>
    /// <strong>TL/DR:</strong> if you want to wait for backplane operations to complete, set this flag to <see langword="false"/>.
    /// <br/><br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Backplane.md"/>
    /// <br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/BackgroundDistributedOperations.md"/>
    /// <br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Options.md"/>
    /// </summary>
    public bool AllowBackgroundBackplaneOperations { get; set; }

    /// <summary>
    ///	Set this to <see langword="true"/> to allow the bubble up of backplane exceptions (default is <see langword="false"/>).
    ///	Please note that, even if set to <see langword="true"/>, in some cases you would also need <see cref="AllowBackgroundBackplaneOperations"/> set to <see langword="false"/> and no timeout (neither soft nor hard) specified.
    /// <br/><br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Backplane.md"/>
    /// <br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Options.md"/>
    /// </summary>
    public bool ReThrowBackplaneExceptions { get; set; }

    /// <summary>
    /// Skip reading from the distributed cache, if any.
    /// <br/><br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/CacheLevels.md"/>
    /// <br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Options.md"/>
    /// </summary>
    public bool SkipDistributedCacheRead { get; set; }

    /// <summary>
    /// Skip writing to the distributed cache, if any.
    /// <br/><br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/CacheLevels.md"/>
    /// <br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Options.md"/>
    /// </summary>
    public bool SkipDistributedCacheWrite { get; set; }

    /// <summary>
    /// When a 2nd level (distributed cache) is used and a cache entry in the 1st level (memory cache) is found but is stale, a read is done on the distributed cache: the reason is that in a multi-node environment another node may have updated the cache entry, so we may found a newer version of it.
    /// <br/><br/>
    /// There are situations though, like in a mobile app with a SQLite 2nd level, where the 2nd level is not really "distributed" but just "out of process" (to ease cold starts): in situations like this no one can have updated the 2nd level, so we can skip that extra read for a perf boost (of course the write part will still be done).
    /// <br/><br/>
    /// <strong>TL/DR:</strong> if your 2nd level is not "distributed" but only "out of process", setting this to <see langword="true"/> can give you a nice performance boost.
    /// <br/><br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/CacheLevels.md"/>
    /// <br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Options.md"/>
    /// </summary>
    public bool SkipDistributedCacheReadWhenStale { get; set; }

    /// <summary>
    /// Skip reading from the memory cache.
    /// <br/><br/>
    /// <strong>NOTE:</strong> this option must be used very carefully and is generally not recommended, as it will not protect you from some problems like Cache Stampede. Also, it can lead to a lot of extra work for the 2nd level (distributed cache) and a lot of extra network traffic.
    /// <br/><br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/CacheLevels.md"/>
    /// <br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Options.md"/>
    /// </summary>
    public bool SkipMemoryCacheRead { get; set; }

    /// <summary>
    /// Skip writing to the memory cache.
    /// <br/><br/>
    /// <strong>NOTE:</strong> this option must be used very carefully and is generally not recommended, as it will not protect you from some problems like Cache Stampede. Also, it can lead to a lot of extra work for the 2nd level (distributed cache) and a lot of extra network traffic.
    /// <br/><br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/CacheLevels.md"/>
    /// <br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Options.md"/>
    /// </summary>
    public bool SkipMemoryCacheWrite { get; set; }

    /// <summary>
    /// Enable automatic cloning of the value being returned from the cache, by using the provided <see cref="IFusionCacheSerializer"/>.
    /// <br></br>
    /// <strong>NOTE:</strong> if no <see cref="IFusionCacheSerializer"/> has been setup or if the value being returned cannot be (de)serialized, an exception will be thrown.
    /// <br/><br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/AutoClone.md"/>
    /// <br/>
    /// <strong>DOCS:</strong> <see href="https://github.com/ZiggyCreatures/FusionCache/blob/main/docs/Options.md"/>
    /// </summary>
    public bool EnableAutoClone { get; set; }
}



public class FusionCacheEntryOptionsWrapper : IFusionCacheEntryOptions
{
    private readonly FusionCacheEntryOptions __options = new();


    public TimeSpan          Duration                                  { get => __options.Duration;                                  set => __options.Duration = value; }
    public float?            EagerRefreshThreshold                     { get => __options.EagerRefreshThreshold;                     set => __options.EagerRefreshThreshold = value; }
    public TimeSpan          LockTimeout                               { get => __options.LockTimeout;                               set => __options.LockTimeout = value; }
    public TimeSpan          JitterMaxDuration                         { get => __options.JitterMaxDuration;                         set => __options.JitterMaxDuration = value; }
    public long?             Size                                      { get => __options.Size;                                      set => __options.Size = value; }
    public CacheItemPriority Priority                                  { get => __options.Priority;                                  set => __options.Priority = value; }
    public bool              AllowStaleOnReadOnly                      { get => __options.AllowStaleOnReadOnly;                      set => __options.AllowStaleOnReadOnly = value; }
    public bool              IsFailSafeEnabled                         { get => __options.IsFailSafeEnabled;                         set => __options.IsFailSafeEnabled = value; }
    public TimeSpan          FailSafeMaxDuration                       { get => __options.FailSafeMaxDuration;                       set => __options.FailSafeMaxDuration = value; }
    public TimeSpan          FailSafeThrottleDuration                  { get => __options.FailSafeThrottleDuration;                  set => __options.FailSafeThrottleDuration = value; }
    public TimeSpan          FactorySoftTimeout                        { get => __options.FactorySoftTimeout;                        set => __options.FactorySoftTimeout = value; }
    public TimeSpan          FactoryHardTimeout                        { get => __options.FactoryHardTimeout;                        set => __options.FactoryHardTimeout = value; }
    public bool              AllowTimedOutFactoryBackgroundCompletion  { get => __options.AllowTimedOutFactoryBackgroundCompletion;  set => __options.AllowTimedOutFactoryBackgroundCompletion = value; }
    public TimeSpan?         DistributedCacheDuration                  { get => __options.DistributedCacheDuration;                  set => __options.DistributedCacheDuration = value; }
    public TimeSpan?         DistributedCacheFailSafeMaxDuration       { get => __options.DistributedCacheFailSafeMaxDuration;       set => __options.DistributedCacheFailSafeMaxDuration = value; }
    public TimeSpan          DistributedCacheSoftTimeout               { get => __options.DistributedCacheSoftTimeout;               set => __options.DistributedCacheSoftTimeout = value; }
    public TimeSpan          DistributedCacheHardTimeout               { get => __options.DistributedCacheHardTimeout;               set => __options.DistributedCacheHardTimeout = value; }
    public bool              AllowBackgroundDistributedCacheOperations { get => __options.AllowBackgroundDistributedCacheOperations; set => __options.AllowBackgroundDistributedCacheOperations = value; }
    public bool              ReThrowDistributedCacheExceptions         { get => __options.ReThrowDistributedCacheExceptions;         set => __options.ReThrowDistributedCacheExceptions = value; }
    public bool              ReThrowSerializationExceptions            { get => __options.ReThrowSerializationExceptions;            set => __options.ReThrowSerializationExceptions = value; }
    public bool              SkipBackplaneNotifications                { get => __options.SkipBackplaneNotifications;                set => __options.SkipBackplaneNotifications = value; }
    public bool              AllowBackgroundBackplaneOperations        { get => __options.AllowBackgroundBackplaneOperations;        set => __options.AllowBackgroundBackplaneOperations = value; }
    public bool              ReThrowBackplaneExceptions                { get => __options.ReThrowBackplaneExceptions;                set => __options.ReThrowBackplaneExceptions = value; }
    public bool              SkipDistributedCacheRead                  { get => __options.SkipDistributedCacheRead;                  set => __options.SkipDistributedCacheRead = value; }
    public bool              SkipDistributedCacheWrite                 { get => __options.SkipDistributedCacheWrite;                 set => __options.SkipDistributedCacheWrite = value; }
    public bool              SkipDistributedCacheReadWhenStale         { get => __options.SkipDistributedCacheReadWhenStale;         set => __options.SkipDistributedCacheReadWhenStale = value; }
    public bool              SkipMemoryCacheRead                       { get => __options.SkipMemoryCacheRead;                       set => __options.SkipMemoryCacheRead = value; }
    public bool              SkipMemoryCacheWrite                      { get => __options.SkipMemoryCacheWrite;                      set => __options.SkipMemoryCacheWrite = value; }
    public bool              EnableAutoClone                           { get => __options.EnableAutoClone;                           set => __options.EnableAutoClone = value; }


    public static implicit operator FusionCacheEntryOptions( FusionCacheEntryOptionsWrapper wrapper ) => wrapper.__options;
}
