namespace Jakar.Extensions.AppCenter;


public sealed class AppCenterApi : IDisposable
{
    private readonly SecureString    _apiToken;
    private readonly AppCenterConfig _config;


    /// <summary> <para><see href="https://stackoverflow.com/a/69327347/9530917">How to convert SecureString to System.String?</see></para> </summary>
    internal string ApiToken => SecureStringToString(_apiToken);


    public AppCenterApi( SecureString apiToken, LocalDirectory directory )
    {
        _apiToken = apiToken;
        _config   = AppCenterConfig.Create(directory);
    }
    public void Dispose() => _apiToken.Dispose();


    public static unsafe SecureString CreateSecureString( in string apiToken )
    {
        fixed ( char* token = apiToken )
        {
            var secure = new SecureString(token, apiToken.Length);
            secure.MakeReadOnly();
            return secure;
        }
    }
    public static string SecureStringToString( in SecureString value )
    {
        IntPtr valuePtr = IntPtr.Zero;

        try
        {
            valuePtr = Marshal.SecureStringToBSTR(value);
            return Marshal.PtrToStringBSTR(valuePtr);
        }
        finally { Marshal.ZeroFreeBSTR(valuePtr); }
    }
}
