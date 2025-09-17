// Jakar :: Jakar.Database
// 04/22/2022  11:10 AM

namespace Jakar.Database;


public interface IEmailTokenService
{
    public ValueTask<string>                CreateContent( string      header, UserRecord user,  ClaimType         types, CancellationToken token = default );
    public ValueTask<string>                CreateHTMLContent( string  header, UserRecord user,  ClaimType         types, CancellationToken token = default );
    public ValueTask<ErrorOrResult<Tokens>> Authenticate( LoginRequest users,  ClaimType  types, CancellationToken token = default );
}
