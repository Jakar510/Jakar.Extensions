﻿#if DEBUG
using Jakar.Database.Auth;
using Microsoft.AspNetCore.Authorization;



namespace Jakar.Database;


// TODO: asp.net authorization dapper
public static partial class DbExtensions
{
    public static WebApplicationBuilder AddAuth( this WebApplicationBuilder builder )
    {
        builder.Services.AddHttpContextAccessor();
        builder.AddTransient<IAuthorizationService, AuthorizationService>();
        return builder;
    }
}



#endif
