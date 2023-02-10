// Jakar.Extensions :: Jakar.Database
// 02/03/2023  2:22 PM

namespace Jakar.Database;


public record MappingRecord<TSource, TTarget> : TableRecord<MappingRecord<TSource, TTarget>> where TSource : TableRecord<TSource>
                                                                                             where TTarget : TableRecord<TTarget>
{
    public long SourceID { get; init; }
    public long TargetID { get; init; }


    public MappingRecord() { }
    public MappingRecord( TSource source, TTarget target ) : base( 0 )
    {
        SourceID = source.ID;
        TargetID = target.ID;
    }
    public MappingRecord( TSource source, TTarget target, UserRecord user ) : base( user )
    {
        SourceID = source.ID;
        TargetID = target.ID;
    }


    public async ValueTask<TSource?> GetSource( DbConnection connection, DbTransaction? transaction, DbTable<TSource> table, CancellationToken token ) => await table.Get( connection, transaction, SourceID, token );
    public async ValueTask<TTarget?> GetTarget( DbConnection connection, DbTransaction? transaction, DbTable<TTarget> table, CancellationToken token ) => await table.Get( connection, transaction, TargetID, token );
}



public record MappingRecord<TTarget> : TableRecord<MappingRecord<TTarget>> where TTarget : TableRecord<TTarget>
{
    public long TargetID { get; init; }


    public MappingRecord() { }
    public MappingRecord( TTarget target, UserRecord user ) : base( user ) => TargetID = target.ID;


    public async ValueTask<UserRecord?> GetTarget( DbConnection connection, DbTransaction? transaction, DbTable<UserRecord> table, CancellationToken token ) => await table.Get( connection, transaction, CreatedBy, token );
    public async ValueTask<TTarget?> GetTarget( DbConnection    connection, DbTransaction? transaction, DbTable<TTarget>    table, CancellationToken token ) => await table.Get( connection, transaction, TargetID,  token );
}
