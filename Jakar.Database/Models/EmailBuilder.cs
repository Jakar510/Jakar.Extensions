namespace Jakar.Database;


/// <summary>
///     <see href="http://www.mimekit.net/docs/html/Creating-Messages.htm"/>
/// </summary>
public sealed class EmailBuilder
{
    private readonly List<Attachment>     _attachments = new();
    private readonly List<MailboxAddress> _recipients  = new();


    private readonly MailboxAddress[] _senders;
    private          string           _subject = string.Empty;
    private          string?          _body;
    private          string?          _html;


    private EmailBuilder( MailboxAddress[] senders ) => _senders = senders;


    public static EmailBuilder From( params MailboxAddress[] senders ) => new(senders);


    public EmailBuilder To( MailboxAddress recipient )
    {
        _recipients.Add( recipient );
        return this;
    }
    public EmailBuilder To( IEnumerable<MailboxAddress> recipients )
    {
        _recipients.AddRange( recipients );
        return this;
    }
    public EmailBuilder To( params MailboxAddress[] recipients )
    {
        _recipients.AddRange( recipients );
        return this;
    }


    public EmailBuilder WithAttachment( Attachment attachment )
    {
        _attachments.Add( attachment );
        return this;
    }
    public EmailBuilder WithAttachment( IEnumerable<Attachment> attachments )
    {
        _attachments.AddRange( attachments );
        return this;
    }
    public EmailBuilder WithAttachment( params Attachment[] attachments )
    {
        _attachments.AddRange( attachments );
        return this;
    }


    public EmailBuilder WithBody( string? body )
    {
        _body = body;
        return this;
    }
    public EmailBuilder WithHTML( string? html )
    {
        _html = html;
        return this;
    }


    public EmailBuilder WithSubject( string subject )
    {
        _subject = subject;
        return this;
    }


    public async ValueTask<MimeMessage> Create()
    {
        var builder = new BodyBuilder
                      {
                          TextBody = _body,
                          HtmlBody = _html
                      };

        foreach ( Attachment element in _attachments ) { await builder.Attachments.AddAsync( element.Name, element.ContentStream ); }

        return new MimeMessage( _senders, _recipients, _subject, builder.ToMessageBody() );
    }
}
