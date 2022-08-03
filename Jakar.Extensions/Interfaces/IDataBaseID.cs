#nullable enable
namespace Jakar.Extensions;


public interface IUniqueID<out TId>
{
    public TId ID { get; }
}



public interface IDataBaseIDString : IUniqueID<string> { }



public interface IDataBaseIDGuid : IUniqueID<Guid> { }



public interface IDataBaseID : IUniqueID<long> { }



public interface IDataBaseIgnore { }
