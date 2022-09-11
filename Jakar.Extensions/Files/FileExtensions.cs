// unset


#nullable enable
namespace Jakar.Extensions;


// ReSharper disable once UnusedType.Global
public static class FileExtensions
{
    public static Uri ToUri(this FileInfo file) => new($"file://{file.FullName}", UriKind.Absolute);
}
