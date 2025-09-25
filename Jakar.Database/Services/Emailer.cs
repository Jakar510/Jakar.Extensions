using MailKit.Net.Smtp;
using ILogger = Microsoft.Extensions.Logging.ILogger;



namespace Jakar.Database;


public interface IEmailer : IEmailTokenService
{
    Uri               Domain { get; }
    ValueTask         SendAsync( string?                      email,   string            subject, string body );
    ValueTask         SendAsync( MailboxAddress               target,  string            subject, string body, CancellationToken token, params ReadOnlySpan<Attachment> attachments );
    ValueTask         SendAsync( EmailBuilder                 builder, CancellationToken token );
    string            GetUrl( Tokens                          result );
    ValueTask<string> GenerateAccessToken( IEnumerable<Claim> claims, CancellationToken token );
    string            CreateContent( Tokens                   result, in string         header );
    string            CreateHTMLContent( Tokens               result, in string         header );
    Task              VerifyEmail( UserRecord                 user,   ClaimType         types, CancellationToken token );
    Task              VerifyHTMLEmail( UserRecord             user,   ClaimType         types, CancellationToken token );
}



/// <summary>
///     <para>
///         <see href="https://codepedia.info/jwt-authentication-in-aspnet-core-web-api-token"/>
///     </para>
///     <para>
///         <see href="https://stackoverflow.com/a/55740879/9530917"> How do I get current user in .NET Core Web API (from JWT Token) </see>
///     </para>
/// </summary>
[SuppressMessage("ReSharper", "SuggestBaseTypeForParameterInConstructor")]
public class Emailer( EmailTokenProvider tokenProvider, IConfiguration configuration, ILogger<Emailer> logger, Database dataBase, IOptions<Emailer.Options> options ) : IEmailer
{
    protected readonly Database                _dataBase      = dataBase;
    protected readonly EmailTokenProvider      _tokenProvider = tokenProvider;
    protected readonly IConfiguration          _configuration = configuration;
    protected readonly ILogger                 _logger        = logger;
    protected readonly JwtSecurityTokenHandler _handler       = new();
    protected readonly Options                 _options       = options.Value;
    protected          EmailSettings?          _settings;


    protected      EmailSettings _Settings => _settings ??= _options.GetSettings(_configuration);
    public virtual Uri           Domain    => _dataBase.Options.Domain;


    public static Emailer Create( IServiceProvider provider )
    {
        EmailTokenProvider tokenProvider = provider.GetRequiredService<EmailTokenProvider>();
        IConfiguration     configuration = provider.GetRequiredService<IConfiguration>();
        ILogger<Emailer>   logger        = provider.GetRequiredService<ILoggerFactory>().CreateLogger<Emailer>();
        Database           dataBase      = provider.GetRequiredService<Database>();
        IOptions<Options>  options       = provider.GetRequiredService<IOptions<Options>>();
        return new Emailer(tokenProvider, configuration, logger, dataBase, options);
    }


    public static       ValueTask SendAsync( EmailSettings settings, MailboxAddress              target,  string                  subject,     string body,    CancellationToken token )                                  => SendAsync(settings, target,                                                                                                            [],          subject, body, token);
    public static       ValueTask SendAsync( EmailSettings settings, IEnumerable<MailboxAddress> targets, string                  subject,     string body,    CancellationToken token )                                  => SendAsync(settings, targets,                                                                                                           [],          subject, body, token);
    public static       ValueTask SendAsync( EmailSettings settings, IEnumerable<MailboxAddress> targets, string                  subject,     string body,    CancellationToken token, params Attachment[] attachments ) => SendAsync(settings, targets,                                                                                                           attachments, subject, body, token);
    public static       ValueTask SendAsync( EmailSettings settings, MailboxAddress              target,  IEnumerable<Attachment> attachments, string subject, string            body,  CancellationToken   token )       => SendAsync(settings, EmailBuilder.From(settings.Address()).To(target).WithAttachment(attachments).WithSubject(subject).WithBody(body),  token);
    public static       ValueTask SendAsync( EmailSettings settings, IEnumerable<MailboxAddress> targets, IEnumerable<Attachment> attachments, string subject, string            body,  CancellationToken   token )       => SendAsync(settings, EmailBuilder.From(settings.Address()).To(targets).WithAttachment(attachments).WithSubject(subject).WithBody(body), token);
    public static async ValueTask SendAsync( EmailSettings settings, EmailBuilder                builder, CancellationToken       token ) => await SendAsync(settings, await builder.Create(), token);
    public static async ValueTask SendAsync( EmailSettings settings, MimeMessage message, CancellationToken token = default )
    {
        using SmtpClient client = new();
        await client.ConnectAsync(settings.Site, settings.Port, settings.Options, token);

        try
        {
            if ( client.Capabilities.HasFlag(SmtpCapabilities.Authentication) ) { await client.AuthenticateAsync(settings.UserName, settings.Password, token); }

            await client.SendAsync(message, token);
        }
        finally { await client.DisconnectAsync(true, token); }
    }


    public virtual string GetUrl( Tokens result ) => $"{Domain.OriginalString}/Token/{result.AccessToken}";


    public virtual async ValueTask<string> GenerateAccessToken( IEnumerable<Claim> claims, CancellationToken token )
    {
        JwtSecurityToken security    = await _dataBase.GetJwtSecurityToken(claims, token);
        string           tokenString = _handler.WriteToken(security);
        return tokenString;
    }


    public virtual string CreateContent( Tokens result, in string header ) =>
        $"""
         {header}

         {GetUrl(result)}
         """;
    public virtual string CreateHTMLContent( Tokens result, in string header ) =>
        $"""
         <h1> {header} </h1>
         <p>
         	<a href='{GetUrl(result)}'>Click to approve</a>
         </p>
         """;


    public virtual ValueTask<ErrorOrResult<Tokens>> Authenticate( LoginRequest request, ClaimType types, CancellationToken token = default ) => _dataBase.Authenticate(request, types, token);


    public virtual async ValueTask<string> CreateContent( string header, UserRecord user, ClaimType types, CancellationToken token = default )
    {
        Tokens result = await _dataBase.GetToken(user, types, token);
        return CreateContent(result, header);
    }
    public virtual async ValueTask<string> CreateHTMLContent( string header, UserRecord user, ClaimType types, CancellationToken token = default )
    {
        Tokens result = await _dataBase.GetToken(user, types, token);
        return CreateHTMLContent(result, header);
    }
    public async Task VerifyEmail( UserRecord user, ClaimType types, CancellationToken token )
    {
        string subject = _options.VerifySubject ?? _options.DefaultSubject;
        string content = await CreateContent(subject, user, types, token);

        EmailBuilder builder = EmailBuilder.From(_options.GetSender()).To(MailboxAddress.Parse(user.Email)).WithSubject(subject).WithBody(content);

        await SendAsync(builder, token);
    }
    public async Task VerifyHTMLEmail( UserRecord user, ClaimType types, CancellationToken token )
    {
        string subject = _options.VerifySubject ?? _options.DefaultSubject;
        string content = await CreateHTMLContent(subject, user, types, token);

        EmailBuilder builder = EmailBuilder.From(_options.GetSender()).To(MailboxAddress.Parse(user.Email)).WithSubject(subject).WithHTML(content);

        await SendAsync(builder, token);
    }


    private ValueTask SendAsync( MimeMessage message, CancellationToken token = default )      => SendAsync(_Settings,                   message, token);
    public  ValueTask SendAsync( string?     email,   string            subject, string body ) => SendAsync(MailboxAddress.Parse(email), subject, body, CancellationToken.None);
    public ValueTask SendAsync( MailboxAddress target, string subject, string body, CancellationToken token, params ReadOnlySpan<Attachment> attachments ) =>
        SendAsync(EmailBuilder.From(_options.GetSender()).To(target).WithAttachment(attachments).WithSubject(subject).WithBody(body), token);
    public async ValueTask SendAsync( EmailBuilder builder, CancellationToken token )
    {
        try
        {
            MimeMessage message = await builder.Create();
            await SendAsync(message, token);
        }
        catch ( Exception e ) { DbLog.Error(_logger, e, this); }
    }



    public sealed class Options : IOptions<Options>
    {
        private EmailSettings? __settings;


        public string             DefaultSubject { get; set; } = string.Empty;
        public MailboxAddress?    Sender         { get; set; }
        public EmailSettings?     Settings       { get; set; }
        Options IOptions<Options>.Value          => this;
        public string?            VerifySubject  { get; set; }


        public Options() { }
        public Options( string                             defaultSubject ) => DefaultSubject = defaultSubject;
        internal EmailSettings GetSettings( IConfiguration configuration ) => __settings ??= EmailSettings.Create(configuration);


        internal MailboxAddress GetSender() => Sender ??= Settings?.Address() ?? throw new InvalidOperationException($"{nameof(Sender)} is not set");
    }
}
