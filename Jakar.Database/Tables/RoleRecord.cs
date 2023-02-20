// ToothFairyDispatch :: ToothFairyDispatch.Cloud
// 10/10/2022  2:15 PM

namespace Jakar.Database;


[Serializable]
[Table( "Roles" )]
public sealed record RoleRecord : TableRecord<RoleRecord>
{
    [MaxLength( 4096 )]
    public string ConcurrencyStamp { get; init; } = Guid.NewGuid()
                                                        .ToString();

    [MaxLength( 1024 )] public string Name           { get; init; } = string.Empty;
    [MaxLength( 1024 )] public string NormalizedName { get; init; } = string.Empty;


    public RoleRecord() { }
    public RoleRecord( IdentityRole role )
    {
        Name             = role.Name ?? string.Empty;
        NormalizedName   = role.NormalizedName ?? string.Empty;
        ConcurrencyStamp = role.ConcurrencyStamp ?? string.Empty;
    }
    public RoleRecord( string name )
    {
        Name           = name;
        NormalizedName = name;
    }
    public RoleRecord( string name, string normalizedName )
    {
        Name           = name;
        NormalizedName = normalizedName;
    }


    public IdentityRole ToIdentityRole() => new()
                                            {
                                                Name             = Name,
                                                NormalizedName   = NormalizedName,
                                                ConcurrencyStamp = ConcurrencyStamp,
                                            };


    public override int CompareTo( RoleRecord? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }


        int nameComparison = string.Compare( Name, other.Name, StringComparison.Ordinal );
        if ( nameComparison != 0 ) { return nameComparison; }

        int normalizedNameComparison = string.Compare( NormalizedName, other.NormalizedName, StringComparison.Ordinal );
        if ( normalizedNameComparison != 0 ) { return normalizedNameComparison; }

        return string.Compare( ConcurrencyStamp, other.ConcurrencyStamp, StringComparison.Ordinal );
    }
    public override int GetHashCode() => HashCode.Combine( base.GetHashCode(), Name, NormalizedName, ConcurrencyStamp );
    public override bool Equals( RoleRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return base.Equals( other ) && Name == other.Name && NormalizedName == other.NormalizedName && ConcurrencyStamp == other.ConcurrencyStamp;
    }


    public async ValueTask<UserRecord[]> GetUsers( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token ) =>
        await UserRoleRecord.Where( connection, transaction, db.UserRoles, db.Users, this, token );
}
