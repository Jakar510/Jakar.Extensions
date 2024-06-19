// Jakar.Extensions :: Jakar.Extensions.Blazor
// 09/26/2022  10:52 AM

namespace Jakar.Extensions.Blazor;


[SuppressMessage( "ReSharper", "NotAccessedPositionalProperty.Global" )]
public sealed record BlazorServices( IModalService           Modal,
                                     IToastService           Toasts,
                                     ILocalStorageService    LocalStorage,
                                     ProtectedLocalStorage   ProtectedLocalStorage,
                                     ProtectedSessionStorage SessionStorage,
                                     ContextMenuService      ContextMenus,
                                     TooltipService          Tooltips,
                                     NotificationService     Notifications,
                                     DialogService           Dialogs,
                                     NavigationManager       Navigation );