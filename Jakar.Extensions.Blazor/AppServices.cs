// Jakar.Extensions :: Jakar.Extensions.Blazor
// 09/26/2022  10:52 AM

using Blazored.Toast.Configuration;



namespace Jakar.Extensions.Blazor;


public class AppServices : BaseViewModel
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ILocalStorageService   _localStorage;
    private readonly IModalService          _modal;
    private readonly IToastService          _toastService;
    private readonly NavigationManager      _navigation;


    public string                  BaseUri               => _navigation.BaseUri;
    public ContextMenuService      ContextMenus          { get; }
    public DialogService           Dialogs               { get; }
    public NotificationService     Notifications         { get; }
    public ProtectedLocalStorage   ProtectedLocalStorage { get; }
    public ProtectedSessionStorage SessionStorage        { get; }
    public TooltipService          Tooltips              { get; }
    public string                  Uri                   => _navigation.Uri;


    public event Action? OnClearAll
    {
        add => _toastService.OnClearAll += value;
        remove => _toastService.OnClearAll -= value;
    }
    public event Action? OnClearCustomToasts
    {
        add => _toastService.OnClearCustomToasts += value;
        remove => _toastService.OnClearCustomToasts -= value;
    }
    public event Action<ToastLevel>? OnClearToasts
    {
        add => _toastService.OnClearToasts += value;
        remove => _toastService.OnClearToasts -= value;
    }
    public event Action<ToastLevel, RenderFragment, Action<ToastSettings>?> OnShow
    {
        add => _toastService.OnShow += value;
        remove => _toastService.OnShow -= value;
    }
    public event Action<Type, ToastParameters?, Action<ToastSettings>?>? OnShowComponent
    {
        add => _toastService.OnShowComponent += value;
        remove => _toastService.OnShowComponent -= value;
    }
    public event Action? OnClearQueue
    {
        add => _toastService.OnClearQueue += value;
        remove => _toastService.OnClearQueue -= value;
    }
    public event Action<ToastLevel>? OnClearQueueToasts
    {
        add => _toastService.OnClearQueueToasts += value;
        remove => _toastService.OnClearQueueToasts -= value;
    }
    public event EventHandler<LocalChangedEventArgs>? Changed
    {
        add => _localStorage.Changed += value;
        remove => _localStorage.Changed -= value;
    }
    public event EventHandler<LocalChangingEventArgs>? Changing
    {
        add => _localStorage.Changing += value;
        remove => _localStorage.Changing -= value;
    }


    public AppServices( IModalService           modal,
                        IToastService           toastService,
                        ILocalStorageService    localStorage,
                        IAuthenticationService  authenticationService,
                        ProtectedLocalStorage   protectedLocalStorage,
                        ProtectedSessionStorage sessionStorage,
                        NavigationManager       navigation,
                        ContextMenuService      contextMenus,
                        TooltipService          tooltips,
                        NotificationService     notifications,
                        DialogService           dialogs
    )
    {
        SessionStorage         = sessionStorage;
        ProtectedLocalStorage  = protectedLocalStorage;
        _navigation            = navigation;
        _modal                 = modal;
        _toastService          = toastService;
        _localStorage          = localStorage;
        _authenticationService = authenticationService;
        ContextMenus           = contextMenus;
        Tooltips               = tooltips;
        Notifications          = notifications;
        Dialogs                = dialogs;
    }


    public void NavigateTo( string uri ) => _navigation.NavigateTo( uri );
    public void NavigateTo( string uri, bool              forceLoad, bool replace = false ) => _navigation.NavigateTo( uri, forceLoad, replace );
    public void NavigateTo( string uri, NavigationOptions options ) => _navigation.NavigateTo( uri, options );


    public Task<AuthenticateResult> AuthenticateAsync( HttpContext context, string? scheme ) => _authenticationService.AuthenticateAsync( context, scheme );
    public Task ChallengeAsync( HttpContext                        context, string? scheme, AuthenticationProperties? properties ) => _authenticationService.ChallengeAsync( context, scheme, properties );
    public Task ForbidAsync( HttpContext                           context, string? scheme, AuthenticationProperties? properties ) => _authenticationService.ForbidAsync( context, scheme, properties );
    public Task SignInAsync( HttpContext                           context, string? scheme, ClaimsPrincipal           principal, AuthenticationProperties? properties ) => _authenticationService.SignInAsync( context, scheme, principal, properties );
    public Task SignOutAsync( HttpContext                          context, string? scheme, AuthenticationProperties? properties ) => _authenticationService.SignOutAsync( context, scheme, properties );


    public ValueTask ClearAsync( CancellationToken        token                          = default ) => _localStorage.ClearAsync( token );
    public ValueTask<bool> ContainKeyAsync( string        key,   CancellationToken token = default ) => _localStorage.ContainKeyAsync( key, token );
    public ValueTask<string> GetItemAsStringAsync( string key,   CancellationToken token = default ) => _localStorage.GetItemAsStringAsync( key, token );
    public ValueTask<T> GetItemAsync<T>( string           key,   CancellationToken token = default ) => _localStorage.GetItemAsync<T>( key, token );
    public ValueTask<string> KeyAsync( int                index, CancellationToken token = default ) => _localStorage.KeyAsync( index, token );

    public ValueTask<IEnumerable<string>> KeysAsync( CancellationToken token                         = default ) => _localStorage.KeysAsync( token );
    public ValueTask<int> LengthAsync( CancellationToken               token                         = default ) => _localStorage.LengthAsync( token );
    public ValueTask RemoveItemAsync( string                           key,  CancellationToken token = default ) => _localStorage.RemoveItemAsync( key, token );
    public ValueTask RemoveItemsAsync( IEnumerable<string>             keys, CancellationToken token = default ) => _localStorage.RemoveItemsAsync( keys, token );

    public ValueTask SetItemAsStringAsync( string key, string data, CancellationToken token = default ) => _localStorage.SetItemAsStringAsync( key, data, token );
    public ValueTask SetItemAsync<T>( string      key, T      data, CancellationToken token = default ) => _localStorage.SetItemAsync( key, data, token );


    public IModalReference Show<TComponent>() where TComponent : IComponent => _modal.Show<TComponent>();
    public IModalReference Show<TComponent>( string title ) where TComponent : IComponent => _modal.Show<TComponent>( title );
    public IModalReference Show<TComponent>( string title, ModalOptions    options ) where TComponent : IComponent => _modal.Show<TComponent>( title,                          options );
    public IModalReference Show<TComponent>( string title, ModalParameters parameters ) where TComponent : IComponent => _modal.Show<TComponent>( title,                       parameters );
    public IModalReference Show<TComponent>( string title, ModalParameters parameters, ModalOptions options ) where TComponent : IComponent => _modal.Show<TComponent>( title, parameters, options );
    public IModalReference Show( Type               component ) => _modal.Show( component );
    public IModalReference Show( Type               component, string title ) => _modal.Show( component,                                                   title );
    public IModalReference Show( Type               component, string title, ModalOptions    options ) => _modal.Show( component,                          title, options );
    public IModalReference Show( Type               component, string title, ModalParameters parameters ) => _modal.Show( component,                       title, parameters );
    public IModalReference Show( Type               component, string title, ModalParameters parameters, ModalOptions options ) => _modal.Show( component, title, parameters, options );


    public void ShowInfo( string            message, Action<ToastSettings>? settings                                 = null ) => _toastService.ShowInfo( message, settings );
    public void ShowInfo( RenderFragment    message, Action<ToastSettings>? settings                                 = null ) => _toastService.ShowInfo( message, settings );
    public void ShowSuccess( string         message, Action<ToastSettings>? settings                                 = null ) => _toastService.ShowSuccess( message, settings );
    public void ShowSuccess( RenderFragment message, Action<ToastSettings>? settings                                 = null ) => _toastService.ShowSuccess( message, settings );
    public void ShowWarning( string         message, Action<ToastSettings>? settings                                 = null ) => _toastService.ShowWarning( message, settings );
    public void ShowWarning( RenderFragment message, Action<ToastSettings>? settings                                 = null ) => _toastService.ShowWarning( message, settings );
    public void ShowError( string           message, Action<ToastSettings>? settings                                 = null ) => _toastService.ShowError( message, settings );
    public void ShowError( RenderFragment   message, Action<ToastSettings>? settings                                 = null ) => _toastService.ShowError( message, settings );
    public void ShowToast( ToastLevel       level,   string                 message, Action<ToastSettings>? settings = null ) => _toastService.ShowToast( level, message, settings );
    public void ShowToast( ToastLevel       level,   RenderFragment         message, Action<ToastSettings>? settings = null ) => _toastService.ShowToast( level, message, settings );
    public void ShowToast<TComponent>() where TComponent : IComponent => _toastService.ShowToast<TComponent>();
    public void ShowToast<TComponent>( ToastParameters       parameters ) where TComponent : IComponent => _toastService.ShowToast<TComponent>( parameters );
    public void ShowToast<TComponent>( Action<ToastSettings> settings ) where TComponent : IComponent => _toastService.ShowToast<TComponent>( settings );
    public void ShowToast<TComponent>( ToastParameters       parameters, Action<ToastSettings> settings ) where TComponent : IComponent => _toastService.ShowToast<TComponent>( parameters, settings );
    public void ClearAll() => _toastService.ClearAll();
    public void ClearCustomToasts() => _toastService.ClearCustomToasts();
    public void ClearQueue() => _toastService.ClearQueue();
    public void ClearQueueToasts( ToastLevel toastLevel ) => _toastService.ClearQueueToasts( toastLevel );
    public void ClearQueueWarningToasts() => _toastService.ClearQueueWarningToasts();
    public void ClearQueueInfoToasts() => _toastService.ClearQueueInfoToasts();
    public void ClearQueueSuccessToasts() => _toastService.ClearQueueSuccessToasts();
    public void ClearQueueErrorToasts() => _toastService.ClearQueueErrorToasts();
    public void ClearErrorToasts() => _toastService.ClearErrorToasts();
    public void ClearInfoToasts() => _toastService.ClearInfoToasts();
    public void ClearSuccessToasts() => _toastService.ClearSuccessToasts();
    public void ClearToasts( ToastLevel toastLevel ) => _toastService.ClearToasts( toastLevel );
    public void ClearWarningToasts() => _toastService.ClearWarningToasts();
}
