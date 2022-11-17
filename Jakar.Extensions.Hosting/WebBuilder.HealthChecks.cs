namespace Jakar.Extensions.Hosting;


public static partial class WebBuilder
{
    public static HealthCheckRegistration CreateHealthCheck<T>() where T : IHealthCheck => CreateHealthCheck<T>( HealthStatus.Unhealthy );
    public static HealthCheckRegistration CreateHealthCheck<T>( HealthStatus status ) where T : IHealthCheck =>
        CreateHealthCheck<T>( status,
                              typeof(T).GetCustomAttributes<HealthCheckTagAttribute>()
                                       .Select( x => x.Name ) );
    public static HealthCheckRegistration CreateHealthCheck<T>( HealthStatus status, IEnumerable<string> tags ) where T : IHealthCheck =>
        new(typeof(T).Name, provider => provider.GetRequiredService<T>(), status, tags);
    public static HealthCheckRegistration CreateHealthCheck<T>( params string[] tags ) where T : IHealthCheck => CreateHealthCheck<T>( HealthStatus.Unhealthy, tags );
    public static HealthCheckRegistration CreateHealthCheck<T>( HealthStatus status, params string[] tags ) where T : IHealthCheck =>
        new(typeof(T).Name, provider => provider.GetRequiredService<T>(), status, tags);
    public static IHealthChecksBuilder AddHealthChecks( this WebApplicationBuilder builder ) => builder.Services.AddHealthChecks();
    public static IHealthChecksBuilder AddHealthChecks( this WebApplicationBuilder builder, HealthCheckRegistration registration ) => builder.AddHealthChecks()
                                                                                                                                             .Add( registration );
    public static WebApplicationBuilder AddHealthChecks( this WebApplicationBuilder builder, params HealthCheckRegistration[] registrations )
    {
        IHealthChecksBuilder checks = builder.Services.AddHealthChecks();
        foreach ( HealthCheckRegistration registration in registrations ) { checks.Add( registration ); }

        return builder;
    }
}
