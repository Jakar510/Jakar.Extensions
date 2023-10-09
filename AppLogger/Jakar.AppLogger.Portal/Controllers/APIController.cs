namespace Jakar.AppLogger.Portal.Controllers;


[ ApiController, RouteAttribute( "[Controller]/[Action]" ) ]
public class APIController : ControllerBase
{
    private readonly LoggerDB _api;


    public APIController( LoggerDB api ) => _api = api;


    [ HttpPost, Route( Routes.REGISTER ) ]
    public async Task<ActionResult<Tokens>> Register( VerifyRequest<UserData> request, CancellationToken token )
    {
        OneOf<Tokens, Error> result = await _api.Register( request, string.Empty, default, token );
        return result.Match();
    }


    [ HttpPost, Route( Routes.VERIFY ) ]
    public async Task<ActionResult<Tokens>> Verify( VerifyRequest request, CancellationToken token )
    {
        OneOf<Tokens, Error> result = await _api.Verify( request, default, token );
        return result.Match();
    }


    [ HttpPost, Route( Routes.REFRESH ) ]
    public async Task<ActionResult<Tokens>> Refresh( string refreshToken, CancellationToken token )
    {
        OneOf<Tokens, Error> result = await _api.Refresh( refreshToken, default, token );
        return result.Match();
    }


    [ HttpPost, Route( Routes.LOG ) ]
    public async Task<ActionResult<bool>> Log( IEnumerable<AppLog> logs, CancellationToken token )
    {
        OneOf<bool, Error> result = await _api.SendLog( logs, token );
        return result.Match();
    }


    [ HttpPost, Route( Routes.Sessions.START ) ]
    public async Task<ActionResult<StartSessionReply>> StartSession( StartSession session, CancellationToken token )
    {
        OneOf<StartSessionReply, Error> result = await _api.StartSession( session, token );
        return result.Match();
    }


    [ HttpPost, Route( Routes.Sessions.END ) ]
    public async Task<ActionResult<bool>> EndSession( Guid sessionID, CancellationToken token )
    {
        OneOf<bool, Error> result = await _api.EndSession( sessionID, token );
        return result.Match();
    }
}
