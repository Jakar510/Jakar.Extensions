// Jakar.Extensions :: Jakar.Extensions.Blazor
// 09/26/2022  10:52 AM

using Blazored.LocalStorage;
using Blazored.Modal.Services;
using Blazored.Toast.Services;
using MudBlazor;
using MudBlazor.Services;
using Radzen;
using DialogService = Radzen.DialogService;



namespace Jakar.Extensions.Blazor;


public sealed class BlazorServices( ProtectedLocalStorage protectedLocalStorage, ProtectedSessionStorage sessionStorage, NavigationManager navigation, BlazoredServices blazored, RadzenServices radzen, MudServices mud )
{
    public readonly ProtectedLocalStorage   protectedLocalStorage = protectedLocalStorage;
    public readonly ProtectedSessionStorage sessionStorage        = sessionStorage;
    public readonly NavigationManager       navigation            = navigation;
    public readonly BlazoredServices        blazored              = blazored;
    public readonly RadzenServices          radzen                = radzen;
    public readonly MudServices             mud                   = mud;


    public static BlazorServices Create( IServiceProvider provider )
    {
        ProtectedLocalStorage   protectedLocalStorage = provider.GetRequiredService<ProtectedLocalStorage>();
        ProtectedSessionStorage sessionStorage        = provider.GetRequiredService<ProtectedSessionStorage>();
        NavigationManager       navigation            = provider.GetRequiredService<NavigationManager>();
        BlazoredServices        blazored              = BlazoredServices.Create( provider );
        RadzenServices          radzen                = RadzenServices.Create( provider );
        MudServices             mud                   = MudServices.Create( provider );

        return new BlazorServices( protectedLocalStorage, sessionStorage, navigation, blazored, radzen, mud );
    }
}



public sealed class BlazoredServices( IModalService modal, IToastService toasts, ILocalStorageService localStorage )
{
    public readonly IModalService        modal        = modal;
    public readonly IToastService        toasts       = toasts;
    public readonly ILocalStorageService localStorage = localStorage;


    public static   BlazoredServices     Create( IServiceProvider provider ) => new(provider.GetRequiredService<IModalService>(), provider.GetRequiredService<IToastService>(), provider.GetRequiredService<ILocalStorageService>());
}



public sealed class RadzenServices( ContextMenuService contextMenus, TooltipService tooltips, NotificationService notifications, DialogService dialogs )
{
    public readonly ContextMenuService  contextMenus  = contextMenus;
    public readonly TooltipService      tooltips      = tooltips;
    public readonly NotificationService notifications = notifications;
    public readonly DialogService       dialogs       = dialogs;


    public static   RadzenServices      Create( IServiceProvider provider ) => new(provider.GetRequiredService<ContextMenuService>(), provider.GetRequiredService<TooltipService>(), provider.GetRequiredService<NotificationService>(), provider.GetRequiredService<DialogService>());
}



public sealed class MudServices( CookieThemeService                cookieTheme,
                                 QueryStringThemeService           queryStringTheme,
                                 IDialogService                    mudDialogs,
                                 ISnackbar                         snackbar,
                                 IBrowserViewportService browserViewport,
                                 IResizeObserver                   resizeObserver,
                                 IKeyInterceptorService            keyInterceptor,
                                 IJsEventFactory                   jsEventFactory,
                                 IJsApiService                     jsApi,
                                 IScrollManager                    scrollManager,
                                 IScrollSpyFactory                 scrollSpyFactory,
                                 IScrollListenerFactory            scrollListenerFactory,
                                 IEventListenerFactory             eventListenerFactory,
                                 ILocalizationInterceptor          localizationInterceptor,
                                 ILocalizationEnumInterceptor      localizationEnumInterceptor,
                                 IPopoverService                   popover )
{
    public readonly CookieThemeService           cookieTheme                 = cookieTheme;
    public readonly QueryStringThemeService      queryStringTheme            = queryStringTheme;
    public readonly IDialogService               mudDialogs                  = mudDialogs;
    public readonly ISnackbar                    snackbar                    = snackbar;
    public readonly IBrowserViewportService      browserViewport             = browserViewport;
    public readonly IResizeObserver              resizeObserver              = resizeObserver;
    public readonly IKeyInterceptorService       keyInterceptor              = keyInterceptor;
    public readonly IJsEventFactory              jsEventFactory              = jsEventFactory;
    public readonly IJsApiService                jsApi                       = jsApi;
    public readonly IScrollManager               scrollManager               = scrollManager;
    public readonly IScrollSpyFactory            scrollSpyFactory            = scrollSpyFactory;
    public readonly IScrollListenerFactory       scrollListenerFactory       = scrollListenerFactory;
    public readonly IEventListenerFactory        eventListenerFactory        = eventListenerFactory;
    public readonly ILocalizationInterceptor     localizationInterceptor     = localizationInterceptor;
    public readonly ILocalizationEnumInterceptor localizationEnumInterceptor = localizationEnumInterceptor;
    public readonly IPopoverService              popover                     = popover;


    public static MudServices Create( IServiceProvider provider ) => new(provider.GetRequiredService<CookieThemeService>(),
                                                                         provider.GetRequiredService<QueryStringThemeService>(),
                                                                         provider.GetRequiredService<IDialogService>(),
                                                                         provider.GetRequiredService<ISnackbar>(),
                                                                         provider.GetRequiredService<IBrowserViewportService>(),
                                                                         provider.GetRequiredService<IResizeObserver>(),
                                                                         provider.GetRequiredService<IKeyInterceptorService>(),
                                                                         provider.GetRequiredService<IJsEventFactory>(),
                                                                         provider.GetRequiredService<IJsApiService>(),
                                                                         provider.GetRequiredService<IScrollManager>(),
                                                                         provider.GetRequiredService<IScrollSpyFactory>(),
                                                                         provider.GetRequiredService<IScrollListenerFactory>(),
                                                                         provider.GetRequiredService<IEventListenerFactory>(),
                                                                         provider.GetRequiredService<ILocalizationInterceptor>(),
                                                                         provider.GetRequiredService<ILocalizationEnumInterceptor>(),
                                                                         provider.GetRequiredService<IPopoverService>());
}
