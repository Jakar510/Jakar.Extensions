// Jakar.Extensions :: Jakar.Extensions
// 04/21/2022  11:48 AM

#nullable enable


namespace Jakar.Extensions;


[Obsolete]
public readonly struct ResponseData
{
    private static readonly ResponseData _none = new("NO RESPONSE");
    public const string ERROR_MESSAGE = "Error Message: ";
    public const string UNKNOWN_ERROR = "Unknown Error";


    public string? Method { get; init; } = default;
    public Uri? URL { get; init; } = default;
    public JToken? ErrorMessage { get; init; } = default;
    public Status? StatusCode { get; init; } = default;
    public string? StatusDescription { get; init; } = default;
    public string? ContentEncoding { get; init; } = default;
    public string? Server { get; init; } = default;
    public string? ContentType { get; init; } = default;


    public ResponseData(in string? error) => ErrorMessage = ParseError(error);
    public ResponseData(in HttpWebResponse response, in string? error) : this(error)
    {
        StatusCode = response.StatusCode.ToStatus();
        URL = response.ResponseUri;
        Method = response.Method;
        StatusDescription = response.StatusDescription;
        ContentType = response.ContentType;
        ContentEncoding = response.ContentEncoding;
        Server = response.Server;
    }
    public ResponseData(in WebResponse response, in string? error) : this(error)
    {
        URL = response.ResponseUri;
        ContentType = response.ContentType;
    }


    public override string ToString() => this.ToPrettyJson();


    public  static JToken? ParseError(ReadOnlySpan<char> error)
    {
        if (error.IsNullOrWhiteSpace()) { return default; }

        if (error.StartsWith(ERROR_MESSAGE, StringComparison.OrdinalIgnoreCase)) { error = error[ERROR_MESSAGE.Length..]; }

        try { return error.FromJson(); }
        catch (Exception) { return error.ToString(); }
    }


    public static async Task<ResponseData> Create(WebException e)
    {
        if (e.Response is null) { return _none; }

        return e.Response is HttpWebResponse response
                   ? await Create(response)
                   : await Create(e.Response);
    }
    public static async Task<ResponseData> Create(HttpWebResponse response)
    {
        await using Stream? stream = response.GetResponseStream();

        string msg;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (stream is not null)
        {
            using var reader = new StreamReader(stream);

            string? errorMessage = await reader.ReadToEndAsync();
            msg = $"Error Message: {errorMessage}";
        }
        else { msg = "UNKNOWN"; }

        return new ResponseData(response, msg);
    }
    public static async Task<ResponseData> Create(WebResponse response)
    {
        await using Stream? stream = response.GetResponseStream();

        string msg;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (stream is not null)
        {
            using var reader = new StreamReader(stream);

            string? errorMessage = await reader.ReadToEndAsync();
            msg = $"Error Message: {errorMessage}";
        }
        else { msg = "UNKNOWN"; }

        return new ResponseData(response, msg);
    }
}
