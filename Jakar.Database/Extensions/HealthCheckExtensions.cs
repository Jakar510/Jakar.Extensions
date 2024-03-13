// Jakar.Extensions :: Jakar.Database
// 05/22/2023  11:31 AM

namespace Jakar.Database;


[AttributeUsage( AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct )]
public sealed class HealthCheckTagAttribute( string name ) : Attribute
{
    public string Name { get; init; } = name;
}



public static class HealthChecks
{
    public static IEnumerable<string> GetHealthCheckTags<T>()
    {
        foreach ( HealthCheckTagAttribute attribute in typeof(T).GetCustomAttributes<HealthCheckTagAttribute>() ) { yield return attribute.Name; }

        foreach ( PropertyInfo property in typeof(T).GetProperties( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic ) )
        {
            foreach ( HealthCheckTagAttribute attribute in property.GetCustomAttributes<HealthCheckTagAttribute>() ) { yield return attribute.Name; }
        }
    }


    public static HealthCheckRegistration Create<T>()
        where T : IHealthCheck => Create<T>( HealthStatus.Unhealthy );
    public static HealthCheckRegistration Create<T>( HealthStatus? failureStatus )
        where T : IHealthCheck => Create<T>( failureStatus, GetHealthCheckTags<T>() );
    public static HealthCheckRegistration Create<T>( HealthStatus? failureStatus, IEnumerable<string> tags )
        where T : IHealthCheck => new(typeof(T).Name, provider => provider.GetRequiredService<T>(), failureStatus, tags);
    public static HealthCheckRegistration Create<T>( params string[] tags )
        where T : IHealthCheck => Create<T>( HealthStatus.Unhealthy, tags );
    public static HealthCheckRegistration Create<T>( HealthStatus? failureStatus, params string[] tags )
        where T : IHealthCheck => new(typeof(T).Name, provider => provider.GetRequiredService<T>(), failureStatus, tags);


    public static IHealthChecksBuilder AddHealthChecks( this WebApplicationBuilder builder )                                       => builder.Services.AddHealthChecks();
    public static IHealthChecksBuilder AddHealthChecks( this WebApplicationBuilder builder, HealthCheckRegistration registration ) => builder.AddHealthChecks().Add( registration );
    public static IHealthChecksBuilder AddHealthChecks( this WebApplicationBuilder builder, params HealthCheckRegistration[] registrations )
    {
        IHealthChecksBuilder checks = builder.AddHealthChecks();
        foreach ( HealthCheckRegistration registration in registrations ) { checks.Add( registration ); }

        return checks;
    }
}
