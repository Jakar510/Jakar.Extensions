﻿// Jakar.Extensions :: Jakar.Database
// 04/11/2023  2:36 PM

namespace Jakar.Database;


public interface IOneTimePassword
{
    bool          ValidateToken( string token, VerificationWindow? window = default );
    public string GetContent( IUserName record );
    public string GetQrCode( IUserName  record, int size,  BarcodeFormat format                       = BarcodeFormat.QR_CODE );
    public string GetQrCode( IUserName  record, int width, int           height, BarcodeFormat format = BarcodeFormat.QR_CODE );
}



public sealed class OneTimePassword : IOneTimePassword
{
    private readonly byte[] _key;
    private readonly string _issuer;
    private readonly string _secretKey;


    private OneTimePassword( string secretKey, string issuer )
    {
        if ( !string.IsNullOrWhiteSpace( secretKey ) ) { throw new InvalidOperationException( $"{nameof(secretKey)} is not set" ); }

        _secretKey = secretKey;
        _issuer    = issuer;
        _key       = Base32Encoding.ToBytes( _secretKey );
    }


    public static IOneTimePassword Create( string secretKey, string issuer ) => new OneTimePassword( secretKey, issuer );
    public static IOneTimePassword Create<T>( string secretKey )
        where T : IAppName => new OneTimePassword( secretKey, typeof(T).Name );


    public static string GenerateSecret()
    {
        byte[] secretKey = new byte[20];
        RandomNumberGenerator.Fill( secretKey );
        return Base32Encoding.ToString( secretKey );
    }


    public bool ValidateToken( string token, VerificationWindow? window = default )
    {
        var totp = new Totp( _key );
        return totp.VerifyTotp( token, out long _, window );
    }


    public string GetContent( IUserName record ) => $"otpauth://totp/{record.UserName}?secret={_secretKey}&issuer={_issuer}";


    public string GetQrCode( IUserName record, int size, BarcodeFormat format = BarcodeFormat.QR_CODE ) => GetQrCode( record, size, size, format );
    public string GetQrCode( IUserName record, int width, int height, BarcodeFormat format = BarcodeFormat.QR_CODE )
    {
        var writer = new BarcodeWriterSvg
                     {
                         Format = format,
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

        string? result = writer.Write( GetContent( record ) ).ToString();
        return result ?? throw new NullReferenceException( nameof(result) );
    }
}
