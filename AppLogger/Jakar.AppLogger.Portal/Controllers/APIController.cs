namespace Jakar.AppLogger.Portal.Controllers;


[ApiController]
[RouteAttribute( "[Controller]/[Action]" )]
public class APIController : ControllerBase
{
    private readonly LoggerDB _api;


    public APIController( LoggerDB api ) => _api = api;


    [HttpPost] public async ValueTask<ActionResult<Tokens>> Register( VerifyRequest<UserData> request,      CancellationToken token ) => await _api.Register( this, request, token );
    [HttpPost] public async ValueTask<ActionResult<Tokens>> Verify( VerifyRequest             request,      CancellationToken token ) => await _api.Verify( this, request, token );
    [HttpPost] public async ValueTask<ActionResult<Tokens>> Refresh( string                   refreshToken, CancellationToken token ) => await _api.Refresh( this, refreshToken, token );


    [HttpPost] public async ValueTask<ActionResult<Guid>> StartSession( StartSession session,   CancellationToken token ) => await _api.StartSession( this, session, token );
    [HttpPost] public async ValueTask<ActionResult> EndSession( Guid                 sessionID, CancellationToken token ) => await _api.EndSession( this, sessionID, token );
    [HttpPost] public async ValueTask<ActionResult> Log( Log                         log,       CancellationToken token ) => await _api.Log( this, log, token );
}
