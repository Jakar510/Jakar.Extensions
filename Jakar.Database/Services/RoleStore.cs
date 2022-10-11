namespace Jakar.Database;


public sealed class RoleStore : IRoleStore<RoleRecord>
{
    private readonly Database _dbContext;


    public RoleStore( Database dbContext ) => _dbContext = dbContext;


    public static WebApplicationBuilder Register( WebApplicationBuilder builder )
    {
        builder.AddScoped<IRoleStore<RoleRecord>, RoleStore>();
        return builder;
    }
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


    public async Task<RoleRecord> FindByIdAsync( string   roleId, CancellationToken token ) => await _dbContext.Roles.Get( long.Parse( roleId ),    token ) ?? throw new RecordNotFoundException( roleId );
    public async Task<RoleRecord> FindByNameAsync( string name,   CancellationToken token ) => await _dbContext.Roles.Get( nameof(RoleRecord.Name), name, token ) ?? throw new RecordNotFoundException( name );


    public Task<string> GetNormalizedRoleNameAsync( RoleRecord role, CancellationToken token ) => Task.FromResult( role.Name );
    public Task<string> GetRoleIdAsync( RoleRecord             role, CancellationToken token ) => Task.FromResult( role.ID.ToString() );
    public Task<string> GetRoleNameAsync( RoleRecord           role, CancellationToken token ) => Task.FromResult( role.Name );

    public async Task SetNormalizedRoleNameAsync( RoleRecord role, string name, CancellationToken token ) =>
        await _dbContext.Roles.Update( role with
                                       {
                                           NormalizedName = name
                                       },
                                       token );

    public async Task SetRoleNameAsync( RoleRecord role, string name, CancellationToken token ) =>
        await _dbContext.Roles.Update( role with
                                       {
                                           Name = name
                                       },
                                       token );

    public async Task<IdentityResult> UpdateAsync( RoleRecord role, CancellationToken token )
    {
        try
        {
            await _dbContext.Roles.Update( role, token );
            return IdentityResult.Success;
        }
        catch (Exception e)
        {
            return IdentityResult.Failed( new IdentityError
                                          {
                                              Description = e.Message
                                          } );
        }
    }
}
