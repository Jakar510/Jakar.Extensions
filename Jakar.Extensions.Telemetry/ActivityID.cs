namespace Jakar.Extensions.Telemetry;


[DefaultValue(nameof(Empty))]
public readonly record struct ActivityID( in Guid ID ) : ISpanParsable<ActivityID>
{
    public static readonly ActivityID Empty    = new(Guid.Empty);
    private readonly       string     __string = ID.ToString();
    public readonly        Guid       ID       = ID;
    public override        string     ToString() => __string;


    public static ActivityID Create()          => Create(Guid.CreateVersion7());
    public static ActivityID Create( Guid id ) => new(id);
    public static ActivityID? Create( [NotNullIfNotNull(nameof(id))] Guid? id ) => id.HasValue
                                                                                       ? new ActivityID(id.Value)
                                                                                       : null;
    public static ActivityID Parse( string s, IFormatProvider? provider ) => Parse(s.AsSpan(), provider);
    public static bool TryParse( string? s, IFormatProvider? provider, out ActivityID result )
    {
        ReadOnlySpan<char> span = s;
        return TryParse(span, provider, out result);
    }
    public static ActivityID Parse( ReadOnlySpan<char> span, IFormatProvider? provider ) => default;
    public static bool TryParse( ReadOnlySpan<char> span, IFormatProvider? provider, out ActivityID result )
    {
        if ( span.TryAsGuid(out Guid? id) )
        {
            result = Create(id.Value);
            return true;
        }

        result = Empty;
        return false;
    }


    // public sealed class Converter : JsonConverter<ActivityID> { }
    // public sealed class NullableConverter : JsonConverter<ActivityID?> { }
}
