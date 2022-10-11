// Jakar.Extensions :: Jakar.Database
// 09/29/2022  8:03 PM

namespace Jakar.Database;


public class SqlException<TParam> : Exception
{
    public bool?  MatchAll   { get; init; }
    public string SQL        { get; init; }
    public TParam Parameters { get; init; }


    public SqlException( string sql, TParam parameters, string message ) : base( message )
    {
        SQL        = sql;
        Parameters = parameters;
    }
    public SqlException( string sql, TParam parameters, string message, Exception inner ) : base( message, inner )
    {
        SQL        = sql;
        Parameters = parameters;
    }
}



public class SqlException : SqlException<object>
{
    public SqlException( string sql, object parameters, string message ) : base( sql, parameters, message ) { }
    public SqlException( string sql, object parameters, string message, Exception inner ) : base( sql, parameters, message, inner ) { }
}
