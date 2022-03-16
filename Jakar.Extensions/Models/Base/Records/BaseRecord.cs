namespace Jakar.Extensions.Models.Base.Records;


[Serializable]
public record BaseRecord : IDataBaseID
{
    [Key] public long ID { get; init; }
}
