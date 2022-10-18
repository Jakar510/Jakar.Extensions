try
{
    "Hello World!".WriteToConsole();

    using var source = new CancellationTokenSource( TimeSpan.FromSeconds( 15 ) );
    var       time   = TimeSpan.FromSeconds( 5 );

    $"Start: {DateTime.Now}".WriteToConsole();
    var task = new DelayValueTask( time, source.Token );
    await task;
    $"End: {DateTime.Now}".WriteToConsole();


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
    // new AppVersion(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue).ToString()
    //                                                                                                   .Length.WriteToDebug();


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
}
catch (Exception e) { e.WriteToDebug(); }
finally { "Bye".WriteToConsole(); }
