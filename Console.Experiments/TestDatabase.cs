/*
// Jakar.Extensions :: Experiments
// 09/28/2023  10:02 AM

using System.Data.Common;
using FluentMigrator.Runner;
using Jakar.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;



namespace Experiments;


public static class ULongHashTests
{
    private static readonly Random _random      = Random.Shared;
    private const           string ALPHANUMERIC = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";


    private static IEnumerable<string> GetRandomStrings( int count, int length )
    {
        for ( int i = 0; i < count; i++ ) { yield return GetRandomString( length ); }
    }
    private static string GetRandomString( int length )
    {
        Span<char> span = stackalloc char[length];

        for ( int i = 0; i < length; i++ )
        {
            char c = ALPHANUMERIC[_random.Next( ALPHANUMERIC.Length )];
            span[i] = c;
        }

        return span.ToString();
    }
    public static int Run( in int max_tries )
    {
        var hashes = new HashSet<int>( max_tries );

        for ( int i = 0; i < max_tries; i++ )
        {
            int count = _random.Next( ALPHANUMERIC.Length / 4 );
            int size  = _random.Next( count );

            int hash = GetRandomStrings( count, size ).GetHash();

            bool added = hashes.Add( hash );
            if ( added is false && hashes.Count > 0 ) { return i; }
        }

        return hashes.Count;
    }
}



public sealed class TestDatabase : Database
{
    // private const string CONNECTION_STRING = "Server=localhost;Database=Experiments;User Id=tester;Password=tester;Encrypt=True;TrustServerCertificate=True";
    private const string CONNECTION_STRING = "User ID=dev;Password=jetson;Host=localhost;Port=5432;Database=Experiments";


    public TestDatabase( IConfiguration                                configuration, ISqlCacheFactory sqlCacheFactory, IOptions<DbOptions> options ) : base( configuration, sqlCacheFactory, options ) => ConnectionString = CONNECTION_STRING;
    protected override DbConnection CreateConnection( in SecuredString secure ) => new NpgsqlConnection( secure );


    public static async Task Test()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.AddDefaultLogging<TestDatabase>( true );

        builder.Services.AddOptions<DbOptions>()
               .Configure( db =>
                           {
                               db.DbType           = DbInstance.Postgres;
                               db.ConnectionString = new SecuredString( CONNECTION_STRING.ToSecureString() );
                               db.TokenAudience    = nameof(TestDatabase);
                               db.TokenIssuer      = nameof(TestDatabase);
                           } );

        builder.Services.AddSingleton<TestDatabase>();
        builder.Services.AddTransient<Database>( provider => provider.GetRequiredService<TestDatabase>() );


        builder.Services.AddFluentMigratorCore()
               .ConfigureRunner( configure =>
                                 {
                                     configure.AddPostgres();

                                     // configure.AddSqlServer2016();

                                     DbOptions.GetConnectionString( configure );

                                     configure.ScanIn( typeof(Database).Assembly, typeof(Program).Assembly ).For.All();
                                 } );

        await using WebApplication app = builder.Build();

        try
        {
            await using ( AsyncServiceScope scope = app.Services.CreateAsyncScope() )
            {
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                runner.ListMigrations();

                if ( runner.HasMigrationsToApplyUp() ) { runner.MigrateUp(); }
            }


            await using ( AsyncServiceScope scope = app.Services.CreateAsyncScope() )
            {
                var               db    = scope.ServiceProvider.GetRequiredService<TestDatabase>();
                CancellationToken token = default;
                var               admin = UserRecord.Create( "Admin", "Admin", string.Empty );
                var               user  = UserRecord.Create( "User",  "User",  string.Empty, admin );

                ImmutableList<UserRecord> users   = ImmutableList.Create( admin, user );
                var                       results = new List<UserRecord>( users.Count );

                await foreach ( UserRecord record in db.Users.Insert( users, token ) ) { results.Add( record ); }

                Debug.Assert( users.Count == results.Count );

                results.Clear();
                await foreach ( UserRecord record in db.Users.All( token ) ) { results.Add( record ); }

                Debug.Assert( users.Count == results.Count );
            }
        }
        finally
        {
        #if DEBUG
            if ( app.Configuration.GetValue( "DISPATCH_DOWN", true ) )
            {
                await using AsyncServiceScope scope  = app.Services.CreateAsyncScope();
                var                           runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                runner.MigrateDown( 0 );
            }
        #endif
        }
    }
}
*/


