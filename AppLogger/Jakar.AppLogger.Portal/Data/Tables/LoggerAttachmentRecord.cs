// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 10/04/2022  5:57 PM

namespace Jakar.AppLogger.Portal.Data.Tables;


[Serializable, Table( "Attachments" )]
public sealed record LoggerAttachmentRecord : LoggerTable<LoggerAttachmentRecord>, ILoggerAttachment, ILogInfo
{
    public                                                  Guid    AppID       { get; init; }
    [MaxLength( LoggerAttachment.MAX_SIZE )]         public string  Content     { get; init; } = string.Empty;
    [MaxLength( LoggerAttachment.DESCRIPTION_SIZE )] public string? Description { get; init; }
    public                                                  Guid    DeviceID    { get; init; }
    [MaxLength( LoggerAttachment.FILE_NAME_SIZE )] public   string? FileName    { get; init; }
    public                                                  bool    IsBinary    { get; init; }
    public                                                  long    Length      { get; init; }
    public                                                  Guid    LogID       { get; init; }
    public                                                  Guid    ScopeID     { get; init; }
    public                                                  Guid?   SessionID   { get; init; }
    [MaxLength( LoggerAttachment.TYPE_SIZE )] public        string? Type        { get; init; }


    public LoggerAttachmentRecord() : base() { }
    public LoggerAttachmentRecord( LoggerAttachment attachment, ILogInfo info, UserRecord? caller = default ) : base( caller )
    {
        if ( attachment.Length > LoggerAttachment.MAX_SIZE ) { LoggerAttachment.ThrowTooLong( attachment.Length ); }

        Length      = attachment.Length;
        Content     = attachment.Content;
        Description = attachment.Description;
        Type        = attachment.Type;
        FileName    = attachment.FileName;
        IsBinary    = attachment.IsBinary;
        LogID       = info.LogID;
        AppID       = info.AppID;
        SessionID   = info.SessionID;
        DeviceID    = info.DeviceID;

        // ScopeIDs = info.ScopeIDs;
    }


    public static IEnumerable<LoggerAttachmentRecord> Create( LogRecord record, AppLog log, UserRecord caller )
    {
        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach ( LoggerAttachment attachment in log.Attachments ) { yield return new LoggerAttachmentRecord( attachment, record, caller ); }
    }


    public static DynamicParameters GetDynamicParameters( LoggerAttachment attachment )
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
        return parameters;
    }


    public byte[]? GetData() => IsBinary
                                    ? Convert.FromBase64String( Content )
                                    : default;

    public LoggerAttachment ToLoggerAttachment() => new(this);
    public LoggerAttachmentRecord Update( LoggerAttachment attachment ) => this with
                                                                           {
                                                                               Length = attachment.Length,
                                                                               Content = attachment.Content,
                                                                               Description = attachment.Description,
                                                                               Type = attachment.Type,
                                                                               FileName = attachment.FileName,
                                                                               IsBinary = attachment.IsBinary,
                                                                           };


    public override bool Equals( LoggerAttachmentRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return string.Equals( Content, other.Content, StringComparison.Ordinal );
    }
    public override int CompareTo( LoggerAttachmentRecord? other ) => string.CompareOrdinal( Content, other?.Content );
    public override int GetHashCode() => HashCode.Combine( Content, base.GetHashCode() );
}



[Serializable, Table( "LogAttachments" )]
public sealed record LoggerAttachmentMappingRecord : Mapping<LoggerAttachmentMappingRecord, LogRecord, LoggerAttachmentRecord>, ICreateMapping<LoggerAttachmentMappingRecord, LogRecord, LoggerAttachmentRecord>
{
    public LoggerAttachmentMappingRecord( LogRecord               key, LoggerAttachmentRecord value ) : base( key, value ) { }
    public static LoggerAttachmentMappingRecord Create( LogRecord key, LoggerAttachmentRecord value ) => new(key, value);
}
