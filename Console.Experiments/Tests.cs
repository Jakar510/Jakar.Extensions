// // Jakar.Extensions :: Console.Experiments
// // 09/16/2022  5:13 PM
//
//
// // using Jakar.Xml;
// // using Jakar.Xml.Deserialization;
//
//
// namespace Console.Experiments;
// #nullable enable
//
//
//
// public enum Page
// {
//     Home,
//     Master,
//     Detail,
// }
//
//
//
// public static class Tests
// {
//     public static IReadOnlyList<long> List { get; } = AsyncLinq.Range( 0L, 10000 )
//                                                                .ToArray();
//
//
//     public static async Task Test_AsyncLinq( IReadOnlyList<long> source, CancellationToken token = default )
//     {
//         await using AsyncEnumerator<long> data = source.AsAsyncEnumerable( token );
//
//         using ( StopWatch.Start() )
//         {
//             (await data.Where( x => x > 0 )
//                        .Where( x => x % 5 == 0 )
//                        .ToList( token )).Count.WriteToConsole();
//         }
//
//
//         using ( StopWatch.Start() )
//         {
//             (await data.Where( x => x > 0 )
//                        .Where( x => x % 5 == 0 )
//                        .ToList( token )).Count.WriteToConsole();
//         }
//
//
//         using ( StopWatch.Start() )
//         {
//             (await data.Where( x => x > 0 )
//                        .Where( x => x % 5 == 0 )
//                        .ToList( token )).Count.WriteToConsole();
//         }
//
//
//         using ( StopWatch.Start() )
//         {
//             (await data.Where( x => x > 0 )
//                        .Where( x => x % 10 == 0 )
//                        .ToList( token )).Count.WriteToConsole();
//         }
//
//
//         using ( StopWatch.Start() )
//         {
//             (await data.Where( x => x > 0 )
//                        .Where( x => x % 10 == 0 )
//                        .ToList( token )).Count.WriteToConsole();
//         }
//
//
//         // using ( StopWatch.Start() )
//         // {
//         //     ( await data.Where(x => x > 0)
//         //                 .Where(x => x % 50 == 0)
//         //                 .ToList(token) ).Count.WriteToConsole();
//         // }
//         //
//         //
//         // using ( StopWatch.Start() )
//         // {
//         //     ( await data.Where(x => x > 0)
//         //                 .Where(x => x % 69 == 0)
//         //                 .ToList(token) ).Count.WriteToConsole();
//         // }
//         //
//         //
//         // using ( StopWatch.Start() )
//         // {
//         //     ( await data.Where(x => x > 0)
//         //                 .Where(x => x % 420 == 0)
//         //                 .ToList(token) ).Count.WriteToConsole();
//         // }
//     }
//
//
//     public static async Task Test_HttpBuilder( CancellationToken token = default )
//     {
//         // var target  = new Uri("https://www.toptal.com/developers/postbin/");
//         var    host    = new Uri( "https://httpbin.org/" );
//         string content = new AppVersion( 1, 2, 3 ).ToString();
//
//         WebRequester builder = WebRequester.Builder.Create( host )
//                                            .With_Timeout( 10 )
//                                            .Build();
//
//
//         (await builder.Get( "/bearer", token )
//                       .AsJson()).WriteToConsole();
//
//         (await builder.Put( "/put", content, token )
//                       .AsJson()).WriteToConsole();
//
//         (await builder.Post( "/post", content, token )
//                       .AsJson()).WriteToConsole();
//
//         (await builder.Get( "/get", token )
//                       .AsJson()).WriteToConsole();
//
//         (await builder.Delete( "/delete", token )
//                       .AsJson()).WriteToConsole();
//
//         (await builder.Patch( "/patch", content, token )
//                       .AsJson()).WriteToConsole();
//
//         (await builder.Get( "/headers", token )
//                       .AsJson()).WriteToConsole();
//
//         (await builder.Get( "/ip", token )
//                       .AsJson()).WriteToConsole();
//
//         (await builder.Get( "/user-agent", token )
//                       .AsJson()).WriteToConsole();
//
//         (await builder.Get( "/cookies", token )
//                       .AsJson()).WriteToConsole();
//
//         WebResponse<LocalFile> response = await builder.Get( "/image/png", token )
//                                                        .AsFile( MimeType.Png );
//
//         using ( response.Payload ) { response.WriteToConsole(); }
//
//
//         (await builder.Get( "/cookies", token )
//                       .AsString()).WriteToConsole();
//
//
//         (await builder.Post( "/anything", content, token )
//                       .AsJson()).WriteToConsole();
//     }
//
//
//     public static void Test_Sql()
//     {
//         // "SELECT * FROM Users"
//         // var sql = new EasySqlBuilder().Select().All().From("Users").Result;
//     }
//
//
//     // try { First(); }
//     // catch ( Exception e )
//     // {
//     //     e.ToString().WriteToConsole();
//     //
//     //     var details = new ExceptionDetails(e);
//     //     details.ToPrettyJson().WriteToConsole();
//     // }
//     // public static void First() => Second();
//     // public static void Second() => Third();
//     // public static void Third() => Last();
//     // public static void Last() => throw new NotImplementedException("", new NullReferenceException(nameof(Program)));
//
//     public static void TestJson()
//     {
//         var first  = new Test( "First",  new Test( "1.1" ), new Test( "1.2" ) );
//         var second = new Test( "Second", new Test( "2.1" ) );
//         var test   = new Test( "Root",   first, second );
//
//
//         test.ToJson()
//             .WriteToConsole();
//
//         string temp = test.ToJson();
//         temp.WriteToConsole();
//
//         "---TEST---".WriteToConsole();
//         JToken json = temp.FromJson();
//
//         json.ToJson( Formatting.Indented )
//             .WriteToConsole();
//     }
//
//
// //     public static void TestXml()
// //     {
// //         var document = new XDocument(@"
// // <Group xmls=""System.Collections.Generic.List"">
// // <Item>Test String</Item>
// // </Group>");
// //
// //         foreach ( XNode node in document ) { }
// //
// //
// //         // var d = new Dictionary<string, object>()
// //         //         {
// //         //             ["IDs"] = new List<double> { 1, 2, 3 },
// //         //             [nameof(User)] = new User()
// //         //                              {
// //         //                                  Address = new List<Address>()
// //         //                                            {
// //         //                                                new()
// //         //                                                {
// //         //                                                    City  = "Plano",
// //         //                                                    State = "Texas"
// //         //                                                }
// //         //                                            },
// //         //                                  UserName   = "User",
// //         //                                  IsActive   = true,
// //         //                                  IsLoggedIn = true,
// //         //                                  FirstName  = "First",
// //         //                                  LastName   = "Last"
// //         //                              },
// //         //             ["Token"] = Guid.NewGuid(),
// //         //             ["Data"] = new Dictionary<string, object>()
// //         //                        {
// //         //                            ["Test"] = "Success",
// //         //                            ["Date"] = DateTime.Now
// //         //                        },
// //         //         };
// //         //
// //         //
// //         // var l = new List<object>()
// //         //         {
// //         //             d,
// //         //             1,
// //         //             1d,
// //         //             "hi",
// //         //             new MultiDict(),
// //         //             new[] { "1", "2", "3" },
// //         //             TimeSpan.MinValue,
// //         //             TimeSpan.MaxValue,
// //         //             DateTime.MinValue,
// //         //             DateTime.MaxValue,
// //         //             (uint)0
// //         //         };
// //         //
// //         // System.Console.WriteLine();
// //         //
// //         // string s = l.ToXml();
// //         //
// //         // s.WriteToConsole();
// //         //
// //         // var file = new LocalFile("Output.xml");
// //         // await file.WriteToFileAsync(s);
// //     }
// }



