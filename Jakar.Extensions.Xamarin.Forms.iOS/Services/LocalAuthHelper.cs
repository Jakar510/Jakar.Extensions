#nullable enable
#nullable enable
using System;
using System.Globalization;
using System.Threading.Tasks;
using Foundation;
using LocalAuthentication;
using UIKit;



namespace Jakar.Extensions.Xamarin.Forms.iOS;


public class LocalAuthHelper : Biometrics.IAuthHelperForIOS
{
    public bool IsLocalAuthAvailable => GetLocalAuthType() != Biometrics.LocalAuthType.None;
    public string GetLocalAuthLabelText()
    {
        Biometrics.LocalAuthType localAuthType = GetLocalAuthType();
        return localAuthType.ToString();
    }

    public string GetLocalAuthIcon()
    {
        Biometrics.LocalAuthType localAuthType = GetLocalAuthType();

        return localAuthType switch
               {
                   Biometrics.LocalAuthType.PassCode => "LockIcon",
                   Biometrics.LocalAuthType.TouchId  => "TouchIdIcon",
                   Biometrics.LocalAuthType.FaceId   => "FaceIdIcon",
                   _                                 => string.Empty,
               };
    }

    public string GetLocalAuthUnlockText()
    {
        Biometrics.LocalAuthType localAuthType = GetLocalAuthType();

        return localAuthType switch
               {
                   Biometrics.LocalAuthType.PassCode => "UnlockWithPasscode",
                   Biometrics.LocalAuthType.TouchId  => "UnlockWithTouchID",
                   Biometrics.LocalAuthType.FaceId   => "UnlockWithFaceID",
                   _                                 => string.Empty,
               };
    }

    public void Authenticate( Action? onSuccess, Action? onFailure )
    {
        using var context = new LAContext();

        if ( context.CanEvaluatePolicy( LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out NSError? authError ) || context.CanEvaluatePolicy( LAPolicy.DeviceOwnerAuthentication, out authError ) )
        {
            var replyHandler = new LAContextReplyHandler( ( success, error ) =>
                                                          {
                                                              if ( success ) { onSuccess?.Invoke(); }
                                                              else { onFailure?.Invoke(); }
                                                          } );

            context.EvaluatePolicy( LAPolicy.DeviceOwnerAuthentication, "Please Authenticate To Proceed", replyHandler );
        }

        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        authError?.Dispose();
    }

    public void Authenticate( Func<Task>? onSuccess, Func<Task>? onFailure )
    {
        using var context = new LAContext();

        if ( context.CanEvaluatePolicy( LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out NSError? authError ) || context.CanEvaluatePolicy( LAPolicy.DeviceOwnerAuthentication, out authError ) )
        {
            var replyHandler = new LAContextReplyHandler( async ( success, error ) =>
                                                          {
                                                              if ( success )
                                                              {
                                                                  if ( onSuccess is not null )
                                                                  {
                                                                      await onSuccess()
                                                                         .ConfigureAwait( false );
                                                                  }
                                                              }
                                                              else
                                                              {
                                                                  if ( onFailure is not null )
                                                                  {
                                                                      await onFailure()
                                                                         .ConfigureAwait( false );
                                                                  }
                                                              }
                                                          } );

            context.EvaluatePolicy( LAPolicy.DeviceOwnerAuthentication, "Please Authenticate To Proceed", replyHandler );
        }

        // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
        authError?.Dispose();
    }

    public Biometrics.LocalAuthType GetLocalAuthType()
    {
        using var localAuthContext = new LAContext();
        if ( !localAuthContext.CanEvaluatePolicy( LAPolicy.DeviceOwnerAuthentication, out NSError _ ) ) { return Biometrics.LocalAuthType.None; }

        if ( !localAuthContext.CanEvaluatePolicy( LAPolicy.DeviceOwnerAuthenticationWithBiometrics, out NSError _ ) ) { return Biometrics.LocalAuthType.PassCode; }

        if ( GetOsMajorVersion() >= 11 && localAuthContext.BiometryType == LABiometryType.FaceId ) { return Biometrics.LocalAuthType.FaceId; }

        return Biometrics.LocalAuthType.TouchId;
    }

    public int GetOsMajorVersion() => int.Parse( UIDevice.CurrentDevice.SystemVersion.Split( '.' )[0], CultureInfo.CurrentCulture );
}
