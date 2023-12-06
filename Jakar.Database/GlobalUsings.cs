// Jakar.Extensions :: Jakar.Database
// 08/14/2022  10:46 PM

global using System.Collections;
global using System.Collections.Concurrent;
global using System.Collections.Immutable;
global using System.Collections.ObjectModel;
global using System.Collections.Specialized;
global using System.ComponentModel;
global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel.DataAnnotations.Schema;
global using System.Data;
global using System.Data.Common;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Globalization;
global using System.IdentityModel.Tokens.Jwt;
global using System.Linq;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using System.Runtime.Versioning;
global using System.Security.Claims;
global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Nodes;
global using System.Text.Json.Serialization;
global using System.Text.Json.Serialization.Metadata;
global using System.Text.RegularExpressions;
global using Dapper;
global using Dapper.Contrib.Extensions;
global using FluentMigrator.Runner.Logging;
global using FluentMigrator;
global using FluentMigrator.Builders;
global using FluentMigrator.Builders.Alter.Table;
global using FluentMigrator.Builders.Create.Table;
global using FluentMigrator.Builders.Insert;
global using FluentMigrator.Builders.Schema.Schema;
global using FluentMigrator.Builders.Schema.Table;
global using FluentMigrator.Builders.Update;
global using FluentMigrator.Infrastructure;
global using FluentMigrator.Infrastructure.Extensions;
global using FluentMigrator.Runner;
global using FluentMigrator.SqlServer;
global using Newtonsoft.Json;
global using Newtonsoft.Json.Linq;
global using Swashbuckle.AspNetCore.SwaggerGen;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Sigil;
global using Jakar.Database;
global using Jakar.Database.Caches;
global using Jakar.Database.DbMigrations;
global using Jakar.Extensions;
global using JetBrains.Annotations;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.ModelBinding;
global using Microsoft.CodeAnalysis;
global using Microsoft.IdentityModel.Tokens;
global using OneOf;
global using CommandFlags = Dapper.CommandFlags;
global using ConnectionStringOptions =
    OneOf.OneOf<Jakar.Extensions.SecuredString, System.Func<Jakar.Extensions.SecuredString>, System.Func<System.Threading.CancellationToken, System.Threading.Tasks.Task<Jakar.Extensions.SecuredString>>,
        System.Func<System.Threading.CancellationToken, System.Threading.Tasks.ValueTask<Jakar.Extensions.SecuredString>>, System.Func<Microsoft.Extensions.Configuration.IConfiguration, Jakar.Extensions.SecuredString>,
        System.Func<Microsoft.Extensions.Configuration.IConfiguration, System.Threading.CancellationToken, Jakar.Extensions.SecuredString>,
        System.Func<Microsoft.Extensions.Configuration.IConfiguration, System.Threading.CancellationToken, System.Threading.Tasks.ValueTask<Jakar.Extensions.SecuredString>>,
        System.Func<Microsoft.Extensions.Configuration.IConfiguration, System.Threading.CancellationToken, System.Threading.Tasks.Task<Jakar.Extensions.SecuredString>>>;
global using JsonSerializer_ = System.Text.Json.JsonSerializer;
global using KeyAttribute = Dapper.Contrib.Extensions.KeyAttribute;
global using MsJsonTypeInfo = System.Text.Json.Serialization.Metadata.JsonTypeInfo;
global using TableAttribute = Dapper.Contrib.Extensions.TableAttribute;
