Console.WriteLine( "Hello World" );

new HashSet<string>( typeof(AsyncLinq).GetMethods().Select( static x => x.Name ) ).ToPrettyJson().WriteToConsole();
