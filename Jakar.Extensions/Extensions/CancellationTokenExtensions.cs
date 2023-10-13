namespace Jakar.Extensions;


public static class CancellationTokenExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool ShouldContinue( this CancellationToken token ) => token.IsCancellationRequested is false;
}
