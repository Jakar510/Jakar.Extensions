// Jakar.Extensions :: Jakar.Database
// 09/29/2022  8:03 PM

namespace Jakar.Database;


public sealed class SqlException : Exception
{
    [JsonProperty] public DynamicParameters? Parameters { get; init; }
    [JsonProperty] public string             SQL        { get; init; }

    // [JsonProperty] public string Value => base.ToString();


    public SqlException( string sql, string message ) : this(sql, null, message) { }
    public SqlException( string sql, DynamicParameters? parameters, string? message = null ) : base(message ?? GetMessage(sql, parameters))
    {
        SQL        = sql;
        Parameters = parameters;
    }
    public SqlException( string        sql, Exception? inner ) : this(sql, null, GetMessage(sql), inner) => SQL = sql;
    public SqlException( string        sql, string     message, Exception? inner ) : this(sql, null, message, inner) { }
    public SqlException( in SqlCommand sql ) : this(sql.sql, sql.parameters, GetMessage(sql.sql,                                      sql.parameters)) { }
    public SqlException( in SqlCommand sql, Exception?         inner ) : this(sql.sql, sql.parameters, GetMessage(sql.sql,            sql.parameters), inner) { }
    public SqlException( string        sql, DynamicParameters? parameters, Exception? inner ) : this(sql, parameters, GetMessage(sql, parameters), inner) { }
    public SqlException( string sql, DynamicParameters? parameters, string message, Exception? inner ) : base(message, inner)
    {
        SQL        = sql;
        Parameters = parameters;
    }


    private static string GetMessage( string sql, DynamicParameters? dynamicParameters = null )
    {
        string parameters = dynamicParameters is null
                                ? "NULL"
                                : string.Join(", ", dynamicParameters.ParameterNames);

        return $"""
                An error occurred with the following sql statement

                {nameof(SQL)}:    {sql}

                {nameof(Parameters)}:   {parameters}
                """;
    }


    /*
    private const string InnerExceptionPrefix     = " ---> ";
    private const string NewLineConst             = "\n";
    private const string EndOfInnerExceptionStack = "--- End Of Inner Exception Stack ---";
    public override string ToString()
    {
        base.ToString();
        Exception? _innerException = InnerException;

        string className = GetType()
           .ToString();

        string? message              = Message;
        string  innerExceptionString = _innerException?.ToString() ?? string.Empty;
        string? stackTrace           = StackTrace;

        // Calculate result string length
        int length = className.Length;


        checked
        {
            if ( Parameters is not null ) { length += Parameters.ParameterNames.Sum( x => x.Length ); }

            if ( !string.IsNullOrEmpty( message ) ) { length += 2 + message.Length; }

            if ( _innerException != null ) { length += NewLineConst.Length + InnerExceptionPrefix.Length + innerExceptionString.Length + NewLineConst.Length + 3 + EndOfInnerExceptionStack.Length; }

            if ( stackTrace != null ) { length += NewLineConst.Length + stackTrace.Length; }
        }

        // Create the string
        Span<char> resultSpan = stackalloc char[length];

        // Fill it in
        int index = 0;
        Write( className, resultSpan, ref index );

        if ( !string.IsNullOrEmpty( message ) )
        {
            Write( ": ",    resultSpan, ref index );
            Write( message, resultSpan, ref index );
        }

        if ( _innerException != null )
        {
            Write( NewLineConst,                resultSpan, ref index );
            Write( InnerExceptionPrefix,        resultSpan, ref index );
            Write( innerExceptionString,        resultSpan, ref index );
            Write( NewLineConst,                resultSpan, ref index );
            Write( "   ",                       resultSpan, ref index );
            Write( endOfInnerExceptionResource, resultSpan, ref index );
        }

        if ( stackTrace != null )
        {
            Write( NewLineConst, resultSpan, ref index );
            Write( stackTrace,   resultSpan, ref index );
        }


        Debug.Assert( resultSpan.Length == length );
        return resultSpan.ToString();


        static void Write( ReadOnlySpan<char> source, Span<char> dest, ref int index )
        {
            source.CopyTo( dest[index..] );
            index += source.Length;
        }
    }
    */
}
