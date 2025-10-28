using Microsoft.CodeAnalysis;



namespace Jakar.Permissions.Generator;


internal static class Diagnostics
{
    public static readonly DiagnosticDescriptor EmptyPermissions = new("PERM001", "No Permissions Found", "The permissions JSON file did not contain any permissions to generate.", Constants.NAME_SPACE, DiagnosticSeverity.Info, true);
    public static readonly Diagnostic           Empty            = Diagnostic.Create(EmptyPermissions, Location.None);

    public static readonly DiagnosticDescriptor InvalidNamespace       = new("PERM002", "Invalid Namespace Name", "The namespace '{0}' is not a valid C# identifier sequence.", Constants.NAME_SPACE, DiagnosticSeverity.Warning, true);
    public static readonly DiagnosticDescriptor InvalidRootClass       = new("PERM003", "Invalid Root Class Name", "The root class name '{0}' is not a valid C# identifier.", Constants.NAME_SPACE, DiagnosticSeverity.Warning, true);
    public static readonly DiagnosticDescriptor MissingPermissionsFile = new("PERM004", "Permissions File Not Found", "No permissions.json file found in the project or AdditionalFiles.", Constants.NAME_SPACE, DiagnosticSeverity.Info, true);
    public static readonly Diagnostic           Missing                = Diagnostic.Create(MissingPermissionsFile, Location.None);

    public static Diagnostic NameSpace( string   nameSpace ) => Diagnostic.Create(InvalidNamespace, Location.None, nameSpace);
    public static Diagnostic InvalidRoot( string root )      => Diagnostic.Create(InvalidRootClass, Location.None, root);
}
