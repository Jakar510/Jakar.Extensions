#nullable enable
namespace Jakar.Extensions.Xamarin.Forms;


public interface IBaseUrl
{
    string GetBaseString();
    Uri GetUri();
}
