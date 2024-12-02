using Microsoft.Extensions.Primitives;



namespace Jakar.Database;


public sealed class RoleStore( Database dbContext ) : IRoleStore<RoleRecord>
{
    public static readonly StringValues _roleNames = new([nameof(RoleRecord.NormalizedName), nameof(RoleRecord.NameOfRole)]);
    private readonly       Database     _dbContext = dbContext;


    public void Dispose() { }

    public static void Register( IServiceCollection builder )
    {
        builder.AddSingleton<RoleStore>();
        builder.AddTransient<IRoleStore<RoleRecord>>( static provider => provider.GetRequiredService<RoleStore>() );
    }


    public async Task<IdentityResult> CreateAsync( RoleRecord role, CancellationToken token )
    {
        await _dbContext.Roles.Insert( role, token );
        return IdentityResult.Success;
    }
    public async Task<IdentityResult> DeleteAsync( RoleRecord role, CancellationToken token )
    {
        await _dbContext.Roles.Delete( role, token );
        return IdentityResult.Success;
    }


    async Task<RoleRecord?> IRoleStore<RoleRecord>.FindByIdAsync( string roleId, CancellationToken token )
    {
        ErrorOrResult<RoleRecord> role = await FindByIdAsync( roleId, token );

        return role.HasValue
                   ? role.Value
                   : null;
    }
    async Task<RoleRecord?> IRoleStore<RoleRecord>.FindByNameAsync( string roleId, CancellationToken token )
    {
        ErrorOrResult<RoleRecord> role = await FindByNameAsync( roleId, token );

        return role.HasValue
                   ? role.Value
                   : null;
    }


    public async ValueTask<ErrorOrResult<RoleRecord>> FindByIdAsync( string roleId, CancellationToken token ) => RecordID<RoleRecord>.TryParse( roleId, out RecordID<RoleRecord> id )
                                                                                                                ? await _dbContext.Roles.Get( id, token )
                                                                                                                : Error.NotFound( roleId );
    public async ValueTask<ErrorOrResult<RoleRecord>> FindByNameAsync( string name, CancellationToken token )
    {
        ErrorOrResult<RoleRecord> role = await _dbContext.Roles.Get( nameof(RoleRecord.NameOfRole), name, token );
        if ( role.HasValue ) { return role; }

        role = await _dbContext.Roles.Get( nameof(RoleRecord.NormalizedName), name, token );

        return role.HasValue
                   ? role
                   : Error.NotFound( _roleNames, name );
    }
    public async Task<string?> GetNormalizedRoleNameAsync( RoleRecord role, CancellationToken token )                         => await ValueTask.FromResult( role.NameOfRole );
    public async Task<string>  GetRoleIdAsync( RoleRecord             role, CancellationToken token )                         => await ValueTask.FromResult( role.ID.ToString() );
    public async Task<string?> GetRoleNameAsync( RoleRecord           role, CancellationToken token )                         => await ValueTask.FromResult( role.NameOfRole );
    public async Task          SetNormalizedRoleNameAsync( RoleRecord role, string?           name, CancellationToken token ) => await _dbContext.Roles.Update( role with { NormalizedName = name ?? string.Empty }, token );
    public async Task          SetRoleNameAsync( RoleRecord           role, string?           name, CancellationToken token ) => await _dbContext.Roles.Update( role with { NameOfRole = name     ?? string.Empty }, token );


    public async Task<IdentityResult> UpdateAsync( RoleRecord role, CancellationToken token )
    {
        try
        {
            await _dbContext.Roles.Update( role, token );
            return IdentityResult.Success;
        }
        catch ( Exception e ) { return IdentityResult.Failed( new IdentityError { Description = e.Message } ); }
    }
}
