using System.IO.Compression;


namespace Jakar.Extensions.FileSystemExtensions;


public static class PathExtensions
{
    public static string Combine( this DirectoryInfo path, string          fileName )   => Path.Combine(path.FullName, fileName);
    public static string Combine( this DirectoryInfo path, params string[] subFolders ) => path.FullName.Combine(subFolders);

    public static string Combine( this string path, params string[] subFolders )
    {
        var items = new List<string>(subFolders.Length + 1)
                    {
                        path
                    };

        items.AddRange(subFolders);

        return Path.Combine(items.ToArray());
    }


    public static IList<string>              FilePaths( this      DirectoryInfo root ) => Directory.GetFiles(root.FullName);
    public static IEnumerable<FileInfo>      Files( this          DirectoryInfo root ) => root.EnumerateFiles();
    public static IEnumerable<DirectoryInfo> SubFolders( this     DirectoryInfo root ) => root.EnumerateDirectories();
    public static IList<string>              SubFolderNames( this DirectoryInfo root ) => root.SubFolders().Select(item => item.Name).ToList();
    public static IList<string>              Directories( this    DirectoryInfo root ) => Directory.GetDirectories(root.FullName);


    /// <summary>
    /// extension method for <see cref="Path.GetFileName(string)"/>
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetFileName( this string path ) => Path.GetFileName(path);

    /// <summary>
    /// extension method for <see cref="Path.GetFileNameWithoutExtension(string)"/>
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetFileNameWithoutExtension( this string path ) => Path.GetFileNameWithoutExtension(path);

    /// <summary>
    /// extension method for <see cref="Path.GetExtension(string)"/>
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetExtension( this string path ) => Path.GetExtension(path);

    public static string Combine( this DirectoryInfo outputDirectory, in string fileName ) => Path.Combine(outputDirectory.FullName, fileName);

    public static string CombineAll( this DirectoryInfo outputDirectory, params string[] args ) => args.Aggregate(outputDirectory.FullName, Path.Combine);


    public static Task<LocalFile> ZipAsync( this LocalFile zipFilePath, params string[]     args )  => zipFilePath.ZipAsync(args.ToList());
    public static Task<LocalFile> ZipAsync( this LocalFile zipFilePath, IEnumerable<string> files ) => zipFilePath.ZipAsync(files.Select(item => new LocalFile(item)));
    public static Task<LocalFile> ZipAsync( this LocalFile zipFilePath, params LocalFile[]  files ) => zipFilePath.ZipAsync(files.ToList());

    public static async Task<LocalFile> ZipAsync( this LocalFile zipFilePath, IEnumerable<LocalFile> items )
    {
        if ( items is null ) throw new ArgumentNullException(nameof(items));

        await using FileStream zipToOpen = File.Create(zipFilePath.FullPath);
        using var              archive   = new ZipArchive(zipToOpen, ZipArchiveMode.Update);

        foreach ( LocalFile file in items )
        {
            ZipArchiveEntry    entry  = archive.CreateEntry(file.FullPath);
            await using Stream stream = entry.Open();

            ReadOnlyMemory<byte> data = await file.RawReadFromFileAsync();
            await stream.WriteAsync(data, CancellationToken.None);
        }

        return zipFilePath;
    }
}
