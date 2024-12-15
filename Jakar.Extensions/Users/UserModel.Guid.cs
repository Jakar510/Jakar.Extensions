// Jakar.Extensions :: Jakar.Database
// 4/4/2024  10:1


// ReSharper disable CheckNamespace

namespace Jakar.Extensions.UserGuid;
// ReSharper restore CheckNamespace



[Serializable]
public sealed class UserAddress : UserAddress<UserAddress, Guid>, IAddress<UserAddress, Guid>
{
    public UserAddress() : base() { }
    public UserAddress( Match                        match ) : base( match ) { }
    public UserAddress( IAddress<Guid>               address ) : base( address ) { }
    public UserAddress( string                       line1, string line2, string city, string postalCode, string country ) : base( line1, line2, city, postalCode, country ) { }
    public static UserAddress Create( Match          match )                                                               => new(match);
    public static UserAddress Create( IAddress<Guid> address )                                                             => new(address);
    public static UserAddress Create( string         line1, string line2, string city, string postalCode, string country ) => new(line1, line2, city, postalCode, country);
    public static UserAddress Parse( string value, IFormatProvider? provider )
    {
        Match match = Validate.Re.Address.Match( value );
        return new UserAddress( match );
    }
    public static bool TryParse( string? value, IFormatProvider? provider, [NotNullWhen( true )] out UserAddress? result )
    {
        try
        {
            result = string.IsNullOrWhiteSpace( value ) is false
                         ? Parse( value, provider )
                         : null;

            return result is not null;
        }
        catch ( Exception )
        {
            result = null;
            return false;
        }
    }
}



[Serializable]
public sealed record GroupModel : GroupModel<GroupModel, Guid>, IGroupModel<GroupModel, Guid>
{
    public GroupModel( string                          NameOfGroup, Guid? OwnerID, Guid? CreatedBy, Guid ID, string Rights ) : base( NameOfGroup, OwnerID, CreatedBy, ID, Rights ) { }
    public GroupModel( IGroupModel<Guid>               model ) : base( model ) { }
    public static GroupModel Create( IGroupModel<Guid> model ) => new(model);
}



[Serializable]
public sealed record RoleModel : RoleModel<RoleModel, Guid>, IRoleModel<RoleModel, Guid>
{
    public RoleModel( string                         NameOfRole, string Rights, Guid ID ) : base( NameOfRole, Rights, ID ) { }
    public RoleModel( IRoleModel<Guid>               model ) : base( model ) { }
    public static RoleModel Create( IRoleModel<Guid> model ) => new(model);
}



[Serializable]
public sealed class UserModel : UserModel<UserModel, Guid, UserAddress, GroupModel, RoleModel>, ICreateUserModel<UserModel, Guid, UserAddress, GroupModel, RoleModel>
{
    public UserModel() : base() { }
    public UserModel( IUserData<Guid> value ) : base( value ) { }
    public UserModel( string          firstName, string lastName ) : base( firstName, lastName ) { }


    public static UserModel Create( IUserData<Guid> model )                                                                                                                                    => new(model);
    public static UserModel Create( IUserData<Guid> model, IEnumerable<UserAddress>            addresses, IEnumerable<GroupModel>            groups, IEnumerable<RoleModel>            roles ) => Create( model ).With( addresses ).With( groups ).With( roles );
    public static UserModel Create( IUserData<Guid> model, scoped in ReadOnlySpan<UserAddress> addresses, scoped in ReadOnlySpan<GroupModel> groups, scoped in ReadOnlySpan<RoleModel> roles ) => Create( model ).With( addresses ).With( groups ).With( roles );
    public static async ValueTask<UserModel> CreateAsync( IUserData<Guid> model, IAsyncEnumerable<UserAddress> addresses, IAsyncEnumerable<GroupModel> groups, IAsyncEnumerable<RoleModel> roles, CancellationToken token = default )
    {
        UserModel user = Create( model );
        await user.Addresses.Add( addresses, token );
        await user.Groups.Add( groups, token );
        await user.Roles.Add( roles, token );
        return user;
    }
}



[Serializable]
public sealed record FileMetaData( string? FileName, string? FileType, string? FileDescription = null, Guid ID = default ) : FileMetaData<FileMetaData, Guid>( FileName, FileType, FileDescription, ID ), IFileMetaData<FileMetaData, Guid>
{
    public FileMetaData( IFileMetaData<Guid>               file ) : this( file.FileName, file.FileType, file.FileDescription, file.ID ) { }
    public FileMetaData( LocalFile                         file ) : this( file.Name, file.ContentType ) { }
    public static FileMetaData Create( IFileMetaData<Guid> data ) => new(data);
    public static FileMetaData? TryCreate( IFileMetaData<Guid>? data ) => data is not null
                                                                              ? Create( data )
                                                                              : null;
}



[Serializable]
public sealed record FileData( MimeType MimeType, long FileSize, string Hash, string Payload, FileMetaData? MetaData, Guid ID = default ) : FileData<FileData, Guid, FileMetaData>( MimeType, FileSize, Hash, Payload, MetaData, ID ), IFileData<FileData, Guid, FileMetaData>
{
    public FileData( IFileData<Guid, FileMetaData> file ) : this( file, file.MetaData ) { }
    public FileData( IFileData<Guid>               file,    FileMetaData? metaData ) : this( file.MimeType, file.FileSize, file.Hash, file.Payload, metaData ) { }
    public FileData( scoped in ReadOnlySpan<byte>  content, MimeType      mime, FileMetaData? metaData ) : this( mime, content.Length, content.GetHash(), Convert.ToBase64String( content ), metaData ) { }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static FileData Create( string                        data,   MimeType mime, FileMetaData? metaData, Encoding? encoding = null ) => new(mime, data.Length, Hashes.Hash_SHA256( data, encoding ?? Encoding.Default ), data, metaData);
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static FileData Create( MemoryStream                  stream, MimeType mime, FileMetaData? metaData ) => Create( stream.AsReadOnlyMemory(), mime, metaData );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static FileData Create( ReadOnlyMemory<byte>          data,   MimeType mime, FileMetaData? metaData ) => Create( data.Span,                 mime, metaData );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static FileData Create( scoped in ReadOnlySpan<byte>  data,   MimeType mime, FileMetaData? metaData ) => new(data, mime, metaData);
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static FileData Create( IFileData<Guid, FileMetaData> data ) => new(data, data.MetaData);


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static FileData? TryCreate( [NotNullIfNotNull( nameof(data) )] IFileData<Guid, FileMetaData>? data ) => data is not null
                                                                                                                       ? Create( data )
                                                                                                                       : null;
    public static async ValueTask<FileData> Create( LocalFile file, CancellationToken token = default )
    {
        ReadOnlyMemory<byte> content = await file.ReadAsync().AsBytes( token );
        return new FileData( content.Span, file.Mime, new FileMetaData( null, file.Name, file.ContentType ) );
    }
    public static async ValueTask<FileData> Create( Stream stream, MimeType mime, FileMetaData? metaData, CancellationToken token = default )
    {
        stream.Seek( 0, SeekOrigin.Begin );
        using MemoryStream memory = new((int)stream.Length);
        await stream.CopyToAsync( memory, token );
        return Create( memory, mime, metaData );
    }
}
