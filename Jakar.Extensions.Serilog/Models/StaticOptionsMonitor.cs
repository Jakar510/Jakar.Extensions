// Jakar.Extensions :: Jakar.Extensions
// 05/23/2024  11:05

using Microsoft.Extensions.Options;



namespace Jakar.Extensions.Serilog;


public sealed class StaticOptionsMonitor<[DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicParameterlessConstructor )] TValue>( TValue currentValue ) : IOptionsMonitor<TValue>
{
    public TValue           CurrentValue                            { get; } = currentValue ?? throw new ArgumentNullException( nameof(currentValue) );
    public TValue           Get( string?                 name )     => CurrentValue;
    public IDisposable OnChange( Action<TValue, string?> listener ) => NullScope.Instance;
}
