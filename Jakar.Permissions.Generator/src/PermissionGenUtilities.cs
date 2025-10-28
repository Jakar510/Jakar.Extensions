using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using Jakar.Permissions.Generator.Runtime;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;



namespace Jakar.Permissions.Generator;


internal static class PermissionGenUtilities
{
    public static readonly ConcurrentQueue<Diagnostic> PendingDiagnostics = new();


    public static (bool IncludeDocs, bool IncludeDebuggerDisplay, string Namespace, string RootClass) GetOptions( Compilation compilation, CancellationToken token )
    {
        bool                          includeDocs     = true;
        bool                          includeDebugger = true;
        string                        name_space      = Constants.App;
        string                        root            = Constants.Permissions;
        ImmutableArray<AttributeData> attributes      = compilation.Assembly.GetAttributes();

        foreach ( AttributeData attribute in attributes.AsSpan() )
        {
            if ( !string.Equals(attribute.AttributeClass?.ToDisplayString(), Constants.Attribute, StringComparison.Ordinal) ) { continue; }

            foreach ( KeyValuePair<string, TypedConstant> arg in attribute.NamedArguments )
            {
                switch ( arg.Key )
                {
                    case nameof(PermissionGenOptionsAttribute.IncludeDocs):
                        includeDocs = arg.Value.Value is true;
                        break;

                    case nameof(PermissionGenOptionsAttribute.IncludeDebuggerDisplay):
                        includeDebugger = arg.Value.Value is true;
                        break;

                    case nameof(PermissionGenOptionsAttribute.Namespace):
                        if ( arg.Value.Value is string s1 && !string.IsNullOrWhiteSpace(s1) ) { name_space = s1.Trim(); }

                        break;

                    case nameof(PermissionGenOptionsAttribute.RootClass):
                        if ( arg.Value.Value is string s2 && !string.IsNullOrWhiteSpace(s2) ) { root = s2.Trim(); }

                        break;
                }
            }
        }

        // Validate identifiers
        if ( !IsValidNamespace(name_space) )
        {
            PendingDiagnostics.Enqueue(Diagnostics.NameSpace(name_space));
            name_space = Constants.App;
        }

        if ( !IsValidIdentifier(root) )
        {
            PendingDiagnostics.Enqueue(Diagnostics.InvalidRoot(root));
            root = Constants.Permissions;
        }

        return ( includeDocs, includeDebugger, name_space, root );
    }

    private static bool IsValidNamespace( string ns )
    {
        string[] parts = ns.Split('.', StringSplitOptions.RemoveEmptyEntries);
        return parts.All(IsValidIdentifier);
    }

    private static bool IsValidIdentifier( string name )
    {
        if ( string.IsNullOrWhiteSpace(name) ) { return false; }

        if ( !SyntaxFacts.IsValidIdentifier(name) ) { return false; }

        return SyntaxFacts.GetKeywordKind(name) != SyntaxKind.IsKeyword;
    }

    public static ImmutableArray<AdditionalText> AutoDiscoverFiles( CancellationToken token )
    {
        string projectDir = Directory.GetCurrentDirectory();
        if ( string.IsNullOrEmpty(projectDir) ) { return ImmutableArray<AdditionalText>.Empty; }

        ImmutableArray<AdditionalText> found = Constants.Candidates.Select(name => Path.Combine(projectDir, name))
                                                        .Where(File.Exists)
                                                        .Select(static AdditionalText ( path ) => new AutoDiscoveredFile(path))
                                                        .ToImmutableArray();

        return found;
    }

    public static List<string> ParsePermissions( string json )
    {
        JsonNode?    node = JsonNode.Parse(json);
        List<string> list = new(64);

        if ( node is not null ) { BuildPermissionList(node, list, null); }

        return list;
    }

    private static void BuildPermissionList( JsonNode node, List<string> result, string? prefix )
    {
        if ( node is JsonObject obj )
        {
            foreach ( KeyValuePair<string, JsonNode?> kvp in obj )
            {
                string path = string.IsNullOrEmpty(prefix)
                                  ? kvp.Key
                                  : $"{prefix}.{kvp.Key}";

                BuildPermissionList(kvp.Value!, result, path);
            }
        }
        else if ( node is JsonArray arr )
        {
            foreach ( JsonNode? child in arr )
            {
                if ( child is JsonValue v && v.TryGetValue(out string? str) ) { result.Add($"{prefix}.{str}"); }
            }
        }
    }

    // Generates PermissionRegistry source
    public static void GenerateRegistry( SourceProductionContext ctx, List<string> permissions, string namespaceName = "Jakar.Permissions.Generator.Runtime" )
    {
        StringBuilder sb = new();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine();
        sb.AppendLine($"namespace {namespaceName};");
        sb.AppendLine();
        sb.AppendLine("public static class PermissionRegistry");
        sb.AppendLine("{");
        sb.AppendLine("    private static readonly string[] _names = new string[]");
        sb.AppendLine("    {");
        foreach ( string name in permissions ) { sb.Append("""        @"").Append(name).AppendLine("","""); }

        sb.AppendLine("    };");
        sb.AppendLine();
        sb.AppendLine("    private static readonly Dictionary<string, int> _lookup = CreateLookup();");
        sb.AppendLine();
        sb.AppendLine("    private static Dictionary<string, int> CreateLookup()");
        sb.AppendLine("    {");
        sb.AppendLine("        Dictionary<string, int> d = new(StringComparer.OrdinalIgnoreCase);");
        sb.AppendLine("        for (int i = 0; i < _names.Length; i++) d[_names[i]] = i + 1;");
        sb.AppendLine("        return d;");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    public static int Count => _names.Length;");
        sb.AppendLine("    public static string GetName(int id) => id >= 1 && id <= _names.Length ? _names[id - 1] : EMPTY;");
        sb.AppendLine("    public static bool TryGetId(string name, out int id) => _lookup.TryGetValue(name, out id);");
        sb.AppendLine("}");
        ctx.AddSource("PermissionRegistry.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }

    // Generates nested Permissions static class
    public static void GenerateConstants( SourceProductionContext ctx, List<string> permissions, bool includeDocs, bool includeDebugger, string ns, string rootClass )
    {
        // Build tree
        Node root = new(rootClass);

        for ( int i = 0; i < permissions.Count; i++ )
        {
            string[] parts = permissions[i]
               .Split('.', StringSplitOptions.RemoveEmptyEntries);

            Node current = root;

            for ( int p = 0; p < parts.Length; p++ )
            {
                string name = ToIdentifier(parts[p]);

                if ( p == parts.Length - 1 ) { current.Fields.Add(( name, i + 1, permissions[i] )); }
                else
                {
                    if ( !current.Children.TryGetValue(name, out Node? next) )
                    {
                        next                   = new Node(name);
                        current.Children[name] = next;
                    }

                    current = next;
                }
            }
        }

        StringBuilder sb = new();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("using Jakar.Permissions.Generator.Runtime;");
        if ( includeDebugger ) { sb.AppendLine("using System.Diagnostics;"); }

        sb.AppendLine();

        sb.Append("namespace ")
          .Append(ns)
          .AppendLine(";");

        sb.AppendLine();
        WriteNode(in sb, in root, 0, in includeDocs, in includeDebugger);
        ctx.AddSource($"{rootClass}.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
    }

    private static void WriteNode( ref readonly StringBuilder sb, ref readonly Node node, in int indent, in bool includeDocs, in bool includeDebugger )
    {
        string ind = new(' ', indent * 4);

        if ( indent > 0 )
        {
            sb.Append(ind)
              .Append("public static class ")
              .Append(node.Name)
              .AppendLine();

            sb.Append(ind)
              .AppendLine("{");
        }

        foreach ( ( string field, int id, string path ) in node.Fields )
        {
            string fieldIndent = ind + "    ";

            if ( includeDocs )
            {
                sb.Append(fieldIndent)
                  .Append("/// <summary>Permission: ")
                  .Append(path)
                  .AppendLine("</summary>");
            }

            if ( includeDebugger )
            {
                sb.Append(fieldIndent)
                  .Append("[DebuggerDisplay(")
                  .Append('"')
                  .Append(path)
                  .Append('"')
                  .Append(')')
                  .AppendLine("]");
            }

            sb.Append(fieldIndent)
              .Append($"public static readonly {nameof(PermissionIndex)} ")
              .Append(field)
              .Append(" = new(")
              .Append(id)
              .Append(", ")
              .Append('"')
              .Append(path)
              .Append('"')
              .AppendLine(");");
        }

        foreach ( Node child in node.Children.Values )
        {
            WriteNode(in sb,
                      in child,
                      indent +
                      ( indent > 0
                            ? 1
                            : 0 ) +
                      1,
                      in includeDocs,
                      in includeDebugger);
        }

        if ( indent > 0 ) { sb.Append(ind); }

        sb.AppendLine("}");
    }

    private static string ToIdentifier( string dotted )
    {
        StringBuilder sb         = new();
        bool          capitalize = true;

        foreach ( char c in dotted )
        {
            if ( char.IsLetterOrDigit(c) )
            {
                sb.Append(capitalize
                              ? char.ToUpperInvariant(c)
                              : c);

                capitalize = false;
            }
            else { capitalize = true; }
        }

        return sb.Length == 0
                   ? "Permission"
                   : sb.ToString();
    }



    private sealed class Node
    {
        public Dictionary<string, Node>                  Children { get; } = [];
        public List<(string Field, int Id, string Path)> Fields   { get; } = [];
        public string                                    Name     { get; }
        public Node( string name ) => Name = name;
    }
}
