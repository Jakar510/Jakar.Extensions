// Jakar.Extensions :: Jakar.Database
// 05/22/2023  11:31 AM

namespace Jakar.Database;


[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class HealthCheckTagAttribute( string name ) : Attribute
{
    public string Name { get; init; } = name;
}



public static class HealthChecks
{
    public static IEnumerable<string> GetHealthCheckTags<TValue>()
    {
        foreach ( HealthCheckTagAttribute attribute in typeof(TValue).GetCustomAttributes<HealthCheckTagAttribute>() ) { yield return attribute.Name; }

        foreach ( PropertyInfo property in typeof(TValue).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) )
        {
            foreach ( HealthCheckTagAttribute attribute in property.GetCustomAttributes<HealthCheckTagAttribute>() ) { yield return attribute.Name; }
        }
    }


    public static HealthCheckRegistration Create<TValue>()
        where TValue : IHealthCheck =>
        Create<TValue>(HealthStatus.Unhealthy);
    public static HealthCheckRegistration Create<TValue>( HealthStatus? failureStatus )
        where TValue : IHealthCheck =>
        Create<TValue>(failureStatus, GetHealthCheckTags<TValue>());
    public static HealthCheckRegistration Create<TValue>( HealthStatus? failureStatus, IEnumerable<string> tags )
        where TValue : IHealthCheck
    {
        return new HealthCheckRegistration(typeof(TValue).Name, provider => provider.GetRequiredService<TValue>(), failureStatus, tags);
    }
    public static HealthCheckRegistration Create<TValue>( params string[] tags )
        where TValue : IHealthCheck =>
        Create<TValue>(HealthStatus.Unhealthy, tags);
    public static HealthCheckRegistration Create<TValue>( HealthStatus? failureStatus, params string[] tags )
        where TValue : IHealthCheck
    {
        return new HealthCheckRegistration(typeof(TValue).Name, provider => provider.GetRequiredService<TValue>(), failureStatus, tags);
    }


    public static IHealthChecksBuilder AddHealthChecks( this WebApplicationBuilder builder ) => builder.Services.AddHealthChecks();
    public static IHealthChecksBuilder AddHealthChecks( this WebApplicationBuilder builder, HealthCheckRegistration registration ) => builder.AddHealthChecks()
                                                                                                                                             .Add(registration);
    public static IHealthChecksBuilder AddHealthChecks( this WebApplicationBuilder builder, params HealthCheckRegistration[] registrations )
    {
        IHealthChecksBuilder checks = builder.AddHealthChecks();
        foreach ( HealthCheckRegistration registration in registrations ) { checks.Add(registration); }

        return checks;
    }
}
