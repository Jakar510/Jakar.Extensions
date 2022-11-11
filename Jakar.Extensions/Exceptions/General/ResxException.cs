// Jakar.Extensions :: Jakar.Extensions
// 09/22/2022  5:37 PM

namespace Jakar.Extensions;


public class ResxException : Exception
{
    public ResxException( SupportedLanguage language, Exception? inner                 = null ) : base( $"language: {language}", inner ) { }
    public ResxException( SupportedLanguage language, long       key, Exception? inner = null ) : base( $"language: {language}  |  key: {key}", inner ) { }
}
