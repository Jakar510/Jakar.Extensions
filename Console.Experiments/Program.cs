using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jakar.Extensions.FileSystemExtensions;
using Jakar.Extensions.General;
using Jakar.Extensions.Models.Base.Records;
using Jakar.Extensions.Models.Collections;
using Jakar.Extensions.Strings;
using Jakar.Xml;


// using Jakar.Xml;



namespace Console.Experiments;


public static class Program
{
    public static void Main( string[] args )
    {
        "Hello World!".WriteToConsole();


        try { First(); }
        catch ( Exception e )
        {
            e.ToString().WriteToConsole();

            var details = new ExceptionDetails(e);
            details.ToPrettyJson().WriteToConsole();
        }


        "Bye".WriteToConsole();
    }


    public static void First() => Second();
    private static void Second() => Third();
    private static void Third() => Last();
    private static void Last() => throw new NotImplementedException("", new NullReferenceException(nameof(Program)));


    public static void TestXml()
    {
        var document = new XDocument(@"
<Group xmls=""System.Collections.Generic.List"">
<Item>Test String</Item>
</Group>");

        foreach ( XNode node in document ) { }


        // var d = new Dictionary<string, object>()
        //         {
        //             ["IDs"] = new List<double> { 1, 2, 3 },
        //             [nameof(User)] = new User()
        //                              {
        //                                  Address = new List<Address>()
        //                                            {
        //                                                new()
        //                                                {
        //                                                    City  = "Plano",
        //                                                    State = "Texas"
        //                                                }
        //                                            },
        //                                  UserName   = "User",
        //                                  IsActive   = true,
        //                                  IsLoggedIn = true,
        //                                  FirstName  = "First",
        //                                  LastName   = "Last"
        //                              },
        //             ["Token"] = Guid.NewGuid(),
        //             ["Data"] = new Dictionary<string, object>()
        //                        {
        //                            ["Test"] = "Success",
        //                            ["Date"] = DateTime.Now
        //                        },
        //         };
        //
        //
        // var l = new List<object>()
        //         {
        //             d,
        //             1,
        //             1d,
        //             "hi",
        //             new MultiDict(),
        //             new[] { "1", "2", "3" },
        //             TimeSpan.MinValue,
        //             TimeSpan.MaxValue,
        //             DateTime.MinValue,
        //             DateTime.MaxValue,
        //             (uint)0
        //         };
        //
        // System.Console.WriteLine();
        //
        // string s = l.ToXml();
        //
        // s.WriteToConsole();
        //
        // var file = new LocalFile("Output.xml");
        // await file.WriteToFileAsync(s);
    }
}
