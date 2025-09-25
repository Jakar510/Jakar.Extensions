// Jakar.Extensions :: Jakar.Extensions
// 07/29/2025  14:45

using Serilog.Events;



namespace Jakar.Extensions;


[Serializable]
[StructLayout(LayoutKind.Auto)]
[method: JsonConstructor]
public readonly struct ThreadInformation( string name, int managedThreadID, Language currentCulture, Language currentUICulture ) : IJsonModel<ThreadInformation>
{
    public ThreadInformation() : this(Thread.CurrentThread) { }
    public ThreadInformation( Thread                          thread ) : this(thread.Name ?? string.Empty, thread.ManagedThreadId, thread.CurrentCulture, thread.CurrentUICulture) { }
    public static implicit operator ThreadInformation( Thread thread ) => new(thread);
    public readonly                 string                            Name             = name;
    public readonly                 int                               ManagedThreadID  = managedThreadID;
    public readonly                 Language                          CurrentCulture   = currentCulture;
    public readonly                 Language                          CurrentUICulture = currentUICulture;
    public static                   JsonSerializerContext             JsonContext   => JakarExtensionsContext.Default;
    public static                   JsonTypeInfo<ThreadInformation>   JsonTypeInfo  => JakarExtensionsContext.Default.ThreadInformation;
    public static                   JsonTypeInfo<ThreadInformation[]> JsonArrayInfo => JakarExtensionsContext.Default.ThreadInformationArray;


    public static ThreadInformation Create() => new();


    public StructureValue   GetStructureValue() => new([Enricher.GetProperty(Name, nameof(ThreadInformation.Name)), Enricher.GetProperty(ManagedThreadID, nameof(ThreadInformation.ManagedThreadID)), Enricher.GetProperty(CurrentCulture.DisplayName, nameof(ThreadInformation.CurrentCulture)), Enricher.GetProperty(CurrentUICulture.DisplayName, nameof(ThreadInformation.CurrentUICulture))]);
    public LogEventProperty GetProperty()       => new(nameof(ThreadInformation), GetStructureValue());


    public static bool TryFromJson( string? json, out ThreadInformation result )
    {
        try
        {
            if ( string.IsNullOrWhiteSpace(json) )
            {
                result = default;
                return false;
            }

            result = FromJson(json);
            return true;
        }
        catch ( Exception e ) { SelfLogger.WriteLine("{Exception}", e.ToString()); }

        result = default;
        return false;
    }
    public static ThreadInformation FromJson( string json ) => Validate.ThrowIfNull(JsonSerializer.Deserialize(json, JsonTypeInfo));


    public int CompareTo( object? other ) => other is ThreadInformation info
                                                 ? CompareTo(info)
                                                 : throw new ExpectedValueTypeException(other, typeof(ThreadInformation));
    public int CompareTo( ThreadInformation other )
    {
        int nameComparison = string.Compare(Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
        if ( nameComparison != 0 ) { return nameComparison; }

        int managedThreadIDComparison = ManagedThreadID.CompareTo(other.ManagedThreadID);
        if ( managedThreadIDComparison != 0 ) { return managedThreadIDComparison; }

        int currentCultureComparison = CurrentCulture.CompareTo(other.CurrentCulture);
        if ( currentCultureComparison != 0 ) { return currentCultureComparison; }

        return CurrentUICulture.CompareTo(other.CurrentUICulture);
    }
    public          bool Equals( ThreadInformation other ) => string.Equals(Name, other.Name, StringComparison.InvariantCultureIgnoreCase) && ManagedThreadID == other.ManagedThreadID && CurrentCulture.Equals(other.CurrentCulture) && CurrentUICulture.Equals(other.CurrentUICulture);
    public override bool Equals( object?           obj )   => obj is ThreadInformation other                                               && Equals(other);
    public override int GetHashCode()
    {
        HashCode hashCode = new HashCode();
        hashCode.Add(Name, StringComparer.InvariantCultureIgnoreCase);
        hashCode.Add(ManagedThreadID);
        hashCode.Add(CurrentCulture);
        hashCode.Add(CurrentUICulture);
        return hashCode.ToHashCode();
    }


    public static bool operator ==( ThreadInformation? left, ThreadInformation? right ) => Nullable.Equals(left, right);
    public static bool operator !=( ThreadInformation? left, ThreadInformation? right ) => !Nullable.Equals(left, right);
    public static bool operator ==( ThreadInformation  left, ThreadInformation  right ) => EqualityComparer<ThreadInformation>.Default.Equals(left, right);
    public static bool operator !=( ThreadInformation  left, ThreadInformation  right ) => !EqualityComparer<ThreadInformation>.Default.Equals(left, right);
    public static bool operator >( ThreadInformation   left, ThreadInformation  right ) => Comparer<ThreadInformation>.Default.Compare(left, right) > 0;
    public static bool operator >=( ThreadInformation  left, ThreadInformation  right ) => Comparer<ThreadInformation>.Default.Compare(left, right) >= 0;
    public static bool operator <( ThreadInformation   left, ThreadInformation  right ) => Comparer<ThreadInformation>.Default.Compare(left, right) < 0;
    public static bool operator <=( ThreadInformation  left, ThreadInformation  right ) => Comparer<ThreadInformation>.Default.Compare(left, right) <= 0;
}
