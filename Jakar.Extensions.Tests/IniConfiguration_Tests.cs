#nullable enable
using System;
using System.Net;
using System.Text;
using NUnit.Framework;



namespace Jakar.Extensions.Tests;


[TestFixture]

// ReSharper disable once InconsistentNaming
public class IniConfig_Tests : Assert
{
    private static readonly Random _random = new();


    [Test]
    public void Test()
    {
        var project = new IniConfig.Section
                      {
                          ["Name"] = nameof(IniConfig_Tests)
                      };

        project.Add( nameof(DateTime), DateTime.Now );
        project.Add( nameof(Guid),     Guid.NewGuid() );

        project.Add( nameof(AppVersion), new AppVersion( 1, 2, 3, 4, 5, 6, AppVersionFlags.Stable ) );

        var server = new IniConfig.Section
                     {
                         ["Name"] = nameof(ServicePoint)
                     };

        server.Add( "Port", 5000 );


        var sb = new StringBuilder();
        sb.AppendJoin( '.', _random.Next( 255 ), _random.Next( 255 ), _random.Next( 255 ), _random.Next( 255 ) );
        server.Add( nameof(IPAddress), sb.ToString() );

        var ini = new IniConfig
                  {
                      [nameof(project)] = project,
                      [nameof(server)]  = server
                  };

        ini[nameof(Random)]
           .Add( nameof(Random.Next), _random.Next() );

        ini[nameof(IniConfig_Tests)]
           .Add( nameof(Random.Next), _random.Next() );

        string actual = ini.ToString();
        $"-- {nameof(actual)} --\n{actual}".WriteToConsole();
        var results = IniConfig.From<IniConfig>( actual );

        $"-- {nameof(results)} --\n{results}".WriteToConsole();
        NotNull( results );
        AreEqual( results,                   ini );
        AreEqual( results?[nameof(project)], project );
    }
}
