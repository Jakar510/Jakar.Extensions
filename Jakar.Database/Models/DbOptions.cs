// Jakar.Extensions :: Jakar.Database
// 10/16/2022  5:46 PM

using Microsoft.AspNetCore.Authentication.JwtBearer;



namespace Jakar.Database;


public sealed class DbOptions : IOptions<DbOptions>
{
    public string               AuthenticationType   { get; set; } = JwtBearerDefaults.AuthenticationScheme;
    public TimeSpan             ClockSkew            { get; set; } = TimeSpan.FromSeconds( 60 );
    public DbInstance           DbType               { get; set; } = DbInstance.MsSql;
    public Uri                  Domain               { get; set; } = new("https://localhost");
    public string               JWTAlgorithm         { get; set; } = SecurityAlgorithms.HmacSha512Signature;
    public string               JWTKey               { get; set; } = "JWT";
    public PasswordRequirements PasswordRequirements { get; set; } = new();
    public string               TokenAudience        { get; set; } = string.Empty;
    public string               TokenIssuer          { get; set; } = string.Empty;
    public string               UserExists           { get; set; } = "User Exists";


    DbOptions IOptions<DbOptions>.Value   => this;
    public AppVersion             Version { get; set; } = new();
}
