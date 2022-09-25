// Jakar.Extensions :: Jakar.Database
// 08/14/2022  8:38 PM

namespace Jakar.Database;


[Serializable]
public abstract record BaseTableRecord<TRecord> : BaseCollectionsRecord<TRecord, long> where TRecord : BaseTableRecord<TRecord>
{
    public static string TableName { get; } = typeof(TRecord).GetTableName();


    protected BaseTableRecord() : base() { }
    protected BaseTableRecord( long id ) : base(id) { }


    public TRecord NewID( in long id ) => (TRecord)( this with
                                                    {
                                                        ID = id
                                                    } );

    protected virtual void VerifyAccess() { }
}
