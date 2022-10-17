using System.Net.Mail;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;



namespace Jakar.Database;


[SuppressMessage( "ReSharper", "SuggestBaseTypeForParameterInConstructor" )]
public class Emailer
{
    private readonly IConfiguration _configuration;
    private readonly ILogger        _logger;
    private readonly ITokenService  _tokenService;
    private readonly Options        _options;
    private          EmailSettings  _Settings => _options.GetSettings( _configuration );


    public Emailer( IConfiguration configuration, ILogger<Emailer> logger, ITokenService tokenService, IOptions<Options> options ) : this( configuration, tokenService, options.Value, logger ) { }
    internal Emailer( IConfiguration configuration, ITokenService tokenService, Options options, ILogger logger )
    {
        _configuration = configuration;
        _tokenService  = tokenService;
        _logger        = logger;
        _options       = options;
    }
    public static Emailer Create( IConfiguration configuration, ITokenService tokenService, Options options, ILogger logger ) => new(configuration, tokenService, options, logger);


    public static ValueTask SendAsync( EmailSettings settings, MailboxAddress target, string subject, string body, CancellationToken token ) => SendAsync( settings, target, Array.Empty<Attachment>(), subject, body, token );
    public static ValueTask SendAsync( EmailSettings settings, MailboxAddress target, IEnumerable<Attachment> attachments, string subject, string body, CancellationToken token ) =>
        SendAsync( settings,
                   EmailBuilder.From( settings.Address() )
                               .To( target )
                               .WithAttachment( attachments )
                               .WithSubject( subject )
                               .WithBody( body ),
                   token );
    public static ValueTask SendAsync( EmailSettings settings, IEnumerable<MailboxAddress> targets, string subject, string body, CancellationToken token ) => SendAsync( settings, targets, Array.Empty<Attachment>(), subject, body, token );
    public static ValueTask SendAsync( EmailSettings settings, IEnumerable<MailboxAddress> targets, string subject, string body, CancellationToken token, params Attachment[] attachments ) =>
        SendAsync( settings, targets, attachments, subject, body, token );
    public static ValueTask SendAsync( EmailSettings settings, IEnumerable<MailboxAddress> targets, IEnumerable<Attachment> attachments, string subject, string body, CancellationToken token ) =>
        SendAsync( settings,
                   EmailBuilder.From( settings.Address() )
                               .To( targets )
                               .WithAttachment( attachments )
                               .WithSubject( subject )
                               .WithBody( body ),
                   token );
    public static async ValueTask SendAsync( EmailSettings settings, MimeMessage message, CancellationToken token = default )
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
    public static async ValueTask SendAsync( EmailSettings settings, EmailBuilder builder, CancellationToken token ) => await SendAsync( settings, await builder.Create(), token );


    private ValueTask SendAsync( MimeMessage message, CancellationToken token = default ) => SendAsync( _Settings,                          message, token );
    public ValueTask SendAsync( string?      email,   string            subject, string body ) => SendAsync( MailboxAddress.Parse( email ), subject, body, default );
    public ValueTask SendAsync( MailboxAddress target, string subject, string body, CancellationToken token, params Attachment[] attachments ) =>
        SendAsync( EmailBuilder.From( _options.GetSender() )
                               .To( target )
                               .WithAttachment( attachments )
                               .WithSubject( subject )
                               .WithBody( body ),
                   token );
    public async ValueTask SendAsync( EmailBuilder builder, CancellationToken token )
    {
        try
        {
            MimeMessage message = await builder.Create();
            await SendAsync( message, token );
        }
        catch (Exception e)
        {
            _logger.LogError( e,
                              "{ClassName}.'{Caller}'",
                              GetType()
                                 .Name,
                              nameof(SendAsync) );
        }
    }


    public async Task VerifyEmail( UserRecord user, CancellationToken token )
    {
        string subject = _options.VerifySubject ?? _options.DefaultSubject;
        string content = await _tokenService.CreateContent( subject, user, token );

        EmailBuilder builder = EmailBuilder.From( _options.GetSender() )
                                           .To( MailboxAddress.Parse( user.Email ) )
                                           .WithSubject( subject )
                                           .WithBody( content );

        await SendAsync( builder, token );
    }
    public async Task VerifyHTMLEmail( UserRecord user, CancellationToken token )
    {
        string subject = _options.VerifySubject ?? _options.DefaultSubject;
        string content = await _tokenService.CreateHTMLContent( subject, user, token );

        EmailBuilder builder = EmailBuilder.From( _options.GetSender() )
                                           .To( MailboxAddress.Parse( user.Email ) )
                                           .WithSubject( subject )
                                           .WithHTML( content );

        await SendAsync( builder, token );
    }



    public sealed class Options : IOptions<Options>
    {
        private EmailSettings?  _settings;
        public  EmailSettings?  Settings { get; set; }
        public  MailboxAddress? Sender   { get; set; }


        Options IOptions<Options>.Value => this;

        public string  DefaultSubject { get; set; } = string.Empty;
        public string? VerifySubject  { get; set; }


        public Options() { }
        public Options( string defaultSubject ) => DefaultSubject = defaultSubject;


        internal MailboxAddress GetSender() => Sender ??= Settings?.Address() ?? throw new InvalidOperationException( $"{nameof(Sender)} is not set" );
        internal EmailSettings GetSettings( IConfiguration configuration ) => _settings ??= configuration.GetSection( nameof(EmailSettings) )
                                                                                                         .Get<EmailSettings>();
    }
}
