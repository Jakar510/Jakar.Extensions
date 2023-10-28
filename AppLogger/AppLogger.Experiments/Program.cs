using System;
using Jakar.Extensions;


// using Jakar.Xml;
// using Jakar.Xml.Deserialization;



namespace AppLogger.Experiments;


public enum Page
{
    Home,
    Master,
    Detail
}



public static class Program
{
    public static void Main( string[] args )
    {
        try
        {
            "Hello World!".WriteToConsole();

            // BenchmarkRunner.Run<MapperBenchmarks>();
            // BenchmarkRunner.Run<JsonizerBenchmarks>();
            // BenchmarkRunner.Run<SpansBenchmarks>();
        }
        catch ( Exception e )
        {
            e.WriteToConsole();
            e.WriteToDebug();
        }
    }
}
