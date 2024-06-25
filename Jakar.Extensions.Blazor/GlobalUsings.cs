// Jakar.Extensions :: Jakar.Extensions.Blazor
// 08/15/2022  12:34 PM

global using System;
global using System.Collections.Generic;
global using System.ComponentModel;
global using System.Diagnostics.CodeAnalysis;
global using System.Security.Claims;
global using System.Threading;
global using System.Threading.Tasks;
global using Blazored.LocalStorage;
global using Blazored.LocalStorage.StorageOptions;
global using Blazored.Modal;
global using Blazored.Modal.Services;
global using Blazored.SessionStorage;
global using Blazored.SessionStorage.StorageOptions;
global using Blazored.Toast;
global using Blazored.Toast.Services;
global using Jakar.Extensions.UserGuid;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc.ModelBinding;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.DependencyInjection.Extensions;
global using Microsoft.Extensions.Logging;
global using Radzen;
global using LocalChangedEventArgs = Blazored.LocalStorage.ChangedEventArgs;
global using LocalChangingEventArgs = Blazored.LocalStorage.ChangingEventArgs;
global using SessionChangedEventArgs = Blazored.SessionStorage.ChangedEventArgs;
global using SessionChangingEventArgs = Blazored.SessionStorage.ChangingEventArgs;
