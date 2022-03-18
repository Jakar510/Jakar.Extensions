using System.Security;
using System.Web;


namespace Jakar.Extensions.FileSystemExtensions;


public interface ILocalFile<TFile, out TDirectory> : TempFile.ITempFile, IEquatable<TFile>, IComparable<TFile> where TFile : ILocalFile<TFile, TDirectory>
                                                                                                                                              where TDirectory : ILocalDirectory<TFile, TDirectory>
{
    public FileInfo    Info          { get; }
    public string      FullPath      { get; init; }
    public string      Name          { get; }
    public string      Extension     { get; }
    public bool        Exists        { get; }
    public string?     DirectoryName { get; }
    public MimeType    Mime          { get; }
    public string      ContentType   { get; }
    public TDirectory? Parent        { get; }
    public string?     Root          { get; }


    /// <summary>
    /// Permanently deletes a file.
    /// </summary>
    public void Delete();

    /// <summary>
    /// Encrypts the file so that only the account used to encrypt the file can decrypt it.
    /// </summary>
    public void Encrypt();

    /// <summary>
    /// Decrypts a file that was encrypted by the current account using the <see cref="File.Encrypt(string)"/> method.
    /// </summary>
    public void Decrypt();

    /// <summary>
    /// Changes the extension of the file.
    /// </summary>
    /// <param name="ext"></param>
    /// <returns>
    /// The modified path information.
    /// On Windows, if ext is null or empty, the path information is unchanged.
    /// If the extension is null, the returned path is with the extension removed.
    /// If the path has no extension, and the extension is not null, the returned string contains the extension appended to the end of the path. 
    /// </returns>
    public TFile ChangeExtension( MimeType ext );

    /// <summary>
    /// Changes the extension of the file.
    /// </summary>
    /// <param name="ext"></param>
    /// <returns>
    /// The modified path information.
    /// On Windows, if ext is null or empty, the path information is unchanged.
    /// If the extension is null, the returned path is with the extension removed.
    /// If the path has no extension, and the extension is not null, the returned string contains the extension appended to the end of the path. 
    /// </returns>
    public TFile ChangeExtension( string? ext );

    /// <summary>
    /// Copies this to the file
    /// </summary>
    /// <param name="newFile"></param>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <returns></returns>
    public Task Clone( TFile newFile );

    /// <summary>
    /// Creates an <see cref="UriKind.Absolute"/> based on the detected <see cref="Mime"/>
    /// </summary>
    /// <param name="mime">To override the detected <see cref="Mime"/>, provide a non-null value</param>
    /// <returns></returns>
    public Uri ToUri( MimeType? mime = null );

    /// <summary>
    /// Creates a <see cref="Uri"/> using provided prefix, and <see cref="HttpUtility.UrlEncode(string)"/> to encode the <see cref="FullPath"/>.
    /// </summary>
    /// <param name="baseUri"></param>
    /// <param name="prefix">The key to attach the <see cref="FullPath"/> to. Defaults to "?path="</param>
    /// <param name="mime">To override the detected <see cref="Mime"/>, provide a non-null value</param>
    /// <returns></returns>
    public Uri ToUri( Uri baseUri, in string prefix = "?path=", MimeType? mime = null );

    public bool Equals( object obj );
    public int  GetHashCode();


    /// <summary>
    /// Copies this file to the <paramref name="newFile"/>
    /// </summary>
    /// <param name="newFile"></param>
    /// <returns></returns>
    public Task Clone( LocalFile newFile );

    /// <summary>
    /// Moves this file to the new <paramref name="path"/>
    /// </summary>
    /// <param name="path"></param>
    public void Move( string path );

    /// <summary>
    /// Moves this file to the new <paramref name="file"/> location
    /// </summary>
    /// <param name="file"></param>
    public void Move( LocalFile file );


    /// <summary>
    /// Calculates a file hash using <see cref="MD5"/>
    /// </summary>
    /// <returns><see cref="string"/></returns>
    public string Hash_MD5();

    /// <summary>
    /// Calculates a file hash using <see cref="SHA1"/>
    /// </summary>
    /// <returns><see cref="string"/></returns>
    public string Hash_SHA1();

    /// <summary>
    /// Calculates a file hash using <see cref="SHA256"/>
    /// </summary>
    /// <returns><see cref="string"/></returns>
    public string Hash_SHA256();

    /// <summary>
    /// Calculates a file hash using <see cref="SHA384"/>
    /// </summary>
    /// <returns><see cref="string"/></returns>
    public string Hash_SHA384();

    /// <summary>
    /// Calculates a file hash using <see cref="SHA512"/>
    /// </summary>
    /// <returns><see cref="string"/></returns>
    public string Hash_SHA512();


#region Openers

    /// <summary>
    /// 
    /// <seealso href="https://stackoverflow.com/a/11541330/9530917"/>
    /// </summary>
    /// <param name="mode"></param>
    /// <param name="access"></param>
    /// <param name="share"></param>
    /// <param name="bufferSize"></param>
    /// <param name="useAsync"></param>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="SecurityException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="PathTooLongException"></exception>
    /// <returns><see cref="FileStream"/></returns>
    public FileStream Open( FileMode   mode,
                            FileAccess access,
                            FileShare  share,
                            int        bufferSize = 4096,
                            bool       useAsync   = true );

    /// <summary>
    /// Opens file for read only actions. If it doesn't exist, <see cref="FileNotFoundException"/> will be raised.
    /// </summary>
    /// <param name="bufferSize"></param>
    /// <param name="useAsync"></param>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="SecurityException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="PathTooLongException"></exception>
    /// <returns><see cref="FileStream"/></returns>
    public FileStream OpenRead( int bufferSize = 4096, bool useAsync = true );

    /// <summary>
    /// Opens file for write only actions. If it doesn't exist, file will be created.
    /// </summary>
    /// <param name="mode"></param>
    /// <param name="bufferSize"></param>
    /// <param name="useAsync"></param>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="IOException"></exception>
    /// <exception cref="SecurityException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="PathTooLongException"></exception>
    /// <returns><see cref="FileStream"/></returns>
    public FileStream OpenWrite( FileMode mode, int bufferSize = 4096, bool useAsync = true );

#endregion


#region Read

    /// <summary>
    /// Reads the contents of the file as a <see cref="string"/>, then calls <see cref="JsonExtensions.FromJson{TResult}(string)"/> on it, asynchronously.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="encoding">Defaults to <see cref="Encoding.Default"/></param>
    /// <exception cref="NullReferenceException">if FullPath is null or empty</exception>
    /// <exception cref="FileNotFoundException">if file is not found</exception>
    /// <exception cref="JsonReaderException">if an error in deserialization occurs</exception>
    /// <returns><typeparamref name="T"/></returns>
    public async Task<T> ReadFromFileAsync<T>( Encoding? encoding = default ) where T : class
    {
        string content = await ReadFromFileAsync(encoding).ConfigureAwait(false);

        return content.FromJson<T>();
    }

    /// <summary>
    /// Reads the contents of the file as a <see cref="string"/>, asynchronously.
    /// </summary>
    /// <param name="encoding"></param>
    /// <exception cref="NullReferenceException">if FullPath is null or empty</exception>
    /// <exception cref="FileNotFoundException">if file is not found</exception>
    /// <returns><see cref="string"/></returns>
    public Task<string> ReadFromFileAsync( Encoding? encoding = default );

    /// <summary>
    /// Reads the contents of the file as a <see cref="string"/>.
    /// </summary>
    /// <param name="encoding">Defaults to <see cref="Encoding.Default"/></param>
    /// <exception cref="NullReferenceException">if FullPath is null or empty</exception>
    /// <exception cref="FileNotFoundException">if file is not found</exception>	'
    /// <returns><see cref="string"/></returns>
    public string ReadFromFile( Encoding? encoding = default );

    /// <summary>
    /// Reads the contents of the file as a <see cref="ReadOnlySpan{byte}"/>.
    /// </summary>
    /// <param name="encoding">Defaults to <see cref="Encoding.Default"/></param>
    /// <exception cref="NullReferenceException">if FullPath is null or empty</exception>
    /// <exception cref="FileNotFoundException">if file is not found</exception>
    /// <returns><see cref="ReadOnlySpan{byte}"/></returns>
    public ReadOnlySpan<char> ReadFromFileAsSpan( Encoding? encoding = default );

    /// <summary>
    /// Reads the contents of the file as a <see cref="ReadOnlyMemory{byte}"/>, asynchronously.
    /// </summary>
    /// <param name="token"></param>
    /// <exception cref="NullReferenceException">if FullPath is null or empty</exception>
    /// <exception cref="FileNotFoundException">if file is not found</exception>
    /// <returns><see cref="ReadOnlyMemory{byte}"/></returns>
    public Task<ReadOnlyMemory<byte>> RawReadFromFileAsync( CancellationToken token = default );

    /// <summary>
    /// Reads the contents of the file as a <see cref="ReadOnlyMemory{byte}"/>.
    /// </summary>
    /// <exception cref="NullReferenceException">if FullPath is null or empty</exception>
    /// <exception cref="FileNotFoundException">if file is not found</exception>
    /// <returns><see cref="ReadOnlyMemory{byte}"/></returns>
    public ReadOnlyMemory<byte> RawReadFromFile();

    /// <summary>
    /// Reads the contents of the file as a byte array.
    /// </summary>
    /// <exception cref="NullReferenceException">if FullPath is null or empty</exception>
    /// <exception cref="FileNotFoundException">if file is not found</exception>
    /// <returns><see cref="byte[]"/></returns>
    public byte[] RawReadFromFileAsBytes();

    /// <summary>
    /// Reads the contents of the file as a byte array.
    /// </summary>
    /// <exception cref="NullReferenceException">if FullPath is null or empty</exception>
    /// <exception cref="FileNotFoundException">if file is not found</exception>
    /// <returns><see cref="byte[]"/></returns>
    public Task<byte[]> RawReadFromFileAsBytesAsync( CancellationToken token = default );

#endregion


#region Write

    /// <summary>
    /// Write the <paramref name="payload"/> to the file.
    /// </summary>
    /// <param name="payload">the data being written to the file</param>
    /// <param name="encoding"></param>
    /// <param name="mode">Defaults to <see cref="FileMode.Create"/></param>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <returns><see cref="Task"/></returns>
    public void WriteToFile( StringBuilder payload, Encoding? encoding = default, FileMode mode = FileMode.Create );

    /// <summary>
    /// Write the <paramref name="payload"/> to the file.
    /// </summary>
    /// <param name="payload">the data being written to the file</param>
    /// <param name="encoding"></param>
    /// <param name="mode">Defaults to <see cref="FileMode.Create"/></param>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <returns><see cref="Task"/></returns>
    public Task WriteToFileAsync( StringBuilder payload, Encoding? encoding = default, FileMode mode = FileMode.Create );


    /// <summary>
    /// Write the <paramref name="payload"/> to the file.
    /// </summary>
    /// <param name="payload">the data being written to the file</param>
    /// <param name="encoding">Defaults to <see cref="Encoding.Default"/></param>
    /// <param name="mode">Defaults to <see cref="FileMode.Create"/></param>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <returns><see cref="Task"/></returns>
    public Task WriteToFileAsync( ReadOnlyMemory<char> payload, Encoding? encoding = default, FileMode mode = FileMode.Create );


    /// <summary>
    /// Write the <paramref name="payload"/> to the file.
    /// </summary>
    /// <param name="payload">the data being written to the file</param>
    /// <param name="encoding">Defaults to <see cref="Encoding.Default"/></param>
    /// <param name="mode">Defaults to <see cref="FileMode.Create"/></param>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <returns><see cref="Task"/></returns>
    public void WriteToFile( string payload, Encoding? encoding = default, FileMode mode = FileMode.Create );

    /// <summary>
    /// Write the <paramref name="payload"/> to the file.
    /// </summary>
    /// <param name="payload">the data being written to the file</param>
    /// <param name="encoding">Defaults to <see cref="Encoding.Default"/></param>
    /// <param name="mode">Defaults to <see cref="FileMode.Create"/></param>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <returns><see cref="Task"/></returns>
    public Task WriteToFileAsync( string payload, Encoding? encoding = default, FileMode mode = FileMode.Create );


    /// <summary>
    /// Write the <paramref name="payload"/> to the file.
    /// </summary>
    /// <param name="payload">the data being written to the file</param>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <returns><see cref="Task"/></returns>
    public void WriteToFile( byte[] payload );

    /// <summary>
    /// Write the <paramref name="payload"/> to the file.
    /// </summary>
    /// <param name="payload">the data being written to the file</param>
    /// <param name="token"></param>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <returns><see cref="Task"/></returns>
    public Task WriteToFileAsync( byte[] payload, CancellationToken token = default );


    /// <summary>
    /// Write the <paramref name="payload"/> to the file.
    /// </summary>
    /// <param name="payload">the data being written to the file</param>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <returns><see cref="Task"/></returns>
    public void WriteToFile( ReadOnlySpan<byte> payload );

    /// <summary>
    /// Write the <paramref name="payload"/> to the file.
    /// </summary>
    /// <param name="payload">the data being written to the file</param>
    /// <param name="token"></param>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <returns><see cref="Task"/></returns>
    public Task WriteToFileAsync( ReadOnlyMemory<byte> payload, CancellationToken token = default );


    /// <summary>
    /// Write the <paramref name="payload"/> to the file.
    /// </summary>
    /// <param name="payload">the data being written to the file</param>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <returns><see cref="Task"/></returns>
    public void WriteToFile( Stream payload );

    /// <summary>
    /// Write the <paramref name="payload"/> to the file.
    /// </summary>
    /// <param name="payload">the data being written to the file</param>
    /// <param name="token"></param>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="FileNotFoundException"></exception>
    /// <returns><see cref="Task"/></returns>
    public Task WriteToFileAsync( Stream payload, CancellationToken token = default );

#endregion
}
