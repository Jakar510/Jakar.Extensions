// Jakar.Extensions :: Jakar.Database
// 04/11/2023  2:36 PM

using System.Security.Cryptography;
using OtpNet;
using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;



namespace Jakar.Database;


public interface IOneTimePassword
{
    bool ValidateToken( string   token,  VerificationWindow? window = default );
    string GetQrCode( UserRecord record, int                 width  = 256, int height = 256, BarcodeFormat? format = default );
}



public sealed class OneTimePassword<T> : IOneTimePassword where T : IAppName
{
    private readonly string _secretKey;
    private readonly string _issuer = typeof(T).Name;


    private OneTimePassword( Guid secretKey ) : this( secretKey.ToString() ) { }
    private OneTimePassword( string secretKey )
    {
        if ( !string.IsNullOrWhiteSpace( secretKey ) ) { throw new InvalidOperationException( $"{nameof(secretKey)} is not set" ); }

        _secretKey = secretKey;
    }
    public static IOneTimePassword Create( Guid   secretKey ) => new OneTimePassword<T>( secretKey );
    public static IOneTimePassword Create( string secretKey ) => new OneTimePassword<T>( secretKey );


    public static string GenerateSecret()
    {
        var secretKey = new byte[20];
        RandomNumberGenerator.Fill( secretKey );
        return Base32Encoding.ToString( secretKey );
    }


    public bool ValidateToken( string token, VerificationWindow? window = default )
    {
        var totp = new Totp( Base32Encoding.ToBytes( _secretKey ) );
        return totp.VerifyTotp( token, out long _, window );
    }


    public string GetQrCode( UserRecord record, int width = 256, int height = 256, BarcodeFormat? format = default )
    {
        var writer = new BarcodeWriterSvg
                     {
                         Format = format ?? BarcodeFormat.QR_CODE,
                         Options = new QrCodeEncodingOptions
                                   {
                                       DisableECI      = true,
                                       PureBarcode     = true,
                                       ErrorCorrection = ErrorCorrectionLevel.H,
                                       CharacterSet    = "UTF-8",
                                       Width           = width,
                                       Height          = height
                                   }
                     };

        return writer.Write( $"otpauth://totp/{record.UserName}?secret={_secretKey}&issuer={_issuer}" )
                     .ToString();
    }
}
