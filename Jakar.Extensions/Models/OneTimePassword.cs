// Jakar.Extensions :: Jakar.Extensions
// 4/3/2024  15:54

using OtpNet;
using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;



namespace Jakar.Extensions;


public sealed class OneTimePassword( string key, string issuer ) : Randoms
{
    // ReSharper disable once StaticMemberInGenericType
    private readonly string __issuer     = issuer;
    private readonly byte[] __keyBytes   = Base32Encoding.ToBytes(key);
    private readonly string __secret_Key = key;


    public static OneTimePassword Create( string            key, string issuer ) => new(key, issuer);
    public static OneTimePassword Create( string            issuer ) => Create(GenerateToken(20), issuer);
    public static OneTimePassword Create( TelemetrySource   source ) => Create(in source.Info);
    public static OneTimePassword Create( in AppInformation info )   => Create(info.AppName);
    public static OneTimePassword Create<TApp>()
        where TApp : IAppName => Create(TApp.AppName);


    public bool ValidateToken( string token, VerificationWindow? window = null ) => ValidateToken(token, out long _, window);
    public bool ValidateToken( string token, out long timeStepMatched, VerificationWindow? window = null )
    {
        Totp totp = new(__keyBytes);
        return totp.VerifyTotp(token, out timeStepMatched, window);
    }


    public string GetContent( IUserName model )    => GetContent(model.UserName);
    public string GetContent( string    userName ) => $"otpauth://totp/{userName}?secret={__secret_Key}&issuer={__issuer}";


    public string GetQrCode( IUserName record, int size, BarcodeFormat format = BarcodeFormat.QR_CODE ) => GetQrCode(record, size, size, format);
    public string GetQrCode( IUserName record, int width, int height, BarcodeFormat format = BarcodeFormat.QR_CODE )
    {
        BarcodeWriterSvg writer = new()
                                  {
                                      Format  = format,
                                      Options = GetOptions(width, height)
                                  };

        string? result = writer.Write(GetContent(record))
                               .ToString();

        return result ?? throw new NullReferenceException(nameof(result));
    }
    public static QrCodeEncodingOptions GetOptions( int width, int height ) => new()
                                                                               {
                                                                                   DisableECI      = true,
                                                                                   PureBarcode     = true,
                                                                                   ErrorCorrection = ErrorCorrectionLevel.H,
                                                                                   CharacterSet    = "UTF-8",
                                                                                   Width           = width,
                                                                                   Height          = height
                                                                               };


    public string GetQrCode( string userName, int size, BarcodeFormat format = BarcodeFormat.QR_CODE ) => GetQrCode(userName, size, size, format);
    public string GetQrCode( string userName, int width, int height, BarcodeFormat format = BarcodeFormat.QR_CODE )
    {
        Guard.IsGreaterThanOrEqualTo(width,  0);
        Guard.IsGreaterThanOrEqualTo(height, 0);

        BarcodeWriterSvg writer = new()
                                  {
                                      Format  = format,
                                      Options = GetOptions(width, height)
                                  };

        return writer.Write(GetContent(userName))
                     .ToString();
    }
}
