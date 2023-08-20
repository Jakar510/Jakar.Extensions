// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 10/04/2022  5:57 PM

namespace Jakar.AppLogger.Portal.Data.Tables;


[Serializable]
[Table( "Attachments" )]
public sealed record AttachmentRecord : LoggerTable<AttachmentRecord>, IAttachment, ILogInfo
{
    public                                            Guid    AppID       { get; init; }
    [MaxLength( Attachment.MAX_SIZE )]         public string  Content     { get; init; } = string.Empty;
    [MaxLength( Attachment.DESCRIPTION_SIZE )] public string? Description { get; init; }
    public                                            Guid    DeviceID    { get; init; }
    [MaxLength( Attachment.FILE_NAME_SIZE )] public   string? FileName    { get; init; }
    public                                            bool    IsBinary    { get; init; }
    public                                            long    Length      { get; init; }
    public                                            Guid    LogID       { get; init; }
    public                                            Guid?   ScopeID     { get; init; }
    public                                            Guid?   SessionID   { get; init; }
    [MaxLength( Attachment.TYPE_SIZE )] public        string? Type        { get; init; }


    public AttachmentRecord() : base() { }
    public AttachmentRecord( Attachment attachment, ILogInfo info, UserRecord? caller = default ) : base( Guid.NewGuid(), caller )
    {
        if ( attachment.Length > Attachment.MAX_SIZE ) { Attachment.ThrowTooLong(); }

        Length      = attachment.Length;
        Content     = attachment.Content;
        Description = attachment.Description;
        Type        = attachment.Type;
        FileName    = attachment.FileName;
        IsBinary    = attachment.IsBinary;
        LogID       = info.LogID;
        AppID       = info.AppID;
        SessionID   = info.SessionID;
        ScopeID     = info.ScopeID;
        DeviceID    = info.DeviceID;
    }

    public static DynamicParameters GetDynamicParameters( Attachment attachment )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(Content), attachment.Content );
        return parameters;
    }
    public static DynamicParameters GetDynamicParameters( ILogInfo info )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(LogID),     info.LogID );
        parameters.Add( nameof(AppID),     info.AppID );
        parameters.Add( nameof(SessionID), info.SessionID );
        parameters.Add( nameof(ScopeID),   info.ScopeID );
        return parameters;
    }


    public byte[]? GetData() => IsBinary
                                    ? Convert.FromBase64String( Content )
                                    : default;

    public Attachment ToAttachment() => new(this);
    public AttachmentRecord Update( Attachment attachment ) => this with
                                                               {
                                                                   Length = attachment.Length,
                                                                   Content = attachment.Content,
                                                                   Description = attachment.Description,
                                                                   Type = attachment.Type,
                                                                   FileName = attachment.FileName,
                                                                   IsBinary = attachment.IsBinary,
                                                               };


    public override bool Equals( AttachmentRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return string.Equals( Content, other.Content, StringComparison.Ordinal );
    }
    public override int CompareTo( AttachmentRecord? other ) => string.CompareOrdinal( Content, other?.Content );
    public override int GetHashCode() => HashCode.Combine( Content, base.GetHashCode() );
}
