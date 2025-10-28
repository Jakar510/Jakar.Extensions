using System;
using Permissions;
using PermissionGen.Runtime;

class Program
{
    static void Main()
    {
        Console.WriteLine($"Total permissions: {PermissionRegistry.Count}");
        Console.WriteLine($"First permission: {PermissionRegistry.GetName(1)}");

        Console.WriteLine("Available top-level classes in Permissions root (via reflection):");
        foreach (var t in typeof(Permissions.Permissions).GetNestedTypes())
        {
            Console.WriteLine(" - " + t.Name);
        }
    }
}
