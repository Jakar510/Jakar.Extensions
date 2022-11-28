// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 10/04/2022  6:21 PM

namespace Jakar.AppLogger.Portal.Data.Tables;


[Serializable]
[Table("ErrorDetails")]
public sealed record ErrorDetailsRecord : LoggerTable<ErrorDetailsRecord>, IErrorDetails
{
    public string Platform { get; init; } = string.Empty;


    public ErrorDetailsRecord() { }
    public ErrorDetailsRecord( IErrorDetails details, UserRecord caller ) : base(caller) { Platform = details.Platform; }


    public static DynamicParameters GetDynamicParameters( ErrorDetails details )
    {
        var parameters = new DynamicParameters();
        parameters.Add(nameof(Platform), details.Platform);
        return parameters;
    }


    public override int CompareTo( ErrorDetailsRecord? other ) => string.CompareOrdinal(default, default);
    public override bool Equals( ErrorDetailsRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return string.Equals(default, default, StringComparison.Ordinal);
    }
    public override int GetHashCode() => HashCode.Combine(base.GetHashCode());
}
