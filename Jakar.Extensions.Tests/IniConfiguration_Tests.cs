using System.Net;


#pragma warning disable IDE0071



namespace Jakar.Extensions.Tests;


[TestFixture, TestOf( typeof(IniConfig) )]

// ReSharper disable once InconsistentNaming
public class IniConfig_Tests : Assert
{
    [Test]
    public void Test()
    {
        IniConfig.Section project = new IniConfig.Section( "Project" ) { ["AppName"] = nameof(IniConfig_Tests) };

        project.Add( nameof(DateTime),       DateTime.Now );
        project.Add( nameof(DateTimeOffset), DateTimeOffset.UtcNow );
        project.Add( nameof(Guid),           Guid.NewGuid() );
        project.Add( nameof(AppVersion),     new AppVersion( 1, 2, 3, 4, 5, 6, AppVersionFlags.Stable ) );


        IniConfig.Section server = new IniConfig.Section( "Server" ) { ["AppName"] = nameof(ServicePoint) };

        server.Add( "Port", Random.Shared.Next( IPEndPoint.MinPort, IPEndPoint.MaxPort ) );


        server.Add( nameof(IPAddress), string.Join( '.', Random.Shared.Next( 255 ), Random.Shared.Next( 255 ), Random.Shared.Next( 255 ), Random.Shared.Next( 255 ) ) );

        IniConfig ini = new IniConfig
                        {
                            project,
                            server
                        };

        ini[nameof(Random)].Add( nameof(Random.Next), Random.Shared.Next() );

        ini[nameof(IniConfig_Tests)].Add( nameof(Random.Next), Random.Shared.Next() );


        string actual = ini.ToString();
        $"-- {nameof(actual)} --\n{actual}".WriteToConsole();
        IniConfig results = IniConfig.Parse( actual );

        $"-- {nameof(results)} --\n{results.ToString()}".WriteToConsole();


        this.NotNull( results );
        this.AreEqual( results,                  ini );
        this.AreEqual( results[nameof(project)], project );
    }
}
