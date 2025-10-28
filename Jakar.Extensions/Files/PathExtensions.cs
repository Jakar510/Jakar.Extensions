namespace Jakar.Extensions;


public static class PathExtensions
{
    public static string[] Directories( this DirectoryInfo root ) => Directory.GetDirectories(root.FullName);
    public static string[] FilePaths( this   DirectoryInfo root ) => Directory.GetFiles(root.FullName);
    public static string[] SubFolderNames( this DirectoryInfo root ) => root.EnumerateDirectories()
                                                                            .Select(static x => x.Name)
                                                                            .ToArray();
    public static string Combine( this DirectoryInfo path, string                      fileName )   => Path.Combine(path.FullName, fileName);
    public static string Combine( this DirectoryInfo path, params ReadOnlySpan<string> subFolders ) => path.FullName.Combine(subFolders);
    public static string Combine( this string path, params ReadOnlySpan<string> subFolders )
    {
        string[] results = GC.AllocateUninitializedArray<string>(subFolders.Length + 1);
        results[0] = path;
        for ( int i = 0; i < subFolders.Length; i++ ) { results[i + 1] = subFolders[i]; }

        return Path.Combine(results);
    }


    /// <summary> extension method for <see cref="Path.GetExtension(string)"/> </summary>
    /// <param name="path"> </param>
    /// <returns> </returns>
    public static string GetExtension( this string path ) => Path.GetExtension(path);


    /// <summary> extension method for <see cref="Path.GetFileName(string)"/> </summary>
    /// <param name="path"> </param>
    /// <returns> </returns>
    public static string GetFileName( this string path ) => Path.GetFileName(path);


    /// <summary> extension method for <see cref="Path.GetFileNameWithoutExtension(string)"/> </summary>
    /// <param name="path"> </param>
    /// <returns> </returns>
    public static string GetFileNameWithoutExtension( this string path ) => Path.GetFileNameWithoutExtension(path);
}
