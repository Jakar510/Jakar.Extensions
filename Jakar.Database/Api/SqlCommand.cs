// Jakar.Extensions :: Jakar.Database
// 12/02/2024  11:12

namespace Jakar.Database;


public readonly record struct SqlCommand( string SQL, DynamicParameters? Parameters = null, CommandType? CommandType = null, CommandFlags Flags = CommandFlags.None )
{
    public static implicit operator SqlCommand( string sql ) => new(sql);

    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public CommandDefinition ToCommandDefinition( DbTransaction? transaction, CancellationToken token,       int?              timeout             = null ) => new(SQL, Parameters, transaction, timeout, CommandType, Flags, token);
    [Pure, MethodImpl( MethodImplOptions.AggressiveInlining )] public Definition        ToCommandDefinition( DbConnection   connection,  DbTransaction?    transaction, CancellationToken token, int? timeout = null ) => new(connection, ToCommandDefinition( transaction, token, timeout ));



    public readonly struct Definition( DbConnection connection, CommandDefinition command )
    {
        public readonly DbConnection      connection = connection;
        public readonly CommandDefinition command    = command;
        
        public static implicit operator CommandDefinition( Definition x ) => x.command;
        public static implicit operator DbConnection( Definition      x ) => x.connection;
    }
}
