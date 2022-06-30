// unset

#nullable enable
using Plugin.Media.Abstractions;
using Xamarin.Essentials;



namespace Jakar.Extensions.Xamarin.Forms;


public static class FileExtensions
{
    public static FileInfo ToFileInfo( this MediaFile file ) => new(file.Path ?? throw new ArgumentNullException(nameof(file)));


    public static async Task<LocalFile?> Pick( PickOptions? options = null )
    {
        FileResult? result = await FilePicker.PickAsync(options).ConfigureAwait(false);

        return new LocalFile(result.FullPath);
    }

    public static async Task<IEnumerable<LocalFile>?> PickMultiple( PickOptions? options = null )
    {
        IEnumerable<FileResult>? items = await FilePicker.PickMultipleAsync(options).ConfigureAwait(false);

        return items?.Select(item => new LocalFile(item.FullPath));
    }
}
