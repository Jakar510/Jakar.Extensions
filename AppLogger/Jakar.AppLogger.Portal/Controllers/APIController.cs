namespace Jakar.AppLogger.Portal.Controllers;


[ApiController]
[RouteAttribute( "[Controller]/[Action]" )]
public class APIController : ControllerBase
{
    private readonly LoggerDB _api;


    public APIController( LoggerDB api ) => _api = api;


    [HttpPost] public async ValueTask<ActionResult<Tokens>> Register( VerifyRequest<UserData> request,      CancellationToken token ) => await _api.Register( request, string.Empty, ClaimType.UserID | ClaimType.UserName, token ); // TODO: rights
    [HttpPost] public async ValueTask<ActionResult<Tokens>> Verify( VerifyRequest             request,      CancellationToken token ) => await _api.Verify( request, ClaimType.UserID | ClaimType.UserName, token );
    [HttpPost] public async ValueTask<ActionResult<Tokens>> Refresh( string                   refreshToken, CancellationToken token ) => await _api.Refresh( refreshToken, ClaimType.UserID | ClaimType.UserName, token );


    [HttpPost] public async ValueTask<ActionResult<Guid>> StartSession( StartSession session,   CancellationToken token ) => await _api.StartSession( this, session, token );
    [HttpPost] public async ValueTask<ActionResult> EndSession( Guid                 sessionID, CancellationToken token ) => await _api.EndSession( this, sessionID, token );
    [HttpPost] public async ValueTask<ActionResult<bool>> Log( IEnumerable<Log>      logs,      CancellationToken token ) => await _api.SendLog( this, logs, token );
}
