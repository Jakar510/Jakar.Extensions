namespace Jakar.Extensions;


[Serializable]
public record BaseRecord
{
    public const int ANSI_CAPACITY    = 8000;
    public const int BINARY_CAPACITY  = int.MaxValue;
    public const int MAX_STRING_SIZE  = int.MaxValue;
    public const int UNICODE_CAPACITY = 4000;
}
