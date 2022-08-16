﻿// Jakar.Extensions :: Jakar.Database
// 08/14/2022  8:38 PM

namespace Jakar.Database;


[Serializable]
public abstract record BaseTableRecord<TClass, TID> : BaseCollectionsRecord<TClass, TID> where TClass : BaseTableRecord<TClass, TID>
                                                                                         where TID : IComparable<TID>, IEquatable<TID>
{
    protected BaseTableRecord() : base() { }
    protected BaseTableRecord( TID id ) : base(id) { }
}
