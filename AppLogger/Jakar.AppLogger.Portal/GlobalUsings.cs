// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 09/12/2022  9:35 AM

global using System.Collections.ObjectModel;
global using System.ComponentModel.DataAnnotations;
global using System.Data.Common;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Reflection;
global using System.Runtime.InteropServices;
global using System.Runtime.Versioning;
global using System.Web.Hosting;
global using Dapper;
global using Dapper.Contrib.Extensions;
global using Jakar.AppLogger.Common;
global using Jakar.AppLogger.Portal;
global using Jakar.AppLogger.Portal.Data;
global using Jakar.AppLogger.Portal.Data.Interfaces;
global using Jakar.AppLogger.Portal.Data.Tables;
global using Jakar.AppLogger.Portal.Shared;
global using Jakar.Database;
global using Jakar.Database.DbMigrations;
global using Jakar.Extensions;
global using Jakar.Extensions.Blazor;
global using Jakar.Extensions.Hosting;
global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.Components.Server.Circuits;
global using Microsoft.AspNetCore.Components.Web;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.Extensions.Options;
global using Npgsql;
global using Radzen;
global using Radzen.Blazor;
global using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
global using ISession = Jakar.AppLogger.Portal.Data.Interfaces.ISession;
