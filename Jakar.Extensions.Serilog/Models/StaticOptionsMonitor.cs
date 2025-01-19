// Jakar.Extensions :: Jakar.Extensions
// 05/23/2024  11:05

using Microsoft.Extensions.Options;



namespace Jakar.Extensions.Serilog;


public sealed class StaticOptionsMonitor<[DynamicallyAccessedMembers( DynamicallyAccessedMemberTypes.PublicParameterlessConstructor )] T>( T currentValue ) : IOptionsMonitor<T>
{
    public T           CurrentValue                            { get; } = currentValue ?? throw new ArgumentNullException( nameof(currentValue) );
    public T           Get( string?                 name )     => CurrentValue;
    public IDisposable OnChange( Action<T, string?> listener ) => NullScope.Instance;
}
