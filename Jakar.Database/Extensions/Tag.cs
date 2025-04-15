// Jakar.Extensions :: Jakar.Database
// 06/02/2024  11:06

namespace Jakar.Database;


public readonly record struct Tag( string Name, object? Value )
{
    public static implicit operator KeyValuePair<string, object?>( Tag tag ) { return new KeyValuePair<string, object?>(tag.Name, tag.Value); }


    public static KeyValuePair<string, object?>[] Convert( params ReadOnlySpan<Tag> tags )
    {
        KeyValuePair<string, object?>[] array = AsyncLinq.GetArray<KeyValuePair<string, object?>>( tags.Length );
        for ( int i = 0; i < tags.Length; i++ ) { array[i] = tags[i]; }

        return array;
    }
}
