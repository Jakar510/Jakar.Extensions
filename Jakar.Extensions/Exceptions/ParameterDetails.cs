// Jakar.Extensions :: Jakar.Extensions
// 12/8/2023  17:13

namespace Jakar.Extensions;


public sealed class ParameterDetails
{
    public bool    HasDefaultValue { get; init; }
    public bool    IsIn            { get; init; }
    public bool    IsOptional      { get; init; }
    public bool    IsOut           { get; init; }
    public string? Name            { get; init; }
    public int     Position        { get; init; }
    public string? Type            { get; init; }


    public ParameterDetails() { }
    public ParameterDetails( ParameterInfo parameter )
    {
        Name            = parameter.Name;
        Position        = parameter.Position;
        IsIn            = parameter.IsIn;
        IsOut           = parameter.IsOut;
        IsOptional      = parameter.IsOptional;
        HasDefaultValue = parameter.HasDefaultValue;
        Type            = parameter.ParameterType.FullName;
    }


    public static ParameterDetails[] Create( MethodBase                 method ) => Create( method.GetParameters() );
    public static ParameterDetails[] Create( IEnumerable<ParameterInfo> items )  => items.Select( x => new ParameterDetails( x ) ).ToArray();
}
