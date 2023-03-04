// Jakar.Extensions :: Jakar.Database
// 10/16/2022  5:46 PM

namespace Jakar.Database;


public sealed class DbOptions : IOptions<DbOptions>
{
    public DbInstance           DbType               { get; set; } = DbInstance.MsSql;
    public AppVersion           Version              { get; set; } = new();
    public PasswordRequirements PasswordRequirements { get; set; } = new();
    public string               UserExists           { get; set; } = "User Exists";
    public string               TokenIssuer          { get; set; } = string.Empty;
    public string               TokenAudience        { get; set; } = string.Empty;


    DbOptions IOptions<DbOptions>.Value => this;


    public DbOptions() { }
}
