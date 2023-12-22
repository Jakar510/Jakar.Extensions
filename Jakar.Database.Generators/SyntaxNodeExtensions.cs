// Jakar.Extensions :: Jakar.Database
// 09/07/2023  10:38 PM

namespace Jakar.Database.Generators;


public static class SyntaxNodeExtensions
{
    public static T GetParent<T>( this SyntaxNode node )
        where T : SyntaxNode
    {
        SyntaxNode? parent = node.Parent;

        while ( parent is not null )
        {
            if ( parent is T value ) { return value; }

            parent = parent.Parent;
        }

        throw new InvalidOperationException( $"Not Found: {typeof(T).FullName ?? typeof(T).Name}" );
    }
}
