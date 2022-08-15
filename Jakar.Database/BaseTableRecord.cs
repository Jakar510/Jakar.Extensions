// Jakar.Extensions :: Jakar.Database
// 08/14/2022  8:38 PM

namespace Jakar.Database;


[Serializable]
public abstract record BaseTableRecord<TClass> : BaseCollectionsRecord<TClass> where TClass : BaseTableRecord<TClass>
{
    protected BaseTableRecord() { }
    protected BaseTableRecord( long id ) => ID = id;
}
