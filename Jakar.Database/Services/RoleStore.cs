namespace Jakar.Database;


public sealed class RoleStore( Database dbContext ) : IRoleStore<RoleRecord>
{
    private readonly Database _dbContext = dbContext;


    public void Dispose() { }

    public static void Register( IServiceCollection builder )
    {
        builder.AddSingleton<RoleStore>();
        builder.AddTransient<IRoleStore<RoleRecord>>( static provider => provider.GetRequiredService<RoleStore>() );
    }


    public Task<IdentityResult> CreateAsync( RoleRecord role, CancellationToken token ) => CreateAsync( Activity.Current, role, token );
    public async Task<IdentityResult> CreateAsync( Activity? activity, RoleRecord role, CancellationToken token )
    {
        await _dbContext.Roles.Insert( activity, role, token );
        return IdentityResult.Success;
    }
    public Task<IdentityResult> DeleteAsync( RoleRecord role, CancellationToken token ) => DeleteAsync( Activity.Current, role, token );
    public async Task<IdentityResult> DeleteAsync( Activity? activity, RoleRecord role, CancellationToken token )
    {
        await _dbContext.Roles.Delete( activity, role, token );
        return IdentityResult.Success;
    }


    public Task<RoleRecord?> FindByIdAsync( string roleId, CancellationToken token ) => FindByNameAsync( null, roleId, token );
    public async Task<RoleRecord?> FindByIdAsync( Activity? activity, string roleId, CancellationToken token ) => RecordID<RoleRecord>.TryParse( roleId, out RecordID<RoleRecord>? id )
                                                                                                                      ? await _dbContext.Roles.Get( activity, id, token )
                                                                                                                      : null;
    public       Task<RoleRecord?> FindByNameAsync( string    name,     CancellationToken token )                         => FindByNameAsync( null, name, token );
    public async Task<RoleRecord?> FindByNameAsync( Activity? activity, string            name, CancellationToken token ) => await _dbContext.Roles.Get( activity, nameof(RoleRecord.NameOfRole), name, token ) ?? await _dbContext.Roles.Get( activity, nameof(RoleRecord.NormalizedName), name, token );


    public async Task<string?> GetNormalizedRoleNameAsync( RoleRecord role, CancellationToken token ) => await ValueTask.FromResult( role.NameOfRole );
    public async Task<string>  GetRoleIdAsync( RoleRecord             role, CancellationToken token ) => await ValueTask.FromResult( role.ID.ToString() );
    public async Task<string?> GetRoleNameAsync( RoleRecord           role, CancellationToken token ) => await ValueTask.FromResult( role.NameOfRole );


    public       Task SetNormalizedRoleNameAsync( RoleRecord role,     string?    name, CancellationToken token )                         => SetNormalizedRoleNameAsync( Activity.Current, role, name, token );
    public async Task SetNormalizedRoleNameAsync( Activity?  activity, RoleRecord role, string?           name, CancellationToken token ) => await _dbContext.Roles.Update( activity, role with { NormalizedName = name ?? string.Empty }, token );
    public       Task SetRoleNameAsync( RoleRecord           role,     string?    name, CancellationToken token )                         => SetRoleNameAsync( Activity.Current, role, name, token );
    public async Task SetRoleNameAsync( Activity?            activity, RoleRecord role, string?           name, CancellationToken token ) => await _dbContext.Roles.Update( activity, role with { NameOfRole = name ?? string.Empty }, token );


    public Task<IdentityResult> UpdateAsync( RoleRecord role, CancellationToken token ) => UpdateAsync( Activity.Current, role, token );
    public async Task<IdentityResult> UpdateAsync( Activity? activity, RoleRecord role, CancellationToken token )
    {
        try
        {
            await _dbContext.Roles.Update( activity, role, token );
            return IdentityResult.Success;
        }
        catch ( Exception e ) { return IdentityResult.Failed( new IdentityError { Description = e.Message } ); }
    }
}
