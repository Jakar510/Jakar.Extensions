namespace Jakar.AppLogger.Portal;


public class ScriptManagerCircuitHandler : CircuitHandler
{
    private readonly IServiceProvider _serviceProvider;

    public ScriptManagerCircuitHandler( IServiceProvider serviceProvider ) => _serviceProvider = serviceProvider;

    public override Task OnCircuitOpenedAsync( Circuit circuit, CancellationToken cancellationToken )
    {
        ScriptManagerHost.AddScoped( _serviceProvider );
        return base.OnCircuitOpenedAsync( circuit, cancellationToken );
    }

    public override Task OnCircuitClosedAsync( Circuit circuit, CancellationToken cancellationToken )
    {
        ScriptManagerHost.RemoveScoped( _serviceProvider );
        return base.OnCircuitClosedAsync( circuit, cancellationToken );
    }
}
