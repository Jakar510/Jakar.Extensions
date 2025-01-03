// Jakar.Extensions :: Jakar.Database
// 06/02/2024  11:06

namespace Jakar.Database;


public readonly record struct Tag( string Name, object? Value )
{
    public static implicit operator KeyValuePair<string, object?>( Tag tag ) => new(tag.Name, tag.Value);


    public static KeyValuePair<string, object?>[] Convert( scoped ref readonly ReadOnlySpan<Tag> tags )
    {
        KeyValuePair<string, object?>[] array = AsyncLinq.GetArray<KeyValuePair<string, object?>>( tags.Length );

        for ( int i = 0; i < tags.Length; i++ )
        {
            Tag tag = tags[i];
            array[i] = new KeyValuePair<string, object>( tag.Name, tag.Value );
        }

        return array;
    }
}
