using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;



namespace Jakar.Database;


[DoNotRename, SuppressMessage( "ReSharper", "ConvertToAutoPropertyWithPrivateSetter" )]
public sealed class AuthStateProvider( AppServices appServices, Database dbContext, ILocalStorageService localStorage ) : AuthenticationStateProvider, IHostEnvironmentAuthenticationStateProvider
{
    public const     string                       AUTHENTICATION_TYPE = nameof(AuthStateProvider);
    public const     string                       CURRENT_USER        = nameof(CURRENT_USER);
    public const     string                       USER_ID_KEY         = $"{USER_KEY}.{nameof(UserModel.UserID)}";
    public const     string                       USER_KEY            = nameof(UserModel);
    private readonly AppServices                  _appServices        = appServices;
    private readonly Database                     _dbContext          = dbContext;
    private readonly ILocalStorageService         _localStorage       = localStorage;
    private readonly WeakEventManager<UserModel?> _manager            = new();
    private          Task<AuthenticationState>?   _authenticationStateTask;
    private          UserModel?                   _currentUser;


    public static AuthenticationState Anonymous             { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; } = new(new ClaimsPrincipal( new ClaimsIdentity() ));
    public        UserModel?          CurrentUser           { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _currentUser; }
    public        HttpRequestHeaders? DefaultRequestHeaders { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; }


    public event Action<UserModel?> LoginChanged { add => _manager.AddEventHandler( value ); remove => _manager.RemoveEventHandler( value ); }


    public async ValueTask Init()
    {
        ProtectedBrowserStorageResult<UserModel> user = await _appServices.ProtectedLocalStorage.GetAsync<UserModel>( USER_KEY );
        await NotifyUserAuthentication( user.Value );
    }
    public ValueTask ExecuteLogin( Widget widget, VerifyRequest request, CancellationToken token = default ) => _dbContext.TryCall( ExecuteLogin, widget, request, token );
    public async ValueTask ExecuteLogin( DbConnection connection, DbTransaction transaction, Widget widget, VerifyRequest request, CancellationToken token = default )
    {
        widget.ErrorText = default;

        try
        {
            Debug.Assert( _dbContext is not null, "Database is null" );
            OneOf<UserRecord, Error> caller = await _dbContext.VerifyUser( connection, transaction, request, BaseController.Trap, token );

            if ( caller.IsT1 )
            {
                widget.ModelState = caller.AsT1.GetState();
                widget.ErrorText  = "There was an error when trying to log in";
                return;
            }

            var user = caller.AsT0.ToUserModel();
            await NotifyUserAuthentication( user );
            _appServices.Navigation.NavigateTo( "/Home" );
        }
        catch ( Exception e )
        {
            e.WriteToDebug();
            widget.ErrorText = e.Message;
        }
    }
    public async Task ExecuteRegister( Widget widget, CreateUserDetails model, CancellationToken token = default )
    {
        widget.ErrorText = default;

        try
        {
            OneOf<UserModel, Error> caller = await _dbContext.TryCall( Register, model, token );

            if ( caller.IsT1 )
            {
                widget.ModelState = caller.AsT1.GetState();
                widget.ErrorText  = "The registration worked but there was an error when trying to log in.";
                return;
            }

            UserModel user = caller.AsT0;
            await NotifyUserAuthentication( user );
            _appServices.Navigation.NavigateTo( "/Home" );
        }
        catch ( Exception e )
        {
            e.WriteToDebug();
            widget.ErrorText = e.Message;
        }
    }
    private async ValueTask<OneOf<UserModel, Error>> Register( DbConnection connection, DbTransaction transaction, CreateUserDetails details, CancellationToken token )
    {
        RecordID<UserRecord> id = await _dbContext.Users.GetIdOfRow( connection, transaction, true, UserRecord.GetDynamicParameters( details ), token );
        if ( id.IsValid() ) { return new Error( Status.Conflict, [details.UserID.ToString()] ); }

        UserRecord record = UserRecord.Register( details, UserRecord.Manager );
        record = await _dbContext.Users.Insert( connection, transaction, record, token );
        return record.ToUserModel();
    }
    public ValueTask Logout() => NotifyUserAuthentication( default(UserModel) );


    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if ( _authenticationStateTask is not null ) { return await _authenticationStateTask; }

        string? token = await _localStorage.GetItemAsync<string>( JwtBearerDefaults.AuthenticationScheme );

        return string.IsNullOrWhiteSpace( token )
                   ? Anonymous
                   : new AuthenticationState( new ClaimsPrincipal( new ClaimsIdentity( JwtParser.ParseClaimsFromJwt( token ), AUTHENTICATION_TYPE ) ) );
    }


    public async ValueTask NotifyUserAuthentication( UserModel? user )
    {
        _currentUser = user;
        _manager.RaiseEvent( this, user, nameof(LoginChanged) );

        if ( user is null )
        {
            NotifyUserAuthentication( Anonymous );
            await _localStorage.RemoveItemAsync( JwtBearerDefaults.AuthenticationScheme );
            await _appServices.ProtectedLocalStorage.DeleteAsync( USER_ID_KEY );
            await _appServices.ProtectedLocalStorage.DeleteAsync( USER_KEY );
        }
        else
        {
            NotifyUserAuthentication( new ClaimsIdentity( UserModel.GetClaims( user ), AUTHENTICATION_TYPE ) );
            await _appServices.ProtectedLocalStorage.SetAsync( USER_ID_KEY, user.UserID.ToString() );
            await _appServices.ProtectedLocalStorage.SetAsync( USER_KEY,    user );
        }
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public void NotifyUserAuthentication( string              token )     => NotifyUserAuthentication( new ClaimsIdentity( JwtParser.ParseClaimsFromJwt( token ), AUTHENTICATION_TYPE ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public void NotifyUserAuthentication( ClaimsIdentity      identity )  => NotifyUserAuthentication( new ClaimsPrincipal( identity ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public void NotifyUserAuthentication( ClaimsPrincipal     principal ) => NotifyUserAuthentication( new AuthenticationState( principal ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public void NotifyUserAuthentication( AuthenticationState state )     => SetAuthenticationState( Task.FromResult( state ) );

    public void SetAuthenticationState( Task<AuthenticationState> state )
    {
        _authenticationStateTask = state;
        NotifyAuthenticationStateChanged( _authenticationStateTask );
    }
}
