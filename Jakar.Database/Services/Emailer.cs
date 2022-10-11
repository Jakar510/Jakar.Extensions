using System.Net.Mail;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;



namespace Jakar.Database;


public sealed class EmailSenderOptions : IOptions<EmailSenderOptions>
{
    private EmailSettings? _settings;


    EmailSenderOptions IOptions<EmailSenderOptions>.Value    => this;
    public EmailSettings?                           Settings { get; set; }
    public MailboxAddress?                          Sender   { get; set; }

    public string  DefaultSubject { get; set; } = string.Empty;
    public string? VerifySubject  { get; set; }


    public EmailSenderOptions() { }
    public EmailSenderOptions( string defaultSubject ) => DefaultSubject = defaultSubject;


    internal MailboxAddress GetSender() => Sender ??= Settings?.Address() ?? throw new InvalidOperationException( $"{nameof(Sender)} is not set" );
    internal EmailSettings GetSettings( IConfiguration configuration ) => _settings ??= configuration.GetSection( nameof(EmailSettings) )
                                                                                                     .Get<EmailSettings>();
}



public class Emailer
{
    private readonly EmailSenderOptions _options;
    private readonly IConfiguration     _configuration;
    private readonly ILogger<Emailer>   _logger;
    private readonly ITokenService      _tokenService;
    private          EmailSettings      _Settings => _options.GetSettings( _configuration );


    public Emailer( IConfiguration configuration, ILogger<Emailer> logger, ITokenService tokenService, IOptions<EmailSenderOptions> options ) : this( configuration, logger, tokenService, options.Value ) { }
    internal Emailer( IConfiguration configuration, ILogger<Emailer> logger, ITokenService tokenService, EmailSenderOptions options )
    {
        _configuration = configuration;
        _tokenService  = tokenService;
        _logger        = logger;
        _options       = options;
    }


    public static Task SendMessageAsync( EmailSettings settings, MailboxAddress target, string subject, string body, CancellationToken token ) => SendMessageAsync( settings, target, Array.Empty<Attachment>(), subject, body, token );
    public static async Task SendMessageAsync( EmailSettings settings, MailboxAddress target, IEnumerable<Attachment> attachments, string subject, string body, CancellationToken token )
    {
        MimeMessage message = await EmailBuilder.From( settings.Address() )
                                                .To( target )
                                                .WithAttachments( attachments )
                                                .WithSubject( subject )
                                                .WithBody( body )
                                                .Create();

        await SendMessageAsync( settings, message, token );
    }
    public static Task SendMessageAsync( EmailSettings settings, IEnumerable<MailboxAddress> targets, string subject, string body, CancellationToken token ) => SendMessageAsync( settings, targets, Array.Empty<Attachment>(), subject, body, token );
    public static Task SendMessageAsync( EmailSettings settings, IEnumerable<MailboxAddress> targets, string subject, string body, CancellationToken token, params Attachment[] attachments ) =>
        SendMessageAsync( settings, targets, attachments, subject, body, token );
    public static async Task SendMessageAsync( EmailSettings settings, IEnumerable<MailboxAddress> targets, IEnumerable<Attachment> attachments, string subject, string body, CancellationToken token )
    {
        MimeMessage message = await EmailBuilder.From( settings.Address() )
                                                .To( targets )
                                                .WithAttachments( attachments )
                                                .WithSubject( subject )
                                                .WithBody( body )
                                                .Create();

        await SendMessageAsync( settings, message, token );
    }
    public static async Task SendMessageAsync( EmailSettings settings, MimeMessage message, CancellationToken token = default )
    {
        using var client = new SmtpClient();
        await client.ConnectAsync( settings.Site, settings.Port, settings.Options, token );

        try
        {
            if (client.Capabilities.HasFlag( SmtpCapabilities.Authentication )) { await client.AuthenticateAsync( settings.UserName, settings.Password, token ); }

            await client.SendAsync( message, token );
        }
        finally { await client.DisconnectAsync( true, token ); }
    }


    public Task SendEmailAsync( string? email, string subject, string body ) => SendEmailAsync( email, subject, body, default );
    public async Task SendEmailAsync( string? email, string subject, string body, CancellationToken token, params Attachment[] attachments )
    {
        MailboxAddress address = MailboxAddress.Parse( email );
        await SendEmailAsync( address, subject, body, token, attachments );
    }
    public async Task SendEmailAsync( MailboxAddress target, string subject, string body, CancellationToken token, params Attachment[] attachments )
    {
        try
        {
            MimeMessage message = await EmailBuilder.From( _options.GetSender() )
                                                    .To( target )
                                                    .WithAttachments( attachments )
                                                    .WithSubject( subject )
                                                    .WithBody( body )
                                                    .Create();

            await SendMessageAsync( message, token );
        }
        catch (Exception e)
        {
            _logger.LogError( e,
                              "{ClassName}.'{Caller}'",
                              GetType()
                                 .Name,
                              nameof(SendEmailAsync) );
        }
    }


    private Task SendMessageAsync( MimeMessage message, CancellationToken token = default ) => SendMessageAsync( _Settings, message, token );


    public async Task VerifyEmail( UserRecord user, CancellationToken token, params Attachment[] attachments )
    {
        string header  = _options.VerifySubject ?? _options.DefaultSubject;
        string content = await _tokenService.CreateContent( header, user, token );
        await SendEmailAsync( user.Email, header, content, token, attachments );
    }
}
