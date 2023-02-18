namespace Jakar.Database;


public sealed class RoleStore : IRoleStore<RoleRecord>
{
    private readonly Database _dbContext;


    public RoleStore( Database dbContext ) => _dbContext = dbContext;


    public void Dispose() { }


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


    public async Task<RoleRecord> FindByIdAsync( string roleId, CancellationToken token )
    {
        RoleRecord? result = long.TryParse( roleId, out long id )
                                 ? await _dbContext.Roles.Get( id,                      token )
                                 : await _dbContext.Roles.Get( nameof(RoleRecord.Name), roleId, token );

        return result ?? throw new ArgumentException( roleId, nameof(roleId) );
    }
    public async Task<RoleRecord> FindByNameAsync( string name, CancellationToken token ) => await _dbContext.Roles.Get( nameof(RoleRecord.Name), name, token ) ?? throw new ArgumentException( name, nameof(name) );


    public async Task<string?> GetNormalizedRoleNameAsync( RoleRecord role, CancellationToken token ) => await ValueTask.FromResult( role.Name );
    public async Task<string> GetRoleIdAsync( RoleRecord              role, CancellationToken token ) => await ValueTask.FromResult( role.ID.ToString() );
    public async Task<string?> GetRoleNameAsync( RoleRecord           role, CancellationToken token ) => await ValueTask.FromResult( role.Name );


    public async Task SetNormalizedRoleNameAsync( RoleRecord role, string? name, CancellationToken token ) => await _dbContext.Roles.Update( role with
                                                                                                                                             {
                                                                                                                                                 NormalizedName = name ?? string.Empty,
                                                                                                                                             },
                                                                                                                                             token );

    public async Task SetRoleNameAsync( RoleRecord role, string? name, CancellationToken token ) => await _dbContext.Roles.Update( role with
                                                                                                                                   {
                                                                                                                                       Name = name ?? string.Empty,
                                                                                                                                   },
                                                                                                                                   token );

    public async Task<IdentityResult> UpdateAsync( RoleRecord role, CancellationToken token )
    {
        try
        {
            await _dbContext.Roles.Update( role, token );
            return IdentityResult.Success;
        }
        catch ( Exception e )
        {
            return IdentityResult.Failed( new IdentityError
                                          {
                                              Description = e.Message,
                                          } );
        }
    }
}
