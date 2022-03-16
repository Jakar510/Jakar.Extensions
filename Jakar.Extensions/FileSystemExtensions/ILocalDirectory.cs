using System.IO.Compression;
using System.Security;


namespace Jakar.Extensions.FileSystemExtensions;


public interface ILocalDirectory<TFile, TDirectory> : IDisposable, IAsyncDisposable, TempFile.ITempFile, IEquatable<TDirectory> where TDirectory : ILocalDirectory<TFile, TDirectory>
                                                                                                                                where TFile : ILocalFile<TFile, TDirectory>
{
    public DirectoryInfo Info              { get; }
    public string        FullPath          { get; init; }
    public bool          Exists            { get; }
    public string        Name              { get; }
    public TDirectory?   Parent            { get; }
    public string?       Root              { get; }
    public DateTime      CreationTimeUtc   { get; set; }
    public DateTime      LastAccessTimeUtc { get; set; }
    public DateTime      LastWriteTimeUtc  { get; set; }


    public void Zip( in TFile outputPath, in CompressionLevel compression = CompressionLevel.Optimal, in Encoding? encoding = null );


    /// <summary>
    /// Gets the <typeparamref name="TDirectory"/> object of the directory in this <typeparamref name="TDirectory"/>
    /// </summary>
    /// <param name="paths"></param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="PathTooLongException"></exception>
    /// <exception cref="SecurityException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <returns><typeparamref name="TDirectory"/></returns>
    public TDirectory CreateSubDirectory( params string[] paths );


    /// <summary>
    /// Gets the <typeparamref name="TFile"/> object of the file in this <typeparamref name="TDirectory"/>
    /// </summary>
    /// <param name="path"></param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="PathTooLongException"></exception>
    /// <exception cref="SecurityException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <returns><typeparamref name="TFile"/></returns>
    public TFile Join( string path );


    /// <summary>
    /// Gets the path of the directory or file in this <typeparamref name="TDirectory"/>
    /// </summary>
    /// <param name="subPaths"></param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="PathTooLongException"></exception>
    /// <exception cref="SecurityException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <returns><see cref="string"/></returns>
    public string Combine( params string[] subPaths );


    public bool Equals( object obj );
    public int  GetHashCode();


#region Deletes

    /// <summary>
    /// Deletes this directory if it is empty.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="SecurityException"></exception>
    public void Delete();


    /// <summary>
    /// Deletes sub-directories and files. This occurs on another thread in Windows, and is not blocking.
    /// </summary>
    /// <param name="recursive"></param>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="SecurityException"></exception>
    public void Delete( bool recursive );


    /// <summary>
    /// Deletes sub-directories and files.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="SecurityException"></exception>
    public void DeleteAllRecursively();

    /// <summary>
    /// Asynchronously deletes sub-directories and files.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="SecurityException"></exception>
    public Task DeleteAllRecursivelyAsync();


    /// <summary>
    /// Deletes sub-directories.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="SecurityException"></exception>
    public void DeleteFiles();

    /// <summary>
    /// Asynchronously deletes files.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="SecurityException"></exception>
    public Task DeleteFilesAsync();


    /// <summary>
    /// Deletes sub-directories.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="SecurityException"></exception>
    public void DeleteSubFolders();

    /// <summary>
    /// Asynchronously deletes sub-directories.
    /// </summary>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="SecurityException"></exception>
    public Task DeleteSubFoldersAsync();

#endregion


#region sub Files

    public IEnumerable<TFile> GetFiles();
    public IEnumerable<TFile> GetFiles( string searchPattern );
    public IEnumerable<TFile> GetFiles( string searchPattern, SearchOption       searchOption );
    public IEnumerable<TFile> GetFiles( string searchPattern, EnumerationOptions enumerationOptions );

#endregion


#region Sub Folders

    public IEnumerable<TDirectory> GetSubFolders();
    public IEnumerable<TDirectory> GetSubFolders( string searchPattern );
    public IEnumerable<TDirectory> GetSubFolders( string searchPattern, SearchOption       searchOption );
    public IEnumerable<TDirectory> GetSubFolders( string searchPattern, EnumerationOptions enumerationOptions );

#endregion
}
