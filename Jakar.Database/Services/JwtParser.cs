namespace Jakar.Database;


public static class JwtParser
{
    private static byte[] ParseBase64WithoutPadding( string base64 )
    {
        switch ( base64.Length % 4 )
        {
            case 2:
                base64 += "==";
                break;

            case 3:
                base64 += "=";
                break;
        }

        return Convert.FromBase64String( base64 );
    }
    public static List<Claim> ParseClaimsFromJwt( string jwt )
    {
        List<Claim> claims        = [];
        string      payload       = jwt.Split( '.' )[1];
        string      jsonBytes     = ParseBase64WithoutPadding( payload ).ConvertToString( Encoding.Default );
        var         keyValuePairs = jsonBytes.FromJson<Dictionary<string, object>>();

        ExtractRolesFromJwt( claims, keyValuePairs );
        claims.AddRange( keyValuePairs.Select( static kvp => new Claim( kvp.Key, kvp.Value.ToString() ?? string.Empty ) ) );

        return claims;
    }
    private static void ExtractRolesFromJwt( List<Claim> claims, Dictionary<string, object> keyValuePairs )
    {
        if ( keyValuePairs.TryGetValue( ClaimTypes.Role, out object? roles ) is false ) { return; }

        ReadOnlySpan<string> parsedRoles = roles.ToString()?.Trim().TrimStart( '[' ).TrimEnd( ']' ).Split( ',' );

        if ( parsedRoles.IsEmpty is false )
        {
            if ( parsedRoles.Length > 1 )
            {
                foreach ( string parsedRole in parsedRoles ) { claims.Add( new Claim( ClaimTypes.Role, parsedRole.Trim( '"' ) ) ); }
            }
            else { claims.Add( new Claim( ClaimTypes.Role, parsedRoles[0] ) ); }
        }

        keyValuePairs.Remove( ClaimTypes.Role );
    }
}
