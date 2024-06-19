// Jakar.Extensions :: Jakar.Database
// 06/02/2024  14:06

using Microsoft.AspNetCore.OutputCaching;



namespace Jakar.Database;


public static class MinApis
{
    public static void ExpireOneMinute( this   OutputCachePolicyBuilder policy )                            => policy.Expire( TimeSpan.FromMinutes( 1 ) );
    public static void ExpireOneMinute( this   OutputCachePolicyBuilder policy, params string[] queryKeys ) => policy.Expire( TimeSpan.FromMinutes( 1 ) ).SetVaryByQuery( queryKeys );
    public static void ExpireFiveMinutes( this OutputCachePolicyBuilder policy )                            => policy.Expire( TimeSpan.FromMinutes( 5 ) );
    public static void ExpireFiveMinutes( this OutputCachePolicyBuilder policy, params string[] queryKeys ) => policy.Expire( TimeSpan.FromMinutes( 5 ) ).SetVaryByQuery( queryKeys );
}
