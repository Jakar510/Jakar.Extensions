// Jakar.Extensions :: Jakar.Database
// 06/02/2024  14:06

using Microsoft.AspNetCore.OutputCaching;



namespace Jakar.Database;


public static class MinApis
{
    public static TBuilder CacheOutputOneMinutes<TBuilder>( this TBuilder builder )
        where TBuilder : IEndpointConventionBuilder => builder.CacheOutput( ExpireOneMinutes );
    public static TBuilder CacheOutputOneMinutes<TBuilder>( this TBuilder builder, params string[] queryKeys )
        where TBuilder : IEndpointConventionBuilder => builder.CacheOutput( x => x.ExpireOneMinutes( queryKeys ) );
    public static void ExpireOneMinutes( this OutputCachePolicyBuilder policy )                            => policy.Expire( TimeSpan.FromMinutes( 1 ) );
    public static void ExpireOneMinutes( this OutputCachePolicyBuilder policy, params string[] queryKeys ) => policy.Expire( TimeSpan.FromMinutes( 1 ) ).SetVaryByQuery( queryKeys );


    public static TBuilder CacheOutputFiveMinutes<TBuilder>( this TBuilder builder )
        where TBuilder : IEndpointConventionBuilder => builder.CacheOutput( ExpireFiveMinutes );
    public static TBuilder CacheOutputFiveMinutes<TBuilder>( this TBuilder builder, params string[] queryKeys )
        where TBuilder : IEndpointConventionBuilder => builder.CacheOutput( x => x.ExpireFiveMinutes( queryKeys ) );
    public static void ExpireFiveMinutes( this OutputCachePolicyBuilder policy )                            => policy.Expire( TimeSpan.FromMinutes( 5 ) );
    public static void ExpireFiveMinutes( this OutputCachePolicyBuilder policy, params string[] queryKeys ) => policy.Expire( TimeSpan.FromMinutes( 5 ) ).SetVaryByQuery( queryKeys );
}
