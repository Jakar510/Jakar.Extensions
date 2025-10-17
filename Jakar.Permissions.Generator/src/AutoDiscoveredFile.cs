using System.IO;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;



namespace Jakar.Permissions.Generator;


internal sealed class AutoDiscoveredFile : AdditionalText
{
    public override string Path { get; }
    public AutoDiscoveredFile( string                      path ) => Path = path;
    public override SourceText GetText( CancellationToken cancellationToken = default ) => SourceText.From(File.ReadAllText(Path), Encoding.Default);
}
