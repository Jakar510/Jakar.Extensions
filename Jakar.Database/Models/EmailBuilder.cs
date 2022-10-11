#nullable enable
using System.Net.Mail;
using MimeKit;



namespace Jakar.Database;


/// <summary>
///     <see href = "http://www.mimekit.net/docs/html/Creating-Messages.htm" />
/// </summary>
public sealed class EmailBuilder : EmailBuilder.IToSenderSyntax, EmailBuilder.IWithAttachmentsSyntax, EmailBuilder.IBodySyntax, EmailBuilder.ICreatorSyntax, EmailBuilder.ISubjectSyntax
{
    private readonly List<Attachment>     _attachments = new();
    private readonly List<MailboxAddress> _recipients  = new();


    private readonly MailboxAddress[] _senders;
    private          string           _subject = string.Empty;
    private          string?          _body;
    private          string?          _html;


    private EmailBuilder( MailboxAddress[] senders ) => _senders = senders;


    public static IToSenderSyntax From( params MailboxAddress[] senders ) => new EmailBuilder( senders );


    public ICreatorSyntax WithBody( string body )
    {
        _body = body;
        return this;
    }
    public ICreatorSyntax WithHTML( string html )
    {
        _html = html;
        return this;
    }


    public async ValueTask<MimeMessage> Create()
    {
        var builder = new BodyBuilder
                      {
                          TextBody = _body,
                          HtmlBody = _html
                      };

        foreach (Attachment element in _attachments) { await builder.Attachments.AddAsync( element.Name, element.ContentStream ); }

        return new MimeMessage( _senders, _recipients, _subject, builder.ToMessageBody() );
    }


    public IBodySyntax WithSubject( string subject )
    {
        _subject = subject;
        return this;
    }


    public IWithAttachmentsSyntax To( MailboxAddress recipient )
    {
        _recipients.Add( recipient );
        return this;
    }
    public IWithAttachmentsSyntax To( IEnumerable<MailboxAddress> recipients )
    {
        _recipients.AddRange( recipients );
        return this;
    }
    public IWithAttachmentsSyntax To( params MailboxAddress[] recipients )
    {
        _recipients.AddRange( recipients );
        return this;
    }


    public ISubjectSyntax WithAttachments( Attachment attachment )
    {
        _attachments.Add( attachment );
        return this;
    }
    public ISubjectSyntax WithAttachments( IEnumerable<Attachment> attachments )
    {
        _attachments.AddRange( attachments );
        return this;
    }
    public ISubjectSyntax WithAttachments( params Attachment[] attachments )
    {
        _attachments.AddRange( attachments );
        return this;
    }



    public interface IFromSyntax
    {
        public IToSenderSyntax From( params MailboxAddress[] senders );
    }



    public interface IToSenderSyntax
    {
        public IWithAttachmentsSyntax To( MailboxAddress              recipient );
        public IWithAttachmentsSyntax To( IEnumerable<MailboxAddress> recipients );
        public IWithAttachmentsSyntax To( params MailboxAddress[]     recipients );
    }



    public interface IWithAttachmentsSyntax
    {
        public ISubjectSyntax WithAttachments( Attachment              attachment );
        public ISubjectSyntax WithAttachments( IEnumerable<Attachment> attachments );
        public ISubjectSyntax WithAttachments( params Attachment[]     attachments );
    }



    public interface ISubjectSyntax
    {
        public IBodySyntax WithSubject( string body );
    }



    public interface IBodySyntax
    {
        public ICreatorSyntax WithBody( string body );
        public ICreatorSyntax WithHTML( string html );
    }



    public interface ICreatorSyntax
    {
        public ValueTask<MimeMessage> Create();
    }
}
