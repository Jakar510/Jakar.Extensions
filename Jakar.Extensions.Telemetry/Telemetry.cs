using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;



namespace Jakar.Extensions.Telemetry;


// OpenTelemetry Logger
#nullable disable



[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public class Telemetry( ActivityCollection collection, ILoggerFactory loggerFactory )
{
    public Activity           Activity      { get; } = collection.StartActivity( collection.AppName );
    public ILoggerFactory     LoggerFactory { get; } = loggerFactory;
    public ActivityCollection Source        { get; } = collection;


    public static Telemetry Get( IServiceProvider provider ) => provider.GetRequiredService<Telemetry>();
}



public class Telemetry<TApp>( ActivityCollection collection, ILoggerFactory factory ) : Telemetry( collection, factory )
    where TApp : IAppName
{
    public Telemetry( ILoggerFactory factory ) : this( ActivityCollection.Create<TApp>(), factory ) { }


    public static Telemetry<TApp> Create( IServiceProvider provider ) => Create( provider.GetRequiredService<ILoggerFactory>() );
    public static Telemetry<TApp> Create( ILoggerFactory   factory )  => new(factory);
}