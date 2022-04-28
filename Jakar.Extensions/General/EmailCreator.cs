using System.Net.Mail;
using MimeKit;



namespace Jakar.Extensions.General;


/// <summary>
/// <see href="http://www.mimekit.net/docs/html/Creating-Messages.htm"/>
/// </summary>
public sealed class EmailBuilder : EmailBuilder.IToSenderSyntax, EmailBuilder.IWithAttachmentsSyntax, EmailBuilder.IBodySyntax, EmailBuilder.ICreatorSyntax, EmailBuilder.ISubjectSyntax
{
    public interface IFromSyntax
    {
        public IToSenderSyntax From( params MailboxAddress[] senders );
    }



    public interface IToSenderSyntax
    {
        public IWithAttachmentsSyntax To( params MailboxAddress[] recipients );
    }



    public interface IWithAttachmentsSyntax
    {
        public ISubjectSyntax WithAttachments( params Attachment[] attachments );
    }



    public interface ISubjectSyntax
    {
        public IBodySyntax WithSubject( string body );
    }



    public interface IBodySyntax
    {
        public ICreatorSyntax WithBody( string body );
    }



    public interface ICreatorSyntax
    {
        public Task<MimeMessage> Create();
    }



    private readonly MailboxAddress[] _senders;
    private          MailboxAddress[] _recipients;
    private          Attachment[]     _attachments;
    private          string           _subject = string.Empty;
    private          string           _body    = string.Empty;


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private EmailBuilder( MailboxAddress[] senders ) => _senders = senders;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


    public static IToSenderSyntax From( params MailboxAddress[] senders ) => new EmailBuilder(senders);

    public IWithAttachmentsSyntax To( params MailboxAddress[] recipients )
    {
        _recipients = recipients;
        return this;
    }

    public ISubjectSyntax WithAttachments( params Attachment[] attachments )
    {
        _attachments = attachments;
        return this;
    }

    public IBodySyntax WithSubject( string subject )
    {
        _subject = subject;
        return this;
    }

    public ICreatorSyntax WithBody( string body )
    {
        _body = body;
        return this;
    }

    public async Task<MimeMessage> Create()
    {
        var builder = new BodyBuilder
                      {
                          // Set the plain-text version of the message text
                          TextBody = _body
                      };

        foreach ( Attachment element in _attachments ) { await builder.Attachments.AddAsync(element.Name, element.ContentStream); }

        return new MimeMessage(_senders, _recipients, _subject, builder.ToMessageBody());
    }
}
