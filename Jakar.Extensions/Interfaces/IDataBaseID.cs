#nullable enable
namespace Jakar.Extensions;


public interface IUniqueID<TID> where TID : IComparable<TID>, IEquatable<TID>
{
    public TID ID { get; init; }
}



public interface IDataBaseIDGuid : IUniqueID<Guid> { }



public interface IDataBaseID : IUniqueID<long> { }



public interface IDataBaseIgnore { }
