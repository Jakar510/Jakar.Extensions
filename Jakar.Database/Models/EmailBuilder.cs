namespace Jakar.Database;


/// <summary>
///     <see href="http://www.mimekit.net/docs/html/Creating-Messages.htm"/>
/// </summary>
public sealed class EmailBuilder
{
    private readonly List<Attachment>     __attachments = new(Buffers.DEFAULT_CAPACITY);
    private readonly List<MailboxAddress> __recipients  = new(Buffers.DEFAULT_CAPACITY);
    private readonly MailboxAddress[]     __senders;
    private          string               __subject = string.Empty;
    private          string?              __body;
    private          string?              __html;


    private EmailBuilder( MailboxAddress[] senders ) => __senders = senders;


    public static EmailBuilder From( params MailboxAddress[] senders ) => new(senders);


    public EmailBuilder To( MailboxAddress recipient )
    {
        __recipients.Add( recipient );
        return this;
    }
    public EmailBuilder To( IEnumerable<MailboxAddress> recipients )
    {
        __recipients.AddRange( recipients );
        return this;
    }
    public EmailBuilder To( params ReadOnlySpan<MailboxAddress> recipients )
    {
        __recipients.AddRange( recipients );
        return this;
    }


    public EmailBuilder WithAttachment( Attachment attachment )
    {
        __attachments.Add( attachment );
        return this;
    }
    public EmailBuilder WithAttachment( IEnumerable<Attachment> attachments )
    {
        __attachments.AddRange( attachments );
        return this;
    }
    public EmailBuilder WithAttachment( params ReadOnlySpan<Attachment> attachments )
    {
        __attachments.AddRange( attachments );
        return this;
    }


    public EmailBuilder WithBody( string? body )
    {
        __body = body;
        return this;
    }
    public EmailBuilder WithHTML( string? html )
    {
        __html = html;
        return this;
    }


    public EmailBuilder WithSubject( string subject )
    {
        __subject = subject;
        return this;
    }


    public async ValueTask<MimeMessage> Create()
    {
        BodyBuilder builder = new()
                              {
                                  TextBody = __body,
                                  HtmlBody = __html
                              };

        foreach ( Attachment element in __attachments ) { await builder.Attachments.AddAsync( element.Name, element.ContentStream ); }

        return new MimeMessage( __senders, __recipients, __subject, builder.ToMessageBody() );
    }
}
