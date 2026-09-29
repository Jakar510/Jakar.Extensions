// #nullable enable
// using System.Net.Mail;
// using MimeKit;
//
//
//
// namespace Jakar.Extensions;
//
//
// /// <summary>
// /// <see href="http://www.mimekit.net/docs/html/Creating-Messages.htm"/>
// /// </summary>
// public sealed class EmailBuilder : EmailBuilder.IToSenderSyntax, EmailBuilder.IWithAttachmentsSyntax, EmailBuilder.IBodySyntax, EmailBuilder.ICreatorSyntax, EmailBuilder.ISubjectSyntax
// {
//     public interface IFromSyntax
//     {
//         public IToSenderSyntax From( params ReadOnlySpan<MailboxAddress> senders );
//     }
//
//
//
//     public interface IToSenderSyntax
//     {
//         public IWithAttachmentsSyntax To( MailboxAddress              recipient );
//         public IWithAttachmentsSyntax To( IEnumerable<MailboxAddress> recipients );
//         public IWithAttachmentsSyntax To( params ReadOnlySpan<MailboxAddress>     recipients );
//     }
//
//
//
//     public interface IWithAttachmentsSyntax
//     {
//         public ISubjectSyntax WithAttachments( Attachment              attachment );
//         public ISubjectSyntax WithAttachments( IEnumerable<Attachment> attachments );
//         public ISubjectSyntax WithAttachments( params ReadOnlySpan<Attachment> attachments );
//     }
//
//
//
//     public interface ISubjectSyntax
//     {
//         public IBodySyntax WithSubject( string body );
//     }
//
//
//
//     public interface IBodySyntax
//     {
//         public ICreatorSyntax WithBody( string body );
//     }
//
//
//
//     public interface ICreatorSyntax
//     {
//         public Task<MimeMessage> Create();
//     }
//
//
//
//     private readonly MailboxAddress[]     _senders;
//     private readonly List<MailboxAddress> _recipients  = new();
//     private readonly List<Attachment>     _attachments = new();
//     private          string               _subject     = EMPTY;
//     private          string               _body        = EMPTY;
//
//
// #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
//     private EmailBuilder( MailboxAddress[] senders ) => _senders = senders;
// #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
//
//
//     public static IToSenderSyntax From( params ReadOnlySpan<MailboxAddress> senders ) => new EmailBuilder(senders);
//
//
//     public IWithAttachmentsSyntax To( MailboxAddress recipient )
//     {
//         _recipients.Add(recipient);
//         return this;
//     }
//     public IWithAttachmentsSyntax To( IEnumerable<MailboxAddress> recipients )
//     {
//         _recipients.AddRange(recipients);
//         return this;
//     }
//     public IWithAttachmentsSyntax To( params ReadOnlySpan<MailboxAddress> recipients )
//     {
//         _recipients.AddRange(recipients);
//         return this;
//     }
//
//
//     public ISubjectSyntax WithAttachments( Attachment attachment )
//     {
//         _attachments.Add(attachment);
//         return this;
//     }
//     public ISubjectSyntax WithAttachments( IEnumerable<Attachment> attachments )
//     {
//         _attachments.AddRange(attachments);
//         return this;
//     }
//     public ISubjectSyntax WithAttachments( params ReadOnlySpan<Attachment> attachments )
//     {
//         _attachments.AddRange(attachments);
//         return this;
//     }
//
//
//     public IBodySyntax WithSubject( string subject )
//     {
//         _subject = subject;
//         return this;
//     }
//
//
//     public ICreatorSyntax WithBody( string body )
//     {
//         _body = body;
//         return this;
//     }
//
//
//     public async Task<MimeMessage> Create()
//     {
//         var builder = new BodyBuilder
//                       {
//                           // Set the plain-text version of the message text
//                           TextBody = _body
//                       };
//
//         foreach ( Attachment element in _attachments ) { await builder.Attachments.AddAsync(element.AppName, element.ContentStream); }
//
//         return new MimeMessage(_senders, _recipients, _subject, builder.ToMessageBody());
//     }
// }



