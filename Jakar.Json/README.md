# Jakar.Json

## Requirements: 

Microsoft.Net.Compilers.Toolset

## Example Class: 

`
    public partial class Example : IJson
    {
        public partial string ToJson();
        public static partial Example FromJson( in string json );
    }
`