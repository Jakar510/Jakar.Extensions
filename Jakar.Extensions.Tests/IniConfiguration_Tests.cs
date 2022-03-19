using System;
using System.Net;
using Jakar.Extensions.Models;
using Jakar.Extensions.Models.IniConfiguration;
using Jakar.Extensions.Strings;
using NUnit.Framework;


namespace Jakar.Extensions.Tests;


[TestFixture]

// ReSharper disable once InconsistentNaming
public class IniConfig_Tests : Assert
{
    [Test]
    public void Test()
    {
        var project = new IniConfig.Section
                      {
                          ["Name"] = nameof(IniConfig_Tests),
                      };

        project.Add(nameof(DateTime), DateTime.Now);
        project.Add(nameof(Guid), Guid.NewGuid());

        project.Add(nameof(AppVersion),
                    new AppVersion(1,
                                   2,
                                   3,
                                   4,
                                   5,
                                   6,
                                   AppVersion.Option.Stable));

        var server = new IniConfig.Section
                     {
                         ["Name"] = nameof(ServicePoint),
                     };

        server.Add("Port", 5000);
        server.Add(nameof(IPAddress), IPAddress.Broadcast);

        var ini = new IniConfig
                  {
                      [nameof(project)] = project,
                      [nameof(server)]  = server
                  };

        ini[nameof(Random)].Add(nameof(Random.Next), new Random().Next());
        ini[nameof(IniConfig_Tests)].Add(nameof(Random.Next), new Random().Next());

        var s = ini.ToString();
        s.WriteToConsole();
        IniConfig? results = IniConfig.FromString(s);

        results?.WriteToConsole();
        NotNull(results);
        AreEqual(results, ini);
        AreEqual(results?[nameof(project)], project);
    }
}
