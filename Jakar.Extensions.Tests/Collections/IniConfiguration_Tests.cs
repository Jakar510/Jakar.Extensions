using System.Net;


#pragma warning disable IDE0071



namespace Jakar.Extensions.Tests.Collections;


[TestFixture, TestOf( typeof(IniConfig) )]

// ReSharper disable once InconsistentNaming
public class IniConfig_Tests : Assert
{
    private const           string    APP_NAME = "AppName";
    private const           string    PROJECT  = "Project";
    private const           string    SERVER   = "Server";
    private static readonly IniConfig __ini     = GetConfig();


    private static IniConfig GetConfig()
    {
        IniConfig ini = new();

        IniConfig.Section project = ini[PROJECT];
        project[APP_NAME] = nameof(IniConfig_Tests);
        project.Add( nameof(DateTime),       DateTime.Now );
        project.Add( nameof(DateTimeOffset), DateTimeOffset.UtcNow );
        project.Add( nameof(Guid),           Guid.CreateVersion7() );
        project.Add( nameof(AppVersion),     new AppVersion( 1, 2, 3, 4, 5, 6, AppVersionFlags.Stable ) );

        IniConfig.Section server = ini[SERVER];
        server[APP_NAME]          = nameof(ServicePoint);
        server[nameof(Uri.Port)]  = GetRandomPort().ToString();
        server[nameof(IPAddress)] = GetRandomIpAddress();

        ini[nameof(Random)].Add( nameof(Random.Next), Random.Shared.Next() );
        ini[nameof(IniConfig_Tests)].Add( nameof(Random.Next), Random.Shared.Next() );
        return ini;
    }
    private static int    GetRandomPort()      => Random.Shared.Next( IPEndPoint.MinPort, IPEndPoint.MaxPort );
    private static string GetRandomIpAddress() => string.Join( '.', Random.Shared.Next( 255 ), Random.Shared.Next( 255 ), Random.Shared.Next( 255 ), Random.Shared.Next( 255 ) );


    [Test]
    public void Test()
    {
        string actual = __ini.ToString();
        $"-- {nameof(actual)} --\n{actual}".WriteToConsole();
        IniConfig results = IniConfig.Parse( actual );

        $"-- {nameof(results)} --\n{results.ToString()}".WriteToConsole();

        this.NotNull( results );
        this.AreEqual( results, __ini );
    }
}
