// Jakar.Extensions :: Jakar.Database
// 09/29/2022  8:03 PM

namespace Jakar.Database;


public sealed class SqlException : Exception
{
    public bool?   MatchAll   { get; init; }
    public string  SQL        { get; init; }
    public object? Parameters { get; init; }


    public SqlException( string sql, string  message ) : this( sql, default, message ) { }
    public SqlException( string sql, string  message,    Exception inner ) : this( sql, default, message, inner ) { }
    public SqlException( string sql, object? parameters, Exception inner ) : this( sql, parameters, inner.Message, inner ) { }
    public SqlException( string sql, object? parameters, string message ) : base( message )
    {
        SQL        = sql;
        Parameters = parameters;
    }
    public SqlException( string sql, object? parameters, string message, Exception inner ) : base( message, inner )
    {
        SQL        = sql;
        Parameters = parameters;
    }
}
