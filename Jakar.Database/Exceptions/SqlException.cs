// Jakar.Extensions :: Jakar.Database
// 09/29/2022  8:03 PM

namespace Jakar.Database;


public sealed class SqlException : Exception
{
    [JsonProperty] public bool?   MatchAll   { get; init; }
    [JsonProperty] public object? Parameters { get; init; }
    [JsonProperty] public string  SQL        { get; init; }

    // [JsonProperty] public string Value => base.ToString();


    public SqlException( string sql, string  message ) : this( sql, default, message ) { }
    public SqlException( string sql, object? parameters, string message ) : this( sql, parameters, default, message ) { }
    public SqlException( string sql, object? parameters, bool? matchAll, string? message = default ) : base( message )
    {
        SQL        = sql;
        Parameters = parameters;
        MatchAll   = matchAll;
    }
    public SqlException( string sql, string  message,    Exception inner ) : this( sql, default, message, inner ) { }
    public SqlException( string sql, object? parameters, Exception inner ) : this( sql, parameters, inner.Message, inner ) { }
    public SqlException( string sql, object? parameters, bool?     matchAll, Exception inner ) : this( sql, parameters, matchAll, inner.Message, inner ) { }
    public SqlException( string sql, object? parameters, string    message,  Exception inner ) : this( sql, parameters, default, message, inner ) { }
    public SqlException( string sql, object? parameters, bool? matchAll, string message, Exception inner ) : base( message, inner )
    {
        SQL        = sql;
        Parameters = parameters;
        MatchAll   = matchAll;
    }
}
