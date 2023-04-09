using OneOf;



namespace Jakar.AppLogger.Portal.Controllers;


[ApiController]
[RouteAttribute( "[Controller]/[Action]" )]
public class APIController : ControllerBase
{
    private readonly LoggerDB _api;


    public APIController( LoggerDB api ) => _api = api;


    [HttpPost]
    public async ValueTask<ActionResult<Tokens>> Register( VerifyRequest<UserData> request, CancellationToken token )
    {
        OneOf<Tokens, Error> result = await _api.Register( request, string.Empty, default, token );
        return result.Match();
    }
    [HttpPost]
    public async ValueTask<ActionResult<Tokens>> Verify( VerifyRequest request, CancellationToken token )
    {
        OneOf<Tokens, Error> result = await _api.Verify( request, default, token );
        return result.Match();
    }
    [HttpPost]
    public async ValueTask<ActionResult<Tokens>> Refresh( string refreshToken, CancellationToken token )
    {
        OneOf<Tokens, Error> result = await _api.Refresh( refreshToken, default, token );
        return result.Match();
    }


    [HttpPost] public async ValueTask<ActionResult<Guid>> StartSession( StartSession session,   CancellationToken token ) => await _api.StartSession( this, session, token );
    [HttpPost] public async ValueTask<ActionResult> EndSession( Guid                 sessionID, CancellationToken token ) => await _api.EndSession( this, sessionID, token );
    [HttpPost] public async ValueTask<ActionResult<bool>> Log( IEnumerable<AppLog>      logs,      CancellationToken token ) => await _api.SendLog( this, logs, token );
}
