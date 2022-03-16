namespace Jakar.Extensions.Models.Base.Classes;


[Serializable]
public class BaseClass : IDataBaseID
{
    [Key] public long ID { get; init; }
}
