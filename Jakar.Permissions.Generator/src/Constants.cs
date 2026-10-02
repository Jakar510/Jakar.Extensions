// Jakar.Extensions :: Jakar.Permissions.Generator
// 10/17/2025  10:31

namespace Jakar.Permissions.Generator;


internal static class Constants
{
    public const string APP         = "App";
    public const string ATTRIBUTE   = $"{NAME_SPACE}.{nameof(PermissionGenOptionsAttribute)}";
    public const string FILE_NAME   = "permissions.json";
    public const string NAME_SPACE  = "Jakar.Permissions.Generator";
    public const string PERMISSIONS = "Permissions";


    public static string[] Candidates =
    [
        FILE_NAME,
        "Permissions.json",
        "rights.json",
        "UserRights.json",
        "user_rights.json",
        "user_permissions.json",
        "UserPermissions.json"
    ];
}
