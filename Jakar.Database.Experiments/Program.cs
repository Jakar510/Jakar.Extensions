using Jakar.Database;


try
{
    "Hello World!".WriteToConsole();
    Console.WriteLine();
    /*
#pragma warning disable OpenTelemetry // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    Jakar.Database.Activities.Tags.Print();
#pragma warning restore OpenTelemetry // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    */


    // Tests.Test_Hashes();

    // await Tests.Test_ConcurrentObservableCollection();

    TestDatabase.TestAsync<DbExperiments>();

    // JsonTest.Run();

    // BenchmarkRunner.Run<SpansBenchmarks>();

    // BenchmarkRunner.Run( typeof(Program).Assembly ); //, new BenchmarkConfig()

    // ULongHashTests.Run( 100_000_000 )
    //               .WriteToDebug();


    // foreach ( string name in typeof(Spans).GetMethods().Select( x => x.Name ).Distinct() ) { Console.WriteLine( name ); }
}
catch ( Exception e ) { e.WriteToConsole(); }
finally { "Bye".WriteToConsole(); }


// var values = Language.Supported;
//
// values
//
//     //
//     // .Where( x => x.DisplayName.Contains( "Czech", StringComparison.OrdinalIgnoreCase ) )
//    .Where( x => x.Version is not null )
//    .ToPrettyJson()
//    .WriteToConsole();


// var p = new DynamicParameters();
// p.Add( nameof(AppVersion), "1.0.0" );
//
// try { throw new SqlException("select * from table", p, true); }
// catch ( Exception e ) { e.WriteToConsole(); }


// const string SOURCE  = "1.2.3.4.5.6";
// bool         success = AppVersion.TryParse( SOURCE, out AppVersion? version );
// success.WriteToDebug();
// version?.WriteToDebug();
// (SOURCE == version.ToString()).WriteToDebug();


// var project = new IniConfig.Section( "Project" )
//               {
//                   ["Name"] = nameof(Program),
//               };
//
// project.Add( nameof(DateTime),       DateTime.Now );
// project.Add( nameof(DateTimeOffset), DateTimeOffset.UtcNow );
// project.Add( nameof(Guid),           Guid.NewGuid() );
//
// project.Add( nameof(AppVersion), new AppVersion( 1, 2, 3, 4, 5, 6, AppVersionFlags.Alpha( 1 ) ) );
//
// var server = new IniConfig.Section( "Server" )
//              {
//                  ["Name"] = nameof(ServicePoint),
//              };
//
// server.Add( "Port", Random.Shared.Next( IPEndPoint.MinPort, IPEndPoint.MaxPort ) );
//
//
// server.Add( nameof(IPAddress), string.Join( '.', Random.Shared.Next( 255 ), Random.Shared.Next( 255 ), Random.Shared.Next( 255 ), Random.Shared.Next( 255 ) ) );
//
// var ini = new IniConfig
//           {
//               project,
//               server,
//           };
//
// ini[nameof(Random)]
//    .Add( nameof(Random.Next), Random.Shared.Next() );
//
// ini[nameof(Program)]
//    .Add( nameof(Random.Next), Random.Shared.Next() );
//
//
// string actual = ini.ToString();
// $"-- {nameof(actual)} --\n{actual}".WriteToConsole();
//
// var results = IniConfig.Parse( actual )
//                        .ToString();
//
// $"-- {nameof(results)} --\n{results}".WriteToConsole();
//
// (actual == results).WriteToDebug();


// byte.MaxValue.ToString()
//     .Length.WriteToDebug();
//
// sbyte.MaxValue.ToString()
//      .Length.WriteToDebug();
//
// short.MaxValue.ToString()
//      .Length.WriteToDebug();
//
// ushort.MaxValue.ToString()
//       .Length.WriteToDebug();
//
// int.MaxValue.ToString()
//    .Length.WriteToDebug();
//
// uint.MaxValue.ToString()
//     .Length.WriteToDebug();
//
// long.MaxValue.ToString()
//     .Length.WriteToDebug();
//
// ulong.MaxValue.ToString()
//      .Length.WriteToDebug();
//
// float.MaxValue.ToString( CultureInfo.InvariantCulture )
//      .Length.WriteToDebug();
//
// double.MaxValue.ToString( CultureInfo.InvariantCulture )
//       .Length.WriteToDebug();
//
// decimal.MaxValue.ToString( CultureInfo.InvariantCulture )
//        .Length.WriteToDebug();
//
// TimeSpan.MaxValue.ToString()
//         .Length.WriteToDebug();
//
// TimeSpan.MinValue.ToString()
//         .Length.WriteToDebug();
//
// DateTime.Now.ToString( CultureInfo.InvariantCulture )
//         .Length.WriteToDebug();
//
// DateTimeOffset.Now.ToString( CultureInfo.InvariantCulture )
//               .Length.WriteToDebug();
//
// new AppVersion(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue).ToString().Length.WriteToDebug();


// var builder = new ValueStringBuilder();
// builder = builder.Append( "ARG? " );
// builder = builder.Append( "this is a test" );
// builder = builder.Append( "!!" );
// builder = builder.Append( '?' );
// builder = builder.Append( ' ' );
// builder = builder.Append( '.', 5 );
// builder = builder.Append( ' ' );
// builder = builder.AppendSpanFormattable( DateTime.Now, "dd/mm/yyyy hh:ss" );
// builder = builder.Replace( 0, 'T' );
//
// builder = builder.Insert( builder.Span.IndexOf( 't' ), "Yes " );
// builder = builder.Insert( 4,                           ' ', 5 );
// builder = builder.Replace( 0, "No! " );
// builder = builder.Replace( 4, '~', 4 );
// builder.WriteToConsole();


// BenchmarkRunner.Run<MapperBenchmarks>();
// BenchmarkRunner.Run<JsonizerBenchmarks>();
// BenchmarkRunner.Run<SpansBenchmarks>();
// BenchmarkRunner.Run<AsyncLinqBenchmarks>();

// TestJson();

// Test_Sql();

// await Test_HttpBuilder();


// var id = Guid.NewGuid();
// id.WriteToConsole();
//
// var b64 = id.ToBase64();
// b64.WriteToConsole();
//
// var result = b64.AsGuid();
// result?.WriteToConsole();
// id.ToString().Length.WriteToConsole();
// ( result == id ).WriteToConsole();


// await Tests.Test_AsyncLinq(Tests.List);
