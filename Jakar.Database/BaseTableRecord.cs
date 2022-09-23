// Jakar.Extensions :: Jakar.Database
// 08/14/2022  8:38 PM

namespace Jakar.Database;


[Serializable]
public abstract record BaseTableRecord<TRecord, TID> : BaseCollectionsRecord<TRecord, TID> where TRecord : BaseTableRecord<TRecord, TID>
                                                                                           where TID : struct, IComparable<TID>, IEquatable<TID>
{
    public static string TableName { get; } = typeof(TRecord).GetTableName();


    protected BaseTableRecord() : base() { }
    protected BaseTableRecord( TID id ) : base(id) { }


    public TRecord NewID( in TID id ) => (TRecord)( this with
                                                    {
                                                        ID = id
                                                    } );

    protected virtual void VerifyAccess() { }
}
