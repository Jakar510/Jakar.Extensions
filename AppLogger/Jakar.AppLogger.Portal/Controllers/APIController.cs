namespace Jakar.AppLogger.Portal.Controllers;


[ApiController]
[RouteAttribute( "[Controller]/[Action]" )]
public class APIController : ControllerBase
{
    private readonly LoggerDB _api;


    public APIController( LoggerDB api ) => _api = api;


    [HttpPost] public async Task<ActionResult<Guid>> StartSession( StartSession session,   CancellationToken token ) => await _api.StartSession( this, session, token );
    [HttpPost] public async Task<ActionResult> EndSession( Guid                 sessionID, CancellationToken token ) => await _api.EndSession( this, sessionID, token );
    [HttpPost] public async Task<ActionResult> Log( Log                         log,       CancellationToken token ) => await _api.Log( this, log, token );
}
