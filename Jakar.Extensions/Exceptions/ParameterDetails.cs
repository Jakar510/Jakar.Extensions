// Jakar.Extensions :: Jakar.Extensions
// 12/8/2023  17:13

namespace Jakar.Extensions;


public sealed class ParameterDetails : BaseClass<ParameterDetails>, IJsonModel<ParameterDetails>, IEqualComparable<ParameterDetails>
{
    public static JsonTypeInfo<ParameterDetails[]> JsonArrayInfo   => JakarExtensionsContext.Default.ParameterDetailsArray;
    public static JsonSerializerContext            JsonContext     => JakarExtensionsContext.Default;
    public static JsonTypeInfo<ParameterDetails>   JsonTypeInfo    => JakarExtensionsContext.Default.ParameterDetails;
    public        bool                             HasDefaultValue { get; init; }
    public        bool                             IsIn            { get; init; }
    public        bool                             IsOptional      { get; init; }
    public        bool                             IsOut           { get; init; }
    public        string?                          Name            { get; init; }
    public        int                              Position        { get; init; }
    public        string?                          Type            { get; init; }


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


    public static ParameterDetails[] Create( MethodBase                 method ) => Create(method.GetParameters());
    public static ParameterDetails[] Create( IEnumerable<ParameterInfo> items )  => items.Select(x => new ParameterDetails(x)).ToArray();


    public override int CompareTo( ParameterDetails? other )
    {
        if ( ReferenceEquals(this, other) ) { return 0; }

        if ( other is null ) { return 1; }

        int positionComparison = Position.CompareTo(other.Position);
        if ( positionComparison != 0 ) { return positionComparison; }

        int nameComparison = string.Compare(Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
        if ( nameComparison != 0 ) { return nameComparison; }

        int typeComparison = string.Compare(Type, other.Type, StringComparison.InvariantCultureIgnoreCase);
        if ( typeComparison != 0 ) { return typeComparison; }

        int hasDefaultValueComparison = HasDefaultValue.CompareTo(other.HasDefaultValue);
        if ( hasDefaultValueComparison != 0 ) { return hasDefaultValueComparison; }

        int isInComparison = IsIn.CompareTo(other.IsIn);
        if ( isInComparison != 0 ) { return isInComparison; }

        int isOptionalComparison = IsOptional.CompareTo(other.IsOptional);
        if ( isOptionalComparison != 0 ) { return isOptionalComparison; }

        return IsOut.CompareTo(other.IsOut);
    }
    public override bool Equals( ParameterDetails? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return Position == other.Position && string.Equals(Name, other.Name, StringComparison.InvariantCultureIgnoreCase) && string.Equals(Type, other.Type, StringComparison.InvariantCultureIgnoreCase) && HasDefaultValue == other.HasDefaultValue && IsIn == other.IsIn && IsOptional == other.IsOptional && IsOut == other.IsOut;
    }
    public override bool Equals( object? obj ) => ReferenceEquals(this, obj) || obj is ParameterDetails other && Equals(other);
    public override int  GetHashCode()         => HashCode.Combine(base.GetHashCode(), Position, Name, Type, HasDefaultValue, IsIn, IsOptional, IsOut);


    public static bool operator ==( ParameterDetails? left, ParameterDetails? right ) => Equals(left, right);
    public static bool operator !=( ParameterDetails? left, ParameterDetails? right ) => !Equals(left, right);
    public static bool operator <( ParameterDetails?  left, ParameterDetails? right ) => Comparer<ParameterDetails>.Default.Compare(left, right) < 0;
    public static bool operator >( ParameterDetails?  left, ParameterDetails? right ) => Comparer<ParameterDetails>.Default.Compare(left, right) > 0;
    public static bool operator <=( ParameterDetails? left, ParameterDetails? right ) => Comparer<ParameterDetails>.Default.Compare(left, right) <= 0;
    public static bool operator >=( ParameterDetails? left, ParameterDetails? right ) => Comparer<ParameterDetails>.Default.Compare(left, right) >= 0;
}
