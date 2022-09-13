namespace Jakar.Extensions;


public static class CancellationTokenExtensions
{
    public static bool ShouldContinue( this CancellationToken token ) => !token.IsCancellationRequested;
}
