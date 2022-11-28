using Microsoft.AspNetCore.Mvc.RazorPages;



namespace Jakar.AppLogger.Portal.Pages;


[ResponseCache( Duration = 0, Location = ResponseCacheLocation.None, NoStore = true )]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    private readonly ILogger<ErrorModel> _logger;

    public bool    ShowRequestId => !string.IsNullOrEmpty( RequestId );
    public string? RequestId     { get; set; }

    public ErrorModel( ILogger<ErrorModel> logger ) => _logger = logger;

    public void OnGet() => RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
}
