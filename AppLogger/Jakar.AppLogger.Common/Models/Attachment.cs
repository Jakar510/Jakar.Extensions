﻿// Jakar.AppLogger :: Jakar.AppLogger.Common
// 09/08/2022  1:50 PM

namespace Jakar.AppLogger.Common;


public interface IAttachment
{
    public bool    IsBinary    { get; init; }
    public long    Length      { get; init; }
    public string  Content     { get; init; }
    public string? Description { get; init; }
    public string? FileName    { get; init; }
    public string? Type        { get; init; }
}



public sealed record Attachment : BaseRecord
{
    public const long MAX_SIZE = 2 ^ 20; // 1MB
    public       bool IsBinary { get; init; }


    [JsonIgnore]
    public byte[]? Data => IsBinary
                               ? Convert.FromBase64String( Content )
                               : default;


    public long    Length      { get; init; }
    public string  Content     { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? FileName    { get; init; }
    public string? Type        { get; init; }


    public Attachment() { }
    public Attachment( IAttachment attachment )
    {
        Length      = attachment.Length;
        Content     = attachment.Content;
        Description = attachment.Description;
        Type        = attachment.Type;
        FileName    = attachment.FileName;
        IsBinary    = attachment.IsBinary;
    }
    public Attachment( MemoryStream         content, string? description = default, string? type = default ) : this( content.ToArray(), description, type ) { }
    public Attachment( ReadOnlyMemory<byte> content, string? description = default, string? type = default ) : this( content.ToArray(), description, type ) { }
    public Attachment( byte[] content, string? description = default, string? type = default )
    {
        if (content.Length > MAX_SIZE) { ThrowTooLong(); }

        Length      = content.Length;
        Content     = Convert.ToBase64String( content );
        Description = description;
        Type        = type;
        IsBinary    = true;
    }
    public Attachment( string content, string? description = default, string? type = default )
    {
        if (content.Length > MAX_SIZE) { ThrowTooLong(); }

        Length      = content.Length;
        Content     = content;
        Description = description;
        Type        = type;
        IsBinary    = false;
    }


    [DoesNotReturn] public static void ThrowTooLong() => throw new ArgumentException( $"{nameof(Content)}.{nameof(Length)} is too long; Must be < {MAX_SIZE}." );


    public static Attachment Create( ReadOnlyMemory<byte> content, string? description = default, string? type = default ) => new(content, description, type);
    public static Attachment Create( string               content, string? description = default, string? type = default ) => new(content, description, type);
    public static Attachment Create( Stream stream, string? description = default, string? type = default )
    {
        using var ms = new MemoryStream();
        stream.CopyTo( ms );

        return new Attachment( ms, description, type );
    }
    public static async Task<Attachment> CreateAsync( Stream stream, string? description = default, string? type = default, CancellationToken token = default )
    {
        using var ms = new MemoryStream();
        await stream.CopyToAsync( ms, token );

        return new Attachment( ms, description, type );
    }
    public static Attachment Create( LocalFile file, string? description = default, string? type = default )
    {
        byte[] content = file.Read()
                             .AsBytes();

        return new Attachment( content, description ?? file.Name, type ?? file.ContentType )
               {
                   FileName = file.Name
               };
    }
    public static async Task<Attachment> CreateAsync( LocalFile file, string? description = default, string? type = default, CancellationToken token = default )
    {
        byte[] content = await file.ReadAsync()
                                   .AsBytes( token );

        return new Attachment( content, description ?? file.Name, type ?? file.ContentType )
               {
                   FileName = file.Name
               };
    }
}