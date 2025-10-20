// Jakar.Extensions :: Jakar.Database
// 12/02/2024  11:12

namespace Jakar.Database;


/*
public readonly struct SqlCommand( string sql, PostgresParameters parameters = null, CommandType? commandType = null, CommandFlags flags = CommandFlags.None )
{
    public readonly string             sql         = sql;
    public readonly PostgresParameters parameters  = parameters;
    public readonly CommandType?       commandType = commandType;
    public readonly CommandFlags       flags       = flags;


    public static implicit operator SqlCommand( string sql ) => new(sql);


    [Pure] public Dapper.CommandDefinition ToCommandDefinition( NpgsqlTransaction?   transaction, CancellationToken token,       int?              timeout             = null ) => new(sql, parameters, transaction, timeout, commandType, flags, token);
    [Pure] public Definition               ToCommandDefinition( NpgsqlConnection connection,  NpgsqlTransaction?    transaction, CancellationToken token, int? timeout = null ) => new(connection, ToCommandDefinition(transaction, token, timeout));



    public readonly struct Definition( NpgsqlConnection connection, Dapper.CommandDefinition command )
    {
        public readonly NpgsqlConnection  connection = connection;
        public readonly CommandDefinition command    = command;

        public static implicit operator CommandDefinition( Definition x ) => x.command;
        public static implicit operator NpgsqlConnection( Definition  x ) => x.connection;
    }
}



public static class SqlCommandExtensions
{
    public static NpgsqlCommand ToNpgsqlCommand( this in SqlCommand<TSelf> sqlCommand, NpgsqlConnection connection, NpgsqlTransaction? transaction = null )
    {
        if ( connection is null ) { throw new ArgumentNullException(nameof(connection)); }

        NpgsqlCommand command = new NpgsqlCommand
                                {
                                    Connection  = connection,
                                    CommandText = sqlCommand.sql,
                                    CommandType = sqlCommand.commandType ?? CommandType.Text,
                                    Transaction = transaction,
                                };

        if ( sqlCommand.parameters is not null )
        {
            foreach ( string name in sqlCommand.parameters.ParameterNames )
            {
                string parameterName = name.StartsWith("@", StringComparison.Ordinal)
                                           ? name
                                           : "@" + name;

                object? value;

                try { value = sqlCommand.parameters.Get<object?>(name); }
                catch { value = null; }

                // Use explicit construction instead of AddWithValue (for clarity)
                NpgsqlParameter parameter = new(parameterName, value ?? DBNull.Value);
                command.Parameters.Add(parameter);
            }
        }

        return command;
    }
}
*/
