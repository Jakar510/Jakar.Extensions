namespace Jakar.AppLogger.Portal.Data.Tables;


[Serializable]
[Table( "Apps" )]
public sealed record AppRecord : LoggerTable<AppRecord>
{
    [MaxLength( 1024 )]  public string AppName { get; init; } = string.Empty;
    [MaxLength( 10240 )] public string Secret  { get; init; } = string.Empty;


    public AppRecord() : base() { }
    public AppRecord( string appName, string secret, UserRecord? caller = default ) : base( caller )
    {
        AppName = appName;
        Secret  = secret;
    }


    public static DynamicParameters GetDynamicParameters( string secret )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(Secret), secret );
        return parameters;
    }


    public override int CompareTo( AppRecord? other ) => string.CompareOrdinal( AppName, other?.AppName );
    public override int GetHashCode() => HashCode.Combine( AppName, base.GetHashCode() );
    public override bool Equals( AppRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return string.Equals( AppName, other?.AppName, StringComparison.Ordinal );
    }
}
