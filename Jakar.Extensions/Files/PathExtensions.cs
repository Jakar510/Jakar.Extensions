#nullable enable
namespace Jakar.Extensions;


public static class PathExtensions
{
    public static string Combine( this DirectoryInfo path, string          fileName ) => Path.Combine(path.FullName, fileName);
    public static string Combine( this DirectoryInfo path, params string[] subFolders ) => path.FullName.Combine(subFolders);
    public static string Combine( this string path, params string[] subFolders )
    {
        var items = new string[subFolders.Length + 1];
        items[0] = path;
        for ( var i = 0; i < subFolders.Length; i++ ) { items[i + 1] = subFolders[i]; }

        return Path.Combine(items);
    }


    public static IReadOnlyList<string> FilePaths( this DirectoryInfo root ) => Directory.GetFiles(root.FullName);
    public static IReadOnlyList<string> SubFolderNames( this DirectoryInfo root ) => root.EnumerateDirectories()
                                                                                         .Select(item => item.Name)
                                                                                         .ToList();
    public static IReadOnlyList<string> Directories( this DirectoryInfo root ) => Directory.GetDirectories(root.FullName);


    /// <summary>
    ///     extension method for
    ///     <see cref = "Path.GetFileName(string)" />
    /// </summary>
    /// <param name = "path" > </param>
    /// <returns> </returns>
    public static string GetFileName( this string path ) => Path.GetFileName(path);


    /// <summary>
    ///     extension method for
    ///     <see cref = "Path.GetFileNameWithoutExtension(string)" />
    /// </summary>
    /// <param name = "path" > </param>
    /// <returns> </returns>
    public static string GetFileNameWithoutExtension( this string path ) => Path.GetFileNameWithoutExtension(path);


    /// <summary>
    ///     extension method for
    ///     <see cref = "Path.GetExtension(string)" />
    /// </summary>
    /// <param name = "path" > </param>
    /// <returns> </returns>
    public static string GetExtension( this string path ) => Path.GetExtension(path);

    
    public static string Combine( this    DirectoryInfo outputDirectory, in     string   fileName ) => Path.Combine(outputDirectory.FullName, fileName);
    public static string CombineAll( this DirectoryInfo outputDirectory, params string[] args ) => args.Aggregate(outputDirectory.FullName, Path.Combine);
}
