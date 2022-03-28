namespace Jakar.Extensions.Models.Base.Records;


[Serializable]
public record BaseRecord : IDataBaseID
{
    [Key] public virtual long ID { get; init; }
}
