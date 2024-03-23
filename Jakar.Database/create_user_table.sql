CREATE TABLE "public"."Users" (
	"ID" uuid NOT NULL
	, "DateCreated" timestamptz NOT NULL DEFAULT(now() at TIME zone 'UTC')
	, "LastModified" timestamptz
	, "UserName" VARCHAR(256) NOT NULL
	, "FirstName" VARCHAR(256) NOT NULL
	, "LastName" VARCHAR(256) NOT NULL
	, "FullName" VARCHAR(512) NOT NULL
	, "Description" VARCHAR(256) NOT NULL
	, "SessionID" uuid
	, "UserID" uuid NOT NULL
	, "Gender" VARCHAR(256)
	, "Website" VARCHAR(4096) NOT NULL
	, "Email" VARCHAR(1024) NOT NULL
	, "PhoneNumber" VARCHAR(256) NOT NULL
	, "Ext" VARCHAR(256) NOT NULL
	, "Title" VARCHAR(256) NOT NULL
	, "Department" VARCHAR(256) NOT NULL
	, "Company" VARCHAR(256) NOT NULL
	, "PreferredLanguage" VARCHAR(256) NOT NULL
	, "EscalateTo" uuid
	, "IsActive" boolean NOT NULL
	, "LastLogin" timestamptz
	, "IsDisabled" boolean NOT NULL
	, "IsLocked" boolean NOT NULL
	, "BadLogins" BIGINT NOT NULL
	, "LastBadAttempt" timestamptz
	, "LockDate" timestamptz
	, "LockoutEnd" timestamptz
	, "RefreshToken" NVARCHAR(256000)
	, "RefreshTokenExpiryTime" timestamptz
	, "AuthenticatorKey" NVARCHAR(256000)
	, "IsEmailConfirmed" boolean NOT NULL
	, "IsPhoneNumberConfirmed" boolean NOT NULL
	, "IsTwoFactorEnabled" boolean NOT NULL
	, "SecurityStamp" NVARCHAR(256000) NOT NULL
	, "ConcurrencyStamp" TEXT(MAX) NOT NULL
	, "AdditionalData" JSON
	, "PasswordHash" NVARCHAR(256000) NOT NULL
	, "Rights" NVARCHAR(256000) NOT NULL
	, CONSTRAINT "PK_Users" PRIMARY KEY ("ID")
	);
