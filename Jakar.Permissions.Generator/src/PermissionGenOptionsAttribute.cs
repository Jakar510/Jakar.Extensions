using System;



namespace Jakar.Permissions.Generator;


/// <summary> Optional assembly-level configuration for Jakar.Permissions.Generator output. </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class PermissionGenOptionsAttribute : Attribute
{
    public bool   IncludeDebuggerDisplay { get; init; } = true;
    public bool   IncludeDocs            { get; init; } = true;
    public string Namespace              { get; init; } = Constants.App;
    public string RootClass              { get; init; } = Constants.Permissions;
}
