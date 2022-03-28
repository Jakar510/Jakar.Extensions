namespace Jakar.Extensions.Models.Base.Classes;


[Serializable]
public class BaseClass : IDataBaseID
{
    [Key] public virtual long ID { get; init; }
}
