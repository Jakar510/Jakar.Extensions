// Jakar.Extensions :: Jakar.Database
// 10/16/2022  5:46 PM

using Microsoft.AspNetCore.Authentication.JwtBearer;



namespace Jakar.Database;


public sealed class DbOptions : IOptions<DbOptions>
{
    public DbInstance           DbType               { get; set; } = DbInstance.MsSql;
    public AppVersion           Version              { get; set; } = new();
    public PasswordRequirements PasswordRequirements { get; set; } = new();
    public string               UserExists           { get; set; } = "User Exists";
    public string               TokenIssuer          { get; set; } = string.Empty;
    public string               TokenAudience        { get; set; } = string.Empty;
    public string               JWTKey               { get; set; } = "JWT";
    public string               JWTAlgorithm         { get; set; } = SecurityAlgorithms.HmacSha512Signature;
    public string               AuthenticationType   { get; set; } = JwtBearerDefaults.AuthenticationScheme;
    public TimeSpan             ClockSkew            { get; set; } = TimeSpan.FromSeconds( 60 );


    DbOptions IOptions<DbOptions>.Value => this;
}
