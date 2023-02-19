// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 10/04/2022  5:57 PM

using Jakar.AppLogger.Portal.Data.Interfaces;



namespace Jakar.AppLogger.Portal.Data.Tables;


[Serializable]
[Table( "Attachments" )]
public sealed record AttachmentRecord : LoggerTable<AttachmentRecord>, IAttachment, ILogInfo
{
    public                       bool    IsBinary    { get; init; }
    public                       Guid    SessionID   { get; init; }
    public                       Guid?   ScopeID     { get; init; }
    public                       string  AppID       { get; init; } = string.Empty;
    public                       string  DeviceID    { get; init; } = string.Empty;
    public                       long    Length      { get; init; }
    public                       string  LogID       { get; init; } = string.Empty;
    [MaxLength( 2 ^ 20 )] public string  Content     { get; init; } = string.Empty;
    [MaxLength( 1024 )]   public string? Description { get; init; }
    [MaxLength( 256 )]    public string? FileName    { get; init; }
    [MaxLength( 256 )]    public string? Type        { get; init; }


    public AttachmentRecord() : base() { }
    public AttachmentRecord( Attachment attachment, ILogInfo info ) : base( Guid.NewGuid() )
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
    public AttachmentRecord( Attachment attachment, ILogInfo info, UserRecord caller ) : base( caller )
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


    public Attachment ToAttachment() => new(this);
    public AttachmentRecord Update( Attachment attachment ) => this with
                                                               {
                                                                   Length = attachment.Length,
                                                                   Content = attachment.Content,
                                                                   Description = attachment.Description,
                                                                   Type = attachment.Type,
                                                                   FileName = attachment.FileName,
                                                                   IsBinary = attachment.IsBinary
                                                               };


    public override int CompareTo( AttachmentRecord? other ) => string.CompareOrdinal( Content, other?.Content );
    public override int GetHashCode() => HashCode.Combine( Content, base.GetHashCode() );
    public override bool Equals( AttachmentRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return string.Equals( Content, other.Content, StringComparison.Ordinal );
    }
}
