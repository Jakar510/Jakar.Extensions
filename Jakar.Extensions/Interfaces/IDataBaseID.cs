#nullable enable
namespace Jakar.Extensions.Interfaces;


public interface IUniqueID<TId>
{
    public TId ID { get; init; }
}



public interface IDataBaseIDString : IUniqueID<string> { }



public interface IDataBaseIDGuid : IUniqueID<Guid> { }



public interface IDataBaseID : IUniqueID<long> { }



public interface IDataBaseIgnore { }
