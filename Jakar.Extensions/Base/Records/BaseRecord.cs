namespace Jakar.Extensions;


[Serializable]
public record BaseRecord
{
    public const int ANSI_STRING_CAPACITY    = 8000;
    public const int ANSI_TEXT_CAPACITY      = 2_147_483_647;
    public const int BINARY_CAPACITY         = ANSI_TEXT_CAPACITY;
    public const int MAX_STRING_SIZE         = 0x3FFFFFDF; // 1GB
    public const int UNICODE_STRING_CAPACITY = 4000;
    public const int UNICODE_TEXT_CAPACITY   = 1_073_741_823;
}
