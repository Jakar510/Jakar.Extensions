﻿using System.Globalization;


try
{
    "Hello World!".WriteToConsole();

    var builder = new ValueStringBuilder(64);

    builder = builder.Append("this is a test");
    builder = builder.Append('!');
    builder = builder.Append('!');
    builder = builder.Append(' ');
    builder = builder.AppendSpanFormattable(DateTime.Now, "yyyy hh:ss");

    builder.ToString()
           .WriteToDebug();


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
catch ( Exception e ) { e.WriteToDebug(); }
finally { "Bye".WriteToConsole(); }
