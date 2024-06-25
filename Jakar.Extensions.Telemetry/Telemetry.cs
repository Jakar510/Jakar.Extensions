using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;



namespace Jakar.Extensions.Telemetry;


// OpenTelemetry Logger
#nullable disable



[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public class Telemetry( ActivitySource source, ILoggerFactory loggerFactory )
{
    public Activity       Activity      { get; } = source.StartActivity( source.Name );
    public ILoggerFactory LoggerFactory { get; } = loggerFactory;
    public ActivitySource Source        { get; } = source;


    public static Telemetry Get( IServiceProvider provider ) => provider.GetRequiredService<Telemetry>();
}



public class Telemetry<TApp>( ActivitySource source, ILoggerFactory factory, ActivityKind kind = ActivityKind.Internal ) : Telemetry( source, factory )
    where TApp : IAppName
{
    public Telemetry( ILoggerFactory factory ) : this( ActivitySource.Create<TApp>(), factory ) { }


    public static Telemetry<TApp> Create( IServiceProvider provider ) => Create( provider.GetRequiredService<ILoggerFactory>() );
    public static Telemetry<TApp> Create( ILoggerFactory   factory )  => new(factory);
}
