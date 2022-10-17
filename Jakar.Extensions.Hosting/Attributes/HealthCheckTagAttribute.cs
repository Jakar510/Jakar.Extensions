namespace Jakar.Extensions.Hosting;


public sealed class HealthCheckTagAttribute : Attribute
{
    public string Name { get; init; }


    public HealthCheckTagAttribute( string name ) => Name = name;
}
