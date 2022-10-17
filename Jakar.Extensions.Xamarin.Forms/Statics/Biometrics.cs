#nullable enable
namespace Jakar.Extensions.Xamarin.Forms;


public class Biometrics
{
    public enum LocalAuthType
    {
        None,
        PassCode,
        TouchId,
        FaceId
    }



    public interface IAuthHelperForIOS
    {
        public bool IsLocalAuthAvailable { get; }
        public void Authenticate( Action?     onSuccess, Action?     onFailure );
        public void Authenticate( Func<Task>? onSuccess, Func<Task>? onFailure );
        public string GetLocalAuthIcon();
        public string GetLocalAuthLabelText();

        public LocalAuthType GetLocalAuthType();

        public string GetLocalAuthUnlockText();
        public int GetOsMajorVersion();
    }
}
