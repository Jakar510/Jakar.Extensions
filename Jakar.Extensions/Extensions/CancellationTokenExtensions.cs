namespace Jakar.Extensions;


public static class CancellationTokenExtensions
{
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool ShouldContinue( this ref readonly CancellationToken       token ) => token.IsCancellationRequested is false;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool ShouldContinue( this              CancellationTokenSource token ) => token.IsCancellationRequested is false;
}
