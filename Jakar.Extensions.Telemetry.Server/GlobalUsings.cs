// Jakar.Extensions :: Jakar.Extensions.Telemetry.Server
// 06/25/2024  11:06

global using System.Data;
global using System.Data.Common;
global using System.Diagnostics;
global using System.Net.Http;
global using System.Net.Http.Json;
global using System.Runtime.CompilerServices;
global using System.Text;
global using Dapper.Contrib.Extensions;
global using Jakar.Database;
global using Jakar.Extensions.Blazor;
global using Jakar.Extensions.Telemetry;
global using Jakar.Extensions.Telemetry.Server;
global using Jakar.Extensions.Telemetry.Server.Data;
global using Jakar.Extensions.Telemetry.Server.Components;
global using Jakar.Extensions.Telemetry.Server.Components.Pages;
global using Jakar.Extensions.Telemetry.Server.Components.Layout;
global using Jakar.Extensions.Telemetry.Server.Data.Tables;
global using Microsoft.AspNetCore.Components.Forms;
global using Microsoft.AspNetCore.Components.Routing;
global using Microsoft.AspNetCore.Components.Web;
global using Microsoft.AspNetCore.Components.Web.Virtualization;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.SignalR;
global using Microsoft.JSInterop;
global using Syncfusion.Blazor;
global using static Microsoft.AspNetCore.Components.Web.RenderMode;
