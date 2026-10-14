// Jakar.Json
// 10/13/2026

namespace Jakar.Json;


/// <summary>
///     The size of the last document written per type, so the next write of that type starts with a buffer about that big instead of growing from the stack buffer by doubling
///     (each doubling copies everything so far). Plain fields: a racing write only makes a hint stale, never wrong, since buffers still grow as needed.
/// </summary>
internal static class JsonSizeHints<T>
{
    public static int Utf16;
    public static int Utf8;
}



internal static class JsonSizeHints
{
    /// <summary> The hint plus an eighth, so a document slightly larger than the last doesn't grow once at the very end. </summary>
    public static int Capacity( int hint ) => (int)Math.Min((long)hint + ( hint >> 3 ) + 16, Array.MaxLength);
}
