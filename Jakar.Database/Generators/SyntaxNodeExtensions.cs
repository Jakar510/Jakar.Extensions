// Jakar.Extensions :: Jakar.Database
// 09/07/2023  10:38 PM

using Microsoft.CodeAnalysis;



namespace Jakar.Database;


public static class SyntaxNodeExtensions
{
    public static T GetParent<T>( this SyntaxNode node )
    {
        SyntaxNode? parent = node.Parent;

        while ( parent is not null )
        {
            if ( parent is T value ) { return value; }
            
            parent = parent.Parent;
        }

        throw new NotFoundException( typeof(T).Name );
    }
}
