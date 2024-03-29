﻿// Jakar.Database ::  Jakar.Database 
// 04/11/2023  11:34 PM

namespace Jakar.Database;


public sealed class SignInManager( UserRecordManager userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<UserRecord> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager> logger, IAuthenticationSchemeProvider schemes, IUserConfirmation<UserRecord> confirmation )
    : SignInManager<UserRecord>( userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation );
