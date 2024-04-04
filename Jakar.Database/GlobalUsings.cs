// Jakar.Extensions :: Jakar.Database
// 08/14/2022  10:46 PM

global using System.Buffers;
global using System.Collections;
global using System.Collections.Concurrent;
global using System.Collections.Frozen;
global using System.Collections.Immutable;
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
global using System.Net;
global using System.Net.Mail;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using System.Runtime.Versioning;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.Text;
global using System.Text.RegularExpressions;
global using CommunityToolkit.Diagnostics;
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
global using Jakar.Extensions.UserGuid;
global using JetBrains.Annotations;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authentication.Cookies;
global using Microsoft.AspNetCore.Authentication.Google;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
global using Microsoft.AspNetCore.Authentication.OpenIdConnect;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.DataProtection;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.ModelBinding;
global using Microsoft.CodeAnalysis;
global using Microsoft.Extensions.Caching.Distributed;
global using Microsoft.Extensions.Caching.StackExchangeRedis;
global using Microsoft.Extensions.Configuration.CommandLine;
global using Microsoft.Extensions.Configuration.Ini;
global using Microsoft.Extensions.Configuration.Json;
global using Microsoft.Extensions.FileProviders;
global using Microsoft.Extensions.Logging.Console;
global using Microsoft.Extensions.Logging.Debug;
global using Microsoft.Extensions.Logging.EventLog;
global using Microsoft.Identity.Web;
global using Microsoft.IdentityModel.JsonWebTokens;
global using Microsoft.IdentityModel.Tokens;
global using MimeKit;
global using Newtonsoft.Json.Serialization;
global using OneOf;
global using OtpNet;
global using ZXing;
global using ZXing.QrCode;
global using ZXing.QrCode.Internal;
global using CommandFlags = Dapper.CommandFlags;
global using FormatException = System.FormatException;
global using IDataProtector = Jakar.Extensions.IDataProtector;
global using IProtectedData = Microsoft.AspNetCore.DataProtection.IDataProtector;
global using JsonIgnoreAttribute = Newtonsoft.Json.JsonIgnoreAttribute;
global using JsonSerializer = Newtonsoft.Json.JsonSerializer;
global using KeyAttribute = Dapper.Contrib.Extensions.KeyAttribute;
global using SmtpClient = MailKit.Net.Smtp.SmtpClient;
global using TableAttribute = Dapper.Contrib.Extensions.TableAttribute;
global using Version = System.Version;
