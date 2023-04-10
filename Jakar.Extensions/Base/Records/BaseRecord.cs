#nullable enable
namespace Jakar.Extensions;


[Serializable]
public record BaseRecord
{
    public const int MAX_STRING_SIZE = 0x3FFFFFDF; // 1GB


    public BaseRecord() { }
}
